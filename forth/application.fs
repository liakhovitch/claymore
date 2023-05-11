\ This file should contain the application layer
<<<core>>>
cr
compiletoflash

0 variable MINE_TEAM
4 buffer: inbuf
create 'mineshots 4 ,
create 'minemega 1 ,
create 'sound -1 ,
create 'explode -1 ,
create 'minecounts 5 ,
create 'grenade 0 ,
create 'autorearm 0 ,

: GRENADE
s" 'grenade @" evaluate
;

: MINE_COUNTS
s" 'minecounts @" evaluate
;

: SOUND
s" 'sound @" evaluate
;

: EXPLODE
s" 'explode @" evaluate
;

: MINE_SHOTS
s" 'mineshots @" evaluate
;

: MINE_MEGA
s" 'minemega @" evaluate
;

: AUTOREARM
s" 'autorearm @" evaluate
;

: aaccept ( addr no -- I )
  swap over 0 DO
    KEY
    dup 13 = IF r> r> DROP DROP DROP DROP DROP I exit THEN
    over I + C!
  LOOP
  DROP
;

: getinput
  cr
  0 inbuf !
  inbuf dup 4 aaccept 
  ?dup 0= IF DROP 0 exit THEN
  evaluate -1
;

: settings
  cr cr ." Welcome to the configurator!"
  cr ." Enter numbers to set options or leave blank to keep current."
  cr ." -1 is yes and 0 is no."
  cr cr ." Reset to defaults?"
  KEY drop
  getinput IF IF
    s" <<<application>>>" evaluate
  THEN THEN
  cr cr ." Number of shots?"
  cr ." Current: " MINE_SHOTS .
  cr ." Default: " 'mineshots @ .
  getinput IF
    dup -1 > IF
      compiletoflash s" create 'mineshots" evaluate , compiletoram
    THEN
  THEN
  cr cr ." Additional damage per shot (0-3)?"
  cr ." Current: " MINE_MEGA .
  cr ." Default: " 'minemega @ .
  getinput IF
    dup dup -1 > swap 4 < AND IF
      compiletoflash s" create 'minemega" evaluate , compiletoram
    THEN
  THEN
  cr cr ." Enable menu sounds?"
  cr ." Current: " SOUND .
  cr ." Default: " 'sound @ .
  getinput IF
    IF
      compiletoflash s" create 'sound" evaluate -1 , compiletoram
    ELSE
      compiletoflash s" create 'sound" evaluate 0 , compiletoram
    THEN
  THEN
  cr cr ." Enable explosion sound?"
  cr ." Current: " EXPLODE .
  cr ." Default: " 'explode @ .
  getinput IF
    IF
      compiletoflash s" create 'explode" evaluate -1 , compiletoram
    ELSE
      compiletoflash s" create 'explode" evaluate 0 , compiletoram
    THEN
  THEN
  cr cr ." Number of counts in countdown?"
  cr ." Current: " MINE_COUNTS .
  cr ." Default: " 'minecounts @ .
  getinput IF
    dup -1 > IF
      compiletoflash s" create 'minecounts" evaluate , compiletoram
    THEN
  THEN
  cr cr ." Grenade mode?"
  cr ." Current: " GRENADE .
  cr ." Default: " 'grenade @ .
  getinput IF
    IF
      compiletoflash s" create 'grenade" evaluate -1 , compiletoram
    ELSE
      compiletoflash s" create 'grenade" evaluate 0 , compiletoram
    THEN
  THEN
  cr cr ." Auto Re-arm?"
  cr ." Current: " AUTOREARM .
  cr ." Default: " 'autorearm @ .
  getinput IF
    IF
      compiletoflash s" create 'autorearm" evaluate -1 , compiletoram
    ELSE
      compiletoflash s" create 'autorearm" evaluate 0 , compiletoram
    THEN
  THEN
  cr cr ." Thank you for shopping at Walmart."
  cr ." Please come again!" cr
  cr ."  | |"
  cr ." \___/" cr
;

: setup-all
  imode-pull BUTT_PIN io-mode!
  imode-pull PIR_PIN io-mode!
  imode-pull PRGRM_PIN io-mode!
  rgb-init
  RECV_PIN init-recv
  IRLED_PIN init-lasers
;

: till-button
  BEGIN
    key? IF settings THEN
    BUTT_PIN io@
    10 ms
  UNTIL
;

: till-notbutton
  BEGIN
    BUTT_PIN io@ NOT
    10 ms
  UNTIL
;

: chkir
  BEGIN
    lqueued @ IF
      locklbuf
      cr ." MSG Receieved"
      sigtype?
      1 = IF
        cr ." MESSAGE CONFIRMED"
        beaconparse
        drop
        MINE_TEAM !
        ulocklbuf
        1 exit
      THEN
      cr ." IGNORING MESSAGE"
      ulocklbuf
    THEN
    10 ms
    BUTT_PIN io@ NOT IF
      rgboff
      0 exit
    THEN
  0 UNTIL
;

: memwipe
  rgb-purple
  SPEAKER_PIN beep-long
  rgboff
  s" <<<application>>>" evaluate
;

: get-team
  BEGIN
    till-button
    PRGRM_PIN io@ NOT IF memwipe THEN
    rgb-blue
    SOUND IF SPEAKER_PIN beep THEN
    ulocklbuf
    chkir
  UNTIL
  rgb-green
  SOUND IF MINE_TEAM @ CASE
    0 OF SPEAKER_PIN beep-long ENDOF
    1 OF SPEAKER_PIN beep ENDOF
    2 OF SPEAKER_PIN 2beep ENDOF
    3 OF SPEAKER_PIN 3beep ENDOF
  ENDCASE THEN
  till-notbutton
  rgboff
;

: till-notpir ( -- interrupted by button? )
  0
  BEGIN
    0 BUTT_PIN io@ IF
      till-notbutton DROP DROP -1 -1
    THEN
    PIR_PIN io@ NOT
    10 ms
    OR
  UNTIL
;

: till-pir ( -- interrupted by button? )
  0 BEGIN
    0 BUTT_PIN io@ IF
      till-notbutton DROP DROP -1 -1 
    THEN
    PIR_PIN io@
    10 ms
    OR
  UNTIL
;

: butwait ( t -- button? )
  0 swap
  0 DO
    BUTT_PIN io@ IF
      till-notbutton
      DROP -1 LEAVE
    THEN
    10 ms
  LOOP
;

: kaboom
  cr ." BAM!!!"
  cr ." Shooting as team " MINE_TEAM @ .
  rgb-red
  MINE_TEAM @ MINE_MEGA UNHOSTED-SHOT
  omode-pp RECV_PIN io-mode!
  EXPLODE IF 1500 SPEAKER_PIN pwm-init 5000 SPEAKER_PIN PWM THEN
  MINE_SHOTS 0 DO
    IRLED_PIN shoot
    75 ms
  LOOP
  imode-pull RECV_PIN io-mode!
  1000 ms
  EXPLODE IF SPEAKER_PIN pwm-deinit THEN
;

: mine ( -- EXIT_CONDITION )
  0
  MINE_COUNTS IF
    MINE_COUNTS 0 DO
      rgb-yellow
      SOUND IF SPEAKER_PIN beep THEN
      rgboff
      100 butwait
      IF DROP -1 LEAVE THEN
    LOOP
  THEN
  IF cr ." Button pressed, exiting" rgboff 0 exit THEN
  rgb-blue
  SOUND IF SPEAKER_PIN beep-long THEN
  rgboff
  GRENADE NOT IF
    till-notpir
    IF cr ." Button pressed, exiting" rgboff 0 exit THEN
    rgb-redblink
    till-pir
    IF cr ." Button pressed, exiting" rgboff 0 exit THEN
    rgboff
  THEN
  kaboom
  -1
;

: main
  imode-pull PA10 io-mode!
  imode-pull PRGRM_PIN io-mode!
  100 ms
  PRGRM_PIN io@ NOT IF cr ." REPL MODE" cr exit THEN
  cr ." APPLICATION MODE"
  cr ." FLIP PRGRM SWITCH OR REMOVE"
  cr ." BOARD FROM SOCKET FOR REPL"
  cr cr ." PRESS ENTER FOR SETUP" cr
  setup-all
  BEGIN
    get-team
    BEGIN
      mine
      AUTOREARM AND NOT
    UNTIL
  0 UNTIL
;

: init
init
main
;

cornerstone <<<application>>>
compiletoram
hello
