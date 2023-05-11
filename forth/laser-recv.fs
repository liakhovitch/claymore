0 variable receive-pin
-1 variable lbuflock
0 variable lqueued
88 buffer: laserbuf

: lbufchk 21 0 do laserbuf i cells + @ cr . loop ;

: lbufclr 21 0 do 0 laserbuf i cells + ! loop ;

: locklbuf ( -- )
0 lbuflock !
;

: ulocklbuf
lbufclr
0 lqueued !
-1 lbuflock !
;

: till-low ( pin --- duration )
clockup
BEGIN
dup io@ clock@ 0 = OR
UNTIL drop
clock@
500 + 1000 /
;

: till-high ( pin --- duration )
clockup
BEGIN
dup io@ NOT clock@ 0 = OR
UNTIL drop
clock@
500 + 1000 /
;

: chksig ( pin -- )
0 >r
BEGIN
dup till-low 
?dup IF 
lbuflock @ IF laserbuf r@ cells + ! ELSE drop THEN
ELSE r> drop exit
THEN
r> 1+ >r
dup till-high 
?dup IF 
lbuflock @ IF laserbuf r@ cells + ! ELSE drop THEN
ELSE r> drop exit
THEN
r> 1+ >r
r@ 21 > UNTIL
r> drop
;

: recv-handler ( -- )
receive-pin @ chksig
lbuflock @ IF -1 lqueued ! THEN
clr-interrupt
;

: init-recv ( pin -- )
dup receive-pin !
['] recv-handler 0 ROT interrupt
clockup
;

: shot? ( -- bool )
laserbuf 2 cells + @ 3 = IF -1 ELSE 0 THEN 
;

: shotvalid? ( -- bool )
laserbuf @ 3 = IF
  laserbuf 1 cells + @ 6 = IF
    laserbuf 2 cells + @ 3 = IF
      -1 15 3 DO laserbuf I cells + @ 2 = NOT IF drop 0 THEN 2 +LOOP IF
        -1 16 4 DO laserbuf I cells + @ dup 1 = over 2 = OR NOT IF drop 0 THEN 2 +LOOP IF
          -1 exit
        THEN
      THEN
    THEN
  THEN
THEN
0
;

: beaconvalid? ( -- bool )
laserbuf @ 3 = IF
  laserbuf 1 cells + @ 6 = IF
    laserbuf 2 cells + @ 6 = IF
      -1 19 3 DO laserbuf I cells + @ 2 = NOT IF drop 0 THEN 2 +LOOP IF
        -1 20 4 DO laserbuf I cells + @ dup 1 = swap 2 = OR NOT IF drop 0 THEN 2 +LOOP IF
          -1 exit
        THEN
      THEN
    THEN
  THEN
THEN
0
;

: sigtype? ( -- n ) \ 0 = error, -1 = shot, 1 = beacon
shot? IF
  shotvalid? IF
    -1
  ELSE
    0
  THEN
ELSE
  beaconvalid? IF
    1
  ELSE
    0
  THEN
THEN
;

: getcell ( cell -- f )
2 * 4 + cells laserbuf + @
2 =
;

: shotparse ( -- team mega )
3 getcell IF
 4 getcell IF
   3
 ELSE
   2
 THEN
ELSE
 4 getcell IF
   1
 ELSE
   0
 THEN
THEN
5 getcell IF
 6 getcell IF
   3
 ELSE
   2
 THEN
ELSE
 6 getcell IF
   1
 ELSE
   0
 THEN
THEN
;

: beaconparse ( -- team shield )
7 getcell IF
 8 getcell IF
   3
 ELSE
   2
 THEN
ELSE
 8 getcell IF
   1
 ELSE
   0
 THEN
THEN
1 getcell
;

: ltestcase
CASE
      0 OF CR ." ERROR" ENDOF
     -1 OF
           CR ." Shot!"
           shotparse
           CR ." Mega: " .
           CR ." Team: " .
        ENDOF
      1 OF
           CR ." Beacon!"
           beaconparse
           CR ." Shield: " .
           CR ." Team: " .
        ENDOF
ENDCASE
;

: lasertest
RECV_PIN init-recv
BEGIN
  lqueued @ IF
   locklbuf
   cr ." MSG Receieved"
   sigtype?
   ltestcase CR
   ulocklbuf
  THEN
  10 ms
key? UNTIL
;





