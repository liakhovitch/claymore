\ EXTI Registers
$40010000 constant AFIO
AFIO $8 + constant AFIO-EXTICR1
AFIO $C + constant AFIO-EXTICR2
$E000E100 constant NVIC-EN0R
$40010400 constant EXTI
EXTI $00 + constant EXTI-IMR
EXTI $04 + constant EXTI-EMR
EXTI $08 + constant EXTI-RTSR
EXTI $0C + constant EXTI-FTSR
EXTI $10 + constant EXTI-SWIER
EXTI $14 + constant EXTI-PR

$40015000 constant TIM10
TIM10 $00 + constant TIM10_CR1
TIM10 $0C + constant TIM10_DIER \ Interrupt enable
TIM10 $10 + constant TIM10_SR \ Bit 1 is set when capture occurs, cleared by software
TIM10 $14 + constant TIM10_EGR \ Bit 0 is UG, useful to update params
TIM10 $18 + constant TIM10_CCMR1
TIM10 $20 + constant TIM10_CCER
TIM10 $24 + constant TIM10_CNT \ Counter
TIM10 $28 + constant TIM10_PSC \ Prescaler
TIM10 $2C + constant TIM10_ARR \ Auto reload
TIM10 $34 + constant TIM10_CCR1 \ Input Compare

: clockup
  1
  dup timer-enabit bis!  \ clock enable
             timer-base >r
          0 bit TIM.CR1 r@ + bic!     \ CEN
                  0 $24 r@ + !
	clock-hz @ 1050000 /
                TIM.PSC r@ + h!    \ upper 16 bits are used to set prescaler
           9000 TIM.ARR r@ + h!    \ period is auto-reload value
         8 bit TIM.DIER r@ + bis!  \ UDE
  %010 4 lshift TIM.CR2 r@ + !     \ MMS = update
          3 bit TIM.CR1 r@ + bis!     \ One-pulse
          0 bit TIM.CR1 r> + bis!     \ CEN
;

: clock@
1 timer-base $24 + @
;

\ Pins
PB7 constant PRGRM_PIN
PB6 constant IRLED_PIN
PA6 constant SPEAKER_PIN
PB3 constant RECV_PIN
PA1 constant RED_PIN
PA2 constant GREEN_PIN
PA3 constant BLUE_PIN
PB9 constant BUTT_PIN
PA7 constant PIR_PIN

: interrupt ( handler rising? pin -- )
imode-pull over io-mode!
256 /mod
over 4 * lshift AFIO-EXTICR1 bis!

dup 5 < IF
dup 6 + bit NVIC-EN0R bis!
ELSE dup 10 < IF
30 bit NVIC-EN0R bis!
ELSE 40 bit NVIC-EN0R bis!
THEN THEN

dup bit EXTI-IMR bis!
swap IF
dup bit EXTI-RTSR bis!
ELSE
dup bit EXTI-FTSR bis!
THEN
CASE
 0 OF irq-exti0 ! ENDOF
 1 OF irq-exti1 ! ENDOF
 2 OF irq-exti2 ! ENDOF
 3 OF irq-exti3 ! ENDOF
 4 OF irq-exti4 ! ENDOF
ENDCASE
;

: clr-interrupt ( pin -- )
256 mod bit EXTI-PR !
;
