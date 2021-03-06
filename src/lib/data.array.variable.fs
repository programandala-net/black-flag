  \ data.array.variable.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201803052149
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Words to create and manage 1-dimension single-cell,
  \ double-cell and character variables arrays, which behave
  \ like `variable`.

  \ Usage example of a single-cell variables array:

    \ 4 avariable bar
    \ 10 0 bar !  20 1 bar !  30 2 bar !  40 3 bar !
    \ 3 bar @ .
    \ 0 bar @ .
    \ 123 3 bar !
    \ 3 bar @ .
    \ 1 3 bar +!
    \ 3 bar @ .

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2016, 2017, 2018.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( avariable 2avariable cavariable )

unneeding avariable ?( need array>

: avariable ( n "name" -- )
  create  cells allot
  does> ( n -- a ) ( n dfa ) array> ; ?)

  \ doc{
  \
  \ avariable ( n "name" -- ) "a-variable"
  \
  \ Create a 1-dimension single-cell variables array _name_
  \ with _n_ elements and the execution semantics defined
  \ below.
  \
  \ _name_ execution:
  \
  \ name ( n -- a )
  \
  \ Return address _a_ of element _n_.
  \
  \ See: `2avariable`, `cavariable`, `faravariable`.
  \
  \ }doc

unneeding 2avariable ?( need 2array>

: 2avariable ( n "name" -- )
  create  [ 2 cells ] cliteral * allot
  does> ( n -- a ) ( n dfa ) 2array> ; ?)

  \ doc{
  \
  \ 2avariable ( n "name" -- ) "two-a-variable"
  \
  \ Create a 1-dimension double-cell variables array _name_
  \ with _n_ elements and the execution semantics defined
  \ below.
  \
  \ _name_ execution:
  \
  \ name ( n -- a )
  \
  \ Return address _a_ of element _n_.
  \
  \ See: `avariable`, `cavariable`, `far2avariable`.
  \
  \ }doc

unneeding cavariable ?( need align

: cavariable ( n "name" -- )
  create  allot align
  does> ( n -- ca ) ( n dfa ) + ; ?)

  \ doc{
  \
  \ cavariable ( n "name" -- ) "c-a-variable"
  \
  \ Create a 1-dimension character variables array _name_ with
  \ _n_ elements and the execution semantics defined below.
  \
  \ _name_ execution:
  \
  \ name ( n -- ca )
  \
  \ Return address _ca_ of element _n_.
  \
  \ See: `avariable`, `2avariable`, `farcavariable`.
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2016-11-22: First version.
  \
  \ 2016-12-19: Fix requirement of `2avariable`.
  \
  \ 2016-12-20: Fix and improve documentation.
  \
  \ 2017-01-07: Fix typo.
  \
  \ 2017-01-18: Fix and update documentation. Improve
  \ `2avariable` with `cliteral`.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-02-17: Fix markup in documentation.  Update cross
  \ references.
  \
  \ 2017-09-09: Update notation "pfa" to the standard "dfa".
  \
  \ 2018-02-05: Improve documentation: add pronunciation to
  \ words that need it.
  \
  \ 2018-03-05: Update `[unneeded]` to `unneeding`.

  \ vim: filetype=soloforth
