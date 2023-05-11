72 buffer: shot
88 buffer: beacon

create initshot
3000 ,
6000 ,
3000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,

create initbeacon
3000 ,
6000 ,
6000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,
1000 ,
2000 ,


: INIT-LASERS ( pin -- )

18 0 DO initshot I cells + @ shot I cells + ! LOOP 
22 0 DO initbeacon I cells + @ beacon I cells + ! LOOP

38000 SWAP SETUP-IRSEND
;

: SETCELL ( bit bool variable-- )
-rot if 2000 else 1000 then -rot
2 * 4 + cells + !
;

: UNHOSTED-SHOT ( team mega -- )
CASE
    0 OF 6 0 shot setcell 5 0 shot setcell ENDOF
    1 OF 6 -1 shot setcell 5 0 shot setcell ENDOF
    2 OF 6 0 shot setcell 5 -1 shot setcell ENDOF
    3 OF 6 -1 shot setcell 5 -1 shot setcell ENDOF
ENDCASE
CASE
    0 OF 4 0 shot setcell 3 0 shot setcell ENDOF
    1 OF 4 -1 shot setcell 3 0 shot setcell ENDOF
    2 OF 4 0 shot setcell 3 -1 shot setcell ENDOF
    3 OF 4 -1 shot setcell 3 -1 shot setcell ENDOF
ENDCASE
2 0 DO I 0 shot setcell loop
;

: UNHOSTED-BEACON ( team shield -- )
0 0 beacon setcell
1 swap beacon setcell
2 -1 beacon setcell
3 -1 beacon setcell
6 4 DO I 0 beacon setcell LOOP
CASE
    0 OF 8 0 beacon setcell 7 0 beacon setcell ENDOF
    1 OF 8 -1 beacon setcell 7 0 beacon setcell ENDOF
    2 OF 8 0 beacon setcell 7 -1 beacon setcell ENDOF
    3 OF 8 -1 beacon setcell 7 -1 beacon setcell ENDOF
ENDCASE
;

: SHOOT ( pin -- )
shot 17 rot irsend
;

: EMIT-BEACON ( pin -- )
beacon 21 rot irsend
;

: MURDER-EVERYONE ( pin -- )
30 0 DO
\ cr ." Murdering, round " I .
3 0 DO
\ cr ." Murdering team " I .
I 3 unhosted-shot dup shoot 10 ms 
loop
loop
drop
;
