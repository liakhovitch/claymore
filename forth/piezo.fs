1500 CONSTANT high-freq
1000 CONSTANT low-freq

: tone ( duration hz pin -- ) 
swap over pwm-init 5000 over pwm swap pause ms pwm-deinit
;

: beep ( pin -- )
200 high-freq rot tone
;

: 2beep ( pin -- )
2 0 DO dup 100 high-freq rot tone 100 ms loop drop
;

: 3beep ( pin -- )
3 0 DO dup 100 high-freq rot tone 50 ms loop drop
;

: beep-long ( pin -- )
500 low-freq rot tone
;
