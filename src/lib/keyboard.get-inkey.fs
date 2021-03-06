  \ keyboard.get-inkey.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201703132012
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ `get-inkey`, a variant of `inkey` that works when system
  \ interrupts are off.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( get-inkey )

  \ Credit:
  \
  \ Code adapted and modified from Abersoft Forth's `inkey`.

need assembler

code get-inkey ( -- c | 0 )

  b push,
  028E call, \ KEY-SCAN ROM routine
  \ 1 or 2 keys in DE, most significant shift first if any
  \ key values 0..39 else 255
  z? rif  \ is key press valid?
    031E call, \ KEY-TEST ROM routine
    c? rif  \ is key code valid?
      \ A = main key
      \ D = ?
      00 c ld#,  \ XXX Spectrum Forth-83 does this
      d dec,  a e ld,
      0333 call, \ KEY-DECODE ROM routine
      \ A = key code
    rthen
  rthen
  FF cp#,  z? rif  a xor,  rthen  \ convert FF to 00
  \ XXX TODO jump to `key` to decode
  b pop,  pusha jp, end-code

  \ doc{
  \
  \ get-inkey ( -- 0 | c )
  \
  \ Leave the value of the key being pressed. If no key being
  \ pressed leave zero.
  \
  \ ``get-inkey`` reads the keyboard, so it works even when the
  \ keyboard is not read by an interrupts routine.
  \
  \ See: `inkey`, `key`.
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2016-12-25: Improve documentation. Convert from `z80-asm`
  \ to `z80-asm,` assembler.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-02-17: Update cross references.
  \
  \ 2017-03-13: Improve documentation.

  \ vim: filetype=soloforth
