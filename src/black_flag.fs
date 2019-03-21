( black-flag )

  \ This file is part of Black Flag
  \ http://programandala.net/en.program.black_flag.html

  \ ============================================================
  \ Description

  \ Black Flag is a simulation and adventure game for the ZX
  \ Spectrum 128 written in Forth with Solo Forth
  \ (http://programandala.net/en.program.solo_forth.html).

  \ Black Flag is a remake of Barry Jones' "Jolly Roger"
  \ (1984),
  \ <http://www.worldofspectrum.org/infoseekid.cgi?id=0002639>.

  \ ============================================================
  \ Authors

  \ Original game in Sinclair BASIC:
  \
  \ Copyright (C) 1984 Barry Jones / Video Vault ltd.

  \ Remake in Forth:
  \
  \ Copyright (C) 2011,2014,2015,2016,2017,2018 Marcos Cruz
  \ (programandala.net)

  \ ============================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ ============================================================

only forth definitions

need printer need order

: section( ( "ccc<paren>" -- )
  ')' parse 2dup cr type printer
                 cr type .s terminal ;
  \ For debugging.

wordlist dup constant game-wordlist  dup >order  set-current

: version$ ( -- ca len ) s" 0.72.0+201903210201" ;

cr section( Black Flag) cr version$ type cr

  \ ============================================================
  section( Requirements)  \ {{{1

only forth definitions

  \ --------------------------------------------
  \ section(   -Assembler)  \ {{{2

  \ need transient
  \ 2000 2000 transient  need assembler  end-transient
  \ XXX TODO --

  \ --------------------------------------------
  section(   -Debugging tools)  \ {{{2

need order  need ~~  need see  need dump  need where
need list \ XXX TMP -- for debugging

  \ --------------------------------------------
  section(   -Definers)  \ {{{2

need alias

  \ --------------------------------------------
  section(   -Control structures)  \ {{{2

need case  need or-of  need j  need 0exit  need default-of
need do

  \ --------------------------------------------
  section(   -Stack manipulation)  \ {{{2

need >true  need >false

  \ --------------------------------------------
  section(   -Math)  \ {{{2

need >=  need <=  need under+  need between  need 2/
need random-between  need randomize0  need -1..1  need d<>
need odd? need */

  \ --------------------------------------------
  \ section(   -Memory)  \ {{{2

  \ --------------------------------------------
  section(   -Time)  \ {{{2

need ms  need seconds  need ?seconds

  \ --------------------------------------------
  section(   -Data and strings)  \ {{{2

need 2avariable  need avariable    need cavariable  need value

need faravariable need far! need far@ need farerase
need farcavariable need farc! need farc@ need farc+!

need far>sconstant
need far>sconstants  need farsconstants  need far,"

need u>str
need uppers1  need s+  need chars>string  need ruler

need s\"  need .\"
need set-esc-order  need esc-standard-chars-wordlist
need esc-block-chars-wordlist  need esc-udg-chars-wordlist

  \ --------------------------------------------
  section(   -Printing and graphics)  \ {{{2

need window  need wcls  need wltype
need whome

need tab  need type-center-field  need at-x  need row
need rows  need columns  need last-column
need inverse  need tabulate
need rom-font
need set-udg  need get-udg  need /udg
need set-font  need get-font

need black   need blue    need red  need green
need cyan    need yellow  need white

need attr!  need papery  need brighty  need blackout
need bright-mask
need attr@ \ XXX TMP -- used by debugging tools

need set-paper  need set-ink  need set-bright

need rdraw176 ' rdraw176 alias rdraw
need plot176  ' plot176  alias plot
  \ XXX TMP --

  \ --------------------------------------------
  section(   -Keyboard)  \ {{{2

need key-left  need key-right  need key-down  need key-up
need new-key-  need -keys

need get-inkey ' get-inkey alias inkey
  \ XXX TMP --

  \ --------------------------------------------
  section(   -Sound)  \ {{{2

need beep

  \ --------------------------------------------

  \ forget-transient
  \ XXX TODO --

game-wordlist  dup >order set-current

  \ ============================================================
  section( Debugging tools [1])  \ {{{1

  \ :  ~~h ( -- ) 2 border new-key- 1 border ;
  \ Break point.
  \ XXX OLD

'q' ~~quit-key c!  bl ~~resume-key c!  20 ~~y c!  ~~? off

: ?break ( -- ) break-key? if cr ." Aborted!" cr quit then ;

  \ ============================================================
  section( Constants)  \ {{{1

15 cconstant sea-length
 9 cconstant sea-breadth

sea-length sea-breadth * constant /sea
  \ cells of the sea map

6 cconstant island-length
5 cconstant island-breadth
  \ XXX TODO -- set them randomly when a new island is created

  \     Island grid

  \ 4| 24 25 26 27 28 29
  \ 3| 18 19 20 21 22 23
  \ 2| 12 13 14 15 16 17
  \ 1| 06 07 08 09 10 11
  \ 0| 00 01 02 03 04 05
  \    _________________
  \     0  1  2  3  4  5

island-length island-breadth * cconstant /island
  \ cells of the island map

10 cconstant men

s" Calavera" far>sconstant island-name$

  \ Sea location types
  \ XXX TODO -- complete
 1 cconstant reef
   \ 7 cconstant
   \ 8 cconstant
   \ 9 cconstant
  \ 10 cconstant
  \ 13 cconstant
  \ 14 cconstant
  \ 15 cconstant
  \ 16 cconstant
21 cconstant shark
22 cconstant treasure-island

  \ Island location types
1 cconstant coast
2 cconstant dubloons-here
3 cconstant hostile-native
4 cconstant just-3-palms-1
5 cconstant snake
6 cconstant just-3-palms-2
7 cconstant native-supplies
8 cconstant native-ammo
9 cconstant native-village

9 cconstant max-offer

                              0 cconstant sky-top-y
                              3 cconstant sky-rows

                              3 cconstant arena-top-y
                             15 cconstant arena-bottom-y
arena-bottom-y arena-top-y - 1+ cconstant arena-rows
  \ The arena is the main space of the scenary,
  \ either island or sea.

                          3 cconstant treasure-island-top-y
                          5 cconstant treasure-island-rows

  \ ============================================================
  section( Variables)  \ {{{1

variable quit-game         \ flag

  \ --------------------------------------------
  section(   -Plot)  \ {{{2

variable crew-loc         \ player position on the island
variable aboard           \ flag
variable alive            \ counter
variable ammo             \ counter
variable cash             \ counter
variable damage           \ counter
variable day              \ counter
variable morale           \ counter
variable sunk-ships       \ counter
variable supplies         \ counter
variable trades           \ counter

: aboard? ( -- f ) aboard @ ;

: ammo+! ( n -- ) ammo @ + 0 max ammo ! ;
  \ Add _n_ to `ammo`, making sure the minimum result is
  \ zero.

: cash+! ( n -- ) cash @ + 0 max cash ! ;
  \ Add _n_ to `cash`, making sure the minimum result is
  \ zero.

: morale+! ( n -- ) morale @ + 0 max morale ! ;
  \ Add _n_ to `morale`, making sure the minimum result is
  \ zero.

: supplies+! ( n -- ) supplies @ + 0 max supplies ! ;
  \ Add _n_ to `supplies`, making sure the minimum result is
  \ zero.

  \ --------------------------------------------
  section(   -Ships)  \ {{{2

variable ship-up      \ flag
variable ship-x
variable ship-y
variable ship-loc

variable enemy-ship-move
variable enemy-ship-x
variable enemy-ship-y
variable enemy-ship-loc

  \ --------------------------------------------
  section(   -Clues)  \ {{{2

variable found-clues       \ counter

variable path
variable tree
variable village
variable turn
variable direction
variable pace

  \ ============================================================
  section( Arrays)  \ {{{1

  \ --------------------------------------------
  section(   -Maps)  \ {{{2

/sea     faravariable sea
/island  faravariable island
/sea     faravariable visited    \ flags for islands
  \ XXX TODO -- character arrays in far memory

  \ --------------------------------------------
  section(   -Crew)  \ {{{2

men farcavariable stamina ( n -- ca )
  \ A byte array variable in far memory.

  \ Crew names are pun funny names in Spanish:

0
  np@ far," Alfredo Minguero"
  np@ far," Armando Bronca"
  np@ far," Borja Monserrano"
  np@ far," Clemente Cato"
  np@ far," César Pullido"  \ XXX TODO -- check
  np@ far," Enrique Sitos"
  np@ far," Erasmo Coso"
  np@ far," Felipe Llejo"
  np@ far," Javi Oneta"
  np@ far," Javier Nesnoche"
  np@ far," Jorge Neral"
  np@ far," Lope Dorreta"
  np@ far," Lope Lotilla"
  np@ far," Manolo Pillo"
  np@ far," Marcos Tilla"
  np@ far," Melchor Icete"
  np@ far," Néstor Nillo"
  np@ far," Néstor Tilla"
  np@ far," Paco Tilla"
  np@ far," Pascual Baricoque"
  np@ far," Pedro Medario"
  np@ far," Policarpio Nero"
  np@ far," Ramiro Inoveo"
  np@ far," Ricardo Borriquero"
  np@ far," Roberto Mate"
  np@ far," Rodrigo Minolas"
  np@ far," Ulises Cocido"
  np@ far," Ulises Tantería"
  np@ far," Vicente Nario"
  np@ far," Vicente Rador"
  np@ far," Víctor Nillo"
  np@ far," Víctor Pedo"
  np@ far," Víctor Tilla"
  np@ far," Zacarías Queroso"
  np@ far," Óscar Nicero"
  np@ far," Óscar Romato"
  np@ far," Óscar Terista"
farsconstants stock-name$ ( n -- ca len )
      constant stock-names

men 2avariable name ( n -- a )
  \ A double-cell array to hold the address and length
  \ of the names of the crew members, compiled in `names$`.

: name$ ( n -- ca len ) name 2@ far>stringer ;

stock-names farcavariable used-name ( n -- ca )
  \ A byte array variable in far memory containing byte flags.
  \ When the correspondent name in `names$` has been used in
  \ `name`, the flag is true ($FF). The goal is to avoid
  \ duplicated names in the crew.

: new-names ( -- ) stock-names 0 do 0 i used-name farc! loop ;
  \ Mark all names unused.

0
  np@ far," en forma"
  np@ far," magullado"
  np@ far," herido leve"
  np@ far," herido grave"
  np@ far," muerto"
far>sconstants stamina$ ( n -- ca len )
   1- constant max-stamina
    0 constant min-stamina

max-stamina 1+ cavariable stamina-attr

 black white papery +         0 stamina-attr c!
   red black papery + brighty 1 stamina-attr c!
   red black papery +         2 stamina-attr c!
yellow black papery +         3 stamina-attr c!
 green black papery +         4 stamina-attr c!

  \ --------------------------------------------
  section(   -Ship damage descriptions)  \ {{{2

0
  np@ far," hundiéndose"          \ worst: sinking
  np@ far," a punto de hundirse"
  np@ far," haciendo agua"
  np@ far," destrozado"
  np@ far," casi destrozado"
  np@ far," gravemente dañado"
  np@ far," muy dañado"
  np@ far," algo dañado"
  np@ far," casi como nuevo"
  np@ far," impecable"            \ best: perfect
far>sconstants >damage$ ( n -- ca len )
  1- cconstant max-damage

  \ --------------------------------------------
  section(   -Village names)  \ {{{2

  \ The names of the villages are Esperanto compound words
  \ whose pronunciation topically resembles African languages,
  \ and have funny meanings.

0
  np@ far," Mislongo"   \ mis-long-o="wrong lenght"
  np@ far," Ombreto"    \ ombr-et-o="little shadow"
  np@ far," Figokesto"  \ fig-o-kest-o="fig basket"
  np@ far," Misedukota" \ mis-eduk-ot-a="one to be miseducated"
  np@ far," Topikega"   \ topik-eg-a=
  np@ far," Fibaloto"   \ fi-balot-o
  np@ far," Pomotruko"  \ pom-o-truk-o
  np@ far," Putotombo"  \ put-o-tomb-o="well tomb"
  np@ far," Ursorelo"   \ urs-orel-o="ear of bear"
  np@ far," Kukumemo"   \ kukum-em-o
far>sconstants village$ ( n -- ca len )
      constant villages

  \ --------------------------------------------
  section(   -Cardinal points)  \ {{{2

0
  np@ far," oeste"
  np@ far," este"
  np@ far," sur"
  np@ far," norte"
far>sconstants cardinal$ ( n -- ca len ) drop

  \ --------------------------------------------
  section(   -Hands)  \ {{{2

0
  np@ far," derecha"    \ right
  np@ far," izquierda"  \ left
far>sconstants hand$ ( n -- ca len ) drop

  \ ============================================================
  section( Functions)  \ {{{1

22528 constant attributes
  \ Address of the screen attributes.

768 constant /attributes
  \ Size of the screen attributes in bytes.  (24 rows of 32
  \ columns).

: attr-line ( l -- a ) columns * attributes + ;
  \ First attribute address of a character line.
  \ XXX TODO -- move to the Solo Forth library

: dubloons$ ( n -- ca len )
  s" dobl" rot 1 > if s" ones" else s" ón" then s+ ;
  \ Return string "doubloon" or "doubloons", depending on _n_.

0
  np@ far," muchos"
  np@ far," dieciséis"
  np@ far," quince"
  np@ far," catorce"
  np@ far," trece"
  np@ far," doce"
  np@ far," once"
  np@ far," diez"
  np@ far," nueve"
  np@ far," ocho"
  np@ far," siete"
  np@ far," seis"
  np@ far," cinco"
  np@ far," cuatro"
  np@ far," tres"
  np@ far," dos"
  np@ far," un"
  np@ far," cero"
far>sconstants number$ ( n -- ca len )
  1- cconstant text-numbers

: highlighted$ ( c -- ca len )
  0 20 rot 1 20 5 chars>string ;
  \ Convert _c_ to a string to print _c_ as a highlighted char,
  \ using control characters.

: >option$ ( ca1 len1 n -- ca2 len2 )
  >r 2dup drop r@
  2over drop r@ + c@ highlighted$ s+
  2swap r> 1+ /string s+ ;
  \ Convert menu option _ca len_ to an active menu option
  \ with character at position _n_ (0..len1-1) highlighted with
  \ control characters.

: ?>option$ ( ca1 len1 n f -- ca1 len1 | ca2 len2 )
  if >option$ else drop then ;
  \ Prepare a panel option _ca1 len1_.  If the option is
  \ active, _f_ is true,  _n_ (0..len1-1) is the position of
  \ its highlighted letter, and _ca2 len2_ is the
  \ option with the given letter highlighted. If the option is
  \ not active, _f_ is false, _n_ is ignored and the string
  \ remains unchanged.

: coins$ ( n -- ca len )
  text-numbers min dup >r number$ s"  " s+ r> dubloons$ s+ ;
  \ Return the text "n doubloons", with letters.

: max-damage? ( -- f ) damage @ max-damage = ;

: failure? ( -- f )
  alive @ 0=
  morale @ 0= or
  max-damage? or
  supplies @ 0= or
  cash @ 0= or ;
  \ Failed mission?
  \
  \ XXX TODO -- use `0=` instead of `1 <`

6 constant max-clues

: success? ( -- f ) found-clues @ max-clues = ;
  \ Success?

: game-over? ( -- f ) failure? success? quit-game @ or or ;
  \ Game over?

: condition$ ( n -- ca len ) stamina farc@ stamina$ ;
  \ Physical condition of a crew member

: blank-line$ ( -- ca len ) bl columns ruler ;
  \ XXX TODO -- use `emits` instead

: damage$ ( -- ca len ) damage @ >damage$ ;
  \ Damage description

  \ ============================================================
  section( UDGs and fonts)  \ {{{1

768 constant /font
  \ Bytes for font (characters 32..127, 8 bytes each).

16 /udg * constant /spanish-chars
  \ Bytes needed by the 16 Spanish chars UDG set

rom-font value graph1-font
rom-font value graph2-font
rom-font value sticks-font
rom-font value twisty-font
       0 value sticks-font-es-udg
       0 value twisty-font-es-udg

here 165 128 - /udg * allot
  \ Reserve data space for the block chars (128..143) and the
  \ BASIC UDG (144..164).

128 /udg * - dup constant graph-udg  set-udg
  \ Point to UDG 0.

need block-chars
  \ Compile the block chars at the UDG data space.

esc-standard-chars-wordlist
esc-block-chars-wordlist
esc-udg-chars-wordlist 3 set-esc-order
  \ Set the escaped strings search order in order to escape not
  \ only the standard chars, but also the block chars and the
  \ UDG chars.

: get-fonts ( -- ) get-font get-udg ;
: set-fonts ( -- ) set-udg set-font ;

  \ 2variable saved-font
  \ : save-font    ( -- ) get-fonts saved-font 2! ;
  \ : restore-font ( -- ) saved-font 2@ set-fonts ;
  \ XXX TODO -- not used yet

: native-font ( -- )
  sticks-font set-font  sticks-font-es-udg set-udg ;
  \ Set the font used for native speech, and the corresponding
  \ UDG set with the Spanish chars.

: text-font ( -- )
  twisty-font set-font  twisty-font-es-udg set-udg ;
  \ Set the font used for ordinary texts, and the corresponding
  \ UDG set with the Spanish chars.

: graphic-udg-set ( -- ) graph-udg set-udg ;
  \ Set the graphic UDG set.

: graphics-1 ( -- )
  graph1-font set-font  graphic-udg-set ;
  \ Set the first graphic font and the graphic UDG set.

: graphics-2 ( -- )
  graph2-font set-font  graphic-udg-set ;
  \ Set the second graphic font and the graphic UDG set.

  \ ============================================================
  section( Windows)  \ {{{1

2 3 28 19 window constant intro-window

1 17 30 3 window constant message-window

0 21 32 3 window constant panel-window

16 5 11 6 window constant native-window

7 12 10 7 window constant sailor-window

12 cconstant sailor-window-cols

7 12 sailor-window-cols 7 window constant sailor-window

1 2 30 14 window constant end-window

  \ ============================================================
  section( Screen)  \ {{{1

: init-screen ( -- )
  default-colors  black border  blackout  graphics-1 ;

16384 constant screen  6912 constant /screen
  \ Address and size of the screen.

farlimit @ /screen - dup constant screen-backup  farlimit !

far-banks 3 + c@ cconstant screen-backup-bank

: move-screen ( ca1 ca2 -- ) /screen cmove  default-bank ;

: save-screen ( -- )
  screen-backup-bank bank screen screen-backup move-screen ;

: restore-screen ( -- )
  screen-backup-bank bank screen-backup screen move-screen ;

  \ ============================================================
  section( Text output)  \ {{{1

: dot ( ca1 len1 --  ca2 len2 ) s" ." s+ ;

: native-pause ( len -- ) 10 / 2 max ?seconds ;

: native-says ( ca len -- )
  tuck get-fonts 2>r
  native-font native-window current-window !
  [ yellow papery ] cliteral attr! wcls wltype
  2r> set-fonts native-pause ;

: wipe-message ( -- )
  message-window current-window !
  [ white black papery + ] cliteral attr!  wcls ;

: message ( ca len -- ) text-font wipe-message wltype ;

  \ ============================================================
  \ section( Sound ) \ {{{1

  \ ============================================================
  section( User input)  \ {{{1

: click ( -- ) 35 0 beep ;
  \ Sound of a pressed key.

: get-digit ( n1 n2 -- n3 )
  begin   2dup key '0' - dup >r -rot between 0=
  while   rdrop
  repeat  2drop r> click ;
  \ Wait for a digit key press, until its value is between _n1_
  \ and _n2_, then return it as _n3_.
  \ XXX TODO -- better sound for fail

  \ ============================================================
  section( Command panel)  \ {{{1

21 constant panel-y
 3 constant panel-rows

: reef? ( n -- f ) sea far@ reef = ;
  \ Is there a reef at sea map position _n_?

: sail-to? ( n -- f ) ship-loc @ + reef? 0= ;
  \ Is it possible to sail to offset location _n_?

: to-north ( -- n ) aboard? if   sea-length
                            else island-length then ;

: to-south ( -- n ) to-north negate ;

 1 constant to-east
-1 constant to-west

: sail-north? ( -- f ) to-north sail-to? ;
: sail-south? ( -- f ) to-south sail-to? ;
: sail-east?  ( -- f )  to-east sail-to? ;
: sail-west?  ( -- f )  to-west sail-to? ;

: coast? ( n -- f ) island far@ coast = ;
  \ Does cell _n_ of the island is coast?

: walk-to? ( n -- f ) crew-loc @ + coast? 0= ;
  \ Is it possible to walk to offset location _n_?

: walk-north? ( -- f ) to-north walk-to? ;
: walk-south? ( -- f ) to-south walk-to? ;
: walk-east?  ( -- f )  to-east walk-to? ;
: walk-west?  ( -- f )  to-west walk-to? ;

: north? ( -- f )
  aboard? if sail-north? else walk-north? then ;

: south? ( -- f )
  aboard? if sail-south? else walk-south? then ;

: east? ( -- f )
  aboard? if sail-east? else walk-east? then ;

: west? ( -- f )
  aboard? if sail-west? else walk-west? then ;

  \ XXX TODO -- use an execution table instead, accessible with
  \ a combination of words

: .direction ( c col row f -- )
  inverse at-xy emit 0 inverse ;

: compass ( -- )
  'N' 30   panel-y               north?  .direction
  'O' 29 [ panel-y 1+ ] cliteral west?   .direction
  'E' 31 [ panel-y 1+ ] cliteral east?   .direction
  'S' 30 [ panel-y 2+ ] cliteral south?  .direction
  '+' 30 [ panel-y 1+ ] cliteral at-xy emit ;
  \ Print the compass of the panel.
  \
  \ XXX TODO -- use a modified  version of "+"?

: feasible-island-attack? ( -- f )
  crew-loc @ island far@
  dup snake           = if >true exit then
  dup native-village  = if >true exit then
  dup hostile-native  = if >true exit then
  dup native-ammo     = if >true exit then
  dup native-supplies = if >true exit then
      >false ;

  \ XXX TODO -- Use `?dup ?exit` instead of `if >true` and
  \ write a wrapper word to do `nip` after the call.

: feasible-sea-attack? ( -- f )
  ship-loc @ sea far@ dup >r 13 <
                           r@ shark = or
                           r> treasure-island = or  0=
  ammo @ 0<> and ;
  \ XXX TODO -- rewrite: use presence of the enemy ship, which
  \ now is associated with certain locations but should be
  \ independent

: feasible-attack? ( -- f )
  aboard? if   feasible-sea-attack?
          else feasible-island-attack? then ;

: common-commands ( -- )
  0 panel-y at-xy s" Información" 0 >option$ type cr
                  s" Tripulación" 0 >option$ type cr
                  s" Puntuación"  0 >option$ type
  feasible-attack? >r
  16 panel-y at-xy s" Atacar" 0 r> ?>option$ type ;

: feasible-disembark? ( -- f )
  ship-loc @ visited far@ 0=
  ship-loc @ sea far@ treasure-island =  or ;
  \ XXX TODO -- not if an enemy ship is present

: ship-commands ( -- )
  feasible-disembark? >r
  16 [ panel-y 1+ ] cliteral at-xy
  s" Desembarcar" 0 r> ?>option$ type ;

: feasible-trade? ( -- f )
  crew-loc @ island far@ native-village = ;

' true alias feasible-embark? ( -- f )
  \ XXX TODO -- only if `crew-loc` is the
  \ disembarking position

: island-commands ( -- )
  feasible-embark? >r
  16 [ panel-y 1+ ] cliteral at-xy
  s" emBarcar" 2 r> ?>option$ type
  feasible-trade? >r
  16 [ panel-y 2+ ] cliteral at-xy
  s" Comerciar" 0 r> ?>option$ type ;

: wipe-panel ( -- )
  [ panel-y attr-line    ] literal
  [ panel-rows columns * ] xliteral erase ;

white black papery + constant panel-attr

: panel-commands ( -- )
  text-font panel-attr attr!
  common-commands aboard? if   ship-commands
                          else island-commands
                          then compass ;

: panel ( -- ) wipe-panel panel-commands ;

  \ ============================================================
  section( Landscape graphics)  \ {{{1

variable west-cloud-x  4 constant /west-cloud
variable east-cloud-x  3 constant /east-cloud

: new-clouds ( -- )
   1  9 random-between west-cloud-x !
  13 21 random-between east-cloud-x ! ;

: sun ( -- )
  26 dup 0 at-xy ." AB"  1 at-xy ." CD" ;

: clouds ( -- )
  west-cloud-x @ dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  east-cloud-x @ dup 0 at-xy ." MNO"  1 at-xy ." PQR" ;

: sun-and-clouds ( b -- )
  graphics-2
  [ yellow cyan papery + ] cliteral over or attr! sun
  [ white  cyan papery + ] cliteral      or attr! clouds
  graphics-1 ;
  \ Draw the sun and the clouds, using _b_ as an attribute
  \ mask: the bits set in _b_ will be set in the attributes.
  \ This is used to set bright on or off.

: color-sky ( c -- )
  [ sky-top-y attr-line  ] literal
  [ sky-rows columns *   ] cliteral rot fill ;
  \ Color the sky with attribute _c_.

2 cconstant /wave
  \ Max chars of a wave.

: wave-coords ( -- x y )
  [ columns /wave - ] cliteral random
  [ arena-top-y     ] cliteral
  [ arena-bottom-y  ] cliteral random-between ;
  \ Return random coordinates _x y_ for a sea wave.

: at-wave-coords ( -- ) wave-coords  at-xy ;
  \ Set the cursor at random coordinates for a sea wave.

: waves ( -- )
  graphics-1 [ cyan blue papery + ] cliteral attr!
  15 0 do  at-wave-coords ." kl"  at-wave-coords ." mn"
  loop ;

cyan dup papery + brighty constant sunny-sky-attr

: sunny-sky ( -- )
  sunny-sky-attr color-sky bright-mask sun-and-clouds ;
  \ Make the sky sunny.

: color-arena ( c -- )
  [ arena-top-y attr-line ] literal
  [ arena-rows columns *  ] xliteral rot fill ;
  \ Color the arena with attribute _c_.

: wipe-arena ( -- ) 0 arena-top-y at-xy
                    [ arena-rows columns * ] xliteral spaces ;
  \ Overwrite the arena with spaces.

: wipe-sea ( -- ) [ blue dup papery + ] cliteral color-arena ;

: new-sunny-sky ( -- ) new-clouds sunny-sky ;

: (sea-and-sky) ( -- ) wipe-sea waves new-sunny-sky ;

: sea-and-sky ( -- ) graphics-1 (sea-and-sky) ;

  \ ============================================================
  section( Sea graphics)  \ {{{1

  \ --------------------------------------------
  section(   -Palms)  \ {{{2

: palm-top ( x y -- x' y' )
  2dup    at-xy ." OPQR"
  2dup 1+ at-xy ." S TU" ;

: palm-trunk ( x y -- x' y' )
  1+ 2dup at-xy 'N' emit
  1+ 2dup at-xy 'M' emit
  1+ 2dup at-xy 'L' emit ;

: palm1 ( x y -- )
  [ green  blue papery + ] cliteral attr!  palm-top
  [ yellow blue papery + ] cliteral attr!
  1 under+  palm-trunk 2drop ;
  \ Print palm model 1 at characters coordinates _x y_.

: palm2 ( x y -- )
  [ green yellow papery + ] cliteral attr!  palm-top
  [ black yellow papery + ] cliteral attr!
  1 under+  palm-trunk 1+ at-xy 'V' emit ;
  \ Print palm model 2 at characters coordinates _x y_.

  \ --------------------------------------------
  section(   -Islands)  \ {{{2

: .big-island5 ( -- )
  [ green blue papery + ] cliteral attr!
  18  7 at-xy ." HI A"
  17  8 at-xy .\" G\::\::\::\::BC"
  16  9 at-xy .\" F\::\::\::\::\::\::\::D"
  14 10 at-xy .\" JK\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::C" ;

: .big-island4 ( -- )
  [ green blue papery + ] cliteral attr!
  16  7 at-xy ." WXYA"
  14  8 at-xy .\" :\::\::\::\::\::\::C F\::\::D"
  13  9 at-xy .\" :\::\::\::\::\::\::\::\::B\::\::\::E"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
 ;

: .little-island2 ( -- )
  [ green blue papery + ] cliteral attr!
  14  8 at-xy .\" :\::\::C"
  16  7 at-xy 'A' emit
  13  9 at-xy .\" :\::\::\::\::D"
  12 10 at-xy .\" F\::\::\::\::\::E" ;

: .little-island1 ( -- )
  [ green blue papery + ] cliteral attr!
  23  8 at-xy .\" JK\::C"
  22  9 at-xy .\" :\::\::\::\::D"
  21 10 at-xy .\" F\::\::\::\::\::E" ;

: .big-island3 ( -- )
  [ green blue papery + ] cliteral attr!
  21  7 at-xy ." Z123"
  19  8 at-xy .\" :\::\::\::\::\::C"
  18  9 at-xy .\" :\::\::\::\::\::\::\::D"
  15 10 at-xy .\" F\::B\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" JK\::\::\::\::\::\::\::\::\::\::\::\::C" ;

: .big-island2 ( -- )
  [ green blue papery + ] cliteral attr!
  17  7 at-xy ." Z123"
  14  8 at-xy .\" F\::B\::\::\::\::\::C"
  13  9 at-xy .\" G\::\::\::\::\::\::\::\::\::D"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::E" ;

: .big-island1 ( -- )
  [ green blue papery + ] cliteral attr!
  20  7 at-xy ." HI A"
  19  8 at-xy .\" G\::\::B\::\::\::C"
  18  9 at-xy .\" F\::\::\::\::\::\::\::\::D"
  16 10 at-xy .\" JK\::\::\::\::\::\::\::\::\::E" ;

: .two-little-islands ( -- )
  [ green blue papery + ] cliteral attr!
  17  6 at-xy ." WXY  A"
  16  7 at-xy .\" A   A   F\::C"
  15  8 at-xy .\" :\::\x7F :\::\x7F G\::\::\::D"
  14  9 at-xy .\" G\::\::\::D   F\::\::\::\::E"
  13 10 at-xy .\" F\::\::\::\::E" ;

: .far-islands ( -- )
  [ green cyan papery + ] cliteral attr!
  0 2 at-xy ." Z123 HI A Z123 HI A Z123 HI Z123" ;

: .treasure-island ( -- )
  get-fonts 2>r graphics-1
  [ green blue papery + ] cliteral attr!
  16  7 at-xy ." A A   HI"
  13  8 at-xy .\" F\::\::\::B\::\::\::B\::\::B\::\::\::C"
  12  9 at-xy .\" G\::\::\::\::\::\::\::"
              .\" \::\::\::\::\::\::\::\::\::D"
  10 10 at-xy .\" JK\::\::\::\::\::\::\::\::\::"
              .\" \::\::\::\::\::\::\::\::E"
   9 11 at-xy .\" :\::\::\::\::\::\::\::\::\::\::"
              .\" \::\::\::\::\::\::\::\::\::\::C"
   8 12 at-xy .\" F\::\::\::\::\::\::\::\::\::\::"
              .\" \::\::\::\::\::\::\::\::\::\::\::\::D"
  31 13 at-xy 'E' emit
  [ blue green papery + ] cliteral attr!
   8 13 at-xy ."  HI Z123  HI A  A A  A "
  20 14 at-xy .\" B\::\::\::\::B"
  19 4 palm1  24 4 palm1  14 4 palm1
  [ black green papery + ] cliteral attr!
  22 9 at-xy .\" \T\U"  \ the treasure
  ship-loc @ visited far@ if
    s" Llegas nuevamente a la isla de " island-name$ s+ s" ."
  else
    s" Has encontrado la perdida isla de "
    island-name$ s+ s" ..."
  then  s+ message  2r> set-fonts ;
  \ XXX TODO -- factor

  \ --------------------------------------------
  section(   -Reefs)  \ {{{2

: .south-reef ( -- )
  [ black blue papery + ] cliteral attr!
  2 14 at-xy ."  A  HI   HI       HI  HI  A"
  0 15 at-xy .\" WXY  :\::\::\x7F     Z123     :\::\::\x7F" ;

: .west-reef ( -- )
  [ black blue papery + ] cliteral attr!
   0 4 at-xy 'A' emit   1 6 at-xy ." HI"  0 8 at-xy ." WXY"
  1 11 at-xy 'A' emit  0 13 at-xy ." HI" ;

: .east-reef ( -- )
  [ black blue papery + ] cliteral attr!
  30 4 at-xy ." HI"   28 6 at-xy 'A' emit
  29 7 at-xy ." WXY"  31 9 at-xy 'A' emit ;

: .reefs ( -- )
  sail-north? 0= if .far-islands then
  sail-south? 0= if .south-reef  then
   sail-east? 0= if .east-reef   then
   sail-west? 0= if .west-reef   then ;

  \ --------------------------------------------
  section(   -Ships)  \ {{{2

: .ship-up ( x y -- )
  2dup    at-xy .\" \A\B\C"
  2dup 1+ at-xy .\" \D\E\F"
       2+ at-xy .\" \G\H\I" ;

: .ship-down ( x y -- )
  2dup    at-xy .\" \J\K\L"
  2dup 1+ at-xy .\" \M\N\O"
       2+ at-xy .\" \P\Q\R" ;

: .ship ( -- )
  graphic-udg-set
  [ white blue papery + ] cliteral attr!  ship-x @ ship-y @
  ship-up @ if   .ship-down ship-up off
            else .ship-up   ship-up on  then ;

: .enemy-ship ( -- )
  [ white blue papery + ] cliteral attr!
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."  ab"
                                2dup 1+ at-xy ."  90"
  [ yellow blue papery + ] cliteral attr!
                                     2+ at-xy ." 678" ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: wipe-enemy-ship ( -- )
  blue set-paper
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."    "
                                2dup 1+ at-xy ."    "
                                     2+ at-xy ."    " ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: .boat ( -- )
  [ yellow blue papery + ] cliteral attr!  11 7 at-xy ." <>" ;
  \ XXX TODO -- random coords at empty space

: .shark ( -- )
  [ white blue papery + ] cliteral attr!
  18 13 at-xy .\" \S" ;
  \ XXX TODO -- `emit-udg` is faster
  \ XXX TODO -- random coords at empty space
  \ XXX TODO -- more of them

: scenery-init-enemy-ship ( -- )
  11 enemy-ship-x ! 4 enemy-ship-y ! ;

: sea-picture ( n -- )
  graphics-1  case
   2 of  .big-island5  19 4 palm1                   endof
   3 of  .big-island4
         14 4 palm1  19 4 palm1  24 4 palm1  .shark endof
   4 of  .little-island2  14 4 palm1                endof
   5 of  .little-island1  24 4 palm1                endof
   6 of  .little-island1  24 4 palm1
         .little-island2  14 4 palm1                endof
   7 of  .big-island3  19 4 palm1                   endof
   8 of  .big-island2  14 4 palm1  .shark           endof
   9 of  .big-island1  24 4 palm1                   endof
  10 of  24 4 palm1  .two-little-islands            endof
  11 of  .shark                                     endof
  13 of  24 4 palm1
         .two-little-islands
         scenery-init-enemy-ship  .enemy-ship       endof
  14 of  .big-island1  24 4 palm1
         scenery-init-enemy-ship  .enemy-ship       endof
  15 of  .big-island2  14 4 palm1
         scenery-init-enemy-ship  .enemy-ship       endof
  16 of  .big-island3  19 4 palm1
         scenery-init-enemy-ship  .enemy-ship       endof
  17 of  .little-island2  14 4 palm1  .boat
         .little-island1  24 4 palm1                endof
  18 of  .little-island1  24 4 palm1  .boat         endof
  19 of  .big-island4
         14 4 palm1  19 4 palm1  24 4 palm1
         .boat  .shark                              endof
  20 of  .big-island5  19 4 palm1  .boat            endof
  shark of  .shark                                  endof
  treasure-island of  .treasure-island              endof
  endcase  .reefs ;
  \ XXX TODO -- `12 of` is not in the original
  \ XXX TODO -- use constants
  \ XXX TODO -- simpler, use an execution table

: sea-scenery ( -- )
  graphics-1
  sea-and-sky .ship  ship-loc @ sea far@ sea-picture ;

  \ ============================================================
  section( Crew stamina)  \ {{{1

: dead? ( n -- f ) stamina farc@ 0= ;
  \ Is man _n_ dead?

: somebody-alive ( -- n )
  begin  men random dup dead?  while  drop  repeat ;
  \ Return a random alive man _n_.

: is-injured ( n -- ) -1 over stamina farc+!  dead? alive +! ;
  \ Man _n_ is injured.

: injured ( -- n ) somebody-alive dup is-injured ;
  \ A random man _n_ is injured.

: is-dead ( n -- ) 0 swap stamina farc!  -1 alive +! ;
  \ Man _n_ is dead.

: dead ( -- n ) somebody-alive dup is-dead ;
  \ A random man _n_ is dead.

  \ ============================================================
  section( Run aground)  \ {{{1

: damaged ( min max -- )
  random-between damage @ + max-damage min damage ! ;
  \ Increase the ship damage with random value in a range.

: .run-aground-reefs ( -- )
  [ black blue papery + ] cliteral attr!
  17 10 at-xy ." WXY     A"
  19  6 at-xy ." A   Z123"
   6 11 at-xy ." A   HI"
   5  4 at-xy ." Z123    HI"
   7  8 at-xy .\" H\..I  A" ;

: run-aground-message ( -- )
  s" ¡Has encallado! El barco está " damage$ s+ dot
  message ;

: run-aground-damages ( -- )
  1 2 damaged injured drop  dead drop
    \ XXX TODO -- random number of dead and injured
  -4 -1 random-between morale+! ;

: run-aground ( -- )
  wipe-message  \ XXX TODO -- remove?
  graphics-1
  wipe-sea .far-islands .south-reef .east-reef .west-reef
  white set-ink 14 8 .ship-up .run-aground-reefs
  run-aground-damages run-aground-message
  3 seconds ;

  \ XXX TODO -- improve message, depending on the damage, e.g.
  \ "Por suerte, ..."
  \
  \ XXX TODO -- choose more men, depending on the damage, and
  \ inform about them

  \ ============================================================
  section( Reports)  \ {{{1

white black papery + constant report-attr

: set-report-color ( -- ) report-attr attr! ;

: begin-report ( -- )
  save-screen set-report-color cls text-font ;
  \ Common task at the start of all reports.

s" Pulsa una tecla" far>sconstant press-any-key$

: end-report ( -- )
  set-report-color
  0 row 2+ at-xy press-any-key$ columns type-center-field
  new-key- click restore-screen ;
  \ Common task at the end of all reports.

: .datum ( a -- ) tabulate @ 2 .r cr cr ;

: main-report ( -- )
  begin-report
  0 1 at-xy s" Informe de situación" columns type-center-field
  0 4 at-xy  18 /tabulate c!
  ." Días:"             day        .datum
  ." Hombres:"          alive      .datum
  ." Moral:"            morale     .datum
  ." Provisiones:"      supplies   .datum
  ." Doblones:"         cash       .datum
  ." Munición:"         ammo       .datum
  ." Barcos hundidos:"  sunk-ships .datum
  ." Barco:"            tabulate damage$ 2dup uppers1 type
  end-report ;

 1 cconstant name-x
  \ x coordinate of the crew member name in the crew report
20 cconstant status-x
  \ x coordinate of the crew member status in the crew report

: set-condition-color ( n -- )
  stamina farc@ stamina-attr c@ attr! ;
  \ Set the proper color for the condition of man _n_.

: .crew-member-data ( n -- )
  >r white attr!
  name-x r@ 6 + at-xy r@ name$ type
  r@ set-condition-color
  status-x r@ 6 + at-xy r> condition$ 2dup uppers1 type ;


s" Estado de la tripulación" far>sconstant "crew-report"$
s" Nombre" far>sconstant "name"$
s" Condición" far>sconstant "condition"$

: .crew-report-header ( -- )
  0 1 at-xy "crew-report"$ columns type-center-field
  name-x   4 at-xy "name"$      type
  status-x 4 at-xy "condition"$ type ;

: crew-report ( -- )
  begin-report .crew-report-header
  men 0 do  i .crew-member-data  loop  end-report ;

: final-score ( -- n )
  found-clues @ 1000 *
  day         @  200 * +
  sunk-ships  @ 1000 * +
  trades      @  200 * +
                4000 success? and + ;

: score-report ( -- )
  begin-report
  0 1 at-xy s" Puntuación" columns type-center-field
  0 4 at-xy
  ." Días"            tab day         @ 4 .r ."  x  200" cr cr
  ." Barcos hundidos" tab sunk-ships  @ 4 .r ."  x 1000" cr cr
  ." Negocios"        tab trades      @ 4 .r ."  x  200" cr cr
  ." Pistas"          tab found-clues @ 4 .r ."  x 1000" cr cr
  ." Tesoro"          tab 4000          4 .r             cr cr
  ." Total"           tab ."        "
                      final-score 4 .r  end-report ;
  \ XXX TODO -- add subtotals (use constants)
  \ XXX TODO -- draw a ruler above "Total"

  \ ============================================================
  section( Ship battle)  \ {{{1

: miss-boat ( -- )
  s" Por suerte el disparo no ha dado en el blanco." message ;

: hit-boat ( -- )
  s" La bala alcanza su objetivo. "
  s" Esto desmoraliza a la tripulación." s+ message
  -2 morale+!
  3 4 random-between 1 ?do  injured drop  loop ;
  \ XXX TODO -- inform about how many injured?

: do-attack-boat ( -- )
  -1 ammo+!
  s" Disparas por error a uno de tus propios botes..." message
  2 seconds
  3 random if   miss-boat
           else hit-boat
           then 5 seconds wipe-message ;

: almost-attack-boat ( -- )
  s" Por suerte no hay munición para disparar..." message
  2 seconds
  s" Pues enseguida te das cuenta de que ibas a hundir "
  s" uno de tus botes." s+ message
  5 seconds wipe-message ;

: attack-boat ( -- ) ammo @ if   do-attack-boat
                            else almost-attack-boat then ;

: .sunk-step-0 ( col row -- )
  2dup    at-xy ."    "
  2dup 1+ at-xy ."  ab"
       2+ at-xy ."  90" ;

: .sunk-step-1 ( col row -- )
  2dup 1+ at-xy ."    "
       2+ at-xy ."  ab" ;

: .sunk-step-2 ( col row -- )
       2+ at-xy ."    " ;

: sunk-delay ( -- ) 100 ms ;

: .sunk ( -- )
  graphics-1  [ white blue papery + ] cliteral attr!
  enemy-ship-x @ enemy-ship-y @ 2dup .sunk-step-0 sunk-delay
                                2dup .sunk-step-1 sunk-delay
                                     .sunk-step-2 ;

variable victory
  \ XXX TODO -- remove; use the stack instead

: sunk ( -- )
  .sunk 2 seconds

  \ ship-loc @ sea far@ 13 >=
  \ ship-loc @ sea far@ 16 <= and
  \ if 1 sunk-ships +! victory on then
    \ XXX OLD

  1 sunk-ships +!  victory on

  ship-loc @ sea far@ case
    13 of  10  endof
    14 of   9  endof
    15 of   8  endof
    16 of   7  endof  dup endcase  ship-loc @ sea far! ;
  \ Sunk the enemy ship
  \
  \ XXX FIXME -- The `case` changes the type of location, what
  \ makes the picture different.  This is a problem of the
  \ original game.  The enemy ship must be independent from the
  \ location type.

: .wave ( -- )
  graphics-1 [ cyan blue papery + ] cliteral attr!
  11 30 random-between 1 20 random-between at-xy ." kl" ;

: (move-enemy-ship) ( -- )
  graphics-1
  5 random 1+ enemy-ship-move !
    \ XXX TODO -- use the stack instead of `enemy-ship-move`?

  \ (enemy-ship-move=1 and enemy-ship-x<28)-(enemy-ship-move=2 and enemy-ship-x>18)
    \ XXX OLD -- original expression
  enemy-ship-move @ 1 =  enemy-ship-x @ 28 <  and abs
  enemy-ship-move @ 2 =  enemy-ship-x @ 18 >  and abs -
    \ XXX TODO -- check the adapted expression
  enemy-ship-x +!

  \ (enemy-ship-move=3 and enemy-ship-y<17)-(enemy-ship-move=4 and enemy-ship-y>1)
    \ XXX OLD -- original expression
  enemy-ship-move @ 3 =  enemy-ship-y @ 17 <  and abs
  enemy-ship-move @ 4 =  enemy-ship-y @  1 >  and abs -
    \ XXX TODO -- check the adapted expression
  enemy-ship-y +!

  [ white blue papery + ] cliteral attr!
  enemy-ship-x @    enemy-ship-y @ 2dup 2dup at-xy ."  ab "
                                   1+        at-xy ."  90 "
  [ yellow blue papery + ] cliteral attr!
  enemy-ship-x @ 1- enemy-ship-y @ 2+        at-xy ."  678 "
                                   2dup  1-  at-xy ."    "
                                         3 + at-xy ."    "
  enemy-ship-move @ 5 = if .wave then ;
  \ XXX TODO -- factor
  \
  \ XXX TODO -- reuse `.enemy-ship` and erase only the minimum
  \ part of the sea, depending on the movement direction
  \
  \ XXX TODO -- redraw waves only around the ship

: move-enemy-ship ( -- )

  \ enemy-ship-x @ enemy-ship-y @
  \ 2dup 2dup -1..1 + swap -1..1 + swap 2 d<>
  \ if (move-enemy-ship) then

  (move-enemy-ship)
 ;
  \ XXX TMP --

: .ammo ( -- ) ammo @ 1 .r ;

: .new-ammo ( -- )
  [ white red papery + ] cliteral attr! 21 23 at-xy .ammo ;

: -ammo ( -- ) -1 ammo+! text-font .new-ammo ;

: .ammo-label ( -- )
  text-font [ white red papery + ] cliteral attr!
  10 23 at-xy ." Munición = " .ammo ;

0 value gun-muzzle-y
  \ y coordinate of the cannon ball

: sunk-range? ( n a -- f ) @ dup 2+ between ;
  \ Is _n_ between the cell hold in _a_ and the cell hold in
  \ _a_ plus 2?

: sunk? ( col -- f )
  false   swap enemy-ship-x sunk-range? 0exit
  gun-muzzle-y enemy-ship-y sunk-range? 0exit  0= ;
  \ Is the enemy ship sunk by the cannon ball, which is
  \ at x coordinate _col_?

9 constant cannon-muzzle-x

: cannon-muzzle-fire-coords ( row -- col row1 col row2 )
  cannon-muzzle-x swap 2- 2dup 1- ;

: .cannon-muzzle-fire ( row -- )
  red set-ink
  cannon-muzzle-fire-coords at-xy '-' emit at-xy '+' emit ;
  \ Print the fire effect of the cannon muzzle, which is at y
  \ coordinate _row_.

: -cannon-muzzle-fire ( row -- )
  cannon-muzzle-fire-coords at-xy space at-xy space ;
  \ Erase the fire effect of the cannon muzzle, which is at y
  \ coordinate _row_.

: gun>label-y ( n -- row ) 7 * 2+ ;
  \ Convert gun number _n_ (0..2) to its gun label _row_.

: gun>muzzle-y ( n -- row ) gun>label-y 1+ ;
  \ Convert gun number _n_ (0..2) to its gun muzzle _row_.

: -cannon-ball ( -- ) last-column gun-muzzle-y at-xy space ;
  \ Erase the cannon ball at the end of its trajectory.

: fire ( n -- )
  graphics-1  blue set-paper
  gun>muzzle-y dup .cannon-muzzle-fire to gun-muzzle-y  -ammo
  move-enemy-ship
  black set-ink cannon-muzzle-x gun-muzzle-y at-xy ."  j"
  gun-muzzle-y -cannon-muzzle-fire
  last-column cannon-muzzle-x do
    i gun-muzzle-y at-xy ."  j"
    i sunk? if sunk unloop exit then
  loop  -cannon-ball ;

: no-ammo-left ( -- )
  panel-commands
  s" Te quedaste sin munición." message  4 seconds ;
  \ XXX TODO -- the enemy wins; our ship sinks,
  \ or the money and part of the crew are captured

: .gun ( col row -- )
  2dup    at-xy ." cde"
       1+ at-xy ." fg" ;
  \ Print a ship gun at _col row_.

: .gun-man ( col row -- )
  2dup 1- at-xy '1' emit
  2dup    at-xy '2' emit
       1+ at-xy '3' emit ;
  \ Print a ship gun man at _col row_.

: battle-init-enemy-ship ( -- )
  20 enemy-ship-x ! 6 enemy-ship-y ! ;
  \   20 random 1+    enemy-ship-x !
  \  11 30 random-between enemy-ship-y ! ;
  \ XXX TODO --

: deck ( -- )
  text-font 22 0 do  0 i at-xy  ." ________ "  loop ;
  \ XXX TODO -- faster

: guns ( -- )
  3 0 do
    i gun>label-y
    white set-paper text-font 0 over at-xy i 1+ 1 .r
    yellow set-paper
    graphics-2  1+ 4 over .gun-man
    graphics-1     6 over .gun
                             1 swap 1+ at-xy ." hi"  \ ammo
  loop ;
  \ XXX TODO -- factor

: clear-for-action ( -- )
  [ black yellow papery + ] cliteral attr! deck guns ;

: battle-scenery ( -- )
  [ blue papery ] cliteral attr-cls 31 1 do  .wave  loop
  clear-for-action .ammo-label battle-init-enemy-ship ;

  \ : trigger ( -- )
  \   inkey case  '1' of  0 fire  endof
  \               '2' of  1 fire  endof
  \               '3' of  2 fire  endof  endcase ;
  \
  \ XXX OLD -- First `trigger` method. Very legible, but slow
  \ and big.

  \ here ] drop fire fire fire drop [ constant triggers
  \   \ Execution table of `trigger`. The three valid keys are
  \   \ indexed to `fire` with their corresponding parameter.
  \   \ All the rest execute a `drop` to discard a fake parameter.

  \ : trigger ( -- ) inkey '0' max '4' min '0' - ( 0..4 ) dup 1-
  \                  swap ( -1..3 0..4 ) cells triggers + perform ;
  \
  \ XXX OLD -- Second `trigger` method. It saves 21 bytes of
  \ data/code space from the first method and it's faster.

: trigger ( -- ) 
  inkey dup '0' '3' between if '0' - fire else drop then ;
  \
  \ XXX NEW -- Third `trigger` method. It saves 42 bytes of
  \ data/code space from the first version and it's faster.

: end-of-battle? ( -- f ) victory @ ammo @ 0= or ;

: (ship-battle) ( -- )
  battle-scenery  victory off
  begin  move-enemy-ship trigger end-of-battle?  until
  ammo @ ?exit no-ammo-left ;

: ship-battle ( -- )
  save-screen (ship-battle) restore-screen ;

: enemy-ship-here? ( -- f )
  ship-loc @ sea far@ 13 16 between ;

: (attack-ship) ( -- )
  enemy-ship-here? if   ship-battle
                   else attack-boat then ;

: attack-ship ( -- ) ammo @ if   (attack-ship)
                            else no-ammo-left then ;

  \ ============================================================
  section( Island map)  \ {{{1

: erase-island ( -- ) 0 island /island cells farerase ;

: is-coast ( n -- ) coast swap island far! ;
  \ Make cell _n_ of the island map be coast.

: (make-coast) ( n1 n2 -- ) bounds do  i is-coast  loop ;
  \ Make _n2_ cells of the island map, starting from cell_ n1_,
  \ be coast.

: make-north-coast ( -- )
  [ /island island-length - ] xliteral island-length
  (make-coast) ;

: make-south-coast ( -- ) 0 island-length (make-coast) ;

: make-east-coast ( -- ) 6 is-coast 12 is-coast 18 is-coast ;
  \ XXX TODO -- generalize for any size of island

: make-west-coast ( -- )
  11 is-coast 17 is-coast 23 is-coast ;
  \ XXX TODO -- generalize for any size of island

: make-coast ( -- ) make-north-coast make-west-coast
                      make-south-coast make-east-coast ;

: location-random-type ( -- n )
  dubloons-here just-3-palms-2 random-between ;

: populate-island ( -- )
  23 7 do  i island far@ coast <>
           if location-random-type i island far! then
  loop
  native-village  19 22 random-between island far!
  native-ammo     13 16 random-between island far!
  native-supplies  7 10 random-between island far! ;
  \ XXX TODO -- improve: adapt to any size:
  \ choose any free non-coast location

: set-crew-loc ( -- )
  7 10 random-between crew-loc ! ;
  \ XXX TODO -- improve: choose a random location on the coast,
  \ except the village

: new-island ( -- )
  erase-island make-coast populate-island ;

  \ ============================================================
  section( Treasure quest)  \ {{{1

          1 4 2constant path-range
          1 4 2constant tree-range
0 villages 1- 2constant village-range
          1 2 2constant turn-range
          1 4 2constant direction-range
          1 9 2constant pace-range

sailor-window-cols 2+ 8 * 4 +
  cconstant sailor-speech-balloon-width

: sailor-speech-balloon ( -- )
  25 44 plot 20 10 rdraw 0  30 rdraw   2  2 rdraw
             sailor-speech-balloon-width 0 rdraw
              2 -2 rdraw 0 -60 rdraw  -2 -2 rdraw
             sailor-speech-balloon-width negate 0 rdraw
             -2  2 rdraw 0  19 rdraw -20  0 rdraw ;

: captain-speech-balloon ( -- )
  220 44 plot -15  5 rdraw 0  20 rdraw -2  2 rdraw -30 0 rdraw
               -2 -2 rdraw 0 -40 rdraw  2 -2 rdraw  30 0 rdraw
                2  2 rdraw 0  14 rdraw 15  0 rdraw ;

: sailor-and-captain ( -- )
  graphics-1  [ cyan black papery + ] cliteral attr!
  0 17 at-xy ."  xy" 28 at-x ." pq" cr
             ."  vs" 28 at-x ." rs" cr
             ."  wu" 28 at-x ." tu"
  sailor-speech-balloon captain-speech-balloon ;

: sailor-says ( ca len -- )
  text-font white attr!  sailor-window current-window !
  wcls wltype ;

: treasure-found ( -- )
  [ 0 attr-line ] literal [ 3 columns * ] xliteral
  [ cyan dup papery + brighty ] cliteral fill
  [ 4 attr-line ] literal [ 18 columns * ] xliteral
  [ yellow dup papery + ] literal fill
    \ XXX TODO -- factor the coloring
    \ XXX TODO -- use constants
  sunny-sky

  23 7 do  i 5 palm2  5 +loop  3 7 palm2  26 7 palm2

  [ black yellow papery + ] cliteral attr!  8 13 at-xy
  ." pq          xy                  "
  ." rs          vs                  tu      "
  .\" \T\U    wu"
  28 11 palm2  0 11 palm2
  graphics-2  [ blue yellow papery + ] cliteral attr!
  13 17 at-xy .\" l\::m"
    \ XXX TODO -- factor the treasure

  s" ¡Capitán, somos ricos!" message
  4 seconds  graphics-1 ;
  \ XXX TODO -- use this proc instead of happy-end?
  \ XXX TODO -- factor

: clue-tried ( x a -- )
  click  wcls  1 seconds  @ = abs found-clues +! ;
  \ Update the clues found with the given answer _x_ for
  \ clue hold in _a_.

: at-clue ( -- ) 23 15 at-xy ;

: .clue-prompt ( -- ) at-clue '?' emit ;

: .clue ( n -- ) black set-paper  at-clue . ;

: wipe-treasure-island ( -- )
  [ treasure-island-top-y attr-line ] literal
  [ treasure-island-rows columns *  ] xliteral
  [ yellow dup papery +             ] cliteral fill ;

: paths-to-choose ( -- )
  wipe-treasure-island
  graphics-2 [ green yellow papery + ] cliteral attr!
  0 3 at-xy ."  5     6       45     6       5"
  graphics-1 black set-ink
  25 0 do
    i 3 + 3 at-xy .\" :\x7F"
    i 2+  4 at-xy .\" :\::\::\x7F"
    i 1+  5 at-xy .\" :\::\::\::\::\x7F"
    i     6 at-xy .\" :\::\::\::\::\::\::\x7F"
  8 +loop
  text-font [ white red papery + ] cliteral attr!
  0 7 at-xy ."    1       2       3       4    " ;

: try-path ( -- )
  paths-to-choose
  s" ¿Qué camino tomamos, capitán?" sailor-says
  .clue-prompt path-range get-digit
  dup .clue path clue-tried ;

: trees-to-choose ( -- )
  wipe-treasure-island
  [ black yellow papery + ] cliteral attr!
  0 7 at-xy ."  1       2       3       4"
  graphics-1  27 2 do  i 3 palm2  8 +loop ;
  \ XXX TODO -- remove the loop

: try-tree ( -- )
  trees-to-choose
  s" ¿En qué árbol paramos, capitán?" sailor-says
  .clue-prompt tree-range get-digit
  dup .clue tree clue-tried ;

: try-way ( -- )
  \ XXX TODO -- draw tree
  s" ¿Vamos a la izquierda (1) o a la derecha (2), capitán?"
  sailor-says
  .clue-prompt turn-range get-digit
  dup .clue turn clue-tried ;
  \ XXX TODO -- use letters instead of digits

: villages-to-choose ( -- )
  wipe-treasure-island
  [ black yellow papery + ] cliteral attr!
  villages 0 do
    1 13 i odd? and + i 2/ treasure-island-top-y + at-xy
    i dup . village$ type
  loop
  graphics-2
  green set-ink  27 5 at-xy .\" S\::T" 27 6 at-xy ." VUW" ;
  \ XXX TODO -- Factor the hut, perhaps also in `.huts`.

: try-village ( -- )
  villages-to-choose
  s" ¿Qué poblado atravesamos, capitán?" sailor-says
  .clue-prompt village-range get-digit
  dup .clue village clue-tried ;

: try-direction ( -- )
  wipe-treasure-island  \ XXX TODO -- draw something instead
  s" ¿En qué dirección vamos, capitán? (1N 2S 3E 4O)"
  sailor-says
  .clue-prompt direction-range get-digit
  dup .clue direction clue-tried ;
  \ XXX TODO -- use letters instead of digits

: try-steps ( -- )
  wipe-treasure-island  \ XXX TODO -- draw something instead
  s" ¿Cuántos pasos damos, capitán?" sailor-says
  .clue-prompt pace-range get-digit
  dup .clue pace clue-tried ;
  \ XXX TODO -- add range to the message

: clear-for-quest ( -- )
  [ 8 attr-line ] literal [ 14 columns * ] xliteral erase ;

: quest ( -- )
  clear-for-quest
  sailor-and-captain try-path    try-tree      try-way
                     try-village try-direction try-steps ;

: enter-treasure-island ( -- )
  blackout wipe-treasure-island new-sunny-sky
  quest success?
  if   s" ¡Hemos encontrado el oro, capitán!"
  else s" Aquí no hay tesoro alguno, capitán."
  then sailor-says 1 seconds ;
  \ XXX TODO -- factor the two results, add longer texts and
  \ draw pictures.

  \ ============================================================
  section( Island graphics)  \ {{{1

: wipe-island-scenery ( -- )
  [ yellow dup papery + ] cliteral color-arena ;
  \ XXX TODO -- Color only the block occupied by the island.
  \ This will save drawing the blue borders before drawing the
  \ waves.

: north-waves ( -- )
  0 arena-top-y at-xy ."  kl  mn     nm    klk   nm nm n " ;
  \ XXX TODO -- show random waves every time, using a random
  \ 32-chars substring from a main one

: south-waves ( -- )
  0 [ arena-bottom-y 1- ] cliteral at-xy
  ."  kl     mn  mn    kl    kl kl  m"
  ."     mn      klmn   mn m  mn     " ;
  \ XXX TODO -- show random waves every time, using a random
  \ 64-chars substring from a main one

: west-waves ( -- )
  [ arena-top-y arena-rows bounds ] 2literal
  do  0 i at-xy ."   "  loop
  0  4 at-xy   'm' emit
  0  6 at-xy ." mn"
  1  8 at-xy    'l' emit
  0 10 at-xy ." kl"
  0 13 at-xy   'k' emit
  graphics-2  yellow set-ink
  walk-north? 0= if 2  4 at-xy 'A' emit then
  walk-south? 0= if 2 13 at-xy 'C' emit then
  graphics-1 ;
  \ XXX TODO -- factor
  \ XXX TODO -- random waves
  \ XXX TODO -- use constants for the base coordinates

: east-waves ( -- )
  [ arena-top-y arena-rows bounds ] 2literal
  do  30 i at-xy ."   "  loop
  30  4 at-xy   'm' emit
  30  6 at-xy ." mn"
  31  8 at-xy    'l' emit
  30 10 at-xy ." kl"
  31 13 at-xy    'k' emit
  yellow set-ink  graphics-2
  walk-north? 0= if 29  4 at-xy 'B' emit then
  walk-south? 0= if 29 13 at-xy 'D' emit then
  graphics-1 ;
  \ XXX TODO -- factor
  \ XXX TODO -- random waves
  \ XXX TODO -- use constants for the base coordinates

: island-waves ( -- )
  graphics-1  [ white blue papery + ] cliteral attr!
  walk-south? 0= if south-waves then
  walk-north? 0= if north-waves then
   walk-east? 0= if east-waves  then
   walk-west? 0= if west-waves  then ;

: .huts ( -- )
  green set-ink
  6  5 at-xy .\"  S\::T    ST   S\::T"
  6  6 at-xy .\"  VUW    78   VUW   4"
  4  8 at-xy .\" S\::T   S\::T    S\::T S\::T  S\::T "
  4  9 at-xy ." VUW   VUW  4 VUW VUW  VUW"
  4 11 at-xy .\" S\::T    S\::T ST  S\::T S\::T"
  4 12 at-xy ." VUW  4 VUW 78  VUW VUW" ;
  \ XXX TODO -- Random, but specific for every island: Choose a
  \ random number and use its groups of 4 bits as identifiers
  \ of what must be drawn: 3 types of hut and nothing.

: .villagers ( -- )
  black set-ink
  10  6 at-xy ." XYZ"
  17  6 at-xy ." YX"
  26  6 at-xy 'Z' emit
   8  9 at-xy ." ZZ"
  13  9 at-xy 'Y' emit
  24  9 at-xy ." ZX"
   7 12 at-xy 'X' emit
  17 12 at-xy 'Y' emit
  22 12 at-xy 'Z' emit
  26 12 at-xy ." XY" ;
  \ XXX TODO -- random

: .village ( -- )
  graphics-2  yellow set-paper .huts .villagers
  graphics-1 ;

: .native ( -- )
  [ black yellow papery + ] cliteral attr!  8 10 at-xy ."  _ `"
                           8 11 at-xy ." }~.,"
                           8 12 at-xy ." {|\?" ;

: .ammo-gift ( -- )
  [ black yellow papery + ] cliteral attr!
  14 12 at-xy ." hi" ;
  \ XXX TODO -- draw graphics depending on the actual ammount

: .supplies ( -- )
  graphics-2 [ black yellow papery + ] cliteral attr!
  14 12 at-xy ." 90  9099 0009"
  graphics-1 ;
  \ XXX TODO -- draw graphics depending on the actual ammount

: .snake ( -- )
  graphics-2
  [ black yellow papery + ] cliteral attr!  14 12 at-xy ." xy"
  graphics-1 ;

: .dubloons ( n -- )
  get-fonts 2>r graphics-2
  [ black yellow papery + ] cliteral attr!
  12 dup at-xy s" vw vw vw vw vw vw vw vw " drop swap 3 * type
  2r> set-fonts ;
  \ XXX TODO -- use a loop
  \ XXX TODO -- other option: print at random empty places

: island-location ( n -- )
  case
    native-village  of .village                         endof
    dubloons-here   of 4 8 palm2 14 5 palm2             endof
      \ XXX TODO -- print dubloons here
    hostile-native  of 14 5 palm2 25 8 palm2 .native    endof
    just-3-palms-1  of 25 8 palm2  4 8 palm2 16 5 palm2 endof
    snake           of 13 5 palm2  5 6 palm2
                       18 8 palm2 23 8 palm2 .snake     endof
    just-3-palms-2  of 23 8 palm2  4 8 palm2 17 5 palm2 endof
    native-supplies of .supplies .native  16 4 palm2    endof
    native-ammo     of .ammo-gift .native 20 5 palm2    endof
  endcase ;

: current-island-location ( -- )
  crew-loc @ island far@ island-location ;

: island-scenery ( -- )
  graphics-1
  wipe-island-scenery sunny-sky island-waves
  current-island-location ;

  \ ============================================================
  section( Events on an island)  \ {{{1

: marsh ( -- )
  dead name$ s"  se hunde en arenas movedizas." s+ message ;

: swamp ( -- )
  dead name$ s"  se hunde en un pantano." s+ message ;

: spider ( -- )
  s" A " injured name$ s+
  s"  le muerde una araña." s+ message ;

: scorpion ( -- )
  s" A " injured name$ s+ s"  le pica un escorpión." s+
  message ;

: hunger ( -- )
  s" La tripulación está hambrienta." message
  -1 morale+! ;
  \ XXX TODO -- only if supplies are not enough

: thirst ( -- )
  s" La tripulación está sedienta." message
  -1 morale+! ;
  \ XXX TODO -- only if supplies are not enough

: dubloons-found ( n -- )
  dup .dubloons  dup cash+!
  s" Encuentras " rot coins$ s+ dot message ;
  \ Find _n_ dubloons.

: some-dubloons-found ( -- )
  2 5 random-between dubloons-found ;

: no-problem ( -- )
  s" Sin novedad, capitán." message ;
  \ XXX TODO -- improve message
  \ XXX TODO -- rename

: no-danger ( -- )
  s" La zona está despejada, capitán." message ;
  \ XXX TODO -- improve message, depending on the location,
  \ e.g. "no hay moros en la costa"
  \ XXX TODO -- rename

create island-events-table ( -- a ) here

] marsh swamp spider scorpion hunger thirst some-dubloons-found
  no-problem no-problem no-danger no-danger noop noop [

here swap - cell / constant island-events

: island-event ( -- )
  island-events random island-events-table array> perform ;

  \ ============================================================
  section( Enter island location)  \ {{{1

: be-hostile-native ( -- )
  hostile-native crew-loc @ island far! ;

: enter-this-island-location ( n -- )

  case

  snake of
    s" Una serpiente muerde a "
    injured name$ s+ dot message
    \ XXX TODO -- inform if the man is dead
  endof

  hostile-native of
    s" Un nativo intenta bloquear el paso y hiere a "
    injured dup >r name$ s+ s" , que resulta " s+
    r> condition$ s+ dot message
  endof

  dubloons-here of

    1 2 random-between dubloons-found

    just-3-palms-1 crew-loc @ island far!
      \ XXX FIXME -- This changes the type of location, what
      \ makes the picture different.  This is a problem of the
      \ original game.  The dubloons must be independent from
      \ the location type.

  endof

  native-ammo of
    s" Un nativo te da algo de munición." message
    1 ammo+!  be-hostile-native
      \ XXX TODO -- random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `island-location`
  endof

  native-supplies of
    s" Un nativo te da provisiones." message
    1 supplies+!  be-hostile-native
      \ XXX TODO -- random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `island-location`
  endof

  native-village of
    s" Descubres un poblado nativo." message
    \ XXX TODO -- Change the message if the village is visited.
  endof

  just-3-palms-1 of island-event endof

  just-3-palms-2 of island-event endof

  endcase ;

: enter-island-location ( -- )
  wipe-message island-scenery panel-commands
  crew-loc @ island far@ enter-this-island-location ;

  \ ============================================================
  section( Disembark)  \ {{{1

: target-island ( -- )
  31  8 at-xy ':' emit
  27  9 at-xy .\" HI :\::"
  25 10 at-xy .\" F\::\::\::\::\::\::"
  23 11 at-xy .\" JK\::\::\::\::\::\::\::" ;

: disembarking-boat ( -- )
  21 0 do  i 11 at-xy ."  <>" 200 ms  loop ;

: disembarking-scene ( -- )
  graphics-1  (sea-and-sky)
  [ green  blue papery + ] cliteral attr!  target-island
  [ yellow blue papery + ] cliteral attr! disembarking-boat ;

: on-treasure-island? ( -- f )
  ship-loc @ sea far@ treasure-island = ;

: enter-ordinary-island ( -- )
  new-island set-crew-loc wipe-panel enter-island-location ;

: enter-island ( -- )
  aboard off
  on-treasure-island? if   enter-treasure-island
                      else enter-ordinary-island then ;

: disembark ( -- ) -2 -1 random-between supplies+!
                   wipe-message wipe-panel
                   disembarking-scene enter-island ;

  \ ============================================================
  section( Storm)  \ {{{1

2 constant rain-y

: at-rain ( a -- ) @ rain-y at-xy ;

: at-west-rain ( -- ) west-cloud-x at-rain ;

: at-east-rain ( -- ) east-cloud-x at-rain ;

: rain-drops ( c -- )
  dup  at-west-rain /west-cloud emits
       at-east-rain /east-cloud emits  60 ms ;

variable storming  storming off
  \ Flag, activated during the storm.

: ship-rocks? ( -- f ) storming @ 0= 128 and 3 + random 0= ;

: ?.ship ( -- ) ship-rocks? if .ship then ;

: +storm ( -- )
  storming on  graphics-1
  70 0 do  [ white cyan papery + ] cliteral attr!
           ';' rain-drops  ']' rain-drops  '[' rain-drops
           ?.ship
  loop ;
  \ Make the rain effect.
  \ XXX TODO -- random duration
  \ XXX TODO -- sound effects
  \ XXX TODO -- lightnings
  \ XXX TODO -- make the enemy ship move, if present
  \ (use the same graphic of the player ship)
  \ XXX TODO -- move the waves

cyan dup papery + constant stormy-sky-attr

: -storm ( -- ) stormy-sky-attr attr!
                at-west-rain /west-cloud spaces
                at-east-rain /east-cloud spaces  storming off ;
  \ Erase the rain effect.
  \ Note the sky keeps the stormy color.
  \ XXX TODO -- improve: make the sky sunny after some time

: stormy-sky ( -- ) stormy-sky-attr color-sky
                      0 sun-and-clouds ;
  \ Make the sky stormy.
  \ XXX TODO -- hide the sun

: damages ( -- ) 1 4 damaged ;

: storm-warning ( -- )
  s" De pronto se desata una fuerte tormenta..." message ;

: storm-report ( -- )
  s" Cuando la mar y el cielo se calman, "
  s" compruebas el estado del barco: " s+ damage$ s+ dot
  message ;

: storm ( -- ) stormy-sky wipe-panel storm-warning
                 +storm damages -storm storm-report panel ;

: storm? ( -- f ) 8912 random 0= ;

: ?storm ( -- ) storm? if storm then ;

  \ ============================================================
  section( Ship command)  \ {{{1

: to-reef? ( n -- f ) ship-loc @ + reef? ;
  \ Does the sea movement offset _n_ leads to a reef?

: (sail) ( n -- ) ship-loc +! sea-scenery panel-commands ;

: sail ( n -- ) dup to-reef? if   drop run-aground
                             else (sail) then ;
  \ Move on the sea map, using offset _n_ from the current
  \ position.

: sail-north ( -- ) to-north sail ;
: sail-south ( -- ) to-south sail ;
: sail-east  ( -- )  to-east sail ;
: sail-west  ( -- )  to-west sail ;

: ship-command? ( c -- f )
  lower case
  'n' key-up    or-of north? dup if click sail-north then endof
  's' key-down  or-of south? dup if click sail-south then endof
  'e' key-right or-of east?  dup if click sail-east  then endof
  'o' key-left  or-of west?  dup if click sail-west  then endof
  'i'              of click main-report true              endof
  'a'              of feasible-sea-attack? dup
                      if click attack-ship then           endof
  't'              of click crew-report true              endof
  'p'              of click score-report true             endof
  'd'              of feasible-disembark? dup
                      if click disembark then             endof
  'f'              of click quit-game on true             endof
  'q'              of click quit                          endof
    \ XXX TMP -- 'q' option for debugging
  false swap  endcase ;
  \ If character _c_ is a valid ship command, execute it and
  \ return true; else return false.
  \
  \ XXX TODO -- use execution table instead? better yet:
  \ `thiscase` structure.

: ship-command ( -- )
  begin ?.ship ?storm inkey ship-command? game-over? or until ;

  \ ============================================================
  section( Misc commands on the island)  \ {{{1

: embark ( -- )
  true ship-loc @ visited far! 1 day +!  aboard on
  sea-scenery panel ;
  \ XXX TODO -- Improve transition with a blackout, instead of
  \ clearing the scenery and the panel apart. There are other
  \ similar cases.

: to-land? ( n -- f ) crew-loc @ + coast? 0= ;
  \ Does the island movement offset _n_ leads to land?

: walk ( n -- )
  dup to-land? if   crew-loc +! enter-island-location
               else drop then ;
  \ Move on the sea map, using offset _n_ from the current
  \ position, if possible.
  \
  \ XXX TODO -- make the movement impossible on the panel if it
  \ leads to the sea, or show a warning

  \ ============================================================
  section( Clues)  \ {{{1

: path-clue$ ( -- ca len )
  s" Tomar camino " path @ number$ s+ dot ;

: tree-clue$ ( -- ca len )
  s" Parar en árbol " tree @ number$ s+ dot ;

: turn-clue$ ( -- ca len )
  s" Ir a " turn @ hand$ s+ s"  en árbol." s+ ;

: village-clue$ ( -- ca len )
  s" Atravesar poblado " village @ village$ s+ dot ;

: direction-clue$ ( -- ca len )
  s" Ir " direction @ cardinal$ s+ s"  desde poblado." s+ ;

: steps-clue$ ( -- ca len )
  s" Dar " pace @ number$ s+ s"  paso" s+
  s" s " pace @ 1 > and s+ s" desde poblado." s+ ;

create clues ( -- a )
] path-clue$    tree-clue$      turn-clue$
  village-clue$ direction-clue$ steps-clue$ [

: clue$ ( -- ca len ) 6 random cells clues + perform ;

: native-tells-clue ( -- )
  s" Bien... Pista ser..." native-says clue$ native-says
  s" ¡Buen viaje a isla de tesoro!" native-says ;

  \ ============================================================
  section( Trading)  \ {{{1

: native-speech-balloon ( -- )
  [ black yellow papery + ] cliteral attr!
  100 100 plot  20 10 rdraw  0 30 rdraw  2 2 rdraw
  100 0 rdraw  2 -2 rdraw  0 -60 rdraw  -2 -2 rdraw
  -100 0 rdraw -2 2 rdraw  0 20 rdraw  -20 0 rdraw ;

: your-offer ( -- n )
  cash @ max-offer min >r ( R: max )
  s" Tienes " cash @ coins$ s+
  s" . ¿Qué oferta le haces? (1-" s+ r@ u>str s+ s" )" s+
  message  1 r> get-digit >r ( R: offer )
  s" Le ofreces " r@ coins$ s+ dot message r> ;
  \ Ask the player for an offer.

: rejected-offer ( -- )
  wipe-message
  s" ¡Tú insultar! ¡Fuera de isla mía!" native-says ;

: accepted-offer ( n -- )
  wipe-message
  negate cash+!  1 trades +!  native-tells-clue  4 seconds ;
  \ Accept the offer of _n_ dubloons.

variable price

: new-price ( -- )
  3 8 random-between dup price ! coins$ 2dup uppers1
  s"  ser nuevo precio, blanco." s+ native-says ;
  \ The native decides a new price.

: lower-price ( -- )
  -3 -2 random-between price +!
  s" Bueno, tú darme... " price @ coins$ s+
  s"  y no hablar más." s+ native-says
  your-offer dup price @ >= if   accepted-offer
                            else rejected-offer then ;
  \ The native lowers the price by several dubloons.

: one-coin-less ( -- )
  your-offer price @ 2dup 1- >= if   drop
                                     accepted-offer exit then
                          1- <  if   rejected-offer
                                else lower-price         then ;
  \ He accepts one dubloon less.

: init-trade ( -- )
  [ yellow yellow papery + ] cliteral dup color-arena
                                          attr! wipe-arena
  [ black yellow papery + ] cliteral attr!
  graphics-1 4 4 palm2  .native
  s" Un comerciante nativo te sale al encuentro." message
  2 ?seconds native-speech-balloon wipe-message ;

: trade ( -- )
  wipe-panel
  init-trade  s" Yo vender pista de tesoro a tú." native-says
  5 9 random-between price !
  s" Precio ser " price @ coins$ s+ dot native-says
  s" ¿Qué dar tú, blanco?" native-says
  your-offer price @ 2dup
  1- >= if drop accepted-offer exit then
    \ One dubloon less is accepted.
  4 - <= if rejected-offer exit then
    \ A too low offer is rejected.
  \ You offered too few:
  4 random case 0 of  lower-price             exit  endof
                1 of  new-price one-coin-less exit  endof
           endcase

  -1 price +!
  s" ¡No! ¡Yo querer más! Tú darme " price @ coins$ s+ dot
  native-says  one-coin-less ;

  \ ============================================================
  section( Attack)  \ {{{1

  \ : impossible ( -- )
  \   s" Lo siento, capitán, no puede hacer eso." message
  \   2 seconds ;
  \ XXX not used yet

: .black-flag ( -- )
  get-fonts 2>r graphics-2
  [ black yellow papery + ] cliteral attr!
  14 10 do  8 i at-xy ." t   "  loop
                           8  9 at-xy  'u' emit
              white attr!  9 10 at-xy ." nop"
                           9 11 at-xy ." qrs"
  2r> set-fonts ;
  \ XXX TODO -- faster: no loop, use "tnop" and "tqrs"

: -native ( -- )
  just-3-palms-1 crew-loc @ island far!  .black-flag ;
  \ XXX TODO -- improve -- don't change the scenery:
  \ first, make natives, animals and things independent from
  \ the location

: hard-to-kill-native ( -- )
  -native
  s" El nativo muere, pero antes mata a "
  dead @ name$ s+ dot message ;

: dead-native-has-supplies ( -- )
  -native
  s" El nativo tiene provisiones "
  s" escondidas en su taparrabos." s+ message  1 supplies+! ;

: dead-native-has-dubloons ( -- )
  -native
  2 3 random-between >r
  s" Encuentras " r@ coins$ s+
  s"  en el cuerpo del nativo muerto." s+ message r> cash+! ;

: attack-native-anyway ( -- )
  5 random case          0 of  hard-to-kill-native       endof
                         1 of  dead-native-has-supplies  endof
                   default-of  dead-native-has-dubloons  endof
  endcase ;

: attack-native-but-snake-kills ( -- )
  s" Matas al nativo, pero la serpiente mata a "
  dead name$ s+ dot message -native ;

: attack-native-village ( -- )
  s" Un poblado entero es un enemigo muy difícil. "
  dead name$ s+ s"  muere en el combate." s+ message ;

: attack-native-there ( n -- )
  case  snake          of  attack-native-but-snake-kills  endof
        native-village of  attack-native-village          endof
               default-of  attack-native-anyway           endof
  endcase ;

: attack-native ( -- )
  crew-loc @ island far@ attack-native-there ;
  \ XXX TODO -- Sound effect.

  \ ============================================================
  section( Command dispatcher on the island)  \ {{{1

: walk-north ( -- ) to-north walk ;
: walk-south ( -- ) to-south walk ;
: walk-east  ( -- )  to-east walk ;
: walk-west  ( -- )  to-west walk ;

: island-command? ( c -- f )
  lower case
    'n' key-up    or-of north? dup
                        if click walk-north    then endof
    's' key-down  or-of south? dup
                        if click walk-south    then endof
    'e' key-right or-of east? dup
                        if click walk-east     then endof
    'o' key-left  or-of west? dup
                        if click walk-west     then endof
    'c'              of feasible-trade? dup
                        if click trade embark  then endof
    'b'              of feasible-embark? dup
                        if click embark        then endof
    'i'              of click main-report      true endof
    'a'              of feasible-island-attack? dup
                        if click attack-native then endof
    't'              of click crew-report      true endof
    'p'              of click score-report     true endof
    'f'              of click quit-game on     true endof
    'q'              of click quit                  endof
      \ XXX TMP -- 'q' option for debugging
  false swap  endcase ;
  \ If character _c_ is a valid command on the island, execute
  \ it and return true; else return false.

: island-command ( -- ) begin key island-command? until ;

  \ ============================================================
  section( Setup)  \ {{{1

: add-row-reefs ( n1 n0 -- ) ?do  reef i sea far!  loop ;

: add-north-reefs ( -- )
  sea-length 0 add-row-reefs ;

: add-south-reefs ( -- )
  [ sea-breadth 1- sea-length * dup sea-length + ]
  xliteral xliteral add-row-reefs ;

: add-col-reefs ( n1 n0 -- )
  ?do  reef i sea far!  sea-length +loop ;

: add-east-reefs ( -- )
  [ sea-breadth 2- sea-length * 1+ ] xliteral sea-length
  add-col-reefs ;

: add-west-reefs ( -- )
  [ sea-length 2* 1-  /sea sea-length - ]
  xliteral xliteral add-col-reefs ;

: add-reefs ( -- ) add-north-reefs add-south-reefs
                     add-east-reefs add-west-reefs ;

: populate-sea ( -- )
  /sea sea-length - sea-length 1+ do
    i reef? 0= if 2 21 random-between i sea far! then
  loop
  treasure-island 94 104 random-between sea far! ;
  \ XXX TODO -- 21 is shark; these are picture types

: -/sea ( a -- ) /sea cells farerase ;
  \ Erase a sea map array _a_ in far memory.

: empty-sea ( -- ) 0 sea -/sea  0 visited -/sea ;

: new-sea ( -- ) empty-sea add-reefs populate-sea ;

: new-ship ( -- )
  32 42 random-between ship-loc !  9 ship-y !  4 ship-x !
  ship-up off ;

: init-clues ( -- )
       path-range random-between      path !
       tree-range random-between      tree !
    village-range random-between   village !
       turn-range random-between      turn !
  direction-range random-between direction !
       pace-range random-between      pace ! ;
  \ XXX TODO -- use `random` for 0..x
  \ XXX TODO -- convert all ranges to 0..x
  \ XXX TODO -- use constant for ranges and reuse them as
  \ parameters of `get-digit`

: new-adventure ( -- )
  init-clues  aboard on  1 crew-loc !
  men alive !  2 ammo !  5 cash !  10 morale !  10 supplies !
  quit-game off  damage off  day off  found-clues off
  sunk-ships off  trades off ;

: unused-name ( -- n )
  0  begin  drop  0 [ stock-names 1- ] xliteral random-between
     dup used-name farc@ 0= until ;
  \ Return the random identifier _n_ of an unused name.

: new-crew-name ( n -- )
  unused-name dup  true swap used-name farc!
  stock-name$ rot name 2! ;
  \ Choose an unused name for crew member _n_.

: new-crew-names ( -- ) new-names
                        men 0 do i new-crew-name loop ;
  \ Choose unused names for the crew members.

: init-crew-stamina ( -- )
  men 0 do  max-stamina i stamina farc!  loop ;
  \ Set the stamina of the crew to its maximum.

: new-crew ( -- ) new-crew-names init-crew-stamina ;

: init ( -- )
  blackout 0 randomize0 text-font
  new-sea new-ship new-crew new-adventure ;

  \ ============================================================
  section( Game over)  \ {{{1

: really-quit ( -- )
  \ Confirm the quit
  \ XXX TODO
 ;

: play-again ( -- )
  \ Play again?
  \ XXX TODO
 ;

: (item ( ca len -- ) s" - " wltype wltype wcr ;

: item ( ca len -- ) wcr (item ;

: sad-end ( -- )
  text-font [ white red papery + ] cliteral attr!
  home s" FIN DEL JUEGO" columns type-center-field
  end-window current-window !
  [ black yellow papery + ] cliteral attr! wcls
  supplies @ 0= if
    s" Las provisiones se han agotado." (item then
  morale @ 0= if
    s" La tripulación se ha amotinado." item then
  ammo @ 0 <= if
    s" La munición se ha agotado." item then
  alive @ 0= if
    s" Toda la tripulación ha muerto." item then
  max-damage? if
    s" El barco está hundiéndose."
    item then
  cash @ 0= if
    s" No te queda dinero." item then ;
  \ XXX REMARK -- All 6 items could not fit the window, but
  \ both items about the crew are exclusive, so the maximum
  \ number of items is 5.


: happy-end ( -- )
  s" Lo lograste, capitán." message ;
  \ XXX TODO --

: the-end ( -- )
  black attr! cls
  success? if happy-end else sad-end then
  s" Pulsa una tecla para ver tu puntuación" message
  new-key- click score-report ;

  \ ============================================================
  section( Intro)  \ {{{1

: skulls ( -- )
  ."   nop  nop  nop  nop  nop  nop  "
  ."   qrs  qrs  qrs  qrs  qrs  qrs  " ;
  \ Draw a row of six skulls.

: skull-border ( -- )
  graphics-2
  home skulls 0 22 at-xy skulls  graphics-1 ;
  \ Draw top and bottom borders of skulls.

s" Viejas leyendas hablan del tesoro "
s" que esconde la perdida isla de " s+ island-name$ s+ dot
far>sconstant intro-text-0$

s" Los nativos del archipiélago recuerdan "
s" las antiguas pistas que conducen al tesoro. " s+
s" Deberás comerciar con ellos para que te las digan." s+
far>sconstant intro-text-1$

s" Visita todas las islas hasta encontrar la isla de "
island-name$ s+ s"  y sigue las pistas hasta el tesoro..." s+
far>sconstant intro-text-2$

: (intro) ( -- )
  skull-border intro-window current-window ! whome
  get-fonts 2>r text-font
  intro-text-0$ wltype wcr wcr
  intro-text-1$ wltype wcr wcr
  intro-text-2$ wltype wcr wcr
  0 row 2+ at-xy press-any-key$ columns type-center-field
  2r> set-fonts ;

: paint-screen ( b -- )
  [ attributes ] literal [ /attributes ] literal rot fill ;
  \ Paint the whole screen with attribute _b_.

: intro ( -- )
  blackout black attr!  (intro)
  [ white brighty ] cliteral paint-screen
  -keys 120 ?seconds click ;

  \ ============================================================
  section( Main)  \ {{{1

: scenery ( -- ) blackout aboard? if   sea-scenery
                                  else island-scenery
                                  then panel ;

: command ( -- ) aboard? if   ship-command
                         else island-command then ;

: game ( -- ) scenery begin command game-over? until ;

: run ( -- ) init-screen begin intro init game the-end again ;

  \ ============================================================
  section( Debugging tools [2])  \ {{{1

2variable current-fonts
create current-attr 0 c,

: before-debug ( -- )
  ~~save-xy  get-fonts current-fonts 2! text-font
  attr@ current-attr c!
  [ white red papery + brighty ] cliteral attr! ;

' before-debug ' ~~before-info defer!

: after-debug ( -- )
  attributes columns 2* erase \ hide the info.
  current-fonts 2@ set-fonts
  current-attr c@ attr!
  ~~restore-xy ;

' after-debug ' ~~after-info defer!

: debug-info ( nt line block -- )
  home columns 2* spaces home
  drop ." L#" 2 .r space name>string 28 min type cr
  aboard? if   ." SHIP:" ship-loc ? ship-loc @ sea
          else ." LAND:" crew-loc ? crew-loc @ island
          then far@ . .s click ;
  \ Display the debug information.

' debug-info ' ~~info defer!

variable checkered

: checkered@ ( -- ) checkered @ ;

: +checkered ( -- ) checkered@ inverse ;

: checkered! ( f -- ) 0= checkered ! ;

: -checkered ( -- ) checkered@ checkered! ;

: ship-here? ( col row -- f ) sea-length * + ship-loc @ = ;

: loc-color ( f -- ) if red else white then set-ink ;

: end0 ( -- ) supplies off morale off ammo off alive off
  max-damage damage ! cash off sad-end ;

: .sea ( -- )
  black set-paper cr
  0 sea-breadth 1- do
    checkered@
    sea-length 0 do
      i j ship-here? loc-color
      +checkered j sea-length * i + sea far@ 2 .r
      -checkered
    loop  cr checkered!
  -1 +loop  default-colors ;

: crew-here? ( col row -- f )
  island-length * + crew-loc @ = ;

: .isl ( -- )
  black set-paper cr
  0 island-breadth 1- do
    checkered@
    island-length 0 do
      i j crew-here? loc-color
      +checkered j island-length * i + island far@ 2 .r
      -checkered
    loop  cr checkered!
  -1 +loop  default-colors ;

: x ( -- ) aboard? if .sea else .isl then ;
: n ( -- ) aboard? if sail-north else walk-north then ;
: s ( -- ) aboard? if sail-south else walk-south then ;
: e ( -- ) aboard? if sail-east  else walk-east  then ;
: o ( -- ) aboard? if sail-west  else walk-west  then ;

: .chars ( c1 c0 -- ) do  i emit  loop ;

: .ascii ( -- ) cr 128  32 .chars ;

: .udg   ( -- ) cr 256 128 .chars ;

: .udgau ( -- ) cr 165 144 .chars ;

: .font ( a -- )
  text-font cr ." font: " dup .  set-font .ascii text-font ;

: .graphs ( -- )
  cls graph1-font .font graph2-font .font .udg ;

: .damages ( -- )
  max-damage 1+ 0 ?do cr i . i >damage$ type new-key- loop ;

: ini ( -- ) init-screen init ;

: f1 ( -- ) graphics-1 ;
: f2 ( -- ) graphics-2 ;
: f  ( -- ) rom-font    set-font ;

  \ ============================================================
  section( Graphics)  \ {{{1

  \ Credit:
  \
  \ The graphic fonts and the UDG set are those of the original
  \ "Jolly Roger", by Barry Jones, 1984.
  \
  \ The sticks and twisty fonts were designed by Paul Howard
  \ for Alchemist PD, 1995, and packed into a viewer called
  \ "Fontbox I".

here

256 -                      dup to graph1-font
                   /font + dup to graph2-font
                   /font + dup to sticks-font
                   /font + dup to twisty-font
256 + 128 /udg * - /font + dup to sticks-font-es-udg
          /spanish-chars +     to twisty-font-es-udg

  \ Update the font pointers with addresses relative to the
  \ current data pointer, were the fonts are being compiled.

 \ vim: filetype=soloforth foldmethod=marker
