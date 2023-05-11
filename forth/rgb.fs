: rgboff ( -- )
   omode-pp RED_PIN io-mode!
  -1 RED_PIN io!
   omode-pp GREEN_PIN io-mode!
  -1 GREEN_PIN io!
   omode-pp BLUE_PIN io-mode!
  -1 BLUE_PIN io!
;

: rgb-init ( -- )
  1 RED_PIN pwm-init
  1 GREEN_PIN pwm-init
  1 BLUE_PIN pwm-init
  rgboff
;

: rgb-red
  rgboff
  0 RED_PIN io!
;

: rgb-green
  rgboff
  0 GREEN_PIN io!
;

: rgb-blue
  rgboff
  0 BLUE_PIN io!
;

: rgb-purple
  rgboff
  0 RED_PIN io!
  0 BLUE_PIN io!
;

: rgb-yellow
  rgboff
  0 RED_PIN io!
  0 GREEN_PIN io!
;

: rgb-redblink
  rgboff
  omode-af-pp RED_PIN io-mode!
  5000 RED_PIN PWM
;
