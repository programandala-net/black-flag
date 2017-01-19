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

: version  ( -- ca len )  s" 0.26.2+201701191909" ;

cr cr .( Bandera Negra) cr version type cr

  \ ============================================================
  cr .( Requirements)  \ {{{1

only forth definitions

  \ --------------------------------------------
  cr .(   -Assembler)  \ {{{2

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

need case  need or-of  need j  need 0exit

  \ --------------------------------------------
  cr .(   -Stack manipulation)  \ {{{2

need pick

  \ --------------------------------------------
  cr .(   -Math)  \ {{{2

need >=  need <=  need under+  need between
need random-range  need randomize0  need -1..1  need d<>

  \ --------------------------------------------
  cr .(   -Memory)  \ {{{2

need move>far  need move<far

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
need columns  need last-column  need inverse  need tabulate
need set-udg  need rom-font  need set-font  need get-font

need black  need blue  need red  need green
need cyan  need yellow  need white
need color!  need permcolor!
need papery  need brighty

need rdraw176 ' rdraw176 alias rdraw
need plot176  ' plot176 alias plot
  \ XXX TMP --

  \ --------------------------------------------
  cr .(   -Keyboard)  \ {{{2

need key-left  need key-right  need key-down  need key-up

need get-inkey ' get-inkey alias inkey
  \ XXX TMP --

  \ --------------------------------------------

  \ forget-transient
  \ XXX TODO --

game-wordlist  dup >order set-current

  \ ============================================================
  cr .( Debugging tools [1])  \ {{{1

:  ~~h  ( -- )  2 border key drop 1 border  ;
  \ Break point.

'q' ~~quit-key !  ~~resume-key on  22 ~~y !  ~~? on

' default-font ' ~~app-info defer!
  \ Make sure the debug information compiled by `~~` is printed
  \ with the ROM font.

: ?break  ( -- )
  break-key? if  cr ." Aborted!" cr quit  then  ;

  \ ============================================================
  cr .( Constants)  \ {{{1

15 constant sea-length
 9 constant sea-breadth

sea-length sea-breadth * constant /sea
  \ cells of the sea map

6 constant island-length
5 constant island-breadth
  \ XXX TODO -- set them randomly when a new island is created

  \     Island grid
  \
  \ 4| 29 28 27 26 25 24
  \ 3| 23 22 21 20 19 18
  \ 2| 17 16 15 14 13 12
  \ 1| 11 10 09 08 07 06
  \ 0| 05 04 03 02 01 00
  \    _________________
  \     5  4  3  2  1  0

island-length island-breadth * constant /island
  \ cells of the island map

10 constant men

: island-name$  ( -- ca len )  s" Calavera"  ;

: ship-name$  ( -- ca len )  s" Furioso"  ;
  \ XXX TODO -- not used yet

  \ Sea location types
  \ XXX TODO complete
 1 constant reef
   \ 7 constant
   \ 8 constant
   \ 9 constant
  \ 10 constant
  \ 13 constant
  \ 14 constant
  \ 15 constant
  \ 16 constant
21 constant shark
22 constant treasure-island

  \ Island location types
1 constant coast
2 constant dubloons-found
3 constant hostile-native
4 constant just-3-palms-1
5 constant snake
6 constant just-3-palms-2
7 constant native-supplies
8 constant native-ammo
9 constant native-village

9 constant max-offer

 3 constant sea-top-y
13 constant sea-height  \ screen lines
 0 constant sky-top-y
 3 constant sky-height  \ screen lines

  \ --------------------------------------------
  \ Windows parameters
                                  0 constant graphic-win-top
                                  0 constant graphic-win-left
                                 32 constant graphic-win-width
                                 16 constant graphic-win-height
                                  0 constant low-win-left
                                 32 constant low-win-width
                                  3 constant low-win-height
     low-win-width low-win-height * constant low-win-chars
                                 17 constant message-win-top
                                  1 constant message-win-left
                                 30 constant message-win-width
                                  3 constant message-win-height

  \ ============================================================
  cr .( Variables)  \ {{{1

variable quit-game         \ flag
variable screen-restored   \ flag  \ XXX TODO -- what for?

  \ --------------------------------------------
  cr .(   -Plot)  \ {{{2

variable i-pos            \ player position on the island
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
variable ship-pos

variable enemy-ship-move
variable enemy-ship-x
variable enemy-ship-y
variable enemy-ship-pos

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
  hp@ far," muy poco dañado"
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
  hp@ far," Mislongo"   \ mis-long-o= "wrong lenght"
  hp@ far," Ombreto"    \ ombr-et-o= "little shadow"
  hp@ far," Figokesto"  \ fig-o-kest-o= "fig basket"
  hp@ far," Misedukota" \ mis-eduk-ot-a= "one to be miseducated"
  hp@ far," Topikega"   \ topik-eg-a=
  hp@ far," Fibaloto"   \ fi-balot-o
  hp@ far," Pomotruko"  \ pom-o-truk-o
  hp@ far," Putotombo"  \ put-o-tomb-o= "well tomb"
  hp@ far," Ursorelo"   \ urs-orel-o= "ear of bear"
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

: condition$  ( n -- ca len )  stamina @ stamina$ 2@  ;
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

window graphic-window
graphic-win-left graphic-win-top
graphic-win-width graphic-win-height
set-window
  \ XXX TODO -- remove, useless

window intro-window  2 3 28 19 set-window

window message-window
message-win-left message-win-top
message-win-width message-win-height
set-window

window native-window 16 6 11 4 set-window

window sailor-window  12 6 12 6 set-window

window the-end-window  5 3 22 20 set-window

  \ ============================================================
  cr .( Screen)  \ {{{1

: init-screen  ( -- )
  default-colors white ink blue paper black border cls
  graphic-window graph-font1 set-font  ;

16384 constant screen  6912 constant /screen
  \ Address and size of the screen.

farlimit @ /screen - dup constant screen-backup
                         farlimit !

far-banks 3 + c@ cconstant screen-backup-bank

: save-screen  ( -- )
  screen-backup-bank bank
  screen screen-backup /screen cmove  default-bank  ;
  \ XXX TODO -- faster, page the bank

: restore-screen  ( -- )
  screen-backup-bank bank
  screen-backup screen /screen cmove  default-bank
  screen-restored on  ;
  \ XXX TODO -- faster, page the bank

  \ ============================================================
  cr .( Text output)  \ {{{1

: native-says  ( ca len -- )
  get-font >r sticks-font set-font native-window wcls wtype
  r> set-font  ;

: wipe-message  ( -- )
  message-window  white ink  black paper  wcls  ;

: message  ( ca len -- )
  text-font set-font wipe-message wtype graphic-window ;

  \ ============================================================
  cr .( Sound )  \ {{{1

: beep  ( "name" -- )  parse-name 2drop  ; immediate
  \ XXX TMP --
  \ XXX TODO --

  \ ============================================================
  cr .( User input)  \ {{{1

: get-digit  ( n1 -- n2 )
  begin  dup key '0' - dup >r
         1 < over r@ < or  while  r> drop beep .1,10
  repeat  drop r>  ;
  \ Wait for a digit to be pressed by the player, until its
  \ value is greater than 0 and less than _n1_, then return it
  \ as _n2_.

  \ ============================================================
  cr .( Command panel)  \ {{{1

21 constant panel-y

variable feasible-disembark      \ flag
variable feasible-embark         \ flag
variable feasible-attack         \ flag
variable feasible-trade          \ flag

variable possible-north          \ flag
variable possible-south          \ flag
variable possible-east           \ flag
variable possible-west           \ flag

: .direction  ( c col row f -- )
  inverse at-xy emit 0 inverse  ;

: directions-menu  ( -- )
  possible-north on
  possible-south on
  possible-east on
  possible-west on
    \ XXX TMP --
    \ XXX TODO -- use conditions
  white ink  black paper
  'N' 30 panel-y    possible-north @ .direction
  'O' 29 panel-y 1+ possible-west  @ .direction
  'E' 31 panel-y 1+ possible-east  @ .direction
  'S' 30 panel-y 2+ possible-south @ .direction
  '+' 30 panel-y 1+ at-xy emit  ;
  \ Print the directions menu.
  \
  \ XXX TODO use a modified  version of "+"?

: feasible-attack?  ( -- f )
  ship-pos @ sea @ dup >r 13 <
                           r@ shark = or
                           r> treasure-island = or  0=
  ammo @ 0<> and  ;
  \ XXX TODO -- rewrite: use presence of the enemy ship, which
  \ now is associated with certain locations but should be
  \ independent

: common-panel-commands  ( -- )
  0 panel-y at-xy s" Información" 0 >option$ type cr
                  s" Tripulación" 0 >option$ type cr
                  s" Puntuación"  0 >option$ type
  feasible-attack? dup >r feasible-attack !
  16 panel-y at-xy s" Atacar" 0 r> ?>option$ type  ;

: feasible-disembark?  ( -- f )
  ship-pos @ visited @ 0=
  ship-pos @ sea @ treasure-island =  or  ;
  \ XXX TODO -- not if an enemy ship is present

: ship-panel-commands  ( -- )
  home ship-pos ? ship-pos @ sea ? .s  \ XXX INFORMER
  feasible-disembark? dup >r feasible-disembark !
  16 panel-y 1+ at-xy s" Desembarcar" 0 r> ?>option$ type  ;
  \ XXX TODO -- factor both conditions

: feasible-trade?  ( -- f )
  i-pos @ island @ native-village =  ;

' true alias feasible-embark?  ( -- f )
  \ XXX TODO -- only if i-pos is coast
  \ XXX TODO -- better yet, only if i-pos is the
  \ disembarking position

: island-panel-commands  ( -- )
  home i-pos ? i-pos @ island ? .s  \ XXX INFORMER
  feasible-embark? dup >r feasible-embark !
  16 panel-y 1+ at-xy s" emBarcar" 2 r> ?>option$ type
  feasible-trade? dup >r feasible-trade !
  16 panel-y 2+ at-xy s" Comerciar" 0 r> ?>option$ type  ;

: wipe-panel  ( -- )
  black paper 0 21 at-xy low-win-chars spaces  ;
  \ XXX TODO -- use window

: panel  ( -- )
  text-font set-font  white ink
  wipe-panel common-panel-commands
  aboard @ if    ship-panel-commands
           else  island-panel-commands
           then  directions-menu  ;
  \ XXX TODO check condition -- what about the enemy ship?
  \ XXX TODO several commands: attack ship/island/shark?

  \ ============================================================
  cr .( Landscape graphics)  \ {{{1

variable west-cloud-x  4 constant /west-cloud
variable east-cloud-x  3 constant /east-cloud

: sun-and-clouds  ( f -- )
  bright  yellow ink  cyan paper
  graph-font2 set-font
  26 0 at-xy ." AB"  26 1 at-xy ." CD"  white ink
  1 9 random-range dup west-cloud-x !
  dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  13 21 random-range dup east-cloud-x !
  dup 0 at-xy ." MNO"  1 at-xy ." PQR"
  graph-font1 set-font  0 bright  ;
  \ XXX TODO -- why the parameter, if this word is used only
  \ once?

: color-sky  ( c -- )
  [ sky-top-y attr-line ] literal
  [ sky-height columns * ] literal rot fill  ;
  \ Color the sky with attribute _c_.

: stormy-sky  ( -- )
  [ cyan dup papery + ] literal color-sky
  false sun-and-clouds  ;
  \ Make the sky stormy.

: sea-wave-coords  ( -- x y )
  1 28 random-range
  4 [ graphic-win-top graphic-win-height + 1- ] literal
  random-range  ;
  \ Return random coordinates _x y_ for a sea wave.

: at-sea-wave-coords  ( -- )  sea-wave-coords  at-xy  ;
  \ Set the cursor at random coordinates for a sea wave.

: sea-waves  ( -- )
  graph-font1 set-font cyan ink  blue paper
  15 0 do  at-sea-wave-coords ." kl"  at-sea-wave-coords ." mn"
  loop  ;

: sunny-sky  ( -- )
  [ cyan dup papery + brighty ] literal color-sky  ;
  \ Make the sky sunny.
  \ XXX FIXME -- why `sun-and-clouds` is not called here,
  \ like in `stormy-sky`?

: color-sea  ( c -- )
  [ sea-top-y attr-line ] literal
  [ sea-height columns * ] literal rot fill  ;
  \ Color the sea with attribute _c_.

: wipe-sea  ( -- )  [ blue dup papery + ] literal color-sea  ;

: sea-and-sky  ( -- )
  graphic-window graph-font1 set-font
  wipe-sea sea-waves sunny-sky  ;
  \ XXX TMP -- `graphic-window` is needed, because of the
  \ `wipe-panel` before the calling

  \ ============================================================
  cr .( Sea graphics)  \ {{{1

  \ --------------------------------------------
  cr .(   -Palms)  \ {{{2

: palm1  ( x y -- )
  green ink  blue paper  2dup    at-xy ." OPQR"
                         2dup 1+ at-xy ." S TU"  yellow ink
  1 under+  \ increment x
  1+ 2dup at-xy ." N"
  1+ 2dup at-xy ." M"
  1+      at-xy ." L"  ;
  \ Print palm model 1 at characters coordinates _x y_.
  \ XXX TODO -- factor the code common to `palm2`

: palm2  ( x y -- )
  green ink  yellow paper  2dup    at-xy ." OPQR"
                           2dup 1+ at-xy ." S TU"  black ink
  1 under+  \ increment x
  1+ 2dup at-xy ." N"
  1+ 2dup at-xy ." M"
  1+ 2dup at-xy ." L"
  1+      at-xy ." V"  ;
  \ Print palm model 2 at characters coordinates _x y_.
  \ XXX TODO -- factor the code common to `palm1`

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
  blue ink  green paper
   8 13 at-xy ."  HI Z123  HI A  A A  A "
  20 14 at-xy .\" B\::\::\::\::B"
  green ink  blue paper
  31 13 at-xy ." E"
  19 4 palm1  24 4 palm1  14 4 palm1
  black ink  green paper
  22 9 at-xy .\" \T\U"  \ the treasure
  ship-pos @ visited @ if
    s" Llegas nuevamente a la isla de " island-name$ s+ s" ."
  else
    s" Has encontrado la perdida isla de "
    island-name$ s+ s" ..."
  then  s+ message  r> set-font  ;
  \ XXX TODO -- factor

: wipe-island  ( -- )
  [ 3 attr-line ] literal
  [ 3 columns * ] literal
  [ yellow dup papery + ] literal fill  ;
  \ XXX TODO -- use constants

  \ --------------------------------------------
  cr .(   -Reefs)  \ {{{2

: .south-reef  ( -- )
  black ink  blue paper
  2 14 at-xy ."  A  HI   HI       HI  HI  A"
  0 15 at-xy .\" WXY  :\::\::\x7F     Z123     :\::\::\x7F"  ;

: .east-reef  ( -- )
  black ink  blue paper
   0 4 at-xy ." A"   1 6 at-xy ." HI"  0 8 at-xy ." WXY"
  1 11 at-xy ." A"  0 13 at-xy ." HI"  ;

: .west-reef  ( -- )
  black ink  blue paper
  30 4 at-xy ." HI"   28 6 at-xy ." A"
  29 7 at-xy ." WXY"  31 9 at-xy ." A"  ;

: reef?  ( n -- f )  sea @ reef =  ;
  \ Is there a reef at sea map position _n_?

: east-of-ship-pos  ( -- n )  ship-pos @ 1+  ;

: west-of-ship-pos  ( -- n )  ship-pos @ 1-  ;

: north-of-ship-pos  ( -- n )  ship-pos @ sea-length +  ;

: south-of-ship-pos  ( -- n )  ship-pos @ sea-length -  ;

: .reefs  ( -- )
  north-of-ship-pos  reef? if  .far-islands   then
  south-of-ship-pos  reef? if  .south-reef   then
  west-of-ship-pos   reef? if  .east-reef     then
  east-of-ship-pos   reef? if  .west-reef    then  ;

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
  ship-up @ if   .ship-down  ship-up off
           else  .ship-up    ship-up on   then  ;

: .enemy-ship  ( -- )
  yellow ink  blue paper
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."  ab"
                                2dup 1+ at-xy ."  90"
                                     2+ at-xy ." 678"  ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: wipe-enemy-ship  ( -- )
  blue paper
  enemy-ship-x @ enemy-ship-y @ 2dup    at-xy ."    "
                                2dup 1+ at-xy ."    "
                                     2+ at-xy ."    "  ;
  \ XXX TODO -- receive coordinates as parameters and reuse

: .boat  ( -- )
  yellow ink  blue paper  11 7 at-xy ." <>"  ;

: .shark  ( -- )
  white ink  blue paper  18 13 at-xy .\" \S"  ;
  \ XXX TODO -- `emit-udg` is faster

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
  graphic-window graph-font1 set-font
  sea-and-sky redraw-ship  ship-pos @ sea @ sea-picture  ;

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
  wipe-message  \ XXX TODO remove?
  graph-font1 set-font
  wipe-sea .far-islands .south-reef .east-reef .west-reef
  white ink 14 8 .ship-up .run-aground-reefs
  run-aground-damages run-aground-message
  3 seconds  ;

  \ XXX TODO improve message, depending on the damage, e.g.
  \ "Por suerte, ..."
  \
  \ XXX TODO choose more men, depending on the damage, and
  \ inform about them

  \ ============================================================
  cr .( Reports)  \ {{{1

white black papery + constant report-color#

: set-report-color  ( -- )
  report-color# color! permanent-colors  ;

: begin-report  ( -- )
  save-screen set-report-color cls text-font set-font  ;
  \ Common task at the start of all reports.

: end-report  ( -- )
  set-report-color
  0 row 2+ at-xy s" Pulsa una tecla" columns type-center
  discard-key key drop  restore-screen  ;
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

: .crew-member-data  ( n -- )
  >r white color!
  name-x r@ 6 + at-xy r@ name$ type
  r@ stamina @ stamina-attr c@ color!
  status-x r@ 6 + at-xy
  r> stamina @ stamina$ 2dup uppers1 type  ;

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

: miss-own-boat  ( -- )
  s" Por suerte el disparo no ha dado en el blanco." message  ;

: hit-own-boat  ( -- )
  s" La bala alcanza su objetivo. "
  s" Esto desmoraliza a la tripulación." s+ message
  -2 morale+!
  3 4 random-range 1 ?do  injured drop  loop  ;
  \ XXX TODO inform about how many injured?

: do-attack-own-boat  ( -- )
  -1 ammo+!
  s" Disparas por error a uno de tus propios botes..." message
  2 seconds
  3 random if    miss-own-boat
           else  hit-own-boat
           then  5 seconds wipe-message  ;

: almost-attack-own-boat  ( -- )
  s" Por suerte no hay munición para disparar..." message
  2 seconds
  s" Pues enseguida te das cuenta de que ibas a hundir "
  s" uno de tus botes." s+ message
  5 seconds wipe-message  ;

: attack-own-boat  ( -- )
  ammo @ if     do-attack-own-boat
         else   almost-attack-own-boat  then  ;

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

variable done
  \ XXX TODO -- remove; use the stack instead

: sunk  ( -- )
  .sunk 2 seconds

  \ ship-pos @ sea @ 13 >=
  \ ship-pos @ sea @ 16 <= and
  \ if  1 sunk-ships +!  1000 score +!  done on  then
    \ XXX OLD

  1 sunk-ships +!  1000 score +!  done on
    \ XXX TODO -- use constant to increase the score, and in
    \ the score report

  ship-pos @ sea @ case
    13 of  10  endof
    14 of   9  endof
    15 of   8  endof
    16 of   7  endof  dup endcase  ship-pos @ sea !  ;
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
  enemy-ship-x @ 1- enemy-ship-y @ 2+        at-xy ."  678 "
                                   2dup  1-  at-xy ."    "
                                         3 + at-xy ."    "
  enemy-ship-move @ 5 = if  .wave  then  ;
  \ XXX UNDER DEVELOPMENT
  \ XXX TODO -- factor

: move-enemy-ship  ( -- )

  \ enemy-ship-x @ enemy-ship-y @
  \ 2dup 2dup -1..1 + swap -1..1 + swap 2 d<>
  \ if  (move-enemy-ship)  then

  (move-enemy-ship) \ XXX TMP --

  ;
  \ XXX UNDER DEVELOPMENT

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
  \ Convert gun number _n_ (0..2) to its label _row_.

: gun>muzzle-y  ( n -- row )  gun>label-y 1+  ;
  \ Convert gun number _n_ (0..2) to its fire _row_.

: -cannon-ball  ( -- )  last-column gun-muzzle-y at-xy space  ;

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
  \ XXX FIXME -- the system crashes after `sunk`.

: no-ammo-left  ( -- )
  feasible-attack off  panel
  s" Te quedaste sin munición." message  4 seconds  ;
  \ XXX TODO the enemy wins; our ship sinks,
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

: clear-for-action  ( -- )
  text-font set-font black ink yellow paper
  22 0 do  0 i at-xy  ." ________ "  loop
  3 0 do
    i gun>label-y
    white paper text-font set-font 0 over at-xy i 1+ 1 .r
    yellow paper
    graph-font2 set-font  1+ 4 over .gun-man
    graph-font1 set-font     6 over .gun
                             1 swap 1+ at-xy ." hi"  \ ammo
  loop  ;
  \ XXX TODO -- factor

: battle-scenery  ( -- )
  blue paper cls 31 1 do  .wave  loop
  clear-for-action .ammo-label battle-init-enemy-ship  ;

: trigger  ( -- )
  inkey case  '1' of  0 fire  endof
              '2' of  1 fire  endof
              '3' of  2 fire  endof  endcase  ;
          \ XXX TODO -- use a table instead?

: (ship-battle)  ( -- )
  battle-scenery  done off
  begin  trigger move-enemy-ship done @ ammo @ 0= or until
  ammo @ ?exit no-ammo-left ;

: ship-battle  ( -- )
  save-screen (ship-battle) restore-screen  ;

: enemy-ship-here?  ( -- f )
  ship-pos @ sea @ 13 16 between  ;

: (attack-ship)  ( -- )
  enemy-ship-here? if    ship-battle
                   else  attack-own-boat  then  ;

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
  [ /island island-length - ] literal island-length
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

: set-island-location  ( -- )
  8 11 random-range i-pos !  ;
  \ XXX TODO -- improve: choose a random location on the coast,
  \ except the village

: new-island  ( -- )
  erase-island make-coast populate-island  ;

  \ ============================================================
  cr .( On the treasure island)  \ {{{1

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

: sailor-says  ( ca len -- )  sailor-window wcls wtype  ;

: trees  ( -- )
  wipe-island  black ink  yellow paper
  0 7 at-xy ."  1       2       3       4"
  graph-font1 set-font  27 2 do  i 3 palm2  8 +loop  ;

: treasure-found  ( -- )
  [ 0 attr-line ] literal [ 3 columns * ] literal
  [ cyan dup papery + brighty ] literal fill
  [ 4 attr-line ] literal [ 18 columns * ] literal
  [ yellow dup papery + ] literal fill
    \ XXX TODO -- factor the coloring
  sunny-sky

  23 7 do  i 5 palm2  5 +loop  3 7 palm2  26 7 palm2

  black ink  yellow paper  8 13 at-xy
  ." pq          xy                  "
  ." rs          vs                  tu      "
  .\" \T\U    wu"
  28 11 palm2  0 11 palm2
  graph-font2 set-font  blue ink  yellow paper
  13 17 at-xy .\" l\::m"
    \ XXX TODO -- factor the treasure

  s" ¡Capitán, somos ricos!" message
  4 seconds  graph-font1 set-font  ;
  \ XXX TODO use this proc instead of happy-end?
  \ XXX TODO -- factor

variable option

: enter-treasure-island  ( -- )

  \ XXX TODO finish the new interface

  cls sunny-sky wipe-island
  graph-font2 set-font green ink  yellow paper
  0 3 at-xy ."  5     6       45     6       5"
  black ink
  25 0 do
    i 3 + 3 at-xy .\" :\x7F"
    i 2+  4 at-xy .\" :\::\::\x7F"
    i 1+  5 at-xy .\" :\::\::\::\::\x7F"
    i     6 at-xy .\" :\::\::\::\::\::\::\x7F"
  8 +loop
  text-font set-font  white ink  red paper
  0 7 at-xy ."    1       2       3       4    "

  white ink  black paper
  22 8 do  0 i at-xy blank-line$ type  loop
    \ XXX TODO improve with `fill`

  sailor-and-captain

  s" ¿Qué camino, capitán?" sailor-says
  23 15 at-xy ." ?" \ XXX TODO better, in all cases
  9 get-digit option !
  black paper
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  option @ path @ = abs found-clues +!

  s" ¿Qué árbol, capitán?" sailor-says
  23 15 at-xy ." ? "
  9 get-digit option !
  text-font set-font
  black paper  23 15 at-xy option ?  beep .2,30
    \ XXX TODO -- factor out
  trees
  2 seconds
  option @ tree @ = abs found-clues +!

  black paper
  7 14 at-xy ." Izquierda Derecha"
  8 16 at-xy ." I=1  D=2 "
  23 15 at-xy ." ? " 9 get-digit option !
    \ XXX TODO use letters instead of digits
  text-font set-font
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  option @ turn @ = abs found-clues +!

  wipe-island
  black ink  yellow paper
  6 2 do
    1  i 1+ at-xy i 2-  dup . ."   " village$ type
    12 i 1+ at-xy i 3 + dup . ."   " village$ type
  loop
  12 7 at-xy ." 0  " villages 1- village$ type
  graph-font2 set-font
  green ink  27 5 at-xy .\" S\::T" 27 6 at-xy ." VUW"

  text-font set-font
  black paper
  7 14 at-xy ."  Poblado  " 7 13 at-xy ." ¿Cuál"
  8 16 at-xy ."  capitán." 23 15 at-xy ." ? "
  9 get-digit option !
  23 15 at-xy option  \ XXX TODO --
  beep .2,30
  2 seconds
  option village @ = if  1 found-clues +!  then  \ XXX TODO --

  7 13 at-xy ." ¿Qué camino"
  7 14 at-xy ." capitán?"
  7 16 at-xy ." 1N 2S 3E 4O"
  23 15 at-xy ." ? " 9 get-digit option !
    \ XXX TODO -- use letters instead of digits
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option direction @ = if  1 found-clues +!  then
    \ XXX TODO --

  7 13 at-xy ." ¿Cuántos"
  7 14 at-xy ." pasos,"
  7 16 at-xy ." capitán?"
  23 15 at-xy ." ? "
  9 get-digit option !
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option pace = if  1 found-clues +!  then  \ XXX TODO --

  black paper
  7 16  7 14  7 13
  success? if
    at-xy ." ¡Hemos encontrado"
    at-xy ." el oro,"
    at-xy ." capitán!"  treasure-found
  else
    at-xy ." ¡Nos hemos"
    at-xy ." equivocado"
    at-xy ." capitán!"
  then  2 seconds  graph-font1 set-font  ;
  \ XXX TODO -- use a window for messages
  \ XXX TODO -- factor

  \ ============================================================
  cr .( Island graphics)  \ {{{1

: wipe-island-scenery  ( -- )
  [ yellow dup papery + ] literal color-sea  ;
  \ XXX TODO -- print spaces instead

: .north-waves  ( -- )
  0 3 at-xy ."  kl  mn     nm    klk   nm nm n "  ;

: .south-waves  ( -- )
  0 14 at-xy ."  kl     mn  mn    kl    kl kl  m"
             ."     mn      klmn   mn m  mn     "  ;

: coast?  ( a -- f )  island @ coast =  ;
  \ Does cell _a_ of the island is coast?

: .west-waves  ( -- )
  16 3 do  0 i at-xy ."   "  loop
  0 6 at-xy ." mn" 0 10 at-xy ." kl" 0 13 at-xy ." k"
  0 4 at-xy ." m" 1 8 at-xy ." l"
  graph-font2 set-font yellow ink
  2 4 i-pos @ island-length + coast? 0= +
  at-xy 'A' emit
  i-pos @ island-length - coast?
  if  2 13 at-xy 'C' emit  then
  graph-font1 set-font  ;

: .east-waves  ( -- )
  16 3 do  30 i at-xy ."   "  loop
  30 6 at-xy ." mn" 30 10 at-xy ." kl" 31 13 at-xy ." k"
  30 4 at-xy ." m" 31 8 at-xy ." l"
  yellow ink  graph-font2 set-font
  i-pos @ island-length + coast?
  if    29  4 at-xy 'B' emit  then
  29  i-pos @ island-length - coast?
  if    13 at-xy 'D'
  else   3 at-xy 'B'
  then  emit  graph-font1 set-font  ;

: .village  ( -- )
  graph-font2 set-font  green ink  yellow paper
  6  5 at-xy .\"  S\::T    ST   S\::T"
  6  6 at-xy .\"  VUW    78   VUW   4"
  4  8 at-xy .\" S\::T   S\::T    S\::T S\::T  S\::T "
  4  9 at-xy ." VUW   VUW  4 VUW VUW  VUW"
  4 11 at-xy .\" S\::T    S\::T ST  S\::T S\::T"
  4 12 at-xy ." VUW  4 VUW 78  VUW VUW"
  black ink  yellow paper
   7 12 at-xy ." X"
  17 12 at-xy ." Y"
  22 12 at-xy ." Z"
  26 12 at-xy ." XY"
   8  9 at-xy ." ZZ"
  13  9 at-xy ." Y"
  24  9 at-xy ." ZX"
  10  6 at-xy ." XYZ"
  17  6 at-xy ." YX"
  26  6 at-xy ." Z"  graph-font1 set-font  ;

: .native  ( -- )
  black ink  yellow paper  8 10 at-xy ."  _ `"
                           8 11 at-xy ." }~.,"
                           8 12 at-xy ." {|\?"  ;

: .ammo-gift  ( -- )
  black ink  yellow paper  14 12 at-xy ." hi"  ;
  \ XXX TODO draw graphics depending on the actual ammount

: .supplies  ( -- )
  graph-font2 set-font
  black ink  yellow paper 14 12 at-xy ." 90  9099 0009"
  graph-font1 set-font  ;
  \ XXX TODO draw graphics depending on the actual ammount

: .snake  ( -- )
  graph-font2 set-font
  black ink  yellow paper  14 12 at-xy ." xy"
  graph-font1 set-font  ;

: .dubloons  ( n -- )
  get-font >r graph-font2 set-font  black ink  yellow paper
  12 dup at-xy s" vw vw vw vw vw vw vw vw " drop swap 3 * type
  r> set-font  ;
  \ XXX TODO -- use a loop

: island-waves  ( -- )
  graph-font1 set-font white ink  blue paper
  i-pos @ island-length - coast?
  if  .south-waves   then
  i-pos @ island-length + coast?
  if  .north-waves  then
  i-pos @ 1-  coast?
  if  .west-waves     then
  i-pos @ 1+  coast?
  if  .east-waves    then  ;

: (.island-location)  ( n -- )
  ~~ case
    native-village  of  .village                         endof
    dubloons-found  of  4 8 palm2 14 5 palm2             endof
      \ XXX TODO -- print dubloons here
    hostile-native  of  ~~ 14 5 palm2 25 8 palm2 .native  endof
    just-3-palms-1  of  25 8 palm2  4 8 palm2 16 5 palm2 endof
    snake of
      13 5 palm2 5 6 palm2 18 8 palm2 23 8 palm2 .snake
                                                         endof
    just-3-palms-2  of  23 8 palm2 17 5 palm2 4 8 palm2  endof
    native-supplies of  ~~ .supplies  .native  16 4 palm2 endof
    native-ammo     of  ~~ .ammo-gift .native 20 5 palm2  endof
  endcase  ~~ ;

: .island-location  ( -- )
  i-pos @ island @ ~~ (.island-location)  ;

: island-scenery  ( -- )
  graphic-window graph-font1 set-font
  wipe-island-scenery sunny-sky island-waves
  ~~ .island-location  ;

  \ ============================================================
  cr .( Events on an island)  \ {{{1

: event1  ( -- )
  dead name$ s"  se hunde en arenas movedizas." s+ message  ;

: event2  ( -- )
  dead name$ s"  se hunde en un pantano." s+ message  ;

: event3  ( -- )
  s" A " injured name$ s+
  s"  le muerde una araña." s+ message  ;

: event4  ( -- )
  s" A " injured name$ s+ s"  le pica un escorpión." s+
  message  ;

: event5  ( -- )
  s" La tripulación está hambrienta." message
  -1 morale+!  ;
  \ XXX TODO only if supplies are not enough

: event6  ( -- )
  s" La tripulación está sedienta." message
  -1 morale+!  ;
  \ XXX TODO only if supplies are not enough

: event7  ( -- )
  2 5 random-range >r
  s" Encuentras " r@ coins$ s+ s" ." s+ message
  r@ cash+!  r> .dubloons  ;

: event8  ( -- )
  s" Sin novedad, capitán." message  ;

: event9  ( -- )
  s" La costa está despejada, capitán." message  ;

create island-events-table  ( -- a )  here

] event1 event2 event3 event4 event5 event6
  event7 event8 event8 event9 event9 noop noop [

here - cell / constant island-events

: island-event  ( -- )
  island-events random island-events-table array> perform  ;

  \ ============================================================
  cr .( Enter island location)  \ {{{1

: be-hostile-native  ( -- )
  hostile-native i-pos @ island !  ;

: (enter-island-location)  ( n -- )

  case

  snake of  ~~
    s" Una serpiente ha mordido a "
    injured name$ s+ s" ." s+ message
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

    just-3-palms-1 i-pos @ island !
      \ XXX FIXME -- This changes the type of location, what
      \ makes the picture different.  This is a problem of the
      \ original game.  The dubloons must be independent from
      \ the location type.

  endof

  native-ammo of  ~~
    s" Un nativo te da algo de munición." message
    1 ammo+!  be-hostile-native
      \ XXX TODO random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `.island-location`
  endof

  native-supplies of  ~~
    s" Un nativo te da provisiones." message
    1 supplies+!  be-hostile-native
      \ XXX TODO random ammount
      \ XXX TODO -- choose it in advance and draw it in
      \ `.island-location`
  endof

  native-village of  ~~
    s" Descubres un poblado nativo." message
  endof

  just-3-palms-1 of  island-event  endof

  just-3-palms-2 of  island-event  endof

  ~~ endcase  ;

: enter-island-location  ( -- )
  wipe-message  \ XXX TODO needed?
  island-scenery
  i-pos @ island @ ~~ (enter-island-location)  ;

  \ ============================================================
  cr .( Disembark)  \ {{{1

: disembarking-scene  ( -- )
  graph-font1 set-font  green ink  blue paper
  31  8 at-xy ." :"
  27  9 at-xy .\" HI :\::"
  25 10 at-xy .\" F\::\::\::\::\::\::"
  23 11 at-xy .\" JK\::\::\::\::\::\::\::"
  yellow ink blue paper
  21 0 do  i 11 at-xy ."  <>" 200 ms  loop  ;

: on-treasure-island?  ( -- f )
  ship-pos @ sea @ treasure-island =  ;

: enter-ordinary-island  ( -- )
  new-island set-island-location enter-island-location  ;

: enter-island  ( -- )
  aboard off  on-treasure-island?
  if    enter-treasure-island
  else  enter-ordinary-island  then  ;

: disembark  ( -- )
  -2 -1 random-range supplies+!
  wipe-message sea-and-sky disembarking-scene enter-island  ;

  \ ============================================================
  cr .( Storm)  \ {{{1

2 constant rain-y

: at-rain  ( a -- )  @ rain-y at-xy  ;

: at-west-cloud-rain  ( -- )  west-cloud-x at-rain  ;

: at-east-cloud-rain  ( -- )  east-cloud-x at-rain  ;

: rain-drops  ( c -- )
  dup  at-west-cloud-rain /west-cloud emits
       at-east-cloud-rain /east-cloud emits  60 ms  ;

: +rain  ( -- )
  graph-font1 set-font
  70 0 do
    white ink  cyan paper
    ';' rain-drops  ']' rain-drops  '[' rain-drops
    3 random 0= if  redraw-ship  then
  loop  ;
  \ Make the rain effect.
  \ XXX TODO -- random duration

: -rain  ( -- )  at-west-cloud-rain /west-cloud spaces
                 at-east-cloud-rain /east-cloud spaces  ;
  \ Erase the rain effect.
  \ XXX FIXME -- wrong color

: storm  ( -- )
  wipe-panel stormy-sky
  s" Se desata una tormenta"
  s"  que causa destrozos en el barco." s+ message
  +rain  10 49 damaged  -rain
  s" Tras la tormenta, el barco está " damage$ s+ s" ." s+
  message  panel  ;
  \ XXX FIXME -- sometimes `damage$` is empty: check the range
  \ of the damage percentage.
  \
  \ XXX TODO bright sky!
  \ XXX TODO -- sound
  \ XXX TODO make the enemy ship to move, if present
  \ (use the same graphic of the player ship)

  \ ============================================================
  cr .( Ship command)  \ {{{1

: to-reef?  ( n -- f )  ship-pos @ + reef?  ;
  \ Does the sea movement offset _n_ leads to a reef?

: sea-move  ( n -- )
  dup to-reef? if    drop run-aground
              else  ship-pos +!  then  ;
  \ Move on the sea map, using offset _n_ from the current
  \ position.

: ?sea-move-north?  ( -- f )
  possible-north @ dup 0exit  sea-length sea-move  ;

: ?sea-move-south?  ( -- f )
  possible-south @ dup 0exit  sea-length negate sea-move  ;

: ?sea-move-east?  ( -- f )
  possible-east @ dup 0exit  1 sea-move  ;

: ?sea-move-west?  ( -- f )
  possible-west @ dup 0exit  -1 sea-move  ;

: ship-command?  ( c -- f )
  dup 0exit  case
  'N' key-up                or-of  ?sea-move-north?    endof
  'S' key-down              or-of  ?sea-move-south?    endof
  'E' key-right             or-of  ?sea-move-west?     endof
  'O' key-left              or-of  ?sea-move-west?     endof
  'I'                          of  main-report    true endof
  'A' feasible-attack @ and    of  attack-ship    true endof
  'T'                          of  crew-report    true endof
  'P'                          of  score-report   true endof
  'D' feasible-disembark @ and of  disembark      true endof
  'F'                          of  quit-game on   true endof
  'Q'                          of  quit                endof
    \ XXX TMP -- for debugging
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
  \ XXX TODO increase the probability of storm every day?

: ?storm  ( -- )  storm? if  storm  then  ;

: ship-command  ( -- )
  begin  ?redraw-ship ?storm  inkey upper ship-command?
  until  ;

  \ ============================================================
  cr .( Misc commands on the island)  \ {{{1

: embark  ( -- )
  ship-pos @ visited on  1 day +!  aboard on  ;

: to-land?  ( n -- f )  i-pos @ + island @ coast <>  ;
  \ Does the island movement offset _n_ leads to land?

: island-move  ( n -- )
  dup to-land? if    i-pos +!  enter-island-location
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
  beep .2,10
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
    \ XXX TODO improve with `fill`
  4 4 palm2  .native native-speech-balloon
  s" Un comerciante nativo te sale al encuentro." message  ;

: trade  ( -- )
  init-trade  s" Yo vender pista de tesoro a tú." native-says
  5 9 random-range price !
  s" Precio ser " price @ coins$ s+ s" ." s+ native-says
  \ XXX TODO pause or join:
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

: label  ( "name" -- )  parse-name 2drop  ; immediate
  \ XXX TMP --
: goto   ( "name" -- )  parse-name 2drop  ; immediate
  \ XXX TMP --

: impossible  ( -- )
  s" Lo siento, capitán, no puede hacer eso." message
  2 seconds  ;
  \ XXX not used yet

: attack  ( -- )

  \ XXX OLD -- commented out in the original
  \ if island-map(i-pos)=2 or island-map(i-pos)=4 or island-map(i-pos)=6 then \
  \   gosub @impossible
  \   gosub @island-panel
  \   exit proc

  s" Atacas al nativo..." message \ XXX OLD
  2 seconds

  i-pos @ island @ 5 = if
    \ XXX TODO --  5=snake?
    s" Lo matas, pero la serpiente mata a "
    dead name$ s+ s" ." s+ message
    goto L6897
  then

  i-pos @ island @ native-village = if
    s" Un poblado entero es un enemigo muy difícil. "
    dead name$ s+ s"  muere en el combate." s+
    message
    goto L6898
  then

  1 5 random-range case
  1 of  s" El nativo muere, pero antes mata a "
        dead @ name$ s+ s" ." s+ message              endof
  2 of  s" El nativo tiene provisiones"
        s"  escondidas en su taparrabos." s+ message
        1 supplies+!                                 endof

    2 3 random-range r>
    s" Encuentras " r@ coins$ s+
    s"  en el cuerpo del nativo muerto." s+ message
    r> cash+!

  endcase

  graph-font2 set-font  black ink  yellow paper yellow
  14 10 do  8 i at-xy ." t   "  loop
  black ink  yellow paper  8  9 at-xy ." u"
  white ink  black  paper  9 10 at-xy ." nop"
                           9 11 at-xy ." qrs"
  graph-font1 set-font

  label L6897

  4 i-pos @ island !
    \ XXX TODO -- constant for 4

  label L6898

  3 seconds  ;

  \ ============================================================
  cr .( Command dispatcher on the island)  \ {{{1

: island-move-north  ( -- )  island-length island-move  ;

: ?island-move-north?  ( -- f )
  possible-north @ dup 0exit  island-move-north  ;

: island-move-south  ( -- )
  island-length negate island-move  ;

: ?island-move-south?  ( -- f )
  possible-south @ dup 0exit  island-move-south  ;

: island-move-east  ( -- )  1 island-move  ;

: ?island-move-east?  ( -- f )
  possible-east @ dup 0exit  island-move-east  ;

: island-move-west  ( -- )  -1 island-move  ;

: ?island-move-west?  ( -- f )
  possible-west @ dup 0exit  island-move-west  ;

: ?trade?  ( -- f )  feasible-trade @ dup 0exit  trade  ;

: ?embark?  ( -- f )  feasible-embark @ dup 0exit  embark  ;

: ?attack?  ( -- f )  feasible-attack @ dup 0exit  attack  ;

: island-command?  ( c -- f )
  case
    'N' key-up    or-of  ?island-move-north? endof
    'S' key-down  or-of  ?island-move-south? endof
    'E' key-right or-of  ?island-move-east?  endof
    'O' key-left  or-of  ?island-move-west?  endof
    'C'              of  ?trade?             endof
    'B'              of  ?embark?            endof
    'I'              of  main-report   true  endof
    'M'              of  ?attack?            endof
    'T'              of  crew-report   true  endof
    'P'              of  score-report  true  endof
    'F'              of  quit-game on  true  endof
    'Q'              of  quit                endof
      \ XXX TMP -- for debugging
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
  literal literal add-row-reefs  ;

: add-col-reefs  ( n1 n0 -- )
  ?do  reef i sea !  sea-length +loop  ;

: add-east-reefs  ( -- )
  [ sea-breadth 2- sea-length * 1+ ] literal sea-length
  add-col-reefs  ;

: add-west-reefs  ( -- )
  [ sea-length 2* 1-  /sea sea-length - ]
  literal literal add-col-reefs  ;

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
  32 42 random-range ship-pos !  9 ship-y !  4 ship-x !
  ship-up off  ;

: init-clues  ( -- )
  1 3 random-range path !  \ XXX TODO -- check range 0..?
  1 3 random-range tree !  \ XXX TODO -- check range 0..?
  0 9 random-range village !
  0 1 random-range turn !
  0 3 random-range direction !
  1 9 random-range pace !  ;  \ XXX TODO -- check range 0..?
  \ XXX TODO -- use `random` for 0..x
  \ XXX TODO -- convert all ranges to 0..x

: init-plot  ( -- )
  init-clues  aboard on  1 i-pos !
  men alive !  2 ammo !  5 cash !  10 morale !  10 supplies !
  quit-game off  damage off  day off  found-clues off
  score off  sunk-ships off  trades off  ;

: unused-name  ( -- n )
  0  begin  drop  0 [ stock-names 1- ] literal random-range
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
  [ 2 attr-line ] literal [ 20 columns * ] literal erase
    \ XXX TODO -- check if needed
    \ XXX TODO -- use constant to define the zone
  white ink  black paper text-font set-font
  0 14 at-xy s" Preparando el viaje..." columns type-center
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
  the-end-window  black ink  yellow paper
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
  key drop beep .2,30 score-report  ;
  \ XXX TODO new graphic, based on the cause of the end

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
  skull-border intro-window whome
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
  aboard @ if    sea-scenery
           else  island-scenery
           then  panel  ;

: command  ( -- )
  aboard @ if  ship-command  else  island-command  then  ;

: game  ( -- )
  cls  screen-restored off
  begin
    screen-restored @ if    screen-restored off
                     else  scenery
                     then  command
  game-over? until  ;

: run  ( -- )
  init-once  begin  intro init game the-end  again  ;

  \ ============================================================
  cr .( Debugging tools [2])  \ {{{1

variable checkered

: +checkered  ( -- )  checkered @ inverse  ;

: -checkered  ( -- )  checkered @ 0= checkered !  ;

: .sea  ( -- )
  cr
  sea-breadth 0 do
    sea-length 0 do
      +checkered j sea-length * i + sea @ 2 .r
      -checkered
    loop  cr
  loop  0 inverse  ;

: .isl  ( -- )
  cr
  island-breadth 0 do
    island-length 0 do
      +checkered j island-length * i + island @ 2 .r
      -checkered
    loop  cr
  loop  0 inverse  ;

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
  max-damage 1+ 0 do
    cr i . damage-index . damage$ type  key drop
  loop  ;

: ini  ( -- )  init-once init  ;

: f  ( -- )  rom-font set-font  ;
  \ XXX TMP for debugging after an error

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
