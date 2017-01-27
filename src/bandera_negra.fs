( bandera-negra )

  \ Bandera negra
  \
  \ A simulation game
  \ for the ZX Spectrum 128
  \ written in Forth with Solo Forth

  \ This game is a translated and improved remake of
  \   "Jolly Roger"
  \   Copyright (C) 1984 Barry Jones / Video Vault ltd.

  \ Copyright (C) 2011,2014,2015,2016,2017 Marcos Cruz (programandala.net)

  \ ============================================================

only forth definitions

wordlist dup constant game-wordlist  dup >order  set-current

: version  ( -- ca len )  s" 0.39.2+201701270054" ;

cr cr .( Bandera Negra) cr version type cr

  \ ============================================================
  cr .( Requirements)  \ {{{1

only forth definitions

  \ --------------------------------------------
  \ cr .(   -Assembler)  \ {{{2

  \ need transient
  \ 2000 2000 transient  need assembler  end-transient
  \ XXX TODO --

  \ --------------------------------------------
  cr .(   -Debugging tools)  \ {{{2

need order  need ~~  need see  need dump  need where

  \ --------------------------------------------
  cr .(   -Definers)  \ {{{2

need alias

  \ --------------------------------------------
  cr .(   -Control structures)  \ {{{2

need case  need or-of  need j  need 0exit  need default-of

  \ --------------------------------------------
  \ cr .(   -Stack manipulation)  \ {{{2

  \ need pick

  \ --------------------------------------------
  cr .(   -Math)  \ {{{2

need >=  need <=  need under+  need between  need 2/
need random-range  need randomize0  need -1..1  need d<>
need odd?

  \ --------------------------------------------
  \ cr .(   -Memory)  \ {{{2

  \ --------------------------------------------
  cr .(   -Time)  \ {{{2

need frames@  need ms  need seconds  need ?seconds

  \ --------------------------------------------
  cr .(   -Data and strings)  \ {{{2

need 2avariable  need avariable    need cavariable  need value

need far>sconstants  need farsconstants  need far,"

need u>str
need uppers1  need s+  need chars>string  need ruler

need s\"  need .\"
need set-esc-order  need esc-standard-chars-wordlist
need esc-block-chars-wordlist  need esc-udg-chars-wordlist

  \ --------------------------------------------
  cr .(   -Printing and graphics)  \ {{{2

need window  need set-window  need wcls  need wtype
need whome

need tab  need type-center  need at-x  need row
need rows  need columns  need last-column
need inverse  need tabulate
need set-udg  need rom-font  need set-font  need get-font

need black   need blue    need red  need green
need cyan    need yellow  need white

need attr!  need papery  need brighty

need rdraw176 ' rdraw176 alias rdraw
need plot176  ' plot176  alias plot
  \ XXX TMP --

  \ --------------------------------------------
  cr .(   -Keyboard)  \ {{{2

need key-left  need key-right  need key-down  need key-up

need get-inkey ' get-inkey alias inkey
  \ XXX TMP --

  \ --------------------------------------------
  cr .(   -Sound)  \ {{{2

need beep

  \ --------------------------------------------

  \ forget-transient
  \ XXX TODO --

game-wordlist  dup >order set-current

  \ ============================================================
  cr .( Debugging tools [1])  \ {{{1

:  ~~h  ( -- )  2 border key drop 1 border  ;
  \ Break point.

'q' ~~quit-key !  ~~resume-key on  22 ~~y !  ~~? off

' default-font ' ~~app-info defer!
  \ Make sure the debug information compiled by `~~` is printed
  \ with the ROM font.

: ?break  ( -- )
  break-key? if  cr ." Aborted!" cr quit  then  ;

defer .debug-info  ( -- )

  \ ============================================================
  cr .( Constants)  \ {{{1

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

: island-name$  ( -- ca len )  s" Calavera"  ;

: ship-name$  ( -- ca len )  s" Furioso"  ;
  \ XXX TODO -- not used yet

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
2 cconstant dubloons-found
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
                          3 cconstant sea-top-y
                         15 cconstant sea-bottom-y
sea-bottom-y sea-top-y - 1+ cconstant sea-rows

                          3 cconstant treasure-island-top-y
                          5 cconstant treasure-island-rows

  \ ============================================================
  cr .( Variables)  \ {{{1

variable quit-game         \ flag

  \ --------------------------------------------
  cr .(   -Plot)  \ {{{2

variable crew-loc         \ player position on the island
variable aboard           \ flag
variable alive            \ counter
variable ammo             \ counter
variable cash             \ counter
variable damage           \ counter
variable day              \ counter
variable morale           \ counter
variable score            \ counter
variable sunk-ships       \ counter
variable supplies         \ counter
variable trades           \ counter

: aboard?  ( -- f )  aboard @  ;

: ammo+!  ( n -- )  ammo @ + 0 max ammo !  ;
  \ Add _n_ to `ammo`, making sure the minimum result is
  \ zero.

: cash+!  ( n -- )  cash @ + 0 max cash !  ;
  \ Add _n_ to `cash`, making sure the minimum result is
  \ zero.

: morale+!  ( n -- )  morale @ + 0 max morale !  ;
  \ Add _n_ to `morale`, making sure the minimum result is
  \ zero.

: supplies+!  ( n -- )  supplies @ + 0 max supplies !  ;
  \ Add _n_ to `supplies`, making sure the minimum result is
  \ zero.

  \ --------------------------------------------
  cr .(   -Ships)  \ {{{2

variable ship-up      \ flag
variable ship-x
variable ship-y
variable ship-loc

variable enemy-ship-move
variable enemy-ship-x
variable enemy-ship-y
variable enemy-ship-loc

  \ --------------------------------------------
  cr .(   -Clues)  \ {{{2

variable found-clues       \ counter

variable path
variable tree
variable village
variable turn
variable direction
variable pace

  \ ============================================================
  cr .( Arrays)  \ {{{1

  \ --------------------------------------------
  cr .(   -Maps)  \ {{{2

/sea     avariable sea
/island  avariable island
/sea     avariable visited    \ flags for islands
  \ XXX TODO -- character arrays in far memory

  \ --------------------------------------------
  cr .(   -Crew)  \ {{{2

men avariable stamina
  \ XXX TODO -- character array in far memory

  \ Crew names are pun funny names in Spanish:

0
  hp@ far," Alfredo Minguero"
  hp@ far," Armando Bronca"
  hp@ far," Borja Monserrano"
  hp@ far," Clemente Cato"
  hp@ far," César Pullido"  \ XXX TODO -- check
  hp@ far," Enrique Sitos"
  hp@ far," Erasmo Coso"
  hp@ far," Felipe Llejo"
  hp@ far," Javi Oneta"
  hp@ far," Javier Nesnoche"
  hp@ far," Jorge Neral"
  hp@ far," Lope Dorreta"
  hp@ far," Lope Lotilla"
  hp@ far," Manolo Pillo"
  hp@ far," Marcos Tilla"
  hp@ far," Melchor Icete"
  hp@ far," Néstor Nillo"
  hp@ far," Néstor Tilla"
  hp@ far," Paco Tilla"
  hp@ far," Pascual Baricoque"
  hp@ far," Pedro Medario"
  hp@ far," Policarpio Nero"
  hp@ far," Ramiro Inoveo"
  hp@ far," Ricardo Borriquero"
  hp@ far," Roberto Mate"
  hp@ far," Rodrigo Minolas"
  hp@ far," Ulises Cocido"
  hp@ far," Ulises Tantería"
  hp@ far," Vicente Nario"
  hp@ far," Vicente Rador"
  hp@ far," Víctor Nillo"
  hp@ far," Víctor Pedo"
  hp@ far," Víctor Tilla"
  hp@ far," Zacarías Queroso"
  hp@ far," Óscar Nicero"
  hp@ far," Óscar Romato"
  hp@ far," Óscar Terista"
farsconstants stock-name$  ( n -- ca len )
      constant stock-names

men 2avariable name  ( n -- a )
  \ A double-cell array to hold the address and length
  \ of the names of the crew members, compiled in `names$`.

: name$  ( n -- ca len )  name 2@ save-farstring  ;

stock-names avariable used-name  ( n -- a )
  \ An array to hold a true flag when the correspondent name
  \ in `names$` has been used in `name`. The goal is to prevent
  \ name duplicates in the crew.
  \
  \ XXX TODO -- Store in far memory.

0
  hp@ far," en forma"
  hp@ far," magullado"
  hp@ far," herido leve"
  hp@ far," herido grave"
  hp@ far," muerto"
far>sconstants stamina$  ( n -- ca len )
   1- constant max-stamina
    0 constant min-stamina

max-stamina 1+ cavariable stamina-attr

 black white papery +         0 stamina-attr c!
   red black papery + brighty 1 stamina-attr c!
   red black papery +         2 stamina-attr c!
yellow black papery +         3 stamina-attr c!
 green black papery +         4 stamina-attr c!

  \ --------------------------------------------
  cr .(   -Ship damage descriptions)  \ {{{2

0
  hp@ far," hundiéndose"            \ worst: sinking
  hp@ far," a punto de hundirse"
  hp@ far," haciendo agua"
  hp@ far," destrozado"
  hp@ far," casi destrozado"
  hp@ far," gravemente dañado"
  hp@ far," muy dañado"
  hp@ far," algo dañado"
  hp@ far," casi como nuevo"
  hp@ far," impecable"            \ best: perfect
far>sconstants damage-level$  ( n -- ca len )
      constant damage-levels

  \ --------------------------------------------
  cr .(   -Village names)  \ {{{2

  \ The names of the villages are Esperanto compound words
  \ whose pronunciation topically resembles African languages,
  \ and have funny meanings.

0
  hp@ far," Mislongo"   \ mis-long-o="wrong lenght"
  hp@ far," Ombreto"    \ ombr-et-o="little shadow"
  hp@ far," Figokesto"  \ fig-o-kest-o="fig basket"
  hp@ far," Misedukota" \ mis-eduk-ot-a="one to be miseducated"
  hp@ far," Topikega"   \ topik-eg-a=
  hp@ far," Fibaloto"   \ fi-balot-o
  hp@ far," Pomotruko"  \ pom-o-truk-o
  hp@ far," Putotombo"  \ put-o-tomb-o="well tomb"
  hp@ far," Ursorelo"   \ urs-orel-="ear of bear"
  hp@ far," Kukumemo"   \ kukum-em-o
far>sconstants village$  ( n -- ca len )
      constant villages

  \ --------------------------------------------
  cr .(   -Cardinal points)  \ {{{2

0
  hp@ far," oeste"
  hp@ far," este"
  hp@ far," sur"
  hp@ far," norte"
far>sconstants cardinal$  ( n -- ca len )  drop

  \ --------------------------------------------
  cr .(   -Hands)  \ {{{2

0
  hp@ far," derecha"    \ right
  hp@ far," izquierda"  \ left
far>sconstants hand$  ( n -- ca len )  drop

  \ ============================================================
  cr .( Functions)  \ {{{1

22528 constant attributes
  \ Address of the screen attributes (768 bytes)

: attr-line  ( l -- a )  columns * attributes +  ;
  \ First attribute address of a character line.
  \ XXX TODO -- move to the Solo Forth library

: dubloons$  ( n -- ca len )
  s" dobl" rot 1 > if  s" ones"  else  s" ón"  then  s+  ;
  \ Return string "doubloon" or "doubloons", depending on _n_.

0
  hp@ far," once"
  hp@ far," diez"
  hp@ far," nueve"
  hp@ far," ocho"
  hp@ far," siete"
  hp@ far," seis"
  hp@ far," cinco"
  hp@ far," cuatro"
  hp@ far," tres"
  hp@ far," dos"
  hp@ far," un"
  hp@ far," cero"
far>sconstants number$  ( n -- ca len )  drop

: highlighted$  ( c -- ca len )
  0 20 rot 1 20 5 chars>string  ;
  \ Convert _c_ to a string to print _c_ as a highlighted char,
  \ using control characters.

: >option$  ( ca1 len1 n -- ca2 len2 )
  >r 2dup drop r@
  2over drop r@ + c@ highlighted$ s+
  2swap r> 1+ /string s+  ;
  \ Convert menu option _ca len_ to an active menu option
  \ with character at position _n_ (0..len1-1) highlighted with
  \ control characters.

: ?>option$  ( ca1 len1 n f -- ca1 len1 | ca2 len2 )
  if  >option$  else  drop  then  ;
  \ Prepare a panel option _ca1 len1_.  If the option is
  \ active, _f_ is true,  _n_ (0..len1-1) is the position of
  \ its highlighted letter, and _ca2 len2_ is the
  \ option with the given letter highlighted. If the option is
  \ not active, _f_ is false, _n_ is ignored and the string
  \ remains unchanged.

: coins$  ( n -- ca len )
  dup >r number$ s"  " s+ r> dubloons$ s+  ;
  \ Return the text "n doubloons", with letters.

100 constant max-damage

: damage-index  ( -- n )
  damage @ damage-levels max-damage */  ;
  \ Return damage index _n_ (0..`damage-levels`) correspondent
  \ to the current value of `damage` (0..`max-damage`).

: max-damage-index?  ( -- f )  damage-index damage-levels =  ;

: failure?  ( -- f )
  alive @ 0=
  morale @ 0= or
  max-damage-index? or
  supplies @ 0= or
  cash @ 0= or  ;
  \ Failed mission?
  \
  \ XXX TODO -- use `0=` instead of `1 <`

6 constant max-clues

: success?  ( -- f )  found-clues @ max-clues =  ;
  \ Success?

: game-over?  ( -- f )  failure? success? quit-game @ or or  ;
  \ Game over?

: condition$  ( n -- ca len )  stamina @ stamina$  ;
  \ Physical condition of a crew member

: blank-line$  ( -- ca len )  bl columns ruler  ;
  \ XXX TODO -- use `emits` instead

: damage$  ( -- ca len )  damage-index damage-level$  ;
  \ Damage description

  \ ============================================================
  cr .( UDGs and fonts)  \ {{{1

768 constant /font
  \ Bytes for font (characters 32..127, 8 bytes each).

rom-font value graph-font1
rom-font value graph-font2
rom-font value sticks-font
rom-font value twisty-font

' twisty-font alias text-font

here set-udg 165 128 - 8 * allot
  \ Reserve data space for the block chars (128..143) and the
  \ UDG (144.164).

need block-chars
  \ Compile the block chars at the UDG data space.

esc-standard-chars-wordlist
esc-block-chars-wordlist
esc-udg-chars-wordlist 3 set-esc-order
  \ Set the escaped strings search order in order to escape not
  \ only the standard chars, but also the block chars and the
  \ UDG chars.

  \ ============================================================
  cr .( Windows)  \ {{{1

2 3 28 19 window intro-window

1 17 30 3 window message-window

0 21 32 3 window panel-window

16 6 11 4 window native-window

7 12 10 7 window sailor-window

5 3 22 20 window the-end-window

  \ ============================================================
  cr .( Screen)  \ {{{1

: init-screen  ( -- )
  default-colors white ink blue paper black border cls
  graph-font1 set-font  ;

16384 constant screen  6912 constant /screen
  \ Address and size of the screen.

farlimit @ /screen - dup constant screen-backup
                         farlimit !

far-banks 3 + c@ cconstant screen-backup-bank

: move-screen  ( ca1 ca2 -- )  /screen cmove  default-bank  ;

: save-screen  ( -- )
  screen-backup-bank bank screen screen-backup move-screen  ;

: restore-screen  ( -- )
  screen-backup-bank bank screen-backup screen move-screen  ;

  \ ============================================================
  cr .( Text output)  \ {{{1

: native-says  ( ca len -- )
  get-font >r sticks-font set-font
  native-window set-window wcls wtype
  r> set-font  ;

: wipe-message  ( -- )
  message-window set-window white ink  black paper  wcls  ;

: message  ( ca len -- )
  text-font set-font wipe-message wtype  ;

  \ ============================================================
  \ cr .( Sound )  \ {{{1

  \ ============================================================
  cr .( User input)  \ {{{1

: get-digit  ( n1 n2 -- n3 )
  begin   2dup key '0' - dup >r -rot between 0=
  while   rdrop 100 10 beep
  repeat  2drop r>  ;
  \ Wait for a digit key press, until its value is between _n1_
  \ and _n2_, then return it as _n3_.
  \ XXX TODO -- better sound for fail

  \ ============================================================
  cr .( Command panel)  \ {{{1

21 constant panel-y
 3 constant panel-rows

variable feasible-disembark      \ flag
variable feasible-embark         \ flag
variable feasible-attack         \ flag
variable feasible-trade          \ flag

: reef?  ( n -- f )  sea @ reef =  ;
  \ Is there a reef at sea map position _n_?

: sail-to?  ( n -- f ) ship-loc @ + reef? 0=  ;
  \ Is it possible to sail to offset location _n_?

: to-north  ( -- n )
  aboard? if  sea-length  else  island-length  then  ;

: to-south  ( -- n )  to-north negate  ;

 1 constant to-east
-1 constant to-west

: sail-north?  ( -- f )  to-north sail-to?  ;
: sail-south?  ( -- f )  to-south sail-to?  ;
: sail-east?   ( -- f )   to-east sail-to?  ;
: sail-west?   ( -- f )   to-west sail-to?  ;

: coast?  ( n -- f )  island @ coast =  ;
  \ Does cell _n_ of the island is coast?

: walk-to?  ( n -- f ) crew-loc @ + coast? 0=  ;
  \ Is it possible to walk to offset location _n_?

: walk-north?  ( -- f )  to-north walk-to?  ;
: walk-south?  ( -- f )  to-south walk-to?  ;
: walk-east?   ( -- f )   to-east walk-to?  ;
: walk-west?   ( -- f )   to-west walk-to?  ;

: north?  ( -- f )
  aboard? if  sail-north?  else  walk-north?  then  ;

: south?  ( -- f )
  aboard? if  sail-south?  else  walk-south?  then  ;

: east?  ( -- f )
  aboard? if  sail-east?  else  walk-east?  then  ;

: west?  ( -- f )
  aboard? if  sail-west?  else  walk-west?  then  ;

  \ XXX TODO -- use an execution table instead, accessible with
  \ a combination of words

: .direction  ( c col row f -- )
  inverse at-xy emit 0 inverse  ;

: compass  ( -- )
  'N' 30   panel-y               north?  .direction
  'O' 29 [ panel-y 1+ ] cliteral west?   .direction
  'E' 31 [ panel-y 1+ ] cliteral east?   .direction
  'S' 30 [ panel-y 2+ ] cliteral south?  .direction
  '+' 30 [ panel-y 1+ ] cliteral at-xy emit  ;
  \ Print the compass of the panel.
  \
  \ XXX TODO -- use a modified  version of "+"?

: feasible-attack?  ( -- f )
  ship-loc @ sea @ dup >r 13 <
                           r@ shark = or
                           r> treasure-island = or  0=
  ammo @ 0<> and  ;
  \ XXX TODO -- rewrite: use presence of the enemy ship, which
  \ now is associated with certain locations but should be
  \ independent

: common-commands  ( -- )
  0 panel-y at-xy s" Información" 0 >option$ type cr
                  s" Tripulación" 0 >option$ type cr
                  s" Puntuación"  0 >option$ type
  feasible-attack? dup >r feasible-attack !
  16 panel-y at-xy s" Atacar" 0 r> ?>option$ type  ;

: feasible-disembark?  ( -- f )
  ship-loc @ visited @ 0=
  ship-loc @ sea @ treasure-island =  or  ;
  \ XXX TODO -- not if an enemy ship is present

: ship-commands  ( -- )
  .debug-info  \ XXX INFORMER
  feasible-disembark? dup >r feasible-disembark !
  16 [ panel-y 1+ ] cliteral at-xy
  s" Desembarcar" 0 r> ?>option$ type  ;

: feasible-trade?  ( -- f )
  crew-loc @ island @ native-village =  ;

' true alias feasible-embark?  ( -- f )
  \ XXX TODO -- only if `crew-loc` is the
  \ disembarking position

: island-commands  ( -- )
  .debug-info  \ XXX INFORMER
  feasible-embark? dup >r feasible-embark !
  16 [ panel-y 1+ ] cliteral at-xy
  s" emBarcar" 2 r> ?>option$ type
  feasible-trade? dup >r feasible-trade !
  16 [ panel-y 2+ ] cliteral at-xy
  s" Comerciar" 0 r> ?>option$ type  ;

: wipe-panel  ( -- )
  [ panel-y attr-line    ] literal
  [ panel-rows columns * ] 1literal erase  ;

white black papery + constant panel-attr

: panel-commands  ( -- )
  text-font set-font panel-attr attr!
  common-commands aboard? if    ship-commands
                          else  island-commands
                          then  compass  ;

: panel  ( -- )  wipe-panel panel-commands  ;

  \ ============================================================
  cr .( Landscape graphics)  \ {{{1

variable west-cloud-x  4 constant /west-cloud
variable east-cloud-x  3 constant /east-cloud

: new-clouds  ( -- )
   1  9 random-range west-cloud-x !
  13 21 random-range east-cloud-x !  ;

: sun  ( -- )
  26 dup 0 at-xy ." AB"  1 at-xy ." CD"  ;

: clouds  ( -- )
  west-cloud-x @ dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  east-cloud-x @ dup 0 at-xy ." MNO"  1 at-xy ." PQR"  ;

: sun-and-clouds  ( -- )
  graph-font2 set-font  cyan paper
  yellow ink sun  white ink clouds
  graph-font1 set-font  ;

: color-sky  ( c -- )
  [ sky-top-y attr-line  ] literal
  [ sky-rows columns *   ] cliteral rot fill  ;
  \ Color the sky with attribute _c_.

2 cconstant /wave
  \ Max chars of a wave.

: wave-coords  ( -- x y )
  [ columns /wave - ] cliteral random
  [ sea-top-y       ] cliteral
  [ sea-bottom-y    ] cliteral random-range  ;
  \ Return random coordinates _x y_ for a sea wave.

: at-wave-coords  ( -- )  wave-coords  at-xy  ;
  \ Set the cursor at random coordinates for a sea wave.

: waves  ( -- )
  graph-font1 set-font cyan ink  blue paper
  15 0 do  at-wave-coords ." kl"  at-wave-coords ." mn"
  loop  ;

cyan dup papery + brighty constant sunny-sky-attr

: sunny-sky  ( -- )  sunny-sky-attr color-sky
                     1 bright sun-and-clouds 0 bright  ;
  \ Make the sky sunny.

: color-sea  ( c -- )
  [ sea-top-y attr-line ] literal
  [ sea-rows columns *  ] 1literal rot fill  ;
  \ Color the sea with attribute _c_.

: wipe-sea  ( -- )  [ blue dup papery + ] cliteral color-sea  ;

: new-sunny-sky  ( -- )  new-clouds sunny-sky  ;

: (sea-and-sky)  ( -- )  wipe-sea waves new-sunny-sky  ;

: sea-and-sky  ( -- )  graph-font1 set-font (sea-and-sky)  ;

  \ ============================================================
  cr .( Sea graphics)  \ {{{1

  \ --------------------------------------------
  cr .(   -Palms)  \ {{{2

: palm-top  ( x y -- x' y' )
  2dup    at-xy ." OPQR"
  2dup 1+ at-xy ." S TU" ;

: palm-trunk  ( x y -- x' y' )
  1+ 2dup at-xy ." N"
  1+ 2dup at-xy ." M"
  1+ 2dup at-xy ." L"  ;

: palm1  ( x y -- )
  green ink  blue paper  palm-top  yellow ink
  1 under+  palm-trunk 2drop  ;
  \ Print palm model 1 at characters coordinates _x y_.

: palm2  ( x y -- )
  green ink  yellow paper  palm-top  black ink
  1 under+  palm-trunk 1+ at-xy ." V"  ;
  \ Print palm model 2 at characters coordinates _x y_.

  \ --------------------------------------------
  cr .(   -Islands)  \ {{{2

: .big-island5  ( -- )
  green ink  blue paper
  18  7 at-xy ." HI A"
  17  8 at-xy .\" G\::\::\::\::BC"
  16  9 at-xy .\" F\::\::\::\::\::\::\::D"
  14 10 at-xy .\" JK\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::C"  ;

: .big-island4  ( -- )
  green ink  blue paper
  16  7 at-xy ." WXYA"
  14  8 at-xy .\" :\::\::\::\::\::\::C F\::\::D"
  13  9 at-xy .\" :\::\::\::\::\::\::\::\::B\::\::\::E"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: .little-island2  ( -- )
  green ink  blue paper  14  8 at-xy .\" :\::\::C"
                         16  7 at-xy ." A"
                         13  9 at-xy .\" :\::\::\::\::D"
                         12 10 at-xy .\" F\::\::\::\::\::E"  ;

: .little-island1  ( -- )
  green ink  blue paper  23  8 at-xy .\" JK\::C"
                         22  9 at-xy .\" :\::\::\::\::D"
                         21 10 at-xy .\" F\::\::\::\::\::E"  ;

: .big-island3  ( -- )
  green ink  blue paper
  21  7 at-xy ." Z123"
  19  8 at-xy .\" :\::\::\::\::\::C"
  18  9 at-xy .\" :\::\::\::\::\::\::\::D"
  15 10 at-xy .\" F\::B\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" JK\::\::\::\::\::\::\::\::\::\::\::\::C"  ;

: .big-island2  ( -- )
  green ink  blue paper
  17  7 at-xy ." Z123"
  14  8 at-xy .\" F\::B\::\::\::\::\::C"
  13  9 at-xy .\" G\::\::\::\::\::\::\::\::\::D"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::E"  ;

: .big-island1  ( -- )
  green ink  blue paper
  20  7 at-xy ." HI A"
  19  8 at-xy .\" G\::\::B\::\::\::C"
  18  9 at-xy .\" F\::\::\::\::\::\::\::\::D"
  16 10 at-xy .\" JK\::\::\::\::\::\::\::\::\::E"  ;

: .two-little-islands  ( -- )
  green ink  blue paper
  17  6 at-xy ." WXY  A"
  16  7 at-xy .\" A   A   F\::C"
  15  8 at-xy .\" :\::\x7F :\::\x7F G\::\::\::D"
  14  9 at-xy .\" G\::\::\::D   F\::\::\::\::E"
  13 10 at-xy .\" F\::\::\::\::E"  ;

: .far-islands  ( -- )
  green ink  cyan paper
  0 2 at-xy ." Z123 HI A Z123 HI A Z123 HI Z123"  ;

: .treasure-island  ( -- )
  get-font >r graph-font1 set-font  green ink  blue paper
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
  31 13 at-xy ." E"
  blue ink  green paper
   8 13 at-xy ."  HI Z123  HI A  A A  A "
  20 14 at-xy .\" B\::\::\::\::B"
  19 4 palm1  24 4 palm1  14 4 palm1
  black ink  green paper
  22 9 at-xy .\" \T\U"  \ the treasure
  ship-loc @ visited @ if
    s" Llegas nuevamente a la isla de " island-name$ s+ s" ."
  else
    s" Has encontrado la perdida isla de "
    island-name$ s+ s" ..."
  then  s+ message  r> set-font  ;
  \ XXX TODO -- factor

  \ --------------------------------------------
  cr .(   -Reefs)  \ {{{2

: .south-reef  ( -- )
  black ink  blue paper
  2 14 at-xy ."  A  HI   HI       HI  HI  A"
  0 15 at-xy .\" WXY  :\::\::\x7F     Z123     :\::\::\x7F"  ;

: .west-reef  ( -- )
  black ink  blue paper
   0 4 at-xy ." A"   1 6 at-xy ." HI"  0 8 at-xy ." WXY"
  1 11 at-xy ." A"  0 13 at-xy ." HI"  ;

: .east-reef  ( -- )
  black ink  blue paper
  30 4 at-xy ." HI"   28 6 at-xy ." A"
  29 7 at-xy ." WXY"  31 9 at-xy ." A"  ;

: .reefs  ( -- )
  sail-north? 0= if  .far-islands  then
  sail-south? 0= if  .south-reef   then
   sail-east? 0= if  .east-reef    then
   sail-west? 0= if  .west-reef    then  ;

  \ --------------------------------------------
  cr .(   -Ships)  \ {{{2

: .ship-up  ( x y -- )
  2dup    at-xy .\" \A\B\C"
  2dup 1+ at-xy .\" \D\E\F"
       2+ at-xy .\" \G\H\I"  ;

: .ship-down  ( x y -- )
  2dup    at-xy .\" \J\K\L"
  2dup 1+ at-xy .\" \M\N\O"
       2+ at-xy .\" \P\Q\R"  ;

: redraw-ship  ( -- )
  white ink blue paper  ship-x @ ship-y @
  ship-up @ if    .ship-down  ship-up off
            else  .ship-up    ship-up on   then  ;

: .enemy-ship  ( -- )
  white ink  blue paper
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."  ab"
                                2dup 1+ at-xy ."  90"
  yellow ink                         2+ at-xy ." 678"  ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: wipe-enemy-ship  ( -- )
  blue paper
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."    "
                                2dup 1+ at-xy ."    "
                                     2+ at-xy ."    "  ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: .boat  ( -- )
  yellow ink  blue paper  11 7 at-xy ." <>"  ;
  \ XXX TODO -- random coords at empty space

: .shark  ( -- )
  white ink  blue paper  18 13 at-xy .\" \S"  ;
  \ XXX TODO -- `emit-udg` is faster
  \ XXX TODO -- random coords at empty space
  \ XXX TODO -- more of them

: scenery-init-enemy-ship  ( -- )
  11 enemy-ship-x ! 4 enemy-ship-y !  ;

: sea-picture  ( n -- )
  graph-font1 set-font  case
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
  endcase  .reefs  ;
  \ XXX TODO -- `12 of` is not in the original
  \ XXX TODO -- use constants
  \ XXX TODO -- simpler, use an execution table

: sea-scenery  ( -- )
  graph-font1 set-font
  sea-and-sky redraw-ship  ship-loc @ sea @ sea-picture  ;

  \ ============================================================
  cr .( Crew stamina)  \ {{{1

: dead?  ( n -- f )  stamina @ 0=  ;
  \ Is man _n_ dead?

: somebody-alive  ( -- n )
  begin  men random dup dead?  while  drop  repeat  ;
  \ Return a random alive man _n_.

: is-injured  ( n -- )  -1 over stamina +!  dead? alive +!  ;
  \ Man _n_ is injured.

: injured  ( -- n )  somebody-alive dup is-injured  ;
  \ A random man _n_ is injured.

: is-dead  ( n -- )  stamina off  -1 alive +!  ;
  \ Man _n_ is dead.

: dead  ( -- n )  somebody-alive dup is-dead  ;
  \ A random man _n_ is dead.

  \ ============================================================
  cr .( Run aground)  \ {{{1

100 constant max-damage

: damaged  ( min max -- )
  random-range damage +!  damage @ max-damage min damage !  ;
  \ Increase the ship damage with random value in a range.

: max-damage?  ( -- f )  damage @ max-damage =  ;

: .run-aground-reefs  ( -- )
  black ink  blue paper
  17 10 at-xy ." WXY     A"
  19  6 at-xy ." A   Z123"
   6 11 at-xy ." A   HI"
   5  4 at-xy ." Z123    HI"
   7  8 at-xy .\" H\..I  A"  ;

: run-aground-message  ( -- )
  s" ¡Has encallado! El barco está " damage$ s+ s" ." s+
  message  ;

: run-aground-damages  ( -- )
  10 29 damaged injured drop  dead drop
    \ XXX TODO -- random number of dead and injured
  -4 -1 random-range morale+!  ;

: run-aground  ( -- )
  wipe-message  \ XXX TODO -- remove?
  graph-font1 set-font
  wipe-sea .far-islands .south-reef .east-reef .west-reef
  white ink 14 8 .ship-up .run-aground-reefs
  run-aground-damages run-aground-message
  3 seconds  ;

  \ XXX TODO -- improve message, depending on the damage, e.g.
  \ "Por suerte, ..."
  \
  \ XXX TODO -- choose more men, depending on the damage, and
  \ inform about them

  \ ============================================================
  cr .( Reports)  \ {{{1

white black papery + constant report-attr

: set-report-color  ( -- )
  report-attr attr! permanent-colors  ;

: begin-report  ( -- )
  save-screen set-report-color cls text-font set-font  ;
  \ Common task at the start of all reports.

: end-report  ( -- )
  set-report-color
  0 row 2+ at-xy s" Pulsa una tecla" columns type-center
  key drop  restore-screen  ;
  \ Common task at the end of all reports.

: .datum  ( a -- )  tabulate @ 2 .r cr cr  ;

: main-report  ( -- )
  begin-report
  0 1 at-xy s" Informe de situación" columns type-center
  0 4 at-xy  18 /tabulate !
  ." Días:"             day        .datum
  ." Hombres:"          alive      .datum
  ." Moral:"            morale     .datum
  ." Provisiones:"      supplies   .datum
  ." Doblones:"         cash       .datum
  ." Munición:"         ammo       .datum
  ." Barcos hundidos:"  sunk-ships .datum
  ." Barco:"            tabulate damage$ 2dup uppers1 type
  end-report  ;

 1 cconstant name-x
  \ x coordinate of the crew member name in the crew report
20 cconstant status-x
  \ x coordinate of the crew member status in the crew report

: set-condition-color  ( n -- )
  stamina @ stamina-attr c@ attr!  ;
  \ Set the proper color for the condition of man _n_.

: .crew-member-data  ( n -- )
  >r white attr!
  name-x r@ 6 + at-xy r@ name$ type
  r@ set-condition-color
  status-x r@ 6 + at-xy
  r> condition$ 2dup uppers1 type  ;

: .crew-report-header  ( -- )
  0 1 at-xy s" Estado de la tripulación" columns type-center
  name-x 4 at-xy ." Nombre"  status-x 4 at-xy ." Condición"  ;

: crew-report  ( -- )
  begin-report .crew-report-header
  men 0 do  i .crew-member-data  loop  end-report  ;

: update-score  ( -- )
  found-clues @ 1000 *
  day        @  200 * +
  sunk-ships  @ 1000 * +
  trades     @  200 * +
               4000 success? and +
             score +!  ;

: score-report  ( -- )
  begin-report
  0 1 at-xy s" Puntuación" columns type-center
  0 4 at-xy
  ." Días"            tab day         @ 4 .r ."  x  200" cr cr
  ." Barcos hundidos" tab sunk-ships  @ 4 .r ."  x 1000" cr cr
  ." Negocios"        tab trades      @ 4 .r ."  x  200" cr cr
  ." Pistas"          tab found-clues @ 4 .r ."  x 1000" cr cr
  ." Tesoro"          tab 4000          4 .r             cr cr
  update-score
  ." Total"           tab ."       "
                      score @ 4 .r  end-report  ;
  \ XXX TODO -- add subtotals (use constants)
  \ XXX TODO -- draw a ruler above "Total"

  \ ============================================================
  cr .( Ship battle)  \ {{{1

: miss-boat  ( -- )
  s" Por suerte el disparo no ha dado en el blanco." message  ;

: hit-boat  ( -- )
  s" La bala alcanza su objetivo. "
  s" Esto desmoraliza a la tripulación." s+ message
  -2 morale+!
  3 4 random-range 1 ?do  injured drop  loop  ;
  \ XXX TODO -- inform about how many injured?

: do-attack-boat  ( -- )
  -1 ammo+!
  s" Disparas por error a uno de tus propios botes..." message
  2 seconds
  3 random if    miss-boat
           else  hit-boat
           then  5 seconds wipe-message  ;

: almost-attack-boat  ( -- )
  s" Por suerte no hay munición para disparar..." message
  2 seconds
  s" Pues enseguida te das cuenta de que ibas a hundir "
  s" uno de tus botes." s+ message
  5 seconds wipe-message  ;

: attack-boat  ( -- )
  ammo @ if     do-attack-boat
         else   almost-attack-boat  then  ;

: .sunk-step-0  ( col row -- )
  2dup    at-xy ."    "
  2dup 1+ at-xy ."  ab"
       2+ at-xy ."  90"  ;

: .sunk-step-1  ( col row -- )
  2dup 1+ at-xy ."    "
       2+ at-xy ."  ab"  ;

: .sunk-step-2  ( col row -- )
       2+ at-xy ."    "  ;

: sunk-delay  ( -- )  100 ms  ;

: .sunk  ( -- )
  graph-font1 set-font  white ink  blue paper
  enemy-ship-x @ enemy-ship-y @ 2dup .sunk-step-0 sunk-delay
                                2dup .sunk-step-1 sunk-delay
                                     .sunk-step-2  ;

variable victory
  \ XXX TODO -- remove; use the stack instead

: sunk  ( -- )
  .sunk 2 seconds

  \ ship-loc @ sea @ 13 >=
  \ ship-loc @ sea @ 16 <= and
  \ if  1 sunk-ships +!  1000 score +!  victory on  then
    \ XXX OLD

  1 sunk-ships +!  1000 score +!  victory on
    \ XXX TODO -- use constant to increase the score, and in
    \ the score report

  ship-loc @ sea @ case
    13 of  10  endof
    14 of   9  endof
    15 of   8  endof
    16 of   7  endof  dup endcase  ship-loc @ sea !  ;
  \ Sunk the enemy ship
  \
  \ XXX FIXME -- The `case` changes the type of location, what
  \ makes the picture different.  This is a problem of the
  \ original game.  The enemy ship must be independent from the
  \ location type.

: .wave  ( -- )
  graph-font1 set-font
  cyan ink 11 30 random-range 1 20 random-range at-xy ." kl"  ;

: (move-enemy-ship)  ( -- )
  graph-font1 set-font
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

  white ink  blue paper
  enemy-ship-x @    enemy-ship-y @ 2dup 2dup at-xy  ."  ab "
                                   1+        at-xy  ."  90 "
  yellow ink
  enemy-ship-x @ 1- enemy-ship-y @ 2+        at-xy ."  678 "
                                   2dup  1-  at-xy ."    "
                                         3 + at-xy ."    "
  enemy-ship-move @ 5 = if  .wave  then  ;
  \ XXX TODO -- factor
  \
  \ XXX TODO -- reuse `.enemy-ship` and erase only the minimum
  \ part of the sea, depending on the movement direction
  \
  \ XXX TODO -- redraw waves only around the ship

: move-enemy-ship  ( -- )

  \ enemy-ship-x @ enemy-ship-y @
  \ 2dup 2dup -1..1 + swap -1..1 + swap 2 d<>
  \ if  (move-enemy-ship)  then

  (move-enemy-ship)
  ;
  \ XXX TMP --

: .ammo  ( -- )  ammo @ 1 .r  ;

: .new-ammo  ( -- )
  white ink red paper 21 23 at-xy .ammo  ;

: -ammo  ( -- )
  -1 ammo+!  text-font set-font .new-ammo  ;

: .ammo-label  ( -- )
  text-font set-font
  white ink red paper 10 23 at-xy ." Munición = " .ammo  ;

0 value gun-muzzle-y
  \ y coordinate of the cannon ball

: sunk-range?  ( n a -- f )  @ dup 2+ between  ;
  \ Is _n_ between the cell hold in _a_ and the cell hold in
  \ _a_ plus 2?

: sunk?  ( col -- f )
  false   swap enemy-ship-x sunk-range? 0exit
  gun-muzzle-y enemy-ship-y sunk-range? 0exit  0=  ;
  \ Is the enemy ship sunk by the cannon ball, which is
  \ at x coordinate _col_?

9 constant cannon-muzzle-x

: cannon-muzzle-fire-coords  ( row -- col row1 col row2 )
  cannon-muzzle-x swap 2- 2dup 1-  ;

: .cannon-muzzle-fire  ( row -- )
  red ink cannon-muzzle-fire-coords at-xy ." -" at-xy ." +"  ;
  \ Print the fire effect of the cannon muzzle, which is at y
  \ coordinate _row_.

: -cannon-muzzle-fire  ( row -- )
  cannon-muzzle-fire-coords at-xy space at-xy space  ;
  \ Erase the fire effect of the cannon muzzle, which is at y
  \ coordinate _row_.

: gun>label-y  ( n -- row )  7 * 2+  ;
  \ Convert gun number _n_ (0..2) to its gun label _row_.

: gun>muzzle-y  ( n -- row )  gun>label-y 1+  ;
  \ Convert gun number _n_ (0..2) to its gun muzzle _row_.

: -cannon-ball  ( -- )  last-column gun-muzzle-y at-xy space  ;
  \ Erase the cannon ball at the end of its trajectory.

: fire  ( n -- )
  graph-font1 set-font  blue paper
  gun>muzzle-y dup .cannon-muzzle-fire to gun-muzzle-y  -ammo
  move-enemy-ship
  black ink cannon-muzzle-x gun-muzzle-y at-xy ."  j"
  gun-muzzle-y -cannon-muzzle-fire
  last-column cannon-muzzle-x do
    i gun-muzzle-y at-xy ."  j"
    i sunk? if  sunk unloop exit  then
  loop  -cannon-ball  ;

: no-ammo-left  ( -- )
  feasible-attack off  panel-commands
  s" Te quedaste sin munición." message  4 seconds  ;
  \ XXX TODO -- the enemy wins; our ship sinks,
  \ or the money and part of the crew are captured

: .gun  ( col row -- )
  2dup    at-xy ." cde"
       1+ at-xy ." fg"  ;
  \ Print a ship gun at _col row_.

: .gun-man  ( col row -- )
  2dup 1- at-xy '1' emit
  2dup    at-xy '2' emit
       1+ at-xy '3' emit  ;
  \ Print a ship gun man at _col row_.

: battle-init-enemy-ship  ( -- )
  20 enemy-ship-x ! 6 enemy-ship-y !  ;
  \   20 random 1+    enemy-ship-x !
  \  11 30 random-range enemy-ship-y ! ;
  \ XXX TODO --

: deck  ( -- )
  text-font set-font
  22 0 do  0 i at-xy  ." ________ "  loop  ;
  \ XXX TODO -- faster

: guns  ( -- )
  3 0 do
    i gun>label-y
    white paper text-font set-font 0 over at-xy i 1+ 1 .r
    yellow paper
    graph-font2 set-font  1+ 4 over .gun-man
    graph-font1 set-font     6 over .gun
                             1 swap 1+ at-xy ." hi"  \ ammo
  loop  ;
  \ XXX TODO -- factor

: clear-for-action  ( -- )  black ink yellow paper deck guns  ;

: battle-scenery  ( -- )
  blue paper cls 31 1 do  .wave  loop
  clear-for-action .ammo-label battle-init-enemy-ship  ;

: trigger  ( -- )
  inkey case  '1' of  0 fire  endof
              '2' of  1 fire  endof
              '3' of  2 fire  endof  endcase  ;

: end-of-battle?  ( -- f )  victory @ ammo @ 0= or  ;

: (ship-battle)  ( -- )
  battle-scenery  victory off
  begin  move-enemy-ship trigger end-of-battle?  until
  ammo @ ?exit no-ammo-left  ;

: ship-battle  ( -- )
  save-screen (ship-battle) restore-screen  ;

: enemy-ship-here?  ( -- f )
  ship-loc @ sea @ 13 16 between  ;

: (attack-ship)  ( -- )
  enemy-ship-here? if    ship-battle
                   else  attack-boat  then  ;

: attack-ship  ( -- )
  ammo @ if  (attack-ship)  else  no-ammo-left  then  ;

  \ ============================================================
  cr .( Island map)  \ {{{1

: erase-island  ( -- )  0 island /island cells erase  ;

: is-coast  ( n -- )  coast swap island !  ;
  \ Make cell _n_ of the island map be coast.

: (make-coast)  ( n1 n2 -- )  bounds do  i is-coast  loop  ;
  \ Make _n2_ cells of the island map, starting from cell_ n1_,
  \ be coast.

: make-north-coast  ( -- )
  [ /island island-length - ] 1literal island-length
  (make-coast)  ;

: make-south-coast  ( -- )  0 island-length (make-coast)  ;

: make-east-coast  ( -- )  6 is-coast 12 is-coast 18 is-coast ;
  \ XXX TODO -- generalize for any size of island

: make-west-coast  ( -- )
  11 is-coast 17 is-coast 23 is-coast  ;
  \ XXX TODO -- generalize for any size of island

: make-coast  ( -- )  make-north-coast make-west-coast
                      make-south-coast make-east-coast  ;

: location-random-type  ( -- n )
  dubloons-found just-3-palms-2 random-range  ;

: populate-island  ( -- )
  23 7 do  i island @ coast <>
           if  location-random-type i island !  then
  loop
  native-village  19 22 random-range island !
  native-ammo     13 16 random-range island !
  native-supplies  7 10 random-range island !  ;
  \ XXX TODO -- improve: adapt to any size:
  \ choose any free non-coast location

: set-crew-loc  ( -- )
  7 10 random-range crew-loc !  ;
  \ XXX TODO -- improve: choose a random location on the coast,
  \ except the village

: new-island  ( -- )
  erase-island make-coast populate-island  ;

  \ ============================================================
  cr .( Treasure quest)  \ {{{1

          1 4 2constant path-range
          1 4 2constant tree-range
0 villages 1- 2constant village-range
          1 2 2constant turn-range
          1 4 2constant direction-range
          1 9 2constant pace-range

: sailor-speech-balloon  ( -- )
  25 44 plot 20 10 rdraw 0  30 rdraw   2  2 rdraw  100 0 rdraw
              2 -2 rdraw 0 -60 rdraw  -2 -2 rdraw -100 0 rdraw
             -2  2 rdraw 0  19 rdraw -20  0 rdraw  ;

: captain-speech-balloon  ( -- )
  220 44 plot -15  5 rdraw 0  20 rdraw -2  2 rdraw -30 0 rdraw
               -2 -2 rdraw 0 -40 rdraw  2 -2 rdraw  30 0 rdraw
                2  2 rdraw 0  14 rdraw 15  0 rdraw  ;

: sailor-and-captain  ( -- )
  graph-font1 set-font  cyan ink  black paper
  0 17 at-xy ."  xy" 28 at-x ." pq" cr
             ."  vs" 28 at-x ." rs" cr
             ."  wu" 28 at-x ." tu"
  sailor-speech-balloon captain-speech-balloon  ;

: sailor-says  ( ca len -- )
  text-font set-font  black paper  white ink
  sailor-window set-window wcls wtype  ;

: treasure-found  ( -- )
  [ 0 attr-line ] literal [ 3 columns * ] 1literal
  [ cyan dup papery + brighty ] cliteral fill
  [ 4 attr-line ] literal [ 18 columns * ] 1literal
  [ yellow dup papery + ] literal fill
    \ XXX TODO -- factor the coloring
    \ XXX TODO -- use constants
  sunny-sky

  23 7 do  i 5 palm2  5 +loop  3 7 palm2  26 7 palm2

  black ink  yellow paper  8 13 at-xy
  ." pq          xy                  "
  ." rs          vs                  tu      "
  .\" \T\U    wu"
  28 11 palm2  0 11 palm2
  graph-font2 set-font  blue ink  yellow paper
    \ XXX TODO -- remove paper
  13 17 at-xy .\" l\::m"
    \ XXX TODO -- factor the treasure

  s" ¡Capitán, somos ricos!" message
  4 seconds  graph-font1 set-font  ;
  \ XXX TODO -- use this proc instead of happy-end?
  \ XXX TODO -- factor

: clue-tried  ( x a -- )
  200 30 beep  wcls  1 seconds  @ = abs found-clues +!  ;
  \ Update the clues found with the given answer _x_ for
  \ clue hold in _a_.

: at-clue  ( -- )  23 15 at-xy  ;

: .clue-prompt  ( -- )  at-clue '?' emit  ;

: .clue  ( n -- )  black paper  at-clue .  ;

: wipe-treasure-island  ( -- )
  [ treasure-island-top-y attr-line ] literal
  [ treasure-island-rows columns *  ] 1literal
  [ yellow dup papery +             ] cliteral fill  ;

: paths-to-choose  ( -- )
  wipe-treasure-island
  graph-font2 set-font green ink  yellow paper
  0 3 at-xy ."  5     6       45     6       5"
  graph-font1 set-font black ink
  25 0 do
    i 3 + 3 at-xy .\" :\x7F"
    i 2+  4 at-xy .\" :\::\::\x7F"
    i 1+  5 at-xy .\" :\::\::\::\::\x7F"
    i     6 at-xy .\" :\::\::\::\::\::\::\x7F"
  8 +loop
  text-font set-font  white ink  red paper
  0 7 at-xy ."    1       2       3       4    "  ;

: try-path  ( -- )
  paths-to-choose
  s" ¿Qué camino tomamos, capitán?" sailor-says
  .clue-prompt path-range get-digit
  dup .clue path clue-tried  ;

: trees-to-choose  ( -- )
  wipe-treasure-island  black ink  yellow paper
  0 7 at-xy ."  1       2       3       4"
  graph-font1 set-font  27 2 do  i 3 palm2  8 +loop  ;
  \ XXX TODO -- remove the loop

: try-tree  ( -- )
  trees-to-choose
  s" ¿En qué árbol paramos, capitán?" sailor-says
  .clue-prompt tree-range get-digit
  dup .clue tree clue-tried  ;

: try-way  ( -- )
  \ XXX TODO -- draw tree
  s" ¿Vamos a la izquierda (1) o a la derecha (2), capitán?"
  sailor-says
  .clue-prompt turn-range get-digit
  dup .clue turn clue-tried  ;
  \ XXX TODO -- use letters instead of digits

: villages-to-choose  ( -- )
  wipe-treasure-island  black ink  yellow paper
  villages 0 do
    1 13 i odd? and + i 2/ treasure-island-top-y + at-xy
    i dup . village$ type
  loop
  graph-font2 set-font
  green ink  27 5 at-xy .\" S\::T" 27 6 at-xy ." VUW"  ;
  \ XXX TODO -- Factor the hut, perhaps also in `.huts`.

: try-village  ( -- )
  villages-to-choose
  s" ¿Qué poblado atravesamos, capitán?" sailor-says
  .clue-prompt village-range get-digit
  dup .clue village clue-tried  ;

: try-direction  ( -- )
  wipe-treasure-island  \ XXX TODO -- draw something instead
  s" ¿En qué dirección vamos, capitán? (1N 2S 3E 4O)"
  sailor-says
  .clue-prompt direction-range get-digit
  dup .clue direction clue-tried  ;
  \ XXX TODO -- use letters instead of digits

: try-steps  ( -- )
  wipe-treasure-island  \ XXX TODO -- draw something instead
  s" ¿Cuántos pasos damos, capitán?" sailor-says
  .clue-prompt pace-range get-digit
  dup .clue pace clue-tried  ;
  \ XXX TODO -- add range to the message

: clear-for-quest  ( -- )
  [ 8 attr-line ] literal [ 14 columns * ] 1literal erase  ;

: quest  ( -- )
  clear-for-quest
  sailor-and-captain try-path    try-tree      try-way
                     try-village try-direction try-steps  ;

: enter-treasure-island  ( -- )
  black paper cls wipe-treasure-island new-sunny-sky
  quest success?
  if    s" ¡Hemos encontrado el oro, capitán!"
  else  s" Aquí no hay tesoro alguno, capitán."
  then  sailor-says  1 seconds  ;
  \ XXX TODO -- factor the two results, add longer texts and
  \ draw pictures.

  \ ============================================================
  cr .( Island graphics)  \ {{{1

: wipe-island-scenery  ( -- )
  [ yellow dup papery + ] cliteral color-sea  ;
  \ XXX TODO -- Color only the block occupied by the island.
  \ This will save drawing the blue borders before drawing the
  \ waves.

: north-waves  ( -- )
  0 sea-top-y at-xy ."  kl  mn     nm    klk   nm nm n "  ;
  \ XXX TODO -- show random waves every time, using a random
  \ 32-chars substring from a main one

: south-waves  ( -- )
  0 [ sea-bottom-y 1- ] cliteral at-xy
  ."  kl     mn  mn    kl    kl kl  m"
  ."     mn      klmn   mn m  mn     "  ;
  \ XXX TODO -- show random waves every time, using a random
  \ 64-chars substring from a main one

: west-waves  ( -- )
  [ sea-top-y sea-rows bounds ] 2literal
  do  0 i at-xy ."   "  loop
  0  4 at-xy ." m"
  0  6 at-xy ." mn"
  1  8 at-xy  ." l"
  0 10 at-xy ." kl"
  0 13 at-xy ." k"
  graph-font2 set-font  yellow ink
  walk-north? 0= if  2  4 at-xy 'A' emit  then
  walk-south? 0= if  2 13 at-xy 'C' emit  then
  graph-font1 set-font  ;
  \ XXX TODO -- factor
  \ XXX TODO -- random waves
  \ XXX TODO -- use constants for the base coordinates

: east-waves  ( -- )
  [ sea-top-y sea-rows bounds ] 2literal
  do  30 i at-xy ."   "  loop
  30  4 at-xy ." m"
  30  6 at-xy ." mn"
  31  8 at-xy  ." l"
  30 10 at-xy ." kl"
  31 13 at-xy  ." k"
  yellow ink  graph-font2 set-font
  walk-north? 0= if  29  4 at-xy 'B' emit  then
  walk-south? 0= if  29 13 at-xy 'D' emit  then
  graph-font1 set-font  ;
  \ XXX TODO -- factor
  \ XXX TODO -- random waves
  \ XXX TODO -- use constants for the base coordinates

: island-waves  ( -- )
  graph-font1 set-font  white ink  blue paper
  walk-south? 0= if  south-waves  then
  walk-north? 0= if  north-waves  then
   walk-east? 0= if  east-waves   then
   walk-west? 0= if  west-waves   then  ;

: .huts  ( -- )
  green ink
  6  5 at-xy .\"  S\::T    ST   S\::T"
  6  6 at-xy .\"  VUW    78   VUW   4"
  4  8 at-xy .\" S\::T   S\::T    S\::T S\::T  S\::T "
  4  9 at-xy ." VUW   VUW  4 VUW VUW  VUW"
  4 11 at-xy .\" S\::T    S\::T ST  S\::T S\::T"
  4 12 at-xy ." VUW  4 VUW 78  VUW VUW"  ;
  \ XXX TODO -- Random, but specific for every island: Choose a
  \ random number and use its groups of 4 bits as identifiers
  \ of what must be drawn: 3 types of hut and nothing.

: .villagers  ( -- )
  black ink
  10  6 at-xy ." XYZ"
  17  6 at-xy ." YX"
  26  6 at-xy ." Z"
   8  9 at-xy ." ZZ"
  13  9 at-xy ." Y"
  24  9 at-xy ." ZX"
   7 12 at-xy ." X"
  17 12 at-xy ." Y"
  22 12 at-xy ." Z"
  26 12 at-xy ." XY"  ;
  \ XXX TODO -- random

: .village  ( -- )
  graph-font2 set-font  yellow paper .huts .villagers
  graph-font1 set-font  ;

: .native  ( -- )
  black ink  yellow paper  8 10 at-xy ."  _ `"
                           8 11 at-xy ." }~.,"
                           8 12 at-xy ." {|\?"  ;

: .ammo-gift  ( -- )
  black ink  yellow paper  14 12 at-xy ." hi"  ;
  \ XXX TODO -- draw graphics depending on the actual ammount

: .supplies  ( -- )
  graph-font2 set-font
  black ink  yellow paper 14 12 at-xy ." 90  9099 0009"
  graph-font1 set-font  ;
  \ XXX TODO -- draw graphics depending on the actual ammount

: .snake  ( -- )
  graph-font2 set-font
  black ink  yellow paper  14 12 at-xy ." xy"
  graph-font1 set-font  ;

: .dubloons  ( n -- )
  get-font >r graph-font2 set-font  black ink  yellow paper
  12 dup at-xy s" vw vw vw vw vw vw vw vw " drop swap 3 * type
  r> set-font  ;
  \ XXX TODO -- use a loop
  \ XXX TODO -- other option: print at random empty places

: island-location  ( n -- )
  ~~ case
    native-village  of  .village                          endof
    dubloons-found  of  4 8 palm2 14 5 palm2              endof
      \ XXX TODO -- print dubloons here
    hostile-native  of  ~~ 14 5 palm2 25 8 palm2 .native  endof
    just-3-palms-1  of  25 8 palm2  4 8 palm2 16 5 palm2  endof
    snake of
      13 5 palm2 5 6 palm2 18 8 palm2 23 8 palm2 .snake
                                                          endof
    just-3-palms-2  of  23 8 palm2 17 5 palm2 4 8 palm2   endof
    native-supplies of  ~~ .supplies  .native  16 4 palm2 endof
    native-ammo     of  ~~ .ammo-gift .native 20 5 palm2  endof
  endcase  ~~ ;

: current-island-location  ( -- )
  crew-loc @ island @ island-location  ;

: island-scenery  ( -- )
  graph-font1 set-font
  wipe-island-scenery sunny-sky island-waves
  ~~ current-island-location  ;

  \ ============================================================
  cr .( Events on an island)  \ {{{1

: marsh  ( -- )
  dead name$ s"  se hunde en arenas movedizas." s+ message  ;

: swamp  ( -- )
  dead name$ s"  se hunde en un pantano." s+ message  ;

: spider  ( -- )
  s" A " injured name$ s+
  s"  le muerde una araña." s+ message  ;

: scorpion  ( -- )
  s" A " injured name$ s+ s"  le pica un escorpión." s+
  message  ;

: hunger  ( -- )
  s" La tripulación está hambrienta." message
  -1 morale+!  ;
  \ XXX TODO -- only if supplies are not enough

: thirst  ( -- )
  s" La tripulación está sedienta." message
  -1 morale+!  ;
  \ XXX TODO -- only if supplies are not enough

: money  ( -- )
  2 5 random-range dup .dubloons dup cash+!
  s" Encuentras " rot coins$ s+ s" ." s+ message  ;
  \ XXX TODO -- factor: repeated in `enter-this-island-location`

: no-problem  ( -- )
  s" Sin novedad, capitán." message  ;
  \ XXX TODO -- improve message
  \ XXX TODO -- rename

: no-danger  ( -- )
  s" La zona está despejada, capitán." message  ;
  \ XXX TODO -- improve message, depending on the location,
  \ e.g. "no hay moros en la costa"
  \ XXX TODO -- rename

create island-events-table  ( -- a )  here

] marsh swamp spider scorpion hunger thirst money
  no-problem no-problem no-danger no-danger noop noop [

here - cell / constant island-events

: island-event  ( -- )
  island-events random island-events-table array> perform  ;

  \ ============================================================
  cr .( Enter island location)  \ {{{1

: be-hostile-native  ( -- )
  hostile-native crew-loc @ island !  ;

: enter-this-island-location  ( n -- )

  .debug-info

  ~~ case

  snake of  ~~
    s" Una serpiente ha mordido a "
    injured name$ s+ s" ." s+ message
    \ XXX TODO -- inform if the man is dead
  endof

  hostile-native of  ~~
    s" Un nativo intenta bloquear el paso y hiere a "
    injured dup >r name$ s+ s" , que resulta " s+
    r> condition$ s+ s" ." s+ message
  endof

  dubloons-found of  ~~

    1 2 random-range >r
    s" Encuentras " r@ coins$ s+ s" ." s+ message
    r@ cash+!
    r> .dubloons
    \ XXX TODO -- factor: repeated in `money`

    just-3-palms-1 crew-loc @ island !
      \ XXX FIXME -- This changes the type of location, what
      \ makes the picture different.  This is a problem of the
      \ original game.  The dubloons must be independent from
      \ the location type.

  endof

  native-ammo of  ~~
    s" Un nativo te da algo de munición." message
    1 ammo+!  be-hostile-native
      \ XXX TODO -- random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `island-location`
  endof

  native-supplies of  ~~
    s" Un nativo te da provisiones." message
    1 supplies+!  be-hostile-native
      \ XXX TODO -- random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `island-location`
  endof

  native-village of  ~~
    s" Descubres un poblado nativo." message
    \ XXX TODO -- Change the message if the village is visited.
  endof

  just-3-palms-1 of  island-event  endof

  just-3-palms-2 of  island-event  endof

  ~~ endcase
  .debug-info  \ XXX INFORMER
  ;

: enter-island-location  ( -- )
  wipe-message island-scenery panel-commands
  crew-loc @ island @ enter-this-island-location  ;

  \ ============================================================
  cr .( Disembark)  \ {{{1

: target-island  ( -- )
  31  8 at-xy ." :"
  27  9 at-xy .\" HI :\::"
  25 10 at-xy .\" F\::\::\::\::\::\::"
  23 11 at-xy .\" JK\::\::\::\::\::\::\::"  ;

: disembarking-boat  ( -- )
  21 0 do  i 11 at-xy ."  <>" 200 ms  loop  ;

: disembarking-scene  ( -- )
  graph-font1 set-font  (sea-and-sky)
  blue paper  green ink  target-island
             yellow ink  disembarking-boat  ;

: on-treasure-island?  ( -- f )
  ship-loc @ sea @ treasure-island =  ;

: enter-ordinary-island  ( -- )
  new-island set-crew-loc wipe-panel enter-island-location  ;

: enter-island  ( -- )
  aboard off  on-treasure-island?
  if    enter-treasure-island
  else  enter-ordinary-island  then  ;

: disembark  ( -- )
  -2 -1 random-range supplies+!
  wipe-message wipe-panel
  disembarking-scene enter-island  ;

  \ ============================================================
  cr .( Storm)  \ {{{1

2 constant rain-y

: at-rain  ( a -- )  @ rain-y at-xy  ;

: at-west-rain  ( -- )  west-cloud-x at-rain  ;

: at-east-rain  ( -- )  east-cloud-x at-rain  ;

: rain-drops  ( c -- )
  dup  at-west-rain /west-cloud emits
       at-east-rain /east-cloud emits  60 ms  ;

: +storm  ( -- )
  graph-font1 set-font
  70 0 do  white ink  cyan paper
           ';' rain-drops  ']' rain-drops  '[' rain-drops
           3 random 0= if  redraw-ship  then
  loop  ;
  \ Make the rain effect.
  \ XXX TODO -- random duration
  \ XXX TODO -- sound effects
  \ XXX TODO -- lightnings
  \ XXX TODO -- make the enemy ship to move, if present
  \ (use the same graphic of the player ship)
  \ XXX TODO -- move the waves

cyan dup papery + constant stormy-sky-attr

: -storm  ( -- )  stormy-sky-attr attr!
                 at-west-rain /west-cloud spaces
                 at-east-rain /east-cloud spaces  ;
  \ Erase the rain effect.
  \ Note the sky keeps the stormy color.
  \ XXX TODO -- improve: make the sky sunny after some time

: stormy-sky  ( -- )  stormy-sky-attr color-sky
                      sun-and-clouds  ;
  \ Make the sky stormy.
  \ XXX TODO -- hide the sun

: damages  ( -- )  10 49 damaged  ;

: storm-warning  ( -- )
  wipe-panel
  s" De pronto se desata una fuerte tormenta..." message  ;

: storm-report  ( -- )
  s" Cuando la mar y el cielo se calman, "
  s" compruebas el estado del barco: " s+ damage$ s+ s" ." s+
  message  ;
  \ XXX FIXME -- sometimes `damage$` is empty: check the range
  \ of the damage percentage.

: storm  ( -- )  stormy-sky storm-warning
                 +storm damages -storm storm-report  ;

  \ ============================================================
  cr .( Ship command)  \ {{{1

: to-reef?  ( n -- f )  ship-loc @ + reef?  ;
  \ Does the sea movement offset _n_ leads to a reef?

: (sail)  ( n -- )  ship-loc +! sea-scenery panel-commands  ;

: sail  ( n -- )
  dup to-reef? if  drop run-aground  else  (sail)  then  ;
  \ Move on the sea map, using offset _n_ from the current
  \ position.

: sail-north  ( -- )  to-north sail  ;
: sail-south  ( -- )  to-south sail  ;
: sail-east   ( -- )   to-east sail  ;
: sail-west   ( -- )   to-west sail  ;

: ?sail-north?  ( -- f )  north? dup 0exit  sail-north  ;
: ?sail-south?  ( -- f )  south? dup 0exit  sail-south  ;
: ?sail-east?   ( -- f )  east?  dup 0exit  sail-east   ;
: ?sail-west?   ( -- f )  west?  dup 0exit  sail-west   ;

: ship-command?  ( c -- f )
  dup 0exit  case
  'N' key-up                or-of  ?sail-north?        endof
  'S' key-down              or-of  ?sail-south?        endof
  'E' key-right             or-of  ?sail-east?         endof
  'O' key-left              or-of  ?sail-west?         endof
  'I'                          of  main-report    true endof
  'A' feasible-attack @ and    of  attack-ship    true endof
  'T'                          of  crew-report    true endof
  'P'                          of  score-report   true endof
  'D' feasible-disembark @ and of  disembark      true endof
  'F'                          of  quit-game on   true endof
  'Q'                          of  quit                endof
    \ XXX TMP -- 'Q' option for debugging
  false swap  endcase  ;
  \ If character _c_ is a valid ship command, execute it and
  \ return true; else return false.
  \
  \ Note: the trigger `dup 0exit` is used at the start because
  \ the default value of _c_ is zero, which would clash with
  \ 'A' and 'D' clauses if their "and-ed" control flags are
  \ off. `dup exit` is smaller than a `0 of false endof`
  \ clause.
  \
  \ XXX TODO -- use execution table instead? better yet:
  \ `thiscase` structure.

: redraw-ship?  ( -- f )  frames@ drop 1024 mod 0=  ;

: ?redraw-ship  ( -- )  redraw-ship? if  redraw-ship  then  ;

: storm?  ( -- f )  8912 random 0=  ;
  \ XXX TODO -- increase the probability of storm every day?

: ?storm  ( -- )  storm? if  storm  then  ;

: ship-command  ( -- )
  begin  ?redraw-ship ?storm  inkey upper ship-command?
  until  ;

  \ ============================================================
  cr .( Misc commands on the island)  \ {{{1

: embark  ( -- )
  ship-loc @ visited on  1 day +!  aboard on
  sea-scenery panel  ;
  \ XXX TODO -- Improve transition with a blackout, instead of
  \ clearing the scenery and the panel apart. There are other
  \ similar cases.

: to-land?  ( n -- f )  crew-loc @ + coast? 0=  ;
  \ Does the island movement offset _n_ leads to land?

: walk  ( n -- )
  dup to-land? if    crew-loc +!  enter-island-location
               else  drop  then  ;
  \ Move on the sea map, using offset _n_ from the current
  \ position, if possible.
  \
  \ XXX TODO -- make the movement impossible on the panel if it
  \ leads to the sea, or show a warning

  \ ============================================================
  cr .( Clues)  \ {{{1

: path-clue$  ( -- ca len )
  s" Tomar camino " path @ number$ s+ s" ." s+  ;

: tree-clue$  ( -- ca len )
  s" Parar en árbol " tree @ number$ s+ s" ." s+  ;

: turn-clue$  ( -- ca len )
  s" Ir a " turn @ hand$ s+ s"  en árbol." s+  ;

: village-clue$  ( -- ca len )
  s" Atravesar poblado " village @ village$ s+ s" ." s+  ;

: direction-clue$  ( -- ca len )
  s" Ir " direction @ cardinal$ s+ s"  desde poblado." s+  ;

: steps-clue$  ( -- ca len )
  s" Dar " pace @ number$ s+ s"  paso" s+
  s" s " pace @ 1 > and s+ s" desde poblado." s+  ;

create clues  ( -- a )
] path-clue$    tree-clue$      turn-clue$
  village-clue$ direction-clue$ steps-clue$ [

: clue$  ( -- ca len )  6 random cells clues + perform  ;

: native-tells-clue  ( -- )
  s" Bien... Pista ser..." native-says
  2 seconds  clue$ native-says
  2 seconds  s" ¡Buen viaje a isla de tesoro!" native-says  ;

  \ ============================================================
  cr .( Trading)  \ {{{1

: native-speech-balloon  ( -- )
  black ink
  100 100 plot  20 10 rdraw  0 30 rdraw  2 2 rdraw
  100 0 rdraw  2 -2 rdraw  0 -60 rdraw  -2 -2 rdraw
  -100 0 rdraw -2 2 rdraw  0 20 rdraw  -20 0 rdraw  ;

variable price  variable offer
  \ XXX TODO -- remove `offer`, use the stack instead

: make-offer  ( -- )
  cash @ max-offer min >r
  s" Tienes " cash @ coins$ s+
  s" . ¿Qué oferta le haces? (1-" s+ r@ u>str s+ ." )" s+
  message
  r> get-digit offer !
  200 10 beep
  s" Le ofreces " offer @ coins$ s+ s" ." s+ message  ;
  \ Ask the player for an offer.
  \ XXX TODO -- check the note about the allowed range
  \ XXX TODO -- remove `offer`, use the stack instead
  \ XXX TODO -- rename to `your-offer`

: rejected-offer  ( -- )
  2 seconds  s" ¡Tú insultar! ¡Fuera de isla mía!" native-says
  4 seconds  embark  ;

: accepted-offer  ( -- )
  wipe-message
  offer @ negate cash+!  200 score +!  1 trades +!
  native-tells-clue  4 seconds  embark  ;

: new-price  ( -- )
  3 8 random-range dup price ! coins$ 2dup uppers1
  s"  ser nuevo precio, blanco." s+ native-says  ;
  \ The native decides a new price.

: lower-price  ( -- )
  -3 -2 random-range price +!
  s" Bueno, tú darme... " price @ coins$ s+
  s"  y no hablar más." s+ native-says
  make-offer offer @ price @ >=
  if  accepted-offer  else  rejected-offer  then  ;
  \ The native lowers the price by several dubloons.

: one-coin-less  ( -- )
  make-offer offer @ price @ 1- >=
  if    accepted-offer
  else  offer @ price @ 1- < if  rejected-offer  then
  then  lower-price  ;
  \ He accepts one dubloon less

: init-trade  ( -- )
  graph-font1 set-font  black ink  yellow paper
  16 3 do  0 i at-xy blank-line$ type  loop
    \ XXX TODO -- improve with `fill`
  4 4 palm2  .native native-speech-balloon
  s" Un comerciante nativo te sale al encuentro." message  ;

: trade  ( -- )
  init-trade  s" Yo vender pista de tesoro a tú." native-says
  5 9 random-range price !
  s" Precio ser " price @ coins$ s+ s" ." s+ native-says
  \ XXX TODO -- pause or join:
  1 seconds  s" ¿Qué dar tú, blanco?" native-says  make-offer
  offer @ price @ 1-  >= if  accepted-offer exit  then
    \ One dubloon less is accepted.
  offer @ price @ 4 - <= if  rejected-offer exit  then
    \ Too low offer is not accepted.

  \ You offered too few
  4 random case 0 of  lower-price             exit  endof
                1 of  new-price one-coin-less exit  endof
           endcase

  -1 price +!
  s" ¡No! ¡Yo querer más! Tú darme " price @ coins$ s+ s" ." s+
  native-says  one-coin-less  ;

  \ ============================================================
  cr .( Attack)  \ {{{1

: impossible  ( -- )
  s" Lo siento, capitán, no puede hacer eso." message
  2 seconds  ;
  \ XXX not used yet

: hard-to-kill-native  ( -- )
  s" El nativo muere, pero antes mata a "
  dead @ name$ s+ s" ." s+ message  ;

: dead-native-has-supplies  ( -- )
  s" El nativo tiene provisiones "
  s" escondidas en su taparrabos." s+ message  1 supplies+!  ;

: dead-native-has-dubloons  ( -- )
  2 3 random-range r>
  s" Encuentras " r@ coins$ s+
  s"  en el cuerpo del nativo muerto." s+ message r> cash+!  ;

: attack-native-anyway  ( -- )
  5 random case          0 of  hard-to-kill-native       endof
                         1 of  dead-native-has-supplies  endof
                   default-of  dead-native-has-dubloons  endof
  endcase  ;

: .black-flag  ( -- )
  get-font >r graph-font2 set-font  black ink  yellow paper
  14 10 do  8 i at-xy ." t   "  loop
                           8  9 at-xy ." u"
  white ink  black  paper  9 10 at-xy ." nop"
                           9 11 at-xy ." qrs"
  r> set-font  ;
  \ XXX TODO -- faster: no loop, use "tnop" and "tqrs"

: -native  ( -- )
  just-3-palms-1 crew-loc @ island !  .black-flag  ;
  \ XXX TODO -- improve -- don't change the scenery:
  \ first, make natives, animals and things independent from
  \ the location

: attack-native-but-snake-kills  ( -- )
  s" Matas al nativo, pero la serpiente mata a "
  dead name$ s+ s" ." s+ message -native  ;

: attack-native-village  ( -- )
  s" Un poblado entero es un enemigo muy difícil. "
  dead name$ s+ s"  muere en el combate." s+ message  ;

: attack-native-there  ( n -- )
  case  snake          of  attack-native-but-snake-kills  endof
        native-village of  attack-native-village          endof
               default-of  attack-native-anyway           endof
  endcase  ;

: attack-native  ( -- )
  2 seconds  crew-loc @ island @ attack-native-there
  3 seconds  ;

  \ ============================================================
  cr .( Command dispatcher on the island)  \ {{{1

: walk-north  ( -- )  to-north walk  ;
: walk-south  ( -- )  to-south walk  ;
: walk-east   ( -- )   to-east walk  ;
: walk-west   ( -- )   to-west walk  ;

: ?walk-north?  ( -- f )  north? dup 0exit  walk-north  ;
: ?walk-south?  ( -- f )  south? dup 0exit  walk-south  ;
: ?walk-east?   ( -- f )  east?  dup 0exit  walk-east   ;
: ?walk-west?   ( -- f )  west?  dup 0exit  walk-west   ;

: ?trade?  ( -- f )  feasible-trade @ dup 0exit  trade  ;

: ?embark?  ( -- f )  feasible-embark @ dup 0exit  embark  ;

: ?attack-native?  ( -- f )
  feasible-attack @ dup 0exit  attack-native  ;

: island-command?  ( c -- f )
  case
    'N' key-up    or-of  ?walk-north?            endof
    'S' key-down  or-of  ?walk-south?            endof
    'E' key-right or-of  ?walk-east?             endof
    'O' key-left  or-of  ?walk-west?             endof
    'C'              of  ?trade?                 endof
    'B'              of  ?embark?                endof
    'I'              of  main-report      true   endof
    'M'              of  ?attack-native?         endof
    'T'              of  crew-report      true   endof
    'P'              of  score-report     true   endof
    'F'              of  quit-game on     true   endof
    'Q'              of  quit                    endof
      \ XXX TMP -- 'Q' option for debugging
  false swap  endcase  ;
  \ If character _c_ is a valid command on the island, execute
  \ it and return true; else return false.

: island-command  ( -- )
  begin  key upper island-command?  until  ;

  \ ============================================================
  cr .( Setup)  \ {{{1

: init-once  ( -- )  init-screen  ;

: add-row-reefs  ( n1 n0 -- )  ?do  reef i sea !  loop  ;

: add-north-reefs  ( -- )
  sea-length 0 add-row-reefs  ;

: add-south-reefs  ( -- )
  [ sea-breadth 1- sea-length * dup sea-length + ]
  1literal 1literal add-row-reefs  ;

: add-col-reefs  ( n1 n0 -- )
  ?do  reef i sea !  sea-length +loop  ;

: add-east-reefs  ( -- )
  [ sea-breadth 2- sea-length * 1+ ] 1literal sea-length
  add-col-reefs  ;

: add-west-reefs  ( -- )
  [ sea-length 2* 1-  /sea sea-length - ]
  1literal 1literal add-col-reefs  ;

: add-reefs  ( -- )  add-north-reefs add-south-reefs
                     add-east-reefs add-west-reefs  ;

: populate-sea  ( -- )
  /sea sea-length - sea-length 1+ do
    i reef? 0= if  2 21 random-range i sea !  then
  loop
  treasure-island 94 104 random-range sea !  ;
  \ XXX TODO -- 21 is shark; these are picture types

: -/sea  ( a -- )  0 swap /sea cells erase  ;
  \ Erase a sea map array _a_.

: empty-sea  ( -- )  sea -/sea  visited -/sea  ;

: new-sea  ( -- )  empty-sea add-reefs populate-sea  ;

: init-ship  ( -- )
  32 42 random-range ship-loc !  9 ship-y !  4 ship-x !
  ship-up off  ;

: init-clues  ( -- )
       path-range random-range      path !
       tree-range random-range      tree !
    village-range random-range   village !
       turn-range random-range      turn !
  direction-range random-range direction !
       pace-range random-range      pace !  ;
  \ XXX TODO -- use `random` for 0..x
  \ XXX TODO -- convert all ranges to 0..x
  \ XXX TODO -- use constant for ranges and reuse them as
  \ parameters of `get-digit`

: init-plot  ( -- )
  init-clues  aboard on  1 crew-loc !
  men alive !  2 ammo !  5 cash !  10 morale !  10 supplies !
  quit-game off  damage off  day off  found-clues off
  score off  sunk-ships off  trades off  ;

: unused-name  ( -- n )
  0  begin  drop  0 [ stock-names 1- ] 1literal random-range
     dup used-name @ 0= until  ;
  \ Return the random identifier _n_ of an unused name.

: new-crew-name  ( n -- )
  unused-name dup used-name on  stock-name$ rot name 2!  ;
  \ Choose an unused name for crew member _n_.

: new-crew-names  ( -- )  men 0 do  i new-crew-name  loop  ;
  \ Choose unused names for the crew members.

: init-crew-stamina  ( -- )
  men 0 do  max-stamina i stamina !  loop  ;
  \ Set the stamina of the crew to its maximum.

: new-crew  ( -- )  new-crew-names init-crew-stamina  ;

: init  ( -- )
  0 randomize0
  text-font set-font  white ink  black paper  cls
  0 [ rows 2 / ] cliteral at-xy
  s" Preparando el viaje..." columns type-center
  new-sea init-ship new-crew init-plot  ;

  \ ============================================================
  cr .( Game over)  \ {{{1

: really-quit  ( -- )
  \ Confirm the quit
  \ XXX TODO
  ;

: play-again  ( -- )
  \ Play again?
  \ XXX TODO
  ;

: sad-end  ( -- )
  text-font set-font  white ink  red paper
  0 1 at-xy s" FIN DEL JUEGO" columns type-center
  the-end-window set-window  black ink  yellow paper
  supplies @ 0= if
    s" - Las provisiones se han agotado." wtype wcr  then
  morale @ 0= if
    s" - La tripulación se ha amotinado." wtype wcr  then
  ammo @ 0 <= if
    s" - La munición se ha terminado." wtype wcr  then
  alive @ 0= if
    s" - Toda tu tripulación ha muerto." wtype wcr  then
  max-damage? = if
    s" - El barco está muy dañado y es imposible repararlo."
    wtype wcr
  then
  cash @ 0= if  s" - No te queda dinero." wtype  then  ;

: happy-end  ( -- )
  s" Lo lograste, capitán." message  ;
  \ XXX TODO --

: the-end  ( -- )
  black ink yellow paper wcls
  graph-font1 set-font  16 1 do  27 i palm2  1 i palm2  7 +loop
  success? if  happy-end  else  sad-end  then
  s" Pulsa una tecla para ver tus puntos" message
  key drop 200 30 beep score-report  ;
  \ XXX TODO -- new graphic, based on the cause of the end

  \ ============================================================
  cr .( Intro)  \ {{{1

: skulls  ( -- )
  ."   nop  nop  nop  nop  nop  nop  "
  ."   qrs  qrs  qrs  qrs  qrs  qrs  "  ;
  \ Draw a row of six skulls.

: skull-border  ( -- )
  graph-font2 set-font white ink  black paper  1 bright
  home skulls 0 22 at-xy skulls  graph-font1 set-font
  0 bright  ;
  \ Draw top and bottom borders of skulls.

: intro  ( -- )
  white ink black paper cls
  skull-border intro-window set-window whome
  get-font >r text-font set-font
  s" Viejas leyendas hablan del tesoro "
  s" que esconde la perdida isla de " s+
  island-name$ s+ s" ." s+ wtype wcr wcr
  s" Los nativos del archipiélago recuerdan "
  s" las antiguas pistas que conducen al tesoro. " s+
  s" Deberás comerciar con ellos para que te las digan." s+
  wtype wcr wcr
  s" Visita todas las islas hasta encontrar la isla de "
  island-name$ s+
  s"  y sigue las pistas hasta el tesoro..." s+ wtype wcr wcr
  0 row 2+ at-xy s" Pulsa una tecla" columns type-center
  120 ?seconds r> set-font  ;

  \ ============================================================
  cr .( Main)  \ {{{1

: scenery  ( -- )
  aboard? if    sea-scenery
          else  island-scenery  then  panel  ;

: command  ( -- )
  aboard? if  ship-command  else  island-command  then  ;

: game  ( -- )
  cls scenery  begin  command game-over?  until  ;

: run  ( -- )
  init-once  begin  intro init game the-end  again  ;

  \ ============================================================
  cr .( Debugging tools [2])  \ {{{1

: (.debug-info)  ( -- )
  get-font >r text-font set-font
  home aboard? if    ship-loc ? ship-loc @ sea
               else  crew-loc ? crew-loc @ island
               then  ? .s  r> set-font  ;

' (.debug-info) ' .debug-info defer!

variable checkered

: checkered@  ( -- )  checkered @  ;

: +checkered  ( -- )  checkered@ inverse  ;

: checkered!  ( f -- )  0= checkered !  ;

: -checkered  ( -- )  checkered @ checkered!  ;

: ship-here?  ( col row -- f )  sea-length * + ship-loc @ =  ;

: loc-color  ( f -- ) if  red  else  white  then  ink  ;

: .sea  ( -- )
  black paper cr
  0 sea-breadth 1- do
    checkered@
    sea-length 0 do
      i j ship-here? loc-color
      +checkered j sea-length * i + sea @ 2 .r
      -checkered
    loop  cr checkered!
  -1 +loop  default-colors  ;

: crew-here?  ( col row -- f )
  island-length * + crew-loc @ =  ;

: .isl  ( -- )
  black paper cr
  0 island-breadth 1- do
    checkered@
    island-length 0 do
      i j crew-here? loc-color
      +checkered j island-length * i + island @ 2 .r
      -checkered
    loop  cr checkered!
  -1 +loop  default-colors  ;

: x  ( -- )  aboard? if  .sea  else  .isl  then  ;
: n  ( -- )  aboard? if  sail-north  else  walk-north  then  ;
: s  ( -- )  aboard? if  sail-south  else  walk-south  then  ;
: e  ( -- )  aboard? if  sail-east   else  walk-east   then  ;
: o  ( -- )  aboard? if  sail-west   else  walk-west   then  ;

: .chars  ( c1 c0 -- )  do  i emit  loop  ;

: .ascii  ( -- )  cr 128  32 .chars  ;

: .udg    ( -- )  cr 256 128 .chars  ;

: .udgau  ( -- )  cr 165 144 .chars  ;

: .font  ( a -- )
  text-font set-font cr ." font: " dup .  set-font .ascii
  text-font set-font  ;

: .graphs  ( -- )
  cls graph-font1 .font graph-font2 .font .udg  ;

: .damages  ( -- )
  max-damage 1+ 0 ?do
    cr i . i damage ! damage-index . damage$ type  key drop
  loop  ;

: ini  ( -- )  init-once init  ;

: f1  ( -- )  graph-font1 set-font  ;
: f2  ( -- )  graph-font2 set-font  ;
: f   ( -- )  rom-font    set-font  ;

  \ ============================================================
  cr .( Graphics)  \ {{{1

  \ Credit:
  \
  \ The graphic fonts and the UDG set are those of the original
  \ "Jolly Roger", by Barry Jones, 1984.
  \
  \ The sticks and twisty fonts were designed by Paul Howard
  \ for Alchemist PD, 1995, and packed into a viewer called
  \ "Fontbox I".

here 256 -         dup to graph-font1
           /font + dup to graph-font2
           /font + dup to sticks-font
           /font +     to twisty-font

  \ Update the font pointers with addresses relative to the
  \ current data pointer, were the fonts are being compiled.

  \ vim: filetype:soloforth foldmethod=marker
