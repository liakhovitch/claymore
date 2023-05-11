<<<board>>>
cr
compiletoflash
( core start: ) here hex.

include embello/flib/mecrisp/multi.fs
include embello/flib/any/timed.fs
include irsend.fs
include laser-emit.fs
include piezo.fs
include stm32f103.fs
include laser-recv.fs
include rgb.fs

pc13 constant LED-BUILTIN

\ include application.fs

cornerstone <<<core>>>
compiletoram
hello
