\ library for sending IR signals

: SETUP-IRSEND ( hz pin -- )
>r clock-hz @ swap /
OMODE-PP r@ io-mode!  r@ ioc!  \ start with pwm zero, i.e. fully off
  r@ p2tim

  dup timer-enabit bis!  \ clock enable
             timer-base >r
              0 TIM.PSC r@ + h!    \ upper 16 bits are used to set prescaler
                TIM.ARR r@ + h!    \ period is auto-reload value
         8 bit TIM.DIER r@ + bis!  \ UDE
  %010 4 lshift TIM.CR2 r@ + !     \ MMS = update
          0 bit TIM.CR1 r> + !     \ CEN

  $78 r@ p2cmp 1 and 8 * lshift ( $0078 or $7800 )
  r@ p2tim timer-base $18 + r@ p2cmp 2 and 2* + bis!
  r@ p2cmp 4 * bit r> p2tim timer-base $20 + bis!
;


: IRLED-ON ( pin -- )
8500 SWAP PWM
;

: IRLED-OFF ( pin -- )
OMODE-PP SWAP IO-MODE!
;

: IRSEND-STACK ( -signal- pin -- )
DEPTH 1 - 2 / 0 DO
DUP IRLED-ON SWAP US
DUP IRLED-OFF SWAP US
LOOP
DROP
;

: IRSEND ( var len pin -- )
swap 1+ 2 / 0 DO
dup IRLED-ON over I 2 * cells + @ us
dup IRLED-OFF over I 2 * 1+ cells + @ us
LOOP DROP DROP
;
