  \ define.MISC.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201712101731
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Miscellaneous definers that can be defined in less than one
  \ block.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( create: ;code :noname nextname )

[unneeded] create:
?\ : create: ( "name" -- ) create hide ] ;

  \ Credit:

  \ The idea for `create:` was borrowed from CP/M-volksForth
  \ 3.80a.

  \ doc{
  \
  \ create: ( "name" -- )
  \
  \ Create a word _name_ which is compiled as a colon word but,
  \ when executed, will return the address of its data field
  \ address.
  \
  \ }doc

[unneeded] ;code ?(

: ;code ( -- ) postpone (;code) finish-code asm
  ; immediate compile-only ?)

  \ doc{
  \
  \ ;code
  \   Compilation: ( C: colon-sys -- )
  \   Run-time: ( -- ) ( R: nest-sys -- )

  \ Define the execution-time action of a word created by a
  \ low-level defining word.  Used in the form:

  \ ----
  \ : namex ... create ... ;code ... end-code
  \
  \ namex name
  \ ----

  \ where `create` could be also any user defined word which
  \ executes `create`.

  \ ``;code`` marks the termination of the defining part of the
  \ defining word _namex_ and then begins the definition of the
  \ execution-time action for words that will later be defined
  \ by _namex_.  When _name_ is later executed, the address of
  \ _name_'s parameter field is placed on the stack and then
  \ the `assembler` code between ``;code`` and `end-code` is
  \ executed.
  \
  \ Detailed description:
  \
  \ Compilation:
  \
  \ Append the run-time semantics  below  to the  current
  \ definition. End  the  current definition, allow it to be
  \ found  in the dictionary, and enter interpretation  state,
  \ consuming _colon-sys_.
  \
  \ Enter `assembler` mode by executing `asm`, until `end-code`
  \ is executed.
  \
  \ Run-time:
  \
  \ Replace the execution semantics of the most recent
  \ definition, which should be defined with `create` or a
  \ user-defined word that calls `create`, with the name
  \ execution semantics given  below.  Return  control  to  the
  \ calling  definition  specified by _nest-sys_.
  \
  \ Initiation: ``( i*x -- i*x dfa ) ( R: -- nest-sys2 )``
  \
  \ Save information _nest-sys2_  about the calling definition.
  \ Place _name_'s data field address _dfa_ on the stack. The
  \ stack effects _i*x_ represent arguments to name.
  \
  \ name Execution:
  \
  \ Perform the machine code sequence that was generated
  \ following ``;code`` and finished by `end-code`.
  \
  \ ``;code`` is an `immediate` and `compile-only` word.
  \
  \ Usage example:

  \ ----
  \ : border-changer ( n -- )
  \   create c,
  \   ;code ( -- )
  \   ( dfa ) h pop, m a ld, FE out, jpnext, end-code
  \
  \ 0 border-changer black-border
  \ 1 border-changer blue-border
  \ 2 border-changer red-border
  \ ----

  \ Which is equivalent to:

  \ ----
  \ : border-changer ( n -- )
  \   create c,
  \   does> ( -- )
  \   ( dfa ) c@ border ;
  \
  \ 0 border-changer black-border
  \ 1 border-changer blue-border
  \ 2 border-changer red-border
  \ ----

  \ Origin: fig-Forth, Forth-79 (Assembler Word Set), Forth-83
  \ (Assembler Extension Word Set), Forth-94 (TOOLS EXT),
  \ Forth-2012 (TOOLS EXT).
  \
  \ See: `does>`, `asm`, `create`, `end-code`.
  \
  \ }doc

[unneeded] :noname ?(

: :noname ( -- xt )
  here  dup lastxt !  last off  !csp
  docolon [ assembler-wordlist >order ] call, [ previous ]
  noname? on  ] ; ?)

  \ doc{
  \
  \ :noname ( -- xt )
  \
  \ Create an execution token _xt_. Enter compilation state and
  \ start the current definition, which can be executed later
  \ by using _xt_.
  \
  \ Origin: Forth-94 (CORE EXT), Forth-2012 (CORE EXT).
  \
  \ See: `nextname`.
  \
  \ }doc

[unneeded] nextname ?( 2variable nextname-string

  \ doc{
  \
  \ nextname-string ( -- a )
  \
  \ A double variable that may hold the address and length of a
  \ name to be used by the next defining word.  This variable
  \ is set by `nextname`.
  \
  \ Origin: Gforth.
  \
  \ See: `nextname-header`.
  \
  \ }doc

: nextname-header ( -- )
  nextname-string 2@ header, default-header ;

  \ doc{
  \
  \ nextname-header ( -- )
  \
  \ Create a dictionary header using the name string set by
  \ `nextname`.  Then restore the default action of
  \ `header`.
  \
  \ Origin: Gforth.
  \
  \ See: `nextname-string`.
  \ `default-header`.
  \
  \ }doc

: nextname ( ca len -- ) nextname-string 2!
  ['] nextname-header ['] header defer! ; ?)

  \ Credit:
  \
  \ `nextname` is borrowed from Gforth.

  \ doc{
  \
  \ nextname ( ca len -- )
  \
  \ The next defined word will have the name _ca len_; the
  \ defining word will leave the input stream alone.
  \ ``nextname`` works with any defining word.
  \
  \ Origin: Gforth.
  \
  \ See: `nextname-header`, `nextname-string`.
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2016-04-24: Move `:noname` from the library.  Move `;code`
  \ to the library.
  \
  \ 2016-11-16: Rename `code-field,` `call,` in `:noname`,
  \ after the changes in the kernel.

  \ 2016-11-26: Create this module to combine the modules that
  \ contain small definers, in order to save blocks:
  \ <define.semicolon.code.fsb>, <define.colon-no-name.fsb>,
  \ <define.colon-nextname.fsb>, <flow.create-colon.fsb>.
  \
  \ 2016-11-26: Improve documentation of `nextname` and family.
  \
  \ 2016-12-06: Improve documentation of `:noname`.
  \
  \ 2016-12-08: Rename the module filename with uppercase
  \ "MISC" after the new convention.
  \
  \ 2016-12-30: Compact the code, saving one block.
  \
  \ 2017-01-05: Update `also assembler` to `assembler-wordlist
  \ >order`.
  \
  \ 2017-01-07: Improve documentation.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-01-19: Remove remaining `exit` at the end of
  \ conditional interpretation, after `immediate` or
  \ `compile-only`.
  \
  \ 2017-02-17: Update notation "behaviour" to "action".
  \ Update cross references.
  \
  \ 2017-02-27: Improve documentation.
  \
  \ 2017-02-28: Fix typo in documentation.
  \
  \ 2017-09-09: Update notation "pfa" to the standard "dfa".
  \
  \ 2017-12-10: Add missing `asm` to `;code`. Improve
  \ documentation.

  \ vim: filetype=soloforth
