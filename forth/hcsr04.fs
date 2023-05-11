\ library for the HC-SR04 ultraonic sensor

PB12 constant TRIG
PB13 constant ECHO
500 57 * constant MAX_TIME

: ping-init ( -- )
omode-pp TRIG io-mode!
imode-pull ECHO io-mode!
;

: ping-trigger ( -- )
TRIG ioc!
4 us
TRIG ios!
10 us
TRIG ioc!
;

: ping ( -- n )
ping-trigger
micros
begin
micros over MAX_TIME + - 0 > if ." overtime!" exit then
ECHO io@ until
micros swap -
;
