; page6.asm - OS-FRIENDLY CORE CODE

Page6Temp16  equ $
Page6Start32 equ $6000
Page6Start16 equ Page6Start32
org          $6000
dispto zeuspage(6)

Start6:
                        di
                        nextreg $52, 13
                        nextreg $53, 12
                        jp Entry6
Entry6:
                        ld iy, $5C3A
                        ld sp, Stack
                        ld a, $80
                        ld i, a
                        im 2
                        Turbo(MHz14)
                        Contention(false)

                        //PortOut($7ffd, %000 1 0 000)
                        //PortOut($1ffd, %00000 1 00)
                        //Freeze()

                        //NextRegRead(5)
                        //and %00000100
                        //ld (CurrentHz), a

                        nextreg $52, 10
                        ClsAttrFull(Teletext.Background)
                        nextreg $52, 13
                        EnableKeyboardScan(false)
                        ei
                        halt
Start2:
                        call SetupBrowserPalette
                        Border(Teletext.Border)
                        PortOut($123B, $00)             ; Hide layer 2 and disable write paging
                        nextreg $15, %0 00 000 1 0      ; Disable sprites, over border, set SLU

                        DisplayBrowser()
                        ld a, a
                        //ESPLogInit()
                        PageBankZX(0, false)            ; Force MMU reset
                        call ClsAttr
                        MMU7(30, false)
                        ei
                        halt
                        di
                        call SetupDataFileSystem
                        call SetupSprites
                        call SetupCopperFlash
                        di
                        MMU7(30, false)
                        call DefinePalettes
                        call InitLayer2
                        NextRegRead(%00)
                        cp 10                           ; Next and CSpect 1.14.1 has clock
                        jp z, IsNext
                        cp 8                            ; ZEsarUX and Zeus doesn't yet
                        jp nz, IsNext
IsNext:                 ld hl, Resources.Table          ; Calculate Pages.Table address dynamically
                        ld a, (ResourcesCount)
                        add a, a
                        add hl, a
                        ld (PagesTable), hl             ; Store Pages.Table address
                        call Welcome
                        MMU6(2, false)
                        MMU7(3, false)
                        call ParseCfgFile
                        call LoadSettings
                        jp MainMenu
RunCarousel:
                        MMU6(0, false)
                        MMU7(1, false)
                        ld hl, Resources.Table          ; Calculate Pages.Table address dynamically
                        ld a, (ResourcesCount)
                        add a, a
                        add hl, a
                        ld (PagesTable), hl             ; Store Pages.Table address
NextPage:
                        MMU6(0, false)
                        MMU7(1, false)
                        ld a, (PagesCount)
                        ld e, a
                        ld a, (PagesCurrent)            ; Load next page
                        inc a
                        cp e
                        jp c, SavePage
                        xor a
SavePage:               ld (PagesCurrent), a
                        ld hl, [PagesTable]SMC
                        add a, a
                        add a, a
                        add hl, a
                        ld a, (hl)
                        ex af, af'
                        inc hl
                        ld b, (hl)
                        inc hl
                        ld e, (hl)
                        inc hl
                        ld d, (hl)
                        ld a, d
                        and %1000 0000
                        nextreg $57, 30
                        jp z, DisableDemoClock
EnableDemoClock:        EnableTime(true, false)
                        jp DemoClockContinue
DisableDemoClock:       EnableTime(false, false)
DemoClockContinue:      ld a, d
                        and %0111 1111
                        ld d, a
                        ex de, hl
                        ld (PageDuration), hl
                        ex af, af'
                        MMU7(30, false)
                        call LoadPage                   ; Bank in a (e.g. 31), Page in b (0..7)
                        call RenderBuffer               ; display page
                        FlipScreen()
MainLoop:
                        ei
                        halt
                        ld bc, zeuskeyaddr("[shift]")
                        in a, (c)
                        and zeuskeymask("[shift]")
                        jp nz, NoCarouselBreak
                        ld b, high zeuskeyaddr("[space]")
                        in a, (c)
                        and zeuskeymask("[space]")
                        jp z, MainMenu
NoCarouselBreak:
                        call DoFlash
                        MMU7(30, true)
                        ld hl, [PageTimer]SMC
                        inc hl
                        ld (PageTimer), hl
                        ld bc, [PageDuration]SMC
                        CpHL(bc)
                        jp nz, MainLoop
                        ld hl, 0
                        ld (PageTimer), hl
                        jp NextPage
PagesCurrent:           db -1

                        include "utilities.asm"         ; Utility routines
                        include "esxDOS.asm"
                        //include "espat.asm"
                        include "esp.asm"

zeusassert $<$8000, "Page 6 has overflowed past $7FFF."

Page6End32   equ $-1
Page6End16   equ Page6End32
Page6Size equ Page6End32-Page6Start32+1
zeusassert !(Page6Size<>(Page6End16-Page6Start16+1)), "Page6Size calculation error"
if enabled ReportBankSizes
  zeusprinthex "Pg6Size = ", Page6Size
endif
org Page6Temp16
disp 0

