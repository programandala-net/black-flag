  \ assembler.macro.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201802051654
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ `macro` and `endm`, compatible with any assembler.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017, 2018.

  \ ===========================================================--
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( macro endm )

: macro ( "name" -- ) : asm ;

  \ doc{
  \
  \ macro ( name -- )
  \
  \ Start the definition of an assembler macro _name_.
  \
  \ Usage example:

  \ ----
  \ macro dos-in, ( -- ) DB c, #231 c, endm
  \   \ Assemble the Z80 instruction `in a,(#231)`, to page in
  \   \ the Plus D memory.
  \ ----

  \ See: `endm`, `asm`, `code`.
  \
  \ }doc

assembler-wordlist >order
: endm ( -- ) end-asm postpone ; ; immediate
previous

  \ doc{
  \
  \ endm ( -- ) "end-m"
  \
  \ Finish the definition of an assembler macro, started by
  \ `macro`.
  \
  \ ``endm`` is an `immediate` word.
  \
  \ }doc

  \ ===========================================================--
  \ Change log

  \ 2016-04-11: Extracted from the assemblers `z80-asm` and
  \ `z80-asm,`. The code was identical in both of them and it
  \ can be useful without an assembler.
  \
  \ 2017-01-05: `need assembler`.
  \
  \ 2017-02-08: Remove the changing of search order and
  \ compilation word list. No need to load the assembler, just
  \ use `assembler-wordlist`.  Document the words.
  \
  \ 2017-02-17: Update cross references.
  \
  \ 2017-02-27: Improve documentation.
  \
  \ 2017-03-07: Update example in documentation.
  \
  \ 2018-02-05: Improve documentation: add pronunciation to
  \ words that need it.

  \ vim: filetype=soloforth

