﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using NXtelData;
using NXtelManager.Models;

namespace NXtelManager
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                var msg = new MailMessage();
                msg.From = new MailAddress(Options.AdminEmailAddress);
                msg.To.Add(new MailAddress(message.Destination));
                foreach (string bcc in Options.AdminEmailBCCList)
                    msg.Bcc.Add(new MailAddress(bcc));
                msg.Subject = (("NXtel " + (Options.Environment.GetDescription() ?? "")).Trim() 
                    + " - " + (message.Subject ?? "").Trim()).Trim();
                msg.Body = message.Body;
                return client.SendMailAsync(msg);
            }
            catch
            {
                return null;
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            //IdentityResult result = null;
            //if (!roleManager.RoleExists("Admin"))
            //    result = roleManager.Create(new IdentityRole("Admin"));
            //if (!roleManager.RoleExists("Page Editor"))
            //    result = roleManager.Create(new IdentityRole("Page Editor"));

            //ApplicationUser user = manager.FindByName("robin.verhagen.guest@gmail.com");
            //if (!manager.IsInRole(user.Id, "Admin"))
            //    result = manager.AddToRole(user.Id, "Admin");
            //if (!manager.IsInRole(user.Id, "Page Editor"))
            //    result = manager.AddToRole(user.Id, "Page Editor");
            //user.Mailbox = "123456789";
            //result = manager.Update(user);

            //user = manager.FindByName("darran@xalior.com");
            //if (!manager.IsInRole(user.Id, "Admin"))
            //    roleResult = manager.AddToRole(user.Id, "Admin");
            //if (!manager.IsInRole(user.Id, "Page Editor"))
            //    roleResult = manager.AddToRole(user.Id, "Page Editor");

            //var user = manager.FindByName("tgilberts@yahoo.com");
            //if (!manager.IsInRole(user.Id, "Admin"))
            //    manager.AddToRole(user.Id, "Admin");
            //if (!manager.IsInRole(user.Id, "Page Editor"))
            //    manager.AddToRole(user.Id, "Page Editor");

            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            var userIdentity = user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, true);
            userIdentity.Result.AddClaim(new Claim("Mailbox", (user.Mailbox ?? "").ToString()));
            userIdentity.Result.SetIsPersistent(true);
            return userIdentity;
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
