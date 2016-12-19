( bandera-negra )

  \ Bandera negra
  \
  \ A simulation game
  \ Written in Forth for the ZX Spectrum 128

  \ This game is a translated and improved remake of
  \   "Jolly Roger"
  \   Copyright (C) 1984 Barry Jones / Video Vault ltd.

  \ Copyright (C) 2011,2014,2015,2016 Marcos Cruz (programandala.net)

  \ Version 0.0.0+201612200014
  \
  \ Note: Version 0.0.0 indicates the conversion from Master
  \ BASIC to Forth is still in progress.

  \ ============================================================
  \ Requirements {{{1

only forth definitions

need chars>string  need string/  need columns  need inverse
need random-range  need at-x  need row  need ruler  need pick
need 2avariable  need avariable  need cavariable
need sconstants  need /sconstants  need case  need >=  need s+
need or-of  need inkey  need <=  need j  need tab  need uppers1
need pause  need under+  need type-center  need set-pixel
need rdraw

need black  need blue  need red  need green
need cyan  need yellow  need white  need color!
need papery  need brighty

wordlist dup constant game-wordlist  dup >order  set-current

  \ ============================================================
  \ Constants {{{1

15 constant seaMapCols
 9 constant seaMapRows

seaMapCols seaMapRows * constant /seaMap
  \ cells of the sea map

30 constant /islandMap
  \ cells of the island map

10 constant men

: islandName$  ( -- ca len )  s" Calavera"  ;

: shipName$  ( -- ca len )  s" Furioso"  ;
  \ XXX TODO -- not used yet

  \ Ids of sea and island cells
  \ XXX TODO complete
 1 constant reef
 1 constant coast
21 constant shark
22 constant treasureIsland

  \ Ids of island cells
  \ XXX TODO complete
2 constant dubloonsFound
3 constant nativeFights
\ 4 constant \ XXX TODO --
5 constant snake
7 constant nativeSupplies
8 constant nativeAmmo
9 constant nativeVillage

9 constant maxOffer

 3 constant seaTopY
13 constant seaHeight  \ screen lines
 0 constant skyTopY
 3 constant skyHeight  \ screen lines

  \ --------------------------------------------
  \ Windows parameters
                                  3 constant introWinTop
                                  0 constant graphicWinTop
                                 15 constant graphicWinBottom
                                  0 constant graphicWinLeft
                                 31 constant graphicWinRight
graphicWinRight graphicWinLeft - 1+ constant graphicWinWidth
graphicWinBottom graphicWinTop - 1+ constant graphicWinHeight
 graphicWinWidth graphicWinHeight * constant graphicWinChars
                                 21 constant lowWinTop
                                 23 constant lowWinBottom
                                  0 constant lowWinLeft
                                 31 constant lowWinRight
        lowWinRight lowWinLeft - 1+ constant lowWinWidth
        lowWinBottom lowWinTop - 1+ constant lowWinHeight
         lowWinWidth lowWinHeight * constant lowWinChars
                                 17 constant messageWinTop
                                 19 constant messageWinBottom
                                  1 constant messageWinLeft
                                 30 constant messageWinRight
messageWinRight messageWinLeft - 1+ constant messageWinWidth
messageWinBottom messageWinTop - 1+ constant messageWinHeight
 messageWinWidth messageWinHeight * constant messageWinChars

  \ ============================================================
  \ Variables {{{1

variable quitGame         \ flag
variable screenRestored   \ flag

  \ --------------------------------------------
  \ Plot {{{2

variable iPos             \ player position on the island
variable aboard           \ flag
variable alive            \ counter
variable ammo             \ counter
variable cash             \ counter
variable damage           \ counter
variable day              \ counter
variable morale           \ counter
variable score            \ counter
variable sunkShips        \ counter
variable supplies         \ counter
variable trades           \ counter

  \ --------------------------------------------
  \ Ships {{{2

variable shipPicture      \ flag
variable shipX
variable shipY
variable shipPos

variable enemyShipMove
variable enemyShipX
variable enemyShipY

  \ --------------------------------------------
  \ Clues {{{2

variable foundClues       \ counter

variable path
variable tree
variable village
variable turn
variable direction
variable pace

  \ ============================================================
  \ Arrays {{{1

  \ --------------------------------------------
  \ Maps {{{2

/seaMap     avariable seaMap
/islandMap  avariable islandMap
/seaMap     avariable visited    \ flags for islands

  \ --------------------------------------------
  \ Crew {{{2

men avariable stamina

  \ Crew names are pun funny names in Spanish:

0
  here ," Alfredo Minguero"
  here ," Armando Bronca"
  here ," Borja Monserrano"
  here ," Clemente Cato"
  here ," César Pullido"
  here ," Enrique Sitos"
  here ," Erasmo Coso"
  here ," Felipe Llejo"
  here ," Javi Oneta"
  here ," Javier Nesnoche"
  here ," Jorge Neral"
  here ," Jorge Ranio"
  here ," Lope Lotilla"
  here ," Manolo Pillo"
  here ," Marcos Tilla"
  here ," Melchor Icete"
  here ," Néstor Nillo"
  here ," Néstor Tilla"
  here ," Paco Tilla"
  here ," Pascual Baricoque"
  here ," Pedro Medario"
  here ," Policarpio Nero"
  here ," Ramiro Inoveo"
  here ," Ricardo Borriquero"
  here ," Roberto Mate"
  here ," Rodrigo Minolas"
  here ," Ulises Cocido"
  here ," Ulises Tantería"
  here ," Vicente Rador"
  here ," Víctor Nillo"
  here ," Víctor Tilla"
  here ," Zacarías Queroso"
  here ," Óscar Romato"
  here ," Óscar Terista"
/sconstants names$  ( n -- ca len )
constant names

men 2avariable name  ( n -- a )
  \ A double-cell array to hold the address and length
  \ of the names of the crew members, compiled in `names$`.

: name$  ( n -- ca len )  name 2@  ;

names men avariable usedNames  ( n -- a )
  \ An array to hold a true flag when the correspondent name
  \ in `names$` has been used in `name`. The goal is to prevent
  \ name duplicates in the crew.

0
  here ," en forma"
  here ," magullado"
  here ," herido leve"
  here ," herido grave"
  here ," muerto"
/sconstants stamina$  ( n -- ca len )
1- constant maxStamina
 0 constant minStamina

maxStamina 1+ cavariable staminaAttr

 black white papery +           0 staminaAttr c!
   red black papery + brighty + 1 staminaAttr c!
   red black papery +           2 staminaAttr c!
yellow black papery +           3 staminaAttr c!
 green black papery +           4 staminaAttr c!

  \ --------------------------------------------
  \ Ship damage descriptions {{{2

0
  here ," hundiéndose"            \ worst: sinking
  here ," a punto de hundirse"
  here ," haciendo agua"
  here ," destrozado"
  here ," casi destrozado"
  here ," gravemente dañado"
  here ," muy dañado"
  here ," algo dañado"
  here ," muy poco dañado"
  here ," casi como nuevo"
  here ," impecable"            \ best: perfect
/sconstants damageLevel$  ( n -- ca len )
            constant damageLevels

  \ --------------------------------------------
  \ Village names {{{2

  \ The names of the villages are Esperanto compound words with
  \ funny sounds and meanings.

0
  here ," Mislongo"   \ mis-long-o= "wrong lenght"
  here ," Ombreto"    \ ombr-et-o= "little shadow"
  here ," Figokesto"  \ fig-o-kest-o
  here ," Misedukota" \ mis-eduk-ot-a= "one to be miseducated"
  here ," Topikega"   \ topik-eg-a=
  here ," Fibaloto"   \ fi-balot-o
  here ," Pomotruko"  \ pom-o-truk-o
  here ," Putotombo"  \ put-o-tomb-o
  here ," Ursorelo"   \ urs-orel-o= "ear of bear"
  here ," Kukumemo"   \ kukum-em-o
/sconstants village$  ( n -- ca len )
            constant villages
  \ XXX TODO -- invert the order of the strings?

  \ --------------------------------------------
  \ Cardinal points {{{2

0
  here ," oeste"
  here ," este"
  here ," sur"
  here ," norte"
sconstants cardinal$  ( n -- ca len )

  \ --------------------------------------------
  \ Hands {{{2

0
  here ," derecha"    \ right
  here ," izquierda"  \ left
sconstants hand$  ( n -- ca len )

  \ ============================================================
  \ Functions {{{1

22528 constant attributes
  \ Address of the screen attributes (768 bytes)

: attrLine  ( l -- a )  columns * attributes +  ;
  \ First attribute address of a character line.

: >attr  ( paper ink bright -- c )  64 * + swap 8 * +  ;
  \ Convert _paper_, _ink_ and _bright_ to an attribute byte
  \ _c_.

: dubloons$  ( n -- ca len )
  s" dobl " rot 1 > if  s" ones"  else  s" On"  then  s+  ;
  \ Return string "doubloon" or "doubloons", depending on _n_.

: number$  ( n -- ca len )
  case   1 of  s" un"     endof
         2 of  s" dos"    endof
         3 of  s" tres"   endof
         4 of  s" cuatro" endof
         5 of  s" cinco"  endof
         6 of  s" seis"   endof
         7 of  s" siete"  endof
         8 of  s" ocho"   endof
         9 of  s" nueve"  endof
        10 of  s" diez"   endof
        11 of  s" once"   endof  endcase  ;
  \ Convert _n_ to text in string _ca len_.
  \
  \ XXX TODO -- use a faster vector table instead, or
  \ `sconstants`

: highlighted$  ( c -- ca len )
  0 20 rot 1 20 5 chars>string  ;
  \ Convert _c_ to a string to print _c_ as a highlighted char.

: activeOption$  ( ca1 len1 n -- ca2 len2 )
  >r 2dup r@ 1- string/
  2over drop r@ + c@ highlighted$ s+
  2swap r> 1+ /string s+  ;
  \ Convert menu option _ca len_ to an active menu option
  \ with character at position _n_ highlighted with control
  \ characters.

: option$  ( ca1 len1 n f -- ca1 len1 | ca2 len2 )
  if  activeOption$  then  ;
  \ Prepare a panel option _ca1 len1_.  If the option is
  \ active, _f_ is true and _n_ is the position of its
  \ highlighted letter.

: coins$  ( x -- ca len )
  dup >r number$ s"  " s+ r> dubloons$ s+  ;
  \ Return the text "x doubloons", with letters.

: damageIndex  ( -- n )  damage @ damageLevels * 101 / 1+  ;
  \ XXX TODO -- use `*/`

: failure?  ( -- f )
  alive @ 0=
  morale @ 1 < or
  damageIndex damageLevels = or
  supplies @ 1 < or
  cash @ 1 < or  ;
  \ Failed mission?

6 constant maxClues

: success?  ( -- f )  foundClues @ maxClues =  ;
  \ Success?

: gameOver?  ( -- f )  failure? success? quitGame or or  ;
  \ Game over?

: condition$  ( n -- ca len )  stamina @ stamina$ 2@  ;
  \ Physical condition of a crew member

: blankLine$  ( -- ca len )  bl columns ruler  ;

: damage$  ( -- ca len )  damageIndex damageLevel$  ;
  \ Damage description

  \ ============================================================
  \ UDGs and charsets {{{1

  \ XXX TODO keep all the charsets and UDGs in RAM

: charset  ( n -- )  drop  ;
  \ XXX TODO --

: c0  ( -- )  0 charset  ;
  \ XXX TMP for debugging after an error

: initUDG  ( -- )
  \ load "udg128" code udg chr$ 128 \ Spanish chars 128-143
  \ load "udg144" code udg chr$ 144 \ Graphics 144-168
  ;
  \ XXX TODO --

  \ ============================================================
  \ Windows {{{1

: cls1  ( -- )  ;
  \ XXX TODO --

: window  ( leftX rightX topY bottomY -- )  2drop 2drop  ;
  \ XXX TODO --

: noWindow  ( -- )  0 31 0 23 window  ;

: wholeWindow  ( -- )  0 31 0 20 window  ;

: graphicWindow  ( -- )
  graphicWinLeft graphicWinRight graphicWinTop graphicWinBottom
  window  1 charset  ;
  \ Zone where graphics are shown.

: introWindow  ( -- )  2 29 introWinTop 21 window  ;
  \ Zone where intro text is shown.

: messageWindow  ( -- )
  messageWinLeft messageWinRight messageWinTop messageWinBottom
  window  ;

: commandWindow  ( -- )
  lowWinLeft lowWinRight lowWinTop lowWinBottom  window  ;

: nativeWindow  ( -- )  16 26 6 9 window  ;
  \ Window for native's speech.

  \ ============================================================
  \ Screen {{{1

: initScreen  ( -- )  cls graphicWindow commandWindow  ;

: wipePanel  ( -- )
  black paper 0 lowWinTop at-xy lowWinChars spaces  ;

: wipeMessage  ( -- )
  messageWindow  white ink  black paper  cls1  ;

: saveScreen  ( -- )
  \ copy screen 1 to 2  \ XXX OLD
  ;
  \ XXX TODO --

: restoreScreen  ( -- )
  \ copy screen 2 to 1  \ XXX OLD
  screenRestored on  ;
  \ XXX TODO --

: screen  ( n -- )  drop  ;
  \ XXX TODO --

: useScreen2  ( -- )  saveScreen 2 screen  ;

: useScreen1  ( -- )  restoreScreen 1 screen  ;

  \ ============================================================
  \ Text output {{{1

: tell  ( ca len -- )
  0 charset
  begin  dup columns >  while
    \ for char=cpl to 1 step -1 \ XXX OLD
    0 columns do
      over i + c@ bl = if
        2dup drop i 1- type  i 1+ string/  unloop leave
      then
    -1 +loop
  repeat  type  ;

: tellCR  ( ca len -- )  tell cr  ;

: nativeSays  ( ca len -- )  nativeWindow cls1 tell  ;

: message  ( ca len -- )
  0 charset  wipeMessage messageWindow tell graphicWindow  ;

: tellZone  ( ca len n x y -- )
  0 charset
  begin  2over <=  while
    2over nip 0 swap ( 0 n ) do
      4 pick i + c@ bl = if
        2dup at-xy
        2>r >r  2dup drop i 1- type  i 1+ string/  r> 2r>
        1+  unloop leave
      then
    -1 +loop
  repeat  at-xy drop type  ;
  \ Print _ca len_ at _x y_ on a window of _n_ chars width.
  \ XXX TODO use WINDOW instead

  \ ============================================================
  \ Sound  {{{1

: beep  ( "ccc" -- )  parse-name 2drop  ; immediate
  \ XXX TMP --
  \ XXX TODO --

  \ ============================================================
  \ User input {{{1

: seconds  ( n -- )  50 * pause  ;

: getDigit  ( n1 -- n2 )
  begin  dup 0 pause inkey '0' - dup >r
         1 < over r@ < or  while  r> drop beep .1,10
  repeat  drop r>  ;
  \ Wait for a digit to be pressed by the player, until its
  \ value is greater than 0 and less than _n1_, then return it
  \ as _n2_.

  \ ============================================================
  \ Command panel {{{1

22 constant panel-y

variable possibleDisembarking   \ flag
variable possibleEmbarking      \ flag
variable possibleAttacking      \ flag
variable possibleTrading        \ flag

variable possibleNorth          \ flag
variable possibleSouth          \ flag
variable possibleEast           \ flag
variable possibleWest           \ flag

: .direction  ( c col row f -- )
  inverse at-xy emit 0 inverse  ;

: directionsMenu  ( -- )
  possibleNorth on
  possibleSouth on
  possibleEast on
  possibleWest on
    \ XXX TMP --
    \ XXX TODO -- use conditions
  white ink  black paper
  'N' 30 panel-y    possibleNorth @ .direction
  'O' 29 panel-y 1+ possibleWest  @ .direction
  'E' 31 panel-y 1+ possibleEast  @ .direction
  'S' 30 panel-y 2+ possibleSouth @ .direction
  '+' 30 panel-y 1+ at-xy emit  ;
  \ Print the directions menu.
  \
  \ XXX TODO use a modified  version of "+"?

: panel  ( -- )
  wipePanel  0 charset  white ink
  0 panel-y at-xy s" Información" 1 true option$ type cr
                  s" Tripulación" 1 true option$ type cr
                  s" Puntuación"  1 true option$ type cr

  aboard @ if

    \ XXX TODO -- `possibleDisembarking` only if no enemy ship
    \ is present

    shipPos @ visited @ 0=
    shipPos @ seaMap @ treasureIsland =  or
      \ XXX TODO -- factor both conditions
    possibleDisembarking !

    0 panel-y at-xy
    s" Desembarcar" 1 possibleDisembarking @ option$ type

  else

    possibleEmbarking on
      \ XXX TODO only if iPos is coast
    16 panel-y at-xy
    s" emBarcar" 3 possibleEmbarking @ option$ type

  then

  \ XXX TODO check condition -- what about the enemy ship?
  \ XXX TODO several commands: attack ship/island/shark?
  shipPos @ seaMap @ 13 < 0=
  shipPos @ seaMap @ shark = or
  shipPos @ seaMap @ treasureIsland = or
  possibleAttacking !
    \ XXX TODO -- improve

  16 panel-y 1+ at-xy
  s" Atacar" 1 possibleAttacking @ option$ type

  iPos @ islandMap @ nativeVillage = possibleTrading !

  16 panel-y 2+ at-xy
  s" Comerciar" 1 possibleTrading @ option$ type

  directionsMenu  ;

: impossible  ( -- )
  s" Lo siento, capitán, no puede hacer eso." message
  2 seconds  ;
  \ XXX not used yet

  \ ============================================================
  \ Landscape graphics {{{1

variable cloud0x
variable cloud1x

: sunAndClouds  ( f -- )
  2 charset  bright  yellow ink  cyan paper
  26 0 at-xy ." AB"  1 26 at-xy ." CD"  white ink
  1 9 random-range dup cloud0x !
  dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  13 21 random-range dup cloud1x !
  dup 0 at-xy ." MNO"  1 at-xy ." PQR"
  1 charset  0 bright  ;
  \ XXX TODO -- why the parameter, if this word is used only
  \ once?

: colorSky  ( c -- )
  [ skyTopY attrLine ] literal
  [ skyHeight columns * ] literal rot fill  ;
  \ Color the sky with attribute _c_.

: stormySky  ( -- )
  [ cyan dup papery + ] literal colorSky
  false sunAndClouds  ;
  \ Make the sky stormy.

: seaWaves  ( -- )
  1 charset cyan ink  blue paper
  16 1 do  1 28 random-range 4 graphicWinBottom @ random-range
           at-xy ." kl"
           1 28 random-range 4 graphicWinBottom @ random-range
           at-xy ." mn"
  loop  ;

: sunnySky  ( -- )
  [ cyan dup papery + brighty ] literal colorSky  ;
  \ Make the sky sunny.

: colorSea  ( c -- )
  [ seaTopY attrLine ] literal
  [ seaHeight columns * ] literal rot fill  ;
  \ Color the sea with attribute _c_.

: wipeSea  ( -- )  [ blue dup papery + ] literal colorSea  ;

: seaAndSky  ( -- )
  graphicWindow wipeSea seaWaves sunnySky  ;
  \ XXX TMP -- `graphicWindow` is needed, because of the
  \ `wipePanel` before the calling

  \ ============================================================
  \ Sea graphics {{{1

  \ --------------------------------------------
  \ Palms {{{2

: palm1  ( x y -- )
  green ink  blue paper  2dup    at-xy ." OPQR"
                         2dup 1+ at-xy ." S TU"  yellow ink
  1+ under+  \ increment x
  1+ 2dup at-xy ." N"
  1+ 2dup at-xy ." M"
  1+      at-xy ." L"  ;
  \ Print palm model 1 at characters coordinates _x y_.

: palm2  ( x y -- )
  green ink  yellow paper  2dup    at-xy ." OPQR"
                           2dup 1+ at-xy ." S TU"  black ink
  1+ under+  \ increment x
  1+ 2dup at-xy ." N"
  1+ 2dup at-xy ." M"
  1+ 2dup at-xy ." L"
  1+      at-xy ." V"  ;
  \ Print palm model 2 at characters coordinates _x y_.

  \ --------------------------------------------
  \ Islands {{{2

: drawBigIsland5  ( -- )
  green ink  blue paper
  18  7 at-xy ." HI A"
  17  8 at-xy ." G\::\::\::\::BC"
  16  9 at-xy ." F\::\::\::\::\::\::\::D"
  14 10 at-xy ." JK\::\::\::\::\::\::\::\::E"
  13 11 at-xy ." F\::\::\::\::\::\::\::\::\::\::\::C"  ;

: drawBigIsland4  ( -- )
  green ink  blue paper
  16  7 at-xy ." WXYA"
  14  8 at-xy ." :\::\::\::\::\::\::C F\::\::D"
  13  9 at-xy ." :\::\::\::\::\::\::\::\::B\::\::\::E"
  12 10 at-xy ." F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawLittleIsland2  ( -- )
  green ink  blue paper  14  8 at-xy ." :\::\::C"
                         16  7 at-xy ." A"
                         13  9 at-xy ." :\::\::\::\::D"
                         12 10 at-xy ." F\::\::\::\::\::E"  ;

: drawLittleIsland1  ( -- )
  green ink  blue paper  23  8 at-xy ." JK\::C"
                         22  9 at-xy ." :\::\::\::\::D"
                         21 10 at-xy ." F\::\::\::\::\::E"  ;

: drawBigIsland3  ( -- )
  green ink  blue paper
  21  7 at-xy ." Z123"
  19  8 at-xy ." :\::\::\::\::\::C"
  18  9 at-xy ." :\::\::\::\::\::\::\::D"
  15 10 at-xy ." F\::B\::\::\::\::\::\::\::\::E"
  13 11 at-xy ." JK\::\::\::\::\::\::\::\::\::\::\::\::C"  ;

: drawBigIsland2  ( -- )
  green ink  blue paper
  17  7 at-xy ." Z123"
  14  8 at-xy ." F\::B\::\::\::\::\::C"
  13  9 at-xy ." G\::\::\::\::\::\::\::\::\::D"
  12 10 at-xy ." F\::\::\::\::\::\::\::\::\::\::E;"  ;

: drawBigIsland1  ( -- )
  green ink  blue paper
  20  7 at-xy ." HI A"
  19  8 at-xy ." G\::\::B\::\::\::C"
  18  9 at-xy ." F\::\::\::\::\::\::\::\::D"
  16 10 at-xy ." JK\::\::\::\::\::\::\::\::\::E"  ;

: drawTwoLittleIslands  ( -- )
  green ink  blue paper
  17  6 at-xy ." WXY  A"
  16  7 at-xy ." A   A   F\::C"
  15  8 at-xy ." :\::\#127 :\::\#127 G\::\::\::D"
  14  9 at-xy ." G\::\::\::D   F\::\::\::\::E"
  13 10 at-xy ." F\::\::\::\::E"  ;

: drawFarIslands  ( -- )
  green ink  cyan paper
  0 2 at-xy ." Z123 HI A Z123 HI A Z123 HI Z123"  ;

: drawTreasureIsland  ( -- )
  1 charset  green ink  blue paper
  16  7 at-xy ." A A   HI"
  13  8 at-xy ." F\::\::\::B\::\::\::B\::\::B\::\::\::C"
  12  9 at-xy ." G\::\::\::\::\::\::\::"
              ." \::\::\::\::\::\::\::\::\::D"
  10 10 at-xy ." JK\::\::\::\::\::\::\::\::\::"
              ." \::\::\::\::\::\::\::\::E"
   9 11 at-xy ." :\::\::\::\::\::\::\::\::\::\::"
              ." \::\::\::\::\::\::\::\::\::\::C"
   8 12 at-xy ." F\::\::\::\::\::\::\::\::\::\::"
              ." \::\::\::\::\::\::\::\::\::\::\::\::D"
  blue ink  green paper
   8 13 at-xy ."  HI Z123  HI A  A A  A "
  20 14 at-xy ." B\::\::\::\::B"
  green ink  blue paper
  31 13 at-xy ." E"
  19 4 palm1  24 4 palm1  14 4 palm1
  black ink  green paper
  22 9 at-xy ." \T\U"  \ the treasure
  shipPos @ visited @ if
    s" Llegas nuevamente a la isla de " islandName$ s+ s" ."
  else
    s" Has encontrado la perdida isla de "
    islandName$ s+ s" ..."
  then  s+ message  1 charset  ;
  \ XXX TODO -- factor

: wipeIsland  ( -- )
  [ 3 attrLine ] literal
  [ 3 columns * ] literal
  [ yellow dup papery + ] literal fill  ;

  \ --------------------------------------------
  \ Reefs {{{2

: bottomReef  ( -- )
  black ink  blue paper
  2 14 at-xy ."  A  HI   HI       HI  HI  A"
  0 15 at-xy ." WXY  :\::\::\#127     Z123     :\::\::\#127"  ;
  \ XXX TODO -- adapt the graphic chars notation
  \
  \ XXX FIXME still "Off the screen" error!
  \ The reason is the window is changed

: leftReef  ( -- )
  black ink  blue paper
   0 4 at-xy ." A"   1 6 at-xy ." HI"  0 8 at-xy ." WXY"
  1 11 at-xy ." A"  0 13 at-xy ." HI"  ;

: rightReef  ( -- )
  black ink  blue paper
  30 4 at-xy ." HI"   28 6 at-xy ." A"
  29 7 at-xy ." WXY"  31 9 at-xy ." A"  ;

: reef?  ( n -- f )  seaMap @ reef =  ;
  \ Is there a reef at ship position _n_?

: drawReefs  ( -- )
  shipPos @ 15 + reef? if  drawFarIslands  then
  shipPos @ 15 - reef? if  bottomReef      then
  shipPos @ 1-   reef? if  leftReef        then
  shipPos @ 1+   reef? if  rightReef       then  ;

  \ --------------------------------------------
  \ Ships {{{2

: drawShipUp  ( -- )
  white ink  blue paper
  shipX @ shipY @ 2dup    at-xy ." \A\B\C"
                  2dup 1+ at-xy ." \D\E\F"
                       2+ at-xy ." \G\H\I"  ;

: drawShipDown  ( -- )
  white ink  blue paper
  shipX @ shipY @ 2dup    at-xy ." \J\K\L"
                  2dup 1+ at-xy ." \M\N\O"
                       2+ at-xy ." \P\Q\R"  ;

: redrawShip  ( -- )
  shipPicture @ if    drawShipDown shipPicture off
                else  drawShipUp   shipPicture on  then  ;

: drawEnemyShip  ( -- )
  yellow ink  blue paper  11 4 2dup    at-xy ."  ab"
                               2dup 1+ at-xy ."  90"
                                    2+ at-xy ." 678"  ;

: wipeEnemyShip  ( -- )
  blue paper  11 4 2dup    at-xy ."    "
                   2dup 1+ at-xy ."    "
                        2+ at-xy ."    "  ;

: drawBoat  ( -- )
  yellow ink  blue paper  11 7 at-xy ." <>"  ;

: drawShark  ( -- )
  white ink  blue paper  18 13 at-xy ." \S"  ;
  \ XXX TODO -- adapt the UDG notation

: seaPicture  ( n -- )
  dup case
   2 of  drawBigIsland5  19 4 palm1                       endof
   3 of  drawBigIsland4                                   endof
         14 4 palm1  19 4 palm1  24 4 palm1  drawShark    endof
   4 of  drawLittleIsland2  14 4 palm1                    endof
   5 of  drawLittleIsland1  24 4 palm1                    endof
   6 of  drawLittleIsland1  24 4 palm1
         drawLittleIsland2  14 4 palm1                    endof
   7 of  drawBigIsland3  19 4 palm1                       endof
   8 of  drawBigIsland2  14 4 palm1  drawShark            endof
   9 of  drawBigIsland1  24 4 palm1                       endof
  10 of  24 4 palm1  drawTwoLittleIslands                 endof
  11 of  drawShark                                        endof
  13 of  24 4 palm1  drawTwoLittleIslands  drawEnemyShip  endof
  14 of  drawBigIsland1  24 4 palm1  drawEnemyShip        endof
  15 of  drawBigIsland2  14 4 palm1  drawEnemyShip        endof
  16 of  drawBigIsland3  19 4 palm1  drawEnemyShip        endof
  17 of  drawLittleIsland2  14 4 palm1  drawBoat
         drawLittleIsland1  24 4 palm1                    endof
  18 of  drawLittleIsland1  24 4 palm1  drawBoat          endof
  19 of  drawBigIsland4  14 4 palm1  19 4 palm1  24 4 palm1
         drawBoat  drawShark                              endof
  20 of  drawBigIsland5  19 4 palm1  drawBoat             endof
  shark of  drawShark                                     endof
    \ XXX TODO needed?
  endcase
  drawReefs treasureIsland = if  drawTreasureIsland  then  ;
  \ XXX TODO -- `12 of` is not in the original
  \ XXX TODO -- simpler, use an execution table

: seaScenery  ( -- )
  graphicWindow
  seaAndSky redrawShip  shipPos @ seaMap @ seaPicture  ;

  \ ============================================================
  \ Crew stamina {{{1

variable injured

: manInjured  ( -- )
  begin
    1 men random-range dup injured !
  stamina @ until
  -1 injured @ stamina +!
  injured @ stamina @ 0<> alive +!  ;
  \ A man is injured.
  \ Output: `injured` = his number
  \ XXX TODO -- return the output on the stack

variable dead

: manDead  ( -- )
  begin
    1 men random-range dup dead !
  stamina @ until
  dead @ stamina off
  -1 alive +!  ;
  \ A man dies
  \ Output: dead = his number
  \ XXX TODO -- return the output on the stack

  \ ============================================================
  \ Run aground {{{1

: damaged  ( min max -- )
  random-range damage +!  damage @ 100 min damage !  ;
  \ Increase the ship damage with random value in a range.

: runAground  ( -- )

  wipeMessage  \ XXX TODO remove?
  1 charset
  wipeSea drawFarIslands bottomReef leftReef rightReef

  white ink
  14  8 at-xy ." \A\B\C"
  14  9 at-xy ." \D\E\F"
  14 10 at-xy ." \G\H\I"
  black ink  blue paper
  17 10 at-xy ." WXY     A"
  19  6 at-xy ." A   Z123"
   6 11 at-xy ." A   HI"
   5  4 at-xy ." Z123    HI"
   7  8 at-xy ." H\..I  A"

  10 29 damaged
  \ XXX TODO improved message: "Por suerte, ..."
  s" ¡Has encallado! El barco está " damage$ s+ s" ." s+
  message
  \ XXX TODO print at the proper zone:
  damage @ 100 =
  if  cyan ink  black paper  7 20 at-xy ." TOTAL"  then
  black ink  green paper
  0 17 at-xy s" INFORME" columns type-center
  \ XXX TODO choose more men, and inform about them
  manInjured manDead
  -4 -1 random-range morale +!  3 seconds  ;

  \ ============================================================
  \ Reports {{{1

: reportStart  ( -- )  saveScreen cls noWindow 0 charset  ;
  \ Common task at the start of all reports.

: reportEnd  ( -- )  1000 pause restoreScreen  ;
  \ Common task at the end of all reports.

: .##  ( u -- )  s>d <# # # #> type  ;
  \ Print _u_ with two digits.

: .####  ( u -- )  s>d <# # # # # #> type  ;
  \ Print _u_ with four digits.

: mainReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de situación" columns type-center
  0 4 at-xy
  ." Días:"         tab day       @ .##           cr cr
  ." Barco:"        tab damage$ 2dup uppers1 type cr cr
  ." Hombres:"      tab alive     @ .##           cr
  ." Moral:"        tab morale    @ .##           cr cr
  ." Provisiones:"  tab supplies  @ .##           cr
  ." Doblones:"     tab cash      @ .##           cr cr
  ." Hundimientos:" tab sunkShips @ .##           cr
  ." Munición:"     tab ammo      @ .##           cr cr
  reportEnd  ;

 1 constant nameX
20 constant dataX

: crewReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de tripulación" columns type-center
  nameX 4 at-xy ." Nombre"  dataX 4 at-xy ." Condición"
  men 0 do
    white ink
    nameX i 5 + at-xy i name$ type
    i stamina @ staminaAttr @ color!
    dataX i 5 + at-xy
    i stamina @ stamina$ 2@ 2dup uppers1 type
  loop  reportEnd  ;

: updateScore  ( -- )
  foundClues @ 1000 *
  day        @  200 * +
  sunkShips  @ 1000 * +
  trades     @  200 * +
               4000 success? and +
             score +!  ;

: scoreReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de puntuación" columns type-center
  0 4 at-xy
  ." Días"         tab day        @ .#### ."  x  200" cr
  ." Hundimientos" tab sunkShips  @ .#### ."  x 1000" cr
  ." Negocios"     tab trades     @ .#### ."  x  200" cr
  ." Pistas"       tab foundClues @ .#### ."  x 1000" cr
  ." Tesoro"       tab 4000         .####             cr
  updateScore
  ." Total"        tab ."       " score @ .####  reportEnd  ;

  \ ============================================================
  \ Ship battle {{{1

variable done
  \ XXX TODO -- rename

: doAttackOwnBoat  ( -- )
  -1 ammo +!
  s" Disparas por error a uno de tus propios botes..." message
  5 seconds
  3 random if
    s" Por suerte el disparo no ha dado en el blanco." message
  else
    \ XXX TODO inform about how many injured?
    s" La bala alcanza su objetivo."
    s"  Esto desmoraliza a la tripulación." s+ message
    -2 morale +!
    3 4 random-range 1 ?do  manInjured  loop
  then  5 seconds  wipeMessage  ;

: attackOwnBoat  ( -- )
  ammo @ if  doAttackOwnBoat exit  then
  s" Por suerte no hay munición para disparar..." message
  3 pause
  s" Enseguida te das cuenta de que ibas a hundir"
  s"  uno de tus botes." s+ message
  3 pause
  wipeMessage \ XXX TODO -- needed?
  ;

: sunk  ( -- )
  white ink  blue paper
  enemyShipX @ enemyShipY @    at-xy ."    "
  enemyShipX @ enemyShipY @ 1+ at-xy ."  ab"
  enemyShipX @ enemyShipY @ 2+ at-xy ."  90"
  enemyShipX @ enemyShipY @    at-xy ."    "
  enemyShipX @ enemyShipY @ 1+ at-xy ."    "
  enemyShipX @ enemyShipY @ 2+ at-xy ."  ab"
  enemyShipX @ enemyShipY @    at-xy ."    "
  enemyShipX @ enemyShipY @ 1+ at-xy ."    "
  enemyShipX @ enemyShipY @ 2+ at-xy ."    "
  2 seconds
  shipPos @ seaMap @ 13 >=
  shipPos @ seaMap @ 16 <= and
    \ XXX why the condition?
    \ XXX TODO -- simplify the condition and factor out
  if  1 sunkShips +!  1000 score +!  done on  then

  shipPos @ seaMap @ case
    13 of  10  endof
    14 of   9  endof
    15 of   8  endof
    16 of   7  endof
  endcase  shipPos @ seaMap !  ;
  \ Sunk the enemy ship
  \ XXX TODO -- use a calculation instead the last `case`

: drawWave  ( -- )
  cyan ink 11 30 random-range 1 20 random-range at-xy ." kl"  ;

: moveEnemyShip  ( -- )
  1 5 random-range enemyShipMove !
    \ XXX TODO -- use the stack instead of `enemyShipMove`?

  \ (enemyShipMove=1 and enemyShipX<28)-(enemyShipMove=2 and enemyShipX>18)
    \ XXX OLD -- original expression
  enemyShipMove @ 1 =  enemyShipX @ 28 <  and abs
  enemyShipMove @ 2 =  enemyShipX @ 18 >  and abs -
    \ XXX TODO -- check the adapted expression
  enemyShipX +!

  \ (enemyShipMove=3 and enemyShipY<17)-(enemyShipMove=4 and enemyShipY>1)
    \ XXX OLD -- original expression
  enemyShipMove @ 3 =  enemyShipY @ 17 <  and abs
  enemyShipMove @ 4 =  enemyShipY @  1 >  and abs -
    \ XXX TODO -- check the adapted expression
  enemyShipY +!

  white ink  blue paper
  enemyShipX @    enemyShipY @     at-xy ."  ab "
  enemyShipX @    enemyShipY @ 1+  at-xy ."  90 "
  enemyShipX @ 1- enemyShipY @ 2+  at-xy ."  678 "
  enemyShipX @    enemyShipY @ 1-  at-xy ."    "
  enemyShipX @    enemyShipY @ 3 + at-xy ."    "
  enemyShipMove @ 5 = if  drawWave  then  ;

0 value fireY

: fire  ( y -- )
  to fireY  -1 ammo +!
  0 charset white ink  red paper 22 21 at-xy ammo ?
  1 charset
  yellow ink  blue paper
  dup 1- 9 swap at-xy ." +" dup 1+ 9 swap at-xy ." -"
  moveEnemyShip
  dup 1- 9 swap at-xy space dup 1+ 9 swap at-xy space
  9 over at-xy ."  j"
  moveEnemyShip
  31 9 do
    dup i swap at-xy ."  j"
    \ enemyShipY=fireY and i=enemyShipX or enemyShipY=fireY-1 and i=enemyShipX or enemyShipY=fireY-2 and i=enemyShipX
      \ XXX OLD -- original expression
    enemyShipY @ fireY    =  enemyShipX @ i =  and
    enemyShipY @ fireY 1- =  enemyShipX @ i =  and or
    enemyShipY @ fireY 2- =  enemyShipX @ i =  and or
      \ XXX TODO -- simplify the expression
    if  sunk  then
    \ enemyShipY=fireY and i=enemyShipX+1 or enemyShipY=fireY-1 and i=enemyShipX+1 or enemyShipY=fireY-2 and i=enemyShipX+1
      \ XXX OLD -- original expression
    enemyShipY @ fireY    =  enemyShipX @ 1+ i =  and
    enemyShipY @ fireY 1- =  enemyShipX @ 1+ i =  and or
    enemyShipY @ fireY 2- =  enemyShipX @ 1+ i =  and or
      \ XXX TODO -- simplify the expression
      \ XXX TODO -- combine both expressions
    if  sunk  then
  loop  blue paper 30 swap at-xy ."  "  ;
  \ XXX TODO -- store _row_ in a variable, as local

: noAmmoLeft  ( -- )
  s" Te quedaste sin munición." message  4 seconds  ;
  \ XXX TODO the enemy wins; our ship sinks,
  \ or the money and part of the crew is captured

: battleScenery  ( -- )
  noWindow  blue paper cls  0 charset
  white ink  red paper  10 21 at-xy ." Munición = " ammo ?

  black ink yellow paper
  22 0 do  0 i at-xy  ." ________ "  loop

  black ink  white paper
  0 2 at-xy ." 1" 0 9 at-xy ." 2" 0 16 at-xy ." 3"

  18 3 do
    black ink  2 charset  4 i 1- at-xy '1' emit
                          4 i    at-xy '2' emit
                          4 1 1+ at-xy '3' emit
    red ink    1 charset  6 i    at-xy ." cde"
                          6 i 1+ at-xy ." fg"
                          1 i 1+ at-xy ." hi"
  7 +loop

  6 enemyShipY !  20 enemyShipX !
  31 1 do  drawWave  loop  ;

: shipBattle  ( -- )
  done off  saveScreen battleScenery
  begin  moveEnemyShip
    inkey case  '1' of   3 fire  endof
                '2' of  10 fire  endof
                '3' of  17 fire  endof  endcase
                  \ XXX TODO -- use a calculation or a
                  \ table instead?
  done @ ammo 0= or until
  restoreScreen  ammo @ 0= if  noAmmoLeft  then  ;

: attackShip  ( -- )
  ammo @ 0= if    noAmmoLeft
            else  shipPos @ seaMap @ 13 >=
                  shipPos @ seaMap @ 16 <= and
                  if  shipBattle  else  attackOwnBoat  then
            then  ;
  \ XXX TODO -- improve the expression with `between`

  \ ============================================================
  \ Island map {{{1

: eraseIslandMap  ( -- )  0 islandMap /islandMap cells erase  ;

: createIslandCoast  ( -- )
  coast  0 islandMap !  coast  1 islandMap !
  coast  2 islandMap !  coast  3 islandMap !
  coast  4 islandMap !  coast  5 islandMap !
  coast  6 islandMap !  coast 11 islandMap !
  coast 12 islandMap !  coast 17 islandMap !
  coast 18 islandMap !  coast 23 islandMap !
  coast 24 islandMap !  coast 25 islandMap !
  coast 26 islandMap !  coast 27 islandMap !
  coast 28 islandMap !  coast 29 islandMap !  ;

: newIslandMap  ( -- )
  eraseIslandMap  createIslandCoast
  23 7 do
    i islandMap @ coast <>
    if  2 5 random-range i islandMap !  then
      \ XXX TODO -- use constants instead of `2 5`
  loop
  nativeVillage 20 23 random-range islandMap !
  nativeAmmo 14 17 random-range islandMap !
  nativeSupplies 8 11 random-range islandMap !
  8 11 random-range iPos !  ;

  \ ============================================================
  \ On the treasure island {{{1

: sailorSpeechBalloon  ( -- )
  25 44 set-pixel
  20 10 rdraw 0 30 rdraw 2 2 rdraw 100 0 rdraw
  2 -2 rdraw 0 -60 rdraw -2 -2 rdraw -100 0 rdraw
  -2 2 rdraw 0 19 rdraw -20 0 rdraw  ;

: captainSpeechBalloon  ( -- )
  220 44 set-pixel
  -15 5 rdraw 0 20 rdraw -2 2 rdraw -30 0 rdraw
  -2 -2 rdraw 0 -40 rdraw 2 -2 rdraw 30 0 rdraw 2 2 rdraw
  0 14 rdraw 15 0 rdraw  ;

: sailorAndCaptain  ( -- )
  1 charset  cyan ink  black paper
  0 17 at-xy ."  xy" 28 at-x ." pq" cr
             ."  vs" 28 at-x ." rs" cr
             ."  wu" 28 at-x ." tu"
  sailorSpeechBalloon captainSpeechBalloon  ;

: wipeSailorSpeech  ( -- )
  19 12 do  6 i at-xy ."            "  loop  ;

: sailorSays  ( ca len -- )
  wipeSailorSpeech  12 12 6 tellZone  ;
  \ XXX TODO use window instead

: trees  ( -- )
  wipeIsland  black ink  yellow paper
  0 7 at-xy ."  1       2       3       4"
  1 charset  27 2 do  i 3 palm2  8 +loop  ;

: treasureFound  ( -- )
  [ 0 attrLine ] literal [ 3 columns * ] literal
  [ cyan dup papery + brighty ] literal fill
  [ 4 attrLine ] literal [ 18 columns * ] literal
  [ yellow dup papery + ] literal fill
    \ XXX TODO -- factor the coloring
  sunnySky

  23 7 do  i 5 palm2  5 +loop  3 7 palm2  26 7 palm2

  black ink  yellow paper  8 13 at-xy
  ." pq          xy                  "
  ." rs          vs                  tu      "
  ." \T\U    wu"
  28 11 palm2  0 11 palm2
  2 charset  blue ink  yellow paper
  13 17 at-xy ." l\::m"
    \ XXX TODO -- factor the treasure

  s" ¡Capitán, somos ricos!" message
  4 seconds  1 charset  ;
  \ XXX TODO use this proc instead of happyEnd?

variable option

: enterTreasureIsland  ( -- )

  \ XXX TODO finish the new interface

  cls
  sunnySky
  wipeIsland
  2 charset
  green ink  yellow paper
  0 3 at-xy ."  5     6       45     6       5"
  black ink
  25 0 do
    i 3 + 3 at-xy ." :\#127"
    i 2+  4 at-xy ." :\::\::\#127"
    i 1+  5 at-xy ." :\::\::\::\::\#127"
    i     6 at-xy ." :\::\::\::\::\::\::\#127"
    \ XXX TODO -- adapt the graphics notation
  8 +loop
  0 charset  white ink  red paper
  0 7 at-xy ."    1       2       3       4    "

  white ink  black paper
  22 8 do  0 i at-xy blankLine$ type  loop
    \ XXX TODO improve with `fill`

  sailorAndCaptain

  s" ¿Qué camino, capitán?" sailorSays
  23 15 at-xy ." ?" \ XXX TODO better, in all cases
  9 getDigit option !
  black paper
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  option @ path @ = abs foundClues +!

  s" ¿Qué árbol, capitán?" sailorSays
  23 15 at-xy ." ? "
  9 getDigit option !
  0 charset
  black paper  23 15 at-xy option ?  beep .2,30
    \ XXX TODO -- factor out
  trees
  2 seconds
  option @ tree @ = abs foundClues +!

  \ XXX TODO better, with letters
  black paper
  7 14 at-xy ." Izquierda Derecha"
  8 16 at-xy ." I=1  D=2 "
  23 15 at-xy ." ? "
  9 getDigit option !
  0 charset
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  option @ turn @ = abs foundClues +!

  wipeIsland
  black ink  yellow paper
  6 2 do
    1  i 1+ at-xy i 2-  dup . ."   " village$ type
    12 i 1+ at-xy i 3 + dup . ."   " village$ type
  loop
  12 7 at-xy ." 0  " villages village$ type
  2 charset
  green ink  27 5 at-xy ." S\::T" 27 6 at-xy ." VUW"

  0 charset
  black paper
  7 14 at-xy ."  Poblado  " 7 13 at-xy ." ¿Cuál"
  8 16 at-xy ."  capitán." 23 15 at-xy ." ? "
  9 getDigit option !
  23 15 at-xy option  \ XXX TODO --
  beep .2,30
  2 seconds
  option village @ = if  1 foundClues +!  then  \ XXX TODO --

  \ XXX TODO better, with letters
  7 13 at-xy ." ¿Qué camino"
  7 14 at-xy ." capitán?"
  7 16 at-xy ." 1N 2S 3E 4O"
  23 15 at-xy ." ? "
  9 getDigit option !
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option direction @ = if  1 foundClues +!  then  \ XXX TODO --

  7 13 at-xy ." ¿Cuántos"
  7 14 at-xy ." pasos,"
  7 16 at-xy ." capitán?"
  23 15 at-xy ." ? "
  9 getDigit option !
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option pace = if  1 foundClues +!  then  \ XXX TODO --

  black paper
  success? if
    7 13 at-xy ." ¡Hemos encontrado"
    7 14 at-xy ." el oro,"
    7 16 at-xy ." capitán!"  treasureFound
  else
    7 13 at-xy ." ¡Nos hemos"
    7 14 at-xy ."  equivocado "
    7 16 at-xy ." capitán!"
  then  2 seconds  1 charset  ;
  \ XXX TODO -- use tellZone for the last message or...
  \ XXX TODO -- ...at least, do not repeat the coordinates

  \ ============================================================
  \ Island graphics {{{1

: wipeIslandScenery  ( -- )
  [ yellow dup papery + ] literal colorSea  ;

: drawHorizonWaves  ( -- )
  white ink  blue paper
  0 3 at-xy ."  kl  mn     nm    klk   nm nm n"  ;

: drawBottomWaves  ( -- )
  white ink  blue paper
  0 14 at-xy ."  kl     mn  mn    kl    kl kl  m"
             ."     mn      klmn   mn m  mn   "  ;

: drawLeftWaves  ( -- )
  white ink blue paper
  16 3 do  0 i at-xy ."  "  loop
  white ink  blue paper
  0 6 at-xy ." mn" 0 10 at-xy ." kl" 0 13 at-xy ." k"
  0 4 at-xy ." m" 1 8 at-xy ." l"
  2 charset
  yellow ink  blue paper
  iPos @ 6 + islandMap @ coast <> if  2  3 at-xy 'A' emit  then
  iPos @ 6 + islandMap @ coast =  if  2  4 at-xy 'A' emit  then
  iPos @ 6 - islandMap @ coast =  if  2 13 at-xy 'C' emit  then
  1 charset  ;

: drawRightWaves  ( -- )
  white ink  blue paper
  16 3 do  30 i at-xy ."  "  loop
  white ink  blue paper
  30 6 at-xy ." mn" 30 10 at-xy ." kl" 31 13 at-xy ." k"
  30 4 at-xy ." m" 31 8 at-xy ." l"
  2 charset
  yellow ink  blue paper
  iPos @ 6 + islandMap @ coast =
  if    29  4 at-xy 'B' emit  then
  iPos @ 6 - islandMap @ coast =
  if    29 13 at-xy 'D'
  else  29  3 at-xy 'B'
  then  emit  1 charset  ;

: drawVillage  ( -- )
  2 charset  green ink  yellow paper
  6  5 at-xy ."  S\::T    ST   S\::T"
  6  6 at-xy ."  VUW    78   VUW   4"
  4  8 at-xy ." S\::T   S\::T    S\::T S\::T  S\::T "
  4  9 at-xy ." VUW   VUW  4 VUW VUW  VUW"
  4 11 at-xy ." S\::T    S\::T ST  S\::T S\::T"
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
  26  6 at-xy ." Z"  1 charset  ;

: drawNative  ( -- )
  black ink  yellow paper  8 10 at-xy ."  _ `"
                           8 11 at-xy ." }~.,"
                           8 12 at-xy ." {|\?"  ;

: drawAmmo  ( -- )
  black ink  yellow paper  14 12 at-xy ." hi"  ;

: drawSupplies  ( -- )
  2 charset
  black ink  yellow paper 14 12 at-xy ." 90  9099 0009"
  1 charset  ;
  \ XXX TODO draw graphics depending on the actual ammount

: drawSnake  ( -- )
  2 charset  black ink  yellow paper  14 12 at-xy ." xy"
  1 charset  ;

: drawDubloons  ( n -- )
  2 charset  black ink  yellow paper
  12 dup at-xy s" vw vw vw vw vw vw vw vw " drop swap 3 * type
  1 charset  ;

: islandScenery  ( -- )
  graphicWindow wipeIslandScenery sunnySky

  iPos @ 6 - islandMap @ coast = if  drawBottomWaves   then
  iPos @ 6 + islandMap @ coast = if  drawHorizonWaves  then
  iPos @ 1-  islandMap @ coast = if  drawLeftWaves     then
  iPos @ 1+  islandMap @ coast = if  drawRightWaves    then

  iPos @ islandMap @ case
    nativeVillage of  drawVillage  endof
    dubloonsFound of  4 8 palm2  14 5 palm2  endof
    nativeFights of  14 5 palm2  25 8 palm2  drawNative  endof
    4 of  25 8 palm2  4 8 palm2  16 5 palm2  endof
    \ XXX TODO constant
    snake of
      13 5 palm2  5 6 palm2  18 8 palm2  23 8 palm2  drawSnake
    endof
    6 of  23 8 palm2  17 5 palm2  4 8 palm2  endof
    \ XXX TODO constant
    nativeSupplies of
      drawSupplies  drawNative  16 4 palm2
    endof
    nativeAmmo of  drawAmmo  drawNative  20 5 palm2  endof
  endcase  ;

  \ ============================================================
  \ Events on an island {{{1

: event1  ( -- )
  manDead
  dead @ name$ s"  se hunde en arenas movedizas." s+ message  ;

: event2  ( -- )
  manDead
  dead @ name$ s"  se hunde en un pantano." s+ message  ;

: event3  ( -- )
  manInjured s" A " injured @ name$ s+
             s"  le muerde una araña." s+ message  ;

: event4  ( -- )
  manInjured
  s" A " injured @ name$ s+ s"  le pica un escorpión." s+
  message  ;

: event5  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulación está hambrienta." message
  -1 morale +!  ;

: event6  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulación está sedienta." message
  -1 morale +!  ;

: event7  ( -- )
  2 5 random-range >r
  s" Encuentras " r@ coins$ s+ s" ." s+ message
  r@ cash +!  r> drawDubloons  ;

: event8  ( -- )
  s" Sin novedad, capitán." message  ;

: event9  ( -- )
  s" La costa está despejada, capitán." message  ;

create islandEvents>  ( -- a )
] event1 event2 event3 event4 event5 event6
  event7 event8 event8 event9 event9 noop noop [

: islandEvents  ( -- )
  0 10 random-range cells islandEvents> + perform  ;

  \ ============================================================
  \ Enter island location {{{1

: enterIslandLocation  ( -- )

  wipeMessage  \ XXX TODO needed?
  islandScenery

  iPos @ islandMap @ case

  snake of
    manInjured
    s" Una serpiente ha mordido a " injured @ name$ s+ s" ." s+
    message
  endof

  nativeFights of
    manInjured
    s" Un nativo intenta bloquear el paso y hiere a "
    injured @ name$ s+ s" , que resulta " s+
    injured @ condition$ s+ s" ." s+ message
  endof

  dubloonsFound of
    1 2 random-range >r
    s" Encuentras " r@ coins$ s+ s" ." s+ message
    r@ cash +!
    r> drawDubloons
    4 iPos @ islandMap !
      \ XXX TODO -- constant for 4
  endof

  nativeAmmo of
    s" Un nativo te da algo de munición." message
    1 ammo +!
    nativeFights iPos @ islandMap !
  endof

  nativeSupplies of
    s" Un nativo te da provisiones." message
    \ XXX TODO random ammount
    1 supplies +!
    nativeFights iPos @ islandMap !
  endof

  nativeVillage of
    s" Descubres un poblado nativo." message
  endof

  4 of
    \ XXX TODO constant for this case
    islandEvents
  endof

  6 of
    \ XXX TODO constant for this case
    islandEvents
  endof

  endcase

  1 charset
  100 pause \ XXX OLD

  ;

  \ ============================================================
  \ Disembark {{{1

: disembarkingScene  ( -- )
  1 charset  green ink  blue paper
  31  8 at-xy ." :"
  37  9 at-xy ." HI :\::"
  25 10 at-xy ." F\::\::\::\::\::\::"
  23 11 at-xy ." JK\::\::\::\::\::\::\::"
  yellow ink blue paper
  21 0 do  i 11 at-xy ."  <>" 10 pause  loop  ;

: enterIsland  ( -- )
  aboard off  shipPos @ seaMap @ treasureIsland =
  if    enterTreasureIsland
  else  newIslandMap enterIslandLocation  then  ;

: disembark  ( -- )
  -2 -1 random-range supplies +!
  wipeMessage seaAndSky disembarkingScene enterIsland  ;

  \ ============================================================
  \ Storm {{{1

: rainDrops  ( c -- )
  white ink  cyan paper
  cloud0x @ 2 at-xy dup 4 ruler type
  cloud1x @ 2 at-xy     3 ruler type  3 pause  ;

: rain  ( -- )
  1 charset  71 1 do
    ';' rainDrops  ']' rainDrops  '[' rainDrops
    3 random 0= if  redrawShip  then
  loop  ;

: storm  ( -- )
  wipePanel stormySky  10 49 damaged
  s" Se desata una tormenta"
  s"  que causa destrozos en el barco." s+ message
  rain  white ink  cyan paper
  cloud0x 2 at-xy ."     " cloud1x 2 at-xy ."    "
  s" Tras la tormenta, el barco está "
  damage$ s+ s" ." s+ message  panel  ;
  \ XXX TODO bright sky!
  \ XXX TODO make the enemy ship to move, if present
  \ (use the same graphic of the player ship)

  \ ============================================================
  \ Ship command {{{1

: seaMove  ( offset -- )
  dup shipPos + seaMap @ reef = if    drop runAground
                                else  shipPos +!
                                then  drop  ;

: shipCommand  ( -- )
  begin
    81 1 do
      inkey upper case
      'N' 11 or-of  \ north
        possibleNorth @ if  15 seaMove exit  then  endof
      'S' 10 or-of  \ south
        possibleSouth @ if  -15 seaMove exit  then  endof
      'E' 9 or-of  \ east
        possibleEast @ if  1 seaMove exit  then  endof
      'O' 8  or-of  \ west
        possibleWest @ if  -1 seaMove -1 exit  then  endof
      'I' of  mainReport exit  endof
      'A' of
        possibleAttacking @ if  attackShip exit then  endof
      'T' of  crewReport exit  endof
      'P' of  scoreReport exit  endof
      'D' of
        possibleDisembarking @ of  disembark exit  then  endof
      'F' of  quitGame on  exit  endof
      endcase
      i 40 mod 0= if  redrawShip  then
        \ XXX TODO -- use system frames instead?
    loop
    80 random 0= if  storm  then
      \ XXX TODO increase the probability every day?
  again  ;
  \ XXX TODO simpler, with searchable string of keys and ON

  \ ============================================================
  \ Trading {{{1

: nativeSpeechBalloon  ( -- )
  black ink
  100 100 set-pixel  20 10 rdraw  0 30 rdraw  2 2 rdraw
  100 0 rdraw  2 -2 rdraw  0 -60 rdraw  -2 -2 rdraw
  -100 0 rdraw -2 2 rdraw  0 20 rdraw  -20 0 rdraw  ;

: trade  ( -- )

  1 charset
  \ XXX TODO factor out:
  black ink  yellow paper
  16 3 do  0 i at-xy blankLine$ type  loop
    \ XXX TODO improve with `fill`
  4 4 palm2
  drawNative
  nativeSpeechBalloon
  s" Un comerciante nativo te sale al encuentro." message
  s" Yo vender pista de tesoro a tú." nativeSays

  5 9 random-range price !
  s" Precio ser " price @ coins$ s+ s" ." s+ nativeSays
  \ XXX TODO pause or join:
  1 seconds
  s" ¿Qué dar tú, blanco?" nativeSays
  makeOffer
  offer @ price @ 1-  >= if  acceptedOffer exit  then
    \ One dubloon less is accepted.
  offer @ price @ 4 - <= if  rejectedOffer exit  then
    \ Too low offer is not accepted.

  \ You offered too few
  1 4 random-range case 1 of  lowerPrice  endof
                        2 of  newPrice    endof  endcase

  \ XXX TODO -- the original does a `goto`, see:
  \ on fn between(1,4)
  \   goto lowerPrice
  \   goto newPrice

  \ He reduces the price by one dubloon
  -1 price +!
  s" ¡No! ¡Yo querer más! Tú darme " price @ coins$ s+ s" ." s+
  nativeSays

  label oneCoinLess
  \ He accepts one dubloon less
  makeOffer
  offer @ price @ 1- >=
  if  acceptedOffer
  else offer @ price @ 1- < if  rejectedOffer  then
  then
  \ XXX TODO -- simplify

  label lowerPrice
  \ XXX TODO -- factor out
  \ He lowers the price by several dubloons
  -3 -2 random-range price +!
  s" Bueno, tú darme... " price @ coins$ s+
  s"  y no hablar más." s+ nativeSays
  makeOffer
  offer @ price @ >= if    acceptedOffer
                     else  rejectedOffer
                     then  exit

  label newPrice
  \ XXX TODO -- factor out
  3 8 random-range dup price ! coins$ 2dup uppers1
  s"  ser nuevo precio, blanco." s+ nativeSays
  goto oneCoinLess

  ;

: makeOffer  ( -- )
  cash @ maxOffer min >r
  s" Tienes " cash @ coins$ s+
  s". ¿Qué oferta le haces? (1-" s+
  r@ u>str s+ ." )" s+ message
  r> getDigit offer !
  beep .2,10
  s" Le ofreces " offer @ coins$ s+ s" ." s+ message  ;
  \ Ask the player for an offer
  \ XXX TODO -- check the note about the allowed range

: rejectedOffer  ( -- )
  2 seconds  s" ¡Tú insultar! ¡Fuera de isla mía!" nativeSays
  4 seconds  embark  ;

: acceptedOffer  ( -- )
  wipeMessage
  offer @ negate cash +!  200 score +!  1 trades +!
  nativeTellsClue  4 seconds  embark  ;

: nativeTellsClue1  ( -- )
  s" Tomar camino " path @ number$ s+ s" ." s+ nativeSays  ;

: nativeTellsClue2  ( -- )
  s" Parar en árbol " tree @ number$ s+ s" ." s+ nativeSays  ;

: nativeTellsClue3  ( -- )
  s" Ir a " turn @ hand$ s+ s"  en árbol." s+ nativeSays  ;

: nativeTellsClue4  ( -- )
  s" Atravesar poblado " village @ village$ s+ s" ." s+
  nativeSays  ;

: nativeTellsClue5  ( -- )
  s" Ir " direction @ cardinal$ s+ s"  desde poblado." s+
  nativeSays  ;

: nativeTellsClue6  ( -- )
  s" Dar " pace @ number$ s+ s"  paso" s+
  s" s" pace @ 1 > and s+
  s" desde poblado." s+ nativeSays  ;

create nativeTellsClues  ( -- a )
] nativeTellsClue1 nativeTellsClue2 nativeTellsClue3
  nativeTellsClue4 nativeTellsClue5 nativeTellsClue6 [

: nativeTellsClue  ( -- )
  s" Bien... Pista ser..." nativeSays
  2 seconds  0 5 random-range cells nativeTellsClues + perform
  2 seconds  s" ¡Buen viaje a isla de tesoro!" nativeSays  ;


  \ ============================================================
  \ Commands on the island {{{1

: islandCommand  ( -- )
  begin  inkey upper  case
    'N' 11 or-of  \ "n" or up -- north
      possibleNorth @ if  islandMove 6      exit  then  endof
    'S' 10 or-of  \ "s" or down -- south
      possibleSouth @ if  islandMove -6     exit  then  endof
    'E' 9 or-of  \ "e" or right -- east
      possibleEast @ if  islandMove 1       exit  then  endof
    'O' 8 or-of  \ "o" or left -- west
      possibleWest @ if  islandMove -1      exit  then  endof
    'C' of  possibleTrading @ if  trade     exit  then  endof
    'B' of  possibleEmbarking @ if  embark  exit  then  endof
    'I' of  mainReport                      exit        endof
    'M' of  possibleAttacking @ if  attack  exit  then  endof
    'T' of  crewReport                      exit        endof
    'P' of  scoreReport                     exit        endof
    'F' of  quitGame on                     exit        endof
    \ XXX TODO -- use constant for control chars
  endcase  repeat  ;
  \ XXX TODO simpler, with searchable string of keys and ON

: islandMove  ( offset -- )
  dup iPos @ + islandMap @ coast <>
  if    iPos +!  enterIslandLocation
  else  drop
  then  ;

: embark  ( -- )
  shipPos @ visited on  1 day +!  aboard on  ;

  \ ============================================================
  \ Attack {{{1

: attack  ( -- )

  \ XXX OLD -- commented out in the original
  \ if islandMap(iPos)=2 or islandMap(iPos)=4 or islandMap(iPos)=6 then \
  \   gosub @impossible
  \   gosub @islandPanel
  \   exit proc

  s" Atacas al nativo..." message \ XXX OLD
  100 pause

  iPos @ islandMap @ 5 = if
    \ XXX TODO --  5=snake?
    manDead
    s" Lo matas, pero la serpiente mata a "
    dead @ name$ s+ s" ." s+ message
    goto L6897
  then

  iPos @ islandMap @ nativeVillage = if
    manDead
    s" Un poblado entero es un enemigo muy difícil. "
    dead @ name$ s+ s"  muere en el combate." s+
    message
    goto L6898
  then

  1 5 random-range case
  1 of  manDead
        s" El nativo muere, pero antes mata a "
        dead @ name$ s+ s" ." s+ message  endof
  2 of  s" El nativo tiene provisiones"
        s"  escondidas en su taparrabos." s+ message
        1 supplies +!  endof

    2 3 random-range r>
    s" Encuentras " r@ coins$ s+
    s"  en el cuerpo del nativo muerto." s+ message
    r> cash +!

  endcase

  2 charset  black ink  yellow paper yellow
  14 10 do  8 i at-xy ." t   "  loop
  black ink  yellow paper  8  9 at-xy ." u"
  white ink  black  paper  9 10 at-xy ." nop"
                           9 11 at-xy ." qrs"
  1 charset

  label L6897

  4 iPos @ islandMap !
    \ XXX TODO -- constant for 4

  label L6898

  3 seconds  ;

  \ ============================================================
  \ Setup {{{1

: initOnce  ( -- )  initScreen  initUDG  ;

: initSeaReefs  ( -- )
            17 1 do  reef i seaMap !      loop     \ north
  /seaMap 1+ 120 do  reef i seaMap !      loop     \ south
          106 30 do  reef i seaMap !  15 +loop     \ east
          107 32 do  reef i seaMap !  15 +loop  ;  \ west

: initSeaIslands  ( -- )
  120 17 do
    i seaMap @ reef <> if
      2 21 random-range i seaMap !  \ random type
      \ XXX TODO -- 21 is shark; these are picture types
    then
  loop
  treasureIsland 94 104 random-range seaMap !  ;

: emptySeaMap  ( -- )
  0 seaMap  /seaMap cells erase
  0 visited /seaMap cells erase  ;

: initSeaMap  ( -- )
  emptySeaMap initSeaReefs initSeaIslands  ;

: initShip  ( -- )
  32 42 random-range shipPos !  9 shipY !  4 shipX !
  shipPicture off  ;

: initClues  ( -- )
  1 3 random-range path !  \ XXX TODO -- check range 0..?
  1 3 random-range tree !  \ XXX TODO -- check range 0..?
  0 9 random-range village !
  0 1 random-range turn !
  0 3 random-range direction !
  1 9 random-range pace !  ;  \ XXX TODO -- check range 0..?

: initPlot  ( -- )
  initClues  aboard on  1 iPos !
  men alive !  2 ammo !  5 cash !  10 morale !  10 supplies !
  quitGame off  damage off  day off  foundClues off  score off
  sunkShips off  trades off  ;

: init  ( -- )
  randomize
  [ 2 attrLine ] literal [ 20 columns * ] literal erase
    \ XXX TODO -- check if needed
    \ XXX TODO -- use constant to define the zone
  white ink  black paper  1 flash
  0 14 at-xy s" Preparando el viaje..." columns type-center
  initSeaMap initShip initCrew initPlot  ;

: unusedName  ( -- n )
  0 begin
    drop  0 [ names 1- ] literal random-range
  dup usedNames @ 0= until  ;
  \ Return the random identifier _n_ of an unused name.

: initCrewName  ( n -- )
  unusedName  dup usedNames on  names$ name 2!  ;
  \ Choose an unused name for crew member _n_.

: initCrewNames  ( -- )  men 0 do  i initCrewName  loop  ;
  \ Choose unused names for the crew members.

: initCrewStamina  ( -- )
  men 0 do  maxStamina i stamina !  loop  ;
  \ Set the stamina of the crew to its maximum.

: initCrew  ( -- )  initCrewNames initCrewStamina  ;

  \ ============================================================
  \ Game over {{{1

: reallyQuit  ( -- )
  \ Confirm the quit
  \ XXX TODO
  ;

: playAgain  ( -- )
  \ Play again?
  \ XXX TODO
  ;

: sadEnd  ( -- )
  0 charset
  white ink  red paper
  0 3 at-xy s" FIN DEL JUEGO" columns type-center
  5 26 2 21 window
  supplies @ 0 <= if
    s" Las provisiones se han agotado." tell  then
  morale @ 0 <= if
    s" La tripulación se ha amotinado." tell  then
  ammo @ 0 <= if
    s" La munición se ha terminado." tell  then
  alive @ 0= if
    s" Toda tu tripulación ha muerto." tell  then
  damage @ 100 = if
    s" El barco está muy dañado y es imposible repararlo." tell
  then
  cash @ 0 <= if
    s" No te queda dinero." tell  then
  noWindow  ;
  \ XXX TODO uset TellZone

: happyEnd  ( -- )
  s" Lo lograste, capitán." message  ;
  \ XXX TODO --

: theEnd  ( -- )
  black ink yellow paper cls1
  1 charset  16 1 do  27 i palm2  1 i palm2  7 +loop
  success? if  happyEnd  else  sadEnd  then
  s" Pulsa una tecla para ver tus puntos" message
  0 pause beep .2,30 scoreReport  ;
  \ XXX TODO new graphic, based on the cause of the end

  \ ============================================================
  \ Intro {{{1

: intro  ( -- )
  cls
  skullBorder
  introWindow
  s" Viejas leyendas hablan del tesoro" tell
  s" que esconde la perdida isla de" tell
  islandName$ s" ." s+ tellCR
  s" Los nativos del archipiélago recuerdan" tell
  s" las antiguas pistas que conducen al tesoro." tell
  s" Deberás comerciar con ellos para que te las digan." tellCR
  s" Visita todas las islas hasta encontrar la isla de" tell
  islandName$ tell
  s" y sigue las pistas hasta el tesoro..." tell
  0 row 1+ s" Pulsa una tecla" columns type-center
  6000 pause  ;

: skulls  ( -- )
  ."   nop  nop  nop  nop  nop  nop  " cr
  ."   qrs  qrs  qrs  qrs  qrs  qrs  "  ;
  \ Draw a row of six skulls.

: skullBorder  ( -- )
  2 charset white ink  black paper  1 bright
            home skulls 0 23 at-xy skulls
  1 charset  ;
  \ Draw top and bottom border of skulls.

  \ ============================================================
  \ Main {{{1

: scenery  ( -- )
  useScreen2
  aboard @ if  seaScenery  else  islandScenery  then
  panel useScreen1  ;
  \ XXX FIXME useScreen2 and usesCreen2 cause the sea
  \ background is missing

: command  ( -- )
  aboard @ if  shipCommand  else  islandCommand  then  ;

: game  ( -- )
  cls
  screenRestored off
  begin
    screenRestored @ if    screenRestored off
                     else  scenery
                     then  command
  gameOver? until  ;
  \ XXX FIXME sometimes scenery is called here without reason
  \ XXX The logic is wrong.

: main  ( -- )
  initOnce  begin  intro init game theEnd  repeat  ;

  \ ============================================================
  \ Meta {{{1

variable invflag
  \ XXX TMP --

: showSea  ( -- )
  cls
  seaMapRows 2* 0 do
    seaMapCols 0 do
      invflag @ inverse  j seaMapRows * i + 1+ seaMap @ .##
      invflag @ 0= invflag !
    loop  cr
  2 +loop  mode 1  0 inverse  ;

: showCharsets  ( -- )
  cls  3 0 do  i showCharset  loop
       0 charset cr ." UDG" showUdg  ;

: showCharset  ( n -- )
  0 charset cr ." charset " n .
  n charset showASCII  ;

: showASCII  ( -- )  128 32 do  i emit  loop  ;

: showUDG  ( -- )  256 128 do  i emit  loop  ;

: showDamages  ( -- )
  101 0 do  i . damageIndex . damage$ type cr  loop  ;

  \ vim: filetype:soloforth foldmethod=marker
