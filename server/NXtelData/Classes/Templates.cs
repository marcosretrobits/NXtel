﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace NXtelData
{
    public class Templates : List<Template>
    {
        public static Templates Load()
        {
            var list = new Templates();
            using (var con = new MySqlConnection(DBOps.ConnectionString))
            {
                con.Open();
                string sql = "SELECT * FROM template ORDER BY Description,TemplateID;";
                var cmd = new MySqlCommand(sql, con);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var item = new Template();
                        item.Read(rdr);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static Templates LoadForPage(int PageID, MySqlConnection ConX = null)
        {
            var list = new Templates();
            bool openConX = ConX == null;
            if (openConX)
            {
                ConX = new MySqlConnection(DBOps.ConnectionString);
                ConX.Open();
            }

            string sql = @"SELECT t.*
                    FROM pagetemplate pt
                    JOIN template t ON pt.TemplateID=t.TemplateID
                    WHERE pt.PageID=" + PageID + @"
                    ORDER BY pt.Seq,t.TemplateID;";
            var cmd = new MySqlCommand(sql, ConX);
            using (var rdr = cmd.ExecuteReader())
            {
                int seq = 10;
                while (rdr.Read())
                {
                    var item = new Template();
                    item.Read(rdr);
                    item.Sequence = seq;
                    seq += 10;
                    list.Add(item);
                }
            }

            if (openConX)
                ConX.Close();

            return list;
        }

        public bool DeleteForPage(int PageID, out string Err, MySqlConnection ConX = null)
        {
            Err = "";
            bool openConX = ConX == null;
            if (openConX)
            {
                ConX = new MySqlConnection(DBOps.ConnectionString);
                ConX.Open();
            }
            try
            {
                string sql = @"DELETE FROM pagetemplate WHERE PageID=" + PageID;
                var cmd = new MySqlCommand(sql, ConX);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return false;
            }
            finally
            {
                if (openConX)
                    ConX.Close();
            }
        }

        public bool SaveForPage(int PageID, out string Err, MySqlConnection ConX = null)
        {
            Err = "";
            bool openConX = ConX == null;
            if (openConX)
            {
                ConX = new MySqlConnection(DBOps.ConnectionString);
                ConX.Open();
            }
            try
            {
                var rv = DeleteForPage(PageID, out Err, ConX);
                if (!string.IsNullOrWhiteSpace(Err))
                    return false;
                int seq = 10;
                foreach (var item in this)
                {
                    item.Sequence = seq;
                    item.SaveForPage(PageID, ConX);
                    seq += 10;
                }
                return true;
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return false;
            }
            finally
            {
                if (openConX)
                    ConX.Close();
            }
        }
    }
}
