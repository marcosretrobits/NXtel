; version.asm
;
; Auto-generated by ZXVersion.exe
; On 30 Oct 2018 at 17:48

BuildNo                 macro()
                        db "78"
mend

BuildNoValue            equ "78"
BuildNoWidth            equ 0 + FW7 + FW8



BuildDate               macro()
                        db "30 Oct 2018"
mend

BuildDateValue          equ "30 Oct 2018"
BuildDateWidth          equ 0 + FW3 + FW0 + FWSpace + FWO + FWc + FWt + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "17:48"
mend

BuildTimeValue          equ "17:48"
BuildTimeWidth          equ 0 + FW1 + FW7 + FWColon + FW4 + FW8



BuildTimeSecs           macro()
                        db "17:48:19"
mend

BuildTimeSecsValue      equ "17:48:19"
BuildTimeSecsWidth      equ 0 + FW1 + FW7 + FWColon + FW4 + FW8 + FWColon + FW1 + FW9
