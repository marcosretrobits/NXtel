; version.asm
;
; Auto-generated by ZXVersion.exe
; On 23 Nov 2018 at 15:10

BuildNo                 macro()
                        db "125"
mend

BuildNoValue            equ "125"
BuildNoWidth            equ 0 + FW1 + FW2 + FW5



BuildDate               macro()
                        db "23 Nov 2018"
mend

BuildDateValue          equ "23 Nov 2018"
BuildDateWidth          equ 0 + FW2 + FW3 + FWSpace + FWN + FWo + FWv + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "15:10"
mend

BuildTimeValue          equ "15:10"
BuildTimeWidth          equ 0 + FW1 + FW5 + FWColon + FW1 + FW0



BuildTimeSecs           macro()
                        db "15:10:15"
mend

BuildTimeSecsValue      equ "15:10:15"
BuildTimeSecsWidth      equ 0 + FW1 + FW5 + FWColon + FW1 + FW0 + FWColon + FW1 + FW5
