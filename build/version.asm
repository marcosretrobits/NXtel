; version.asm
;
; Auto-generated by ZXVersion.exe
; On 30 Sep 2018 at 15:57

BuildNo                 macro()
                        db "58"
mend

BuildNoValue            equ "58"
BuildNoWidth            equ 0 + FW5 + FW8



BuildDate               macro()
                        db "30 Sep 2018"
mend

BuildDateValue          equ "30 Sep 2018"
BuildDateWidth          equ 0 + FW3 + FW0 + FWSpace + FWS + FWe + FWp + FWSpace + FW2 + FW0 + FW1 + FW8



BuildTime               macro()
                        db "15:57"
mend

BuildTimeValue          equ "15:57"
BuildTimeWidth          equ 0 + FW1 + FW5 + FWColon + FW5 + FW7



BuildTimeSecs           macro()
                        db "9/30/2018 3:57:46 PM"
mend

BuildTimeSecsValue      equ "9/30/2018 3:57:46 PM"
BuildTimeSecsWidth      equ 0 + FW9 + FW/ + FW3 + FW0 + FW/ + FW2 + FW0 + FW1 + FW8 + FWSpace + FW3 + FWColon + FW5 + FW7 + FWColon + FW4 + FW6 + FWSpace + FWP + FWM
