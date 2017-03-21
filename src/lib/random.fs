  \ random.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201703161920
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Pseudo-random number generators.
  \
  \ See benchmark results in <meta.benchmark.rng.fsb>.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

( rnd random random-range fast-rnd fast-random )

[unneeded] rnd [unneeded] random and ?(

2variable rnd-seed  $0111 rnd-seed !

: rnd ( -- u )
  rnd-seed 2@ $62DC um* rot 0 d+ over rnd-seed 2! ;

  \ doc{
  \
  \ rnd ( -- u )
  \
  \ Return a random number _u_.
  \
  \ See also: `random`, `random-range`, `fast-rnd`.
  \
  \ }doc

: random ( n1 -- n2 ) rnd um* nip ; ?)

  \ doc{
  \
  \ random ( n1 -- n2 )
  \
  \ Return a random number _n2_ from 0 to _n1_ minus 1.
  \
  \ See also: `rnd`, `random-range`, `fast-random`.
  \
  \ }doc

  \ Credit:
  \
  \ Random Number Generator by C. G. Montgomery: `random` and
  \ `rnd`.
  \
  \ Found here (2015-12-13):
  \ http://web.archive.org/web/20060707001752/http://www.tinyboot.com/index.html

[unneeded] random-range  ?( need random

: random-range ( n1 n2 -- n3 ) over - 1+ random + ; ?)

  \ doc{
  \
  \ random-range ( n1 n2 -- n3 )
  \
  \ Return a random number from _n1_ (min) to _n2_ (max).
  \
  \ See also: `random`.
  \
  \ }doc

[unneeded] fast-rnd ?( need os-seed

code fast-rnd ( -- u )
  2A c, os-seed , 54 c, 5D c, 29 c, 19 c, 29 c, 19 c,
    \ ld hl,(seed)
    \ ld d,h
    \ ld e,l
    \ add hl,hl
    \ add hl,de
    \ add hl,hl
    \ add hl,de
  29 c, 19 c, 29 c, 29 c, 29 c, 29 c, 19 c,
    \ add hl,hl
    \ add hl,de
    \ add hl,hl
    \ add hl,hl
    \ add hl,hl
    \ add hl,hl
    \ add hl,de
  24 c, 23 c, 22 c, os-seed , jppushhl, end-code ?)
    \ inc h
    \ inc hl
    \ ld (seed),hl
    \ jp push_hl

  \ doc{
  \
  \ fast-rnd ( -- u )
  \
  \ Return a random number _u_.
  \
  \ ``fast-rnd`` generates a sequence of pseudo-random values
  \ that has a cycle of 65536 (so it will hit every single
  \ number): ``f(n+1)=241f(n)+257``.
  \
  \ See also: `fast-random`, `rnd`.
  \
  \ }doc

[unneeded] fast-random ?( need fast-rnd

: fast-random ( n1 -- n2 ) fast-rnd um* nip ; ?)

  \ doc{
  \
  \ fast-random ( n1 -- n2 )
  \
  \ Return a random number _n2_ from 0 to _n1_ minus 1.
  \
  \ See also: `fast-rnd`, `random`.
  \
  \ }doc

  \ Credit:
  \
  \ Code adapted from:
  \ http://z80-heaven.wikidot.com/math#toc40

  \ Original code:

  \ ----
  \ PseudoRandWord:
  \
  \ ; this generates a sequence of pseudo-random values
  \ ; that has a cycle of 65536 (so it will hit every
  \ ; single number):
  \
  \ ;f(n+1)=241f(n)+257   ;65536
  \ ;181 cycles, add 17 if called
  \
  \ ;Outputs:
  \ ;     BC was the previous pseudorandom value
  \ ;     HL is the next pseudorandom value
  \ ;Notes:
  \ ;     You can also use B,C,H,L as pseudorandom 8-bit values
  \ ;     this will generate all 8-bit values
  \      .db 21h    ;start of ld hl,**
  \ randSeed:
  \      .dw 0
  \      ld c,l
  \      ld b,h
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,hl
  \      add hl,hl
  \      add hl,hl
  \      add hl,bc
  \      inc h
  \      inc hl
  \      ld (randSeed),hl
  \      ret
  \ ----

( crnd crandom -1|1 -1..1 randomize randomize0 )

[unneeded] crnd ?( need os-seed

code crnd ( -- b )
  2A c, os-seed , ED c, 5F c, 57 c, 5E c, 19 c,
    \ ld      hl,(randData)
    \ ld      a,r
    \ ld      d,a
    \ ld      e,(hl)
    \ add     hl,de
  85 c, AC c, 22 c, os-seed , pusha jp, end-code ?)
    \ add     a,l
    \ xor     h
    \ ld      (randData),hl
    \ jp push_a

  \ Credit:
  \
  \ http://wikiti.brandonw.net/index.php?title=Z80_Routines:Math:Random
  \ Joe Wingbermuehle

  \ Original code:

  \ ----
  \ ; ouput a=answer 0<=a<=255
  \ ; all registers are preserved except: af
  \ random:
  \         push    hl
  \         push    de
  \         ld      hl,(randData)
  \         ld      a,r
  \         ld      d,a
  \         ld      e,(hl)
  \         add     hl,de
  \         add     a,l
  \         xor     h
  \         ld      (randData),hl
  \         pop     de
  \         pop     hl
  \         ret
  \ ----

  \ doc{
  \
  \ crnd ( -- b )
  \
  \ Return a random 8-bit number _b_ (0..255).
  \
  \ See also: `crandom`, `rnd`.
  \
  \ }doc

[unneeded] crandom
?\ need crnd  : crandom ( b1 -- b2 ) crnd um* nip ;

  \ doc{
  \
  \ crandom ( b1 -- b2 )
  \
  \ Return a random 8-bit number _b2_ in range _0..b1-1_
  \
  \ }doc

[unneeded] -1|1
?\ need random : -1|1 ( -- -1|1 ) 2 random 2* 1- ;

  \ doc{
  \
  \ -1|1 ( -- -1|1 )
  \
  \ Return a random number: -1 or 1.
  \
  \ See also: `-1..1`, `rnd`, `fast-random`.
  \
  \ }doc

[unneeded] -1..1
?\ need random : -1..1 ( -- -1|0|1 ) 3 random 1- ;

  \ doc{
  \
  \ -1..1 ( -- -1|0|1 )
  \
  \ Return a random number: -1, 0 or 1.
  \
  \ See also: `-1|1`, `rnd`, `fast-random`.
  \
  \ }doc

[unneeded] randomize
?\ need os-seed : randomize ( n -- ) os-seed ! ;

  \ doc{
  \
  \ randomize ( n -- )
  \
  \ Set the seed used by `fast-rnd` and `fast-random` to _n_.
  \
  \ See also: `randomize0`.
  \
  \ }doc

[unneeded] randomize0 ?( need os-frames need randomize

: randomize0 ( n -- )
  ?dup 0= if os-frames @ then randomize ; ?)

  \ doc{
  \
  \ randomize0 ( -- )
  \
  \ Set the seed used by `fast-rnd` and `fast-random` to _n_;
  \ if _n_ is zero use the system frames counter instead.
  \
  \ See also: `randomize`.
  \
  \ }doc

  \ ===========================================================
  \ Change log

  \ 2015-12-25: Add `crnd`.
  \
  \ 2016-03-31: Adapted C. G. Montgomery's `rnd`.
  \
  \ 2016-04-08: Updated the literal in C. G. Montgomery's `rnd`
  \ after the latest benchmarks.
  \
  \ 2016-10-18: Update the name of the benchmarks library
  \ module.
  \
  \ 2016-12-06: Add `-1|1`. Improve documentation and needing
  \ of `randomize` and `randomize0`.
  \
  \ 2016-12-12: Fix needing of `-1|1` and `randomize`.
  \
  \ 2016-12-20: Rename `jppushhl` to `jppushhl,` after the
  \ change in the kernel.
  \
  \ 2016-12-30: Compact the code, saving one block.
  \
  \ 2017-01-02: Convert `fast-rnd` from `z80-asm` to
  \ `z80-asm,`.
  \
  \ 2017-01-04: Convert `crnd` from `z80-asm` to `z80-asm,`;
  \ add its missing requirement. Make `crnd` and `crandom`
  \ accessible to `need`; improve their documentation.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-01-12: Add `-1..1`.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-02-17: Update cross references.
  \
  \ 2017-03-02: Fix `crnd` (a bug introduced when the word was
  \ convernet to the new assembler).
  \
  \ 2017-03-16: Compact the code, saving two blocks.  Complete
  \ and improve documentation. Rewrite `fast-rnd` and `crnd`
  \ with Z80 opcodes.

  \ vim: filetype=soloforth
