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

: version  ( -- ca len )  s" 0.13.3+201701072306" ;

cr cr .( Bandera Negra) cr version type cr

  \ ============================================================
  cr .( Requirements)  \ {{{1

forth-wordlist set-current

  \ --------------------------------------------
  cr .(   -Debugging tools)  \ {{{2

need ~~  need see  need dump  need abort"

  \ --------------------------------------------
  cr .(   -Definers)  \ {{{2

need alias
need 2avariable  need avariable    need cavariable
need sconstants  need /sconstants  need value

  \ --------------------------------------------
  cr .(   -Control structures)  \ {{{2

need case  need or-of  need j  need 0exit

  \ --------------------------------------------
  cr .(   -Stack manipulation)  \ {{{2

need pick

  \ --------------------------------------------
  cr .(   -Math)  \ {{{2

need >=  need <=  need under+
need random-range  need randomize0

  \ --------------------------------------------
  cr .(   -Memory)  \ {{{2

need move>far  need move<far

  \ --------------------------------------------
  cr .(   -Time)  \ {{{2

need frames@  need pause

  \ --------------------------------------------
  cr .(   -Strings)  \ {{{2

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
need columns  need inverse
need set-udg  need rom-font  need set-font  need get-font

need black  need blue  need red  need green
need cyan  need yellow  need white  need color!
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

game-wordlist  set-current

  \ ============================================================
  cr .( Debugging tools [1])  \ {{{1

:  ~~h  ( -- )  2 border key drop 1 border  ;
  \ Break point.

'q' ~~quit-key !  ~~resume-key on  22 ~~y !  ~~? on

: ?break  ( -- )  break-key? abort" Aborted!" ;

  \ ============================================================
  cr .( Constants)  \ {{{1

15 constant seaMapCols
 9 constant seaMapRows

seaMapCols seaMapRows * constant /seaMap
  \ cells of the sea map

6 constant islandMapCols
5 constant islandMapRows

islandMapCols islandMapRows * constant /islandMap
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
                                  0 constant graphicWinTop
                                  0 constant graphicWinLeft
                                 32 constant graphicWinWidth
                                 16 constant graphicWinHeight
                                  0 constant lowWinLeft
                                 32 constant lowWinWidth
                                  3 constant lowWinHeight
         lowWinWidth lowWinHeight * constant lowWinChars
                                 17 constant messageWinTop
                                  1 constant messageWinLeft
                                 30 constant messageWinWidth
                                  3 constant messageWinHeight

  \ ============================================================
  cr .( Variables)  \ {{{1

variable quitGame         \ flag
variable screenRestored   \ flag  \ XXX TODO -- what for?

  \ --------------------------------------------
  cr .(   -Plot)  \ {{{2

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
  cr .(   -Ships)  \ {{{2

variable shipUp      \ flag
variable shipX
variable shipY
variable shipPos

variable enemyShipMove
variable enemyShipX
variable enemyShipY

  \ --------------------------------------------
  cr .(   -Clues)  \ {{{2

variable foundClues       \ counter

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

/seaMap     avariable seaMap
/islandMap  avariable islandMap
/seaMap     avariable visited    \ flags for islands

  \ --------------------------------------------
  cr .(   -Crew)  \ {{{2

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
  here ," Vicente Nario"
  here ," Vicente Rador"
  here ," Víctor Nillo"
  here ," Víctor Pedo"
  here ," Víctor Tilla"
  here ," Zacarías Queroso"
  here ," Óscar Nicero"
  here ," Óscar Romato"
  here ," Óscar Terista"
/sconstants stockName$  ( n -- ca len )
constant stockNames

men 2avariable name  ( n -- a )
  \ A double-cell array to hold the address and length
  \ of the names of the crew members, compiled in `names$`.

: name$  ( n -- ca len )  name 2@  ;

stockNames avariable usedName  ( n -- a )
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

 black white papery +         0 staminaAttr c!
   red black papery + brighty 1 staminaAttr c!
   red black papery +         2 staminaAttr c!
yellow black papery +         3 staminaAttr c!
 green black papery +         4 staminaAttr c!

  \ --------------------------------------------
  cr .(   -Ship damage descriptions)  \ {{{2

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
  cr .(   -Village names)  \ {{{2

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
  cr .(   -Cardinal points)  \ {{{2

0
  here ," oeste"
  here ," este"
  here ," sur"
  here ," norte"
sconstants cardinal$  ( n -- ca len )

  \ --------------------------------------------
  cr .(   -Hands)  \ {{{2

0
  here ," derecha"    \ right
  here ," izquierda"  \ left
sconstants hand$  ( n -- ca len )

  \ ============================================================
  cr .( Functions)  \ {{{1

22528 constant attributes
  \ Address of the screen attributes (768 bytes)

: attrLine  ( l -- a )  columns * attributes +  ;
  \ First attribute address of a character line.

: >attr  ( paper ink bright -- c )  64 * + swap 8 * +  ;
  \ Convert _paper_, _ink_ and _bright_ to an attribute byte
  \ _c_.

: dubloons$  ( n -- ca len )
  s" dobl " rot 1 > if  s" ones"  else  s" ón"  then  s+  ;
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
  \ active, _f_ is true and _n_ is the position of its
  \ highlighted letter (0..len1-1).

: coins$  ( n -- ca len )
  dup >r number$ s"  " s+ r> dubloons$ s+  ;
  \ Return the text "n doubloons", with letters.

: damageIndex  ( -- n )  damage @ damageLevels * 101 / 1+  ;
  \ XXX TODO -- use `*/`

: failure?  ( -- f )
  alive @ 0=
  morale @ 1 < or
  damageIndex damageLevels = or
  supplies @ 1 < or
  cash @ 1 < or  ;
  \ Failed mission?
  \
  \ XXX TODO -- use `0=` instead of `1 <`

6 constant maxClues

: success?  ( -- f )  foundClues @ maxClues =  ;
  \ Success?

: gameOver?  ( -- f )  failure? success? quitGame @ or or  ;
  \ Game over?

: condition$  ( n -- ca len )  stamina @ stamina$ 2@  ;
  \ Physical condition of a crew member

: blankLine$  ( -- ca len )  bl columns ruler  ;

: damage$  ( -- ca len )  damageIndex damageLevel$  ;
  \ Damage description

  \ ============================================================
  cr .( UDGs and fonts)  \ {{{1

768 constant /font
  \ Bytes for font (characters 32..127, 8 bytes each).

rom-font value textFont
rom-font value graphFont1
rom-font value graphFont2

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

window graphicWindow
graphicWinLeft graphicWinTop graphicWinWidth graphicWinHeight
set-window
  \ XXX TODO -- remove, useless

window introWindow  2 3 28 19 set-window

window messageWindow
messageWinLeft messageWinTop messageWinWidth messageWinHeight
set-window

window nativeWindow 16 6 11 4 set-window

window sailorWindow  12 6 12 6 set-window

window theEndWindow  5 2 22 20 set-window

  \ ============================================================
  cr .( Screen)  \ {{{1

: initScreen  ( -- )
  white ink blue dup paper border cls
  graphicWindow graphFont1 set-font  ;

16384 constant screen  6912 constant /screen
  \ Address and size of the screen.

farlimit @ /screen - dup constant screenBackup
                         farlimit !

far-banks 3 + c@ cconstant screenBackupBank

: saveScreen  ( -- )
  screenBackupBank bank
  screen screenBackup /screen cmove  default-bank  ;
  \ XXX TODO -- faster, page the bank

: restoreScreen  ( -- )
  screenBackupBank bank
  screenBackup screen /screen cmove  default-bank
  screenRestored on  ;
  \ XXX TODO -- faster, page the bank

  \ ============================================================
  cr .( Text output)  \ {{{1

: nativeSays  ( ca len -- )  nativeWindow wcls wtype  ;

: wipeMessage  ( -- )
  messageWindow  white ink  black paper  wcls  ;

: message  ( ca len -- )
  textFont set-font wipeMessage wtype graphicWindow ;

  \ ============================================================
  cr .( Sound )  \ {{{1

: beep  ( "name" -- )  parse-name 2drop  ; immediate
  \ XXX TMP --
  \ XXX TODO --

  \ ============================================================
  cr .( User input)  \ {{{1

: seconds  ( n -- )  50 * pause  ;

: getDigit  ( n1 -- n2 )
  begin  dup 0 pause inkey '0' - dup >r
         1 < over r@ < or  while  r> drop beep .1,10
  repeat  drop r>  ;
  \ Wait for a digit to be pressed by the player, until its
  \ value is greater than 0 and less than _n1_, then return it
  \ as _n2_.

  \ ============================================================
  cr .( Command panel)  \ {{{1

21 constant panel-y

variable feasibleDisembark      \ flag
variable feasibleEmbark         \ flag
variable feasibleAttack         \ flag
variable feasibleTrade          \ flag

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

: feasibleAttack?  ( -- f )
  shipPos @ seaMap @
  dup >r 13 <  r@ shark = or  r> treasureIsland = or 0=  ;

: commonPanelCommands  ( -- )
  0 panel-y at-xy s" Información" 0 >option$ type cr
                  s" Tripulación" 0 >option$ type cr
                  s" Puntuación"  0 >option$ type
  feasibleAttack? dup >r feasibleAttack !
  16 panel-y at-xy s" Atacar" 0 r> ?>option$ type  ;

: feasibleDisembark?  ( -- f )
  shipPos @ visited @ 0=
  shipPos @ seaMap @ treasureIsland =  or  ;

: shipPanelCommands  ( -- )
  home shipPos ?  \ XXX INFORMER
  feasibleDisembark? dup >r feasibleDisembark !
  16 panel-y 1+ at-xy s" Desembarcar" 0 r> ?>option$ type  ;
  \ XXX TODO -- factor both conditions
  \ XXX TODO -- `feasibleDisembark` only if no enemy ship
  \ is present

: feasibleTrade?  ( -- f )
  iPos @ islandMap @ nativeVillage =  ;

' true alias feasibleEmbark?  ( -- f )
  \ XXX TODO -- only if iPos is coast
  \ XXX TODO -- better yet, only if iPos is the
  \ disembarking position

: islandPanelCommands  ( -- )
  home iPos ?  \ XXX INFORMER
  feasibleEmbark? dup >r feasibleEmbark !
  16 panel-y 1+ at-xy s" emBarcar" 2 r> ?>option$ type
  feasibleTrade? dup >r feasibleTrade !
  16 panel-y 2+ at-xy s" Comerciar" 0 r> ?>option$ type  ;

: wipePanel  ( -- )
  black paper 0 21 at-xy lowWinChars spaces  ;
  \ XXX TODO -- use window

: panel  ( -- )
  textFont set-font  white ink  wipePanel commonPanelCommands
  aboard @ if    shipPanelCommands
           else  islandPanelCommands  then  directionsMenu  ;
  \ XXX TODO check condition -- what about the enemy ship?
  \ XXX TODO several commands: attack ship/island/shark?

  \ ============================================================
  cr .( Landscape graphics)  \ {{{1

variable cloud0x
variable cloud1x

: sunAndClouds  ( f -- )
  bright  yellow ink  cyan paper
  graphFont2 set-font
  26 0 at-xy ." AB"  26 1 at-xy ." CD"  white ink
  1 9 random-range dup cloud0x !
  dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  13 21 random-range dup cloud1x !
  dup 0 at-xy ." MNO"  1 at-xy ." PQR"
  graphFont1 set-font  0 bright  ;
  \ XXX TODO -- why the parameter, if this word is used only
  \ once?

: colorSky  ( c -- )
  [ skyTopY attrLine ] literal
  [ skyHeight columns * ] literal rot fill  ;
  \ Color the sky with attribute _c_.

: stormySky  ( -- )
  [ cyan dup papery + ] literal colorSky  false sunAndClouds  ;
  \ Make the sky stormy.

: seaWaveCoords  ( -- x y )
  1 28 random-range
  4 [ graphicWinTop graphicWinHeight + 1- ] literal
  random-range  ;
  \ Return random coordinates _x y_ for a sea wave.

: atSeaWaveCoords  ( -- )  seaWaveCoords  at-xy  ;
  \ Set the cursor at random coordinates for a sea wave.

: seaWaves  ( -- )
  graphFont1 set-font cyan ink  blue paper
  15 0 do  atSeaWaveCoords ." kl"  atSeaWaveCoords ." mn"
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
  graphicWindow graphFont1 set-font
  wipeSea seaWaves sunnySky  ;
  \ XXX TMP -- `graphicWindow` is needed, because of the
  \ `wipePanel` before the calling

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

: drawBigIsland5  ( -- )
  green ink  blue paper
  18  7 at-xy ." HI A"
  17  8 at-xy .\" G\::\::\::\::BC"
  16  9 at-xy .\" F\::\::\::\::\::\::\::D"
  14 10 at-xy .\" JK\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::C"  ;

: drawBigIsland4  ( -- )
  green ink  blue paper
  16  7 at-xy ." WXYA"
  14  8 at-xy .\" :\::\::\::\::\::\::C F\::\::D"
  13  9 at-xy .\" :\::\::\::\::\::\::\::\::B\::\::\::E"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawLittleIsland2  ( -- )
  green ink  blue paper  14  8 at-xy .\" :\::\::C"
                         16  7 at-xy ." A"
                         13  9 at-xy .\" :\::\::\::\::D"
                         12 10 at-xy .\" F\::\::\::\::\::E"  ;

: drawLittleIsland1  ( -- )
  green ink  blue paper  23  8 at-xy .\" JK\::C"
                         22  9 at-xy .\" :\::\::\::\::D"
                         21 10 at-xy .\" F\::\::\::\::\::E"  ;

: drawBigIsland3  ( -- )
  green ink  blue paper
  21  7 at-xy ." Z123"
  19  8 at-xy .\" :\::\::\::\::\::C"
  18  9 at-xy .\" :\::\::\::\::\::\::\::D"
  15 10 at-xy .\" F\::B\::\::\::\::\::\::\::\::E"
  13 11 at-xy .\" JK\::\::\::\::\::\::\::\::\::\::\::\::C"  ;

: drawBigIsland2  ( -- )
  green ink  blue paper
  17  7 at-xy ." Z123"
  14  8 at-xy .\" F\::B\::\::\::\::\::C"
  13  9 at-xy .\" G\::\::\::\::\::\::\::\::\::D"
  12 10 at-xy .\" F\::\::\::\::\::\::\::\::\::\::E;"  ;

: drawBigIsland1  ( -- )
  green ink  blue paper
  20  7 at-xy ." HI A"
  19  8 at-xy .\" G\::\::B\::\::\::C"
  18  9 at-xy .\" F\::\::\::\::\::\::\::\::D"
  16 10 at-xy .\" JK\::\::\::\::\::\::\::\::\::E"  ;

: drawTwoLittleIslands  ( -- )
  green ink  blue paper
  17  6 at-xy ." WXY  A"
  16  7 at-xy .\" A   A   F\::C"
  15  8 at-xy .\" :\::\x7F :\::\x7F G\::\::\::D"
  14  9 at-xy .\" G\::\::\::D   F\::\::\::\::E"
  13 10 at-xy .\" F\::\::\::\::E"  ;

: drawFarIslands  ( -- )
  green ink  cyan paper
  0 2 at-xy ." Z123 HI A Z123 HI A Z123 HI Z123"  ;

: drawTreasureIsland  ( -- )
  graphFont1 set-font  green ink  blue paper
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
  shipPos @ visited @ if
    s" Llegas nuevamente a la isla de " islandName$ s+ s" ."
  else
    s" Has encontrado la perdida isla de "
    islandName$ s+ s" ..."
  then  s+ message  graphFont1 set-font  ;
  \ XXX TODO -- factor

: wipeIsland  ( -- )
  [ 3 attrLine ] literal
  [ 3 columns * ] literal
  [ yellow dup papery + ] literal fill  ;

  \ --------------------------------------------
  cr .(   -Reefs)  \ {{{2

: bottomReef  ( -- )
  black ink  blue paper
  2 14 at-xy ."  A  HI   HI       HI  HI  A"
  0 15 at-xy .\" WXY  :\::\::\x7F     Z123     :\::\::\x7F"  ;

: leftReef  ( -- )
  black ink  blue paper
   0 4 at-xy ." A"   1 6 at-xy ." HI"  0 8 at-xy ." WXY"
  1 11 at-xy ." A"  0 13 at-xy ." HI"  ;

: rightReef  ( -- )
  black ink  blue paper
  30 4 at-xy ." HI"   28 6 at-xy ." A"
  29 7 at-xy ." WXY"  31 9 at-xy ." A"  ;

: reef?  ( n -- f )  seaMap @ reef =  ;
  \ Is there a reef at sea map position _n_?

: drawReefs  ( -- )
  shipPos @ seaMapCols + reef? if  drawFarIslands  then
  shipPos @ seaMapCols - reef? if  bottomReef      then
  shipPos @ 1-           reef? if  leftReef        then
  shipPos @ 1+           reef? if  rightReef       then  ;

  \ --------------------------------------------
  cr .(   -Ships)  \ {{{2

: drawShipUp  ( x y -- )
  2dup    at-xy .\" \A\B\C"
  2dup 1+ at-xy .\" \D\E\F"
       2+ at-xy .\" \G\H\I"  ;

: drawShipDown  ( x y -- )
  2dup    at-xy .\" \J\K\L"
  2dup 1+ at-xy .\" \M\N\O"
       2+ at-xy .\" \P\Q\R"  ;

: redrawShip  ( -- )
  white ink blue paper  shipX @ shipY @
  shipUp @ if    drawShipDown  shipUp off
           else  drawShipUp    shipUp on   then  ;

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
  white ink  blue paper  18 13 at-xy .\" \S"  ;
  \ XXX TODO -- check if `emit-udg` is faster

: seaPicture  ( n -- )
  graphFont1 set-font  case
   2 of  drawBigIsland5  19 4 palm1                       endof
   3 of  drawBigIsland4
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
  treasureIsland of  drawTreasureIsland                   endof
  endcase  drawReefs  ;
  \ XXX TODO -- `12 of` is not in the original
  \ XXX TODO -- use constants
  \ XXX TODO -- simpler, use an execution table

: seaScenery  ( -- )
  graphicWindow graphFont1 set-font
  seaAndSky redrawShip  shipPos @ seaMap @ seaPicture  ;

  \ ============================================================
  cr .( Crew stamina)  \ {{{1

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
  cr .( Run aground)  \ {{{1

: damaged  ( min max -- )
  random-range damage +!  damage @ 100 min damage !  ;
  \ Increase the ship damage with random value in a range.

: runAground  ( -- )

  wipeMessage  \ XXX TODO remove?
  graphFont1 set-font
  wipeSea drawFarIslands bottomReef leftReef rightReef

  white ink
  14  8 at-xy .\" \A\B\C"
  14  9 at-xy .\" \D\E\F"
  14 10 at-xy .\" \G\H\I"
  black ink  blue paper
  17 10 at-xy ." WXY     A"
  19  6 at-xy ." A   Z123"
   6 11 at-xy ." A   HI"
   5  4 at-xy ." Z123    HI"
   7  8 at-xy .\" H\..I  A"

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
  \ XXX TODO -- factor

  \ ============================================================
  cr .( Reports)  \ {{{1

white black papery + constant report-color#

: set-report-color  ( -- )  report-color# color!  ;

: reportStart  ( -- )
  saveScreen set-report-color cls textFont set-font  ;
  \ Common task at the start of all reports.

: reportEnd  ( -- )
  set-report-color
  0 row 2+ at-xy s" Pulsa una tecla" columns type-center
  discard-key key drop  restoreScreen  ;
  \ Common task at the end of all reports.

: mainReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de situación" columns type-center
  0 4 at-xy
  ." Días:"         tab day       @ 2 .r           cr cr
  ." Hombres:"      tab alive     @ 2 .r           cr cr
  ." Moral:"        tab morale    @ 2 .r           cr cr
  ." Provisiones:"  tab supplies  @ 2 .r           cr cr
  ." Doblones:"     tab cash      @ 2 .r           cr cr
  ." Hundimientos:" tab sunkShips @ 2 .r           cr cr
  ." Munición:"     tab ammo      @ 2 .r           cr cr
  ." Barco:"        tab damage$ 2dup uppers1 type
  reportEnd  ;

 1 constant nameX
20 constant dataX

: crewReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de tripulación" columns type-center
  nameX 4 at-xy ." Nombre"  dataX 4 at-xy ." Condición"
  men 0 do
    white ink
    nameX i 6 + at-xy i name$ type
    i stamina @ staminaAttr c@ color!
    dataX i 6 + at-xy
    i stamina @ stamina$ 2dup uppers1 type
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
  ." Días"         tab day        @ 4 .r ."  x  200" cr
  ." Hundimientos" tab sunkShips  @ 4 .r ."  x 1000" cr
  ." Negocios"     tab trades     @ 4 .r ."  x  200" cr
  ." Pistas"       tab foundClues @ 4 .r ."  x 1000" cr
  ." Tesoro"       tab 4000         4 .r             cr
  updateScore
  ." Total"        tab ."       " score @ 4 .r  reportEnd  ;
  \ XXX TODO -- add subtotals

  \ ============================================================
  cr .( Ship battle)  \ {{{1

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
  \ XXX TODO -- factor

: attackOwnBoat  ( -- )
  ammo @ if  doAttackOwnBoat exit  then
  s" Por suerte no hay munición para disparar..." message
  3 pause
  s" Enseguida te das cuenta de que ibas a hundir"
  s"  uno de tus botes." s+ message
  3 pause
  wipeMessage \ XXX TODO -- needed?
  ;
  \ XXX TODO -- factor

: sunk  ( -- )
  white ink  blue paper  enemyShipX @ enemyShipY @
  2dup    at-xy ."    "
  2dup 1+ at-xy ."  ab"
  2dup 2+ at-xy ."  90"
  2dup    at-xy ."    "
  2dup 1+ at-xy ."    "
  2dup 2+ at-xy ."  ab"
  2dup    at-xy ."    "
  2dup 1+ at-xy ."    "
       2+ at-xy ."    "
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
  \ XXX TODO -- factor

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
  textFont set-font white ink  red paper 22 21 at-xy ammo ?
  graphFont1 set-font
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
  blue paper cls  textFont set-font
  white ink  red paper  10 21 at-xy ." Munición = " ammo ?

  black ink yellow paper
  22 0 do  0 i at-xy  ." ________ "  loop

  black ink  white paper
  0 2 at-xy ." 1" 0 9 at-xy ." 2" 0 16 at-xy ." 3"

  18 3 do
    black ink  graphFont2 set-font
    4 i 1- at-xy '1' emit
    4 i    at-xy '2' emit
    4 1 1+ at-xy '3' emit
    red ink  graphFont1 set-font
    6 i    at-xy ." cde"
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
  cr .( Island map)  \ {{{1

: emptyIslandMap  ( -- )  0 islandMap /islandMap cells erase  ;

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
  \ XXX TODO -- use a byte table and loop

: newIslandMap  ( -- )
  emptyIslandMap  createIslandCoast
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
  cr .( On the treasure island)  \ {{{1

: sailorSpeechBalloon  ( -- )
  25 44 plot
  20 10 rdraw 0 30 rdraw 2 2 rdraw 100 0 rdraw
  2 -2 rdraw 0 -60 rdraw -2 -2 rdraw -100 0 rdraw
  -2 2 rdraw 0 19 rdraw -20 0 rdraw  ;

: captainSpeechBalloon  ( -- )
  220 44 plot
  -15 5 rdraw 0 20 rdraw -2 2 rdraw -30 0 rdraw
  -2 -2 rdraw 0 -40 rdraw 2 -2 rdraw 30 0 rdraw 2 2 rdraw
  0 14 rdraw 15 0 rdraw  ;

: sailorAndCaptain  ( -- )
  graphFont1 set-font  cyan ink  black paper
  0 17 at-xy ."  xy" 28 at-x ." pq" cr
             ."  vs" 28 at-x ." rs" cr
             ."  wu" 28 at-x ." tu"
  sailorSpeechBalloon captainSpeechBalloon  ;

: sailorSays  ( ca len -- )  sailorWindow wcls wtype  ;

: trees  ( -- )
  wipeIsland  black ink  yellow paper
  0 7 at-xy ."  1       2       3       4"
  graphFont1 set-font  27 2 do  i 3 palm2  8 +loop  ;

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
  .\" \T\U    wu"
  28 11 palm2  0 11 palm2
  graphFont2 set-font  blue ink  yellow paper
  13 17 at-xy .\" l\::m"
    \ XXX TODO -- factor the treasure

  s" ¡Capitán, somos ricos!" message
  4 seconds  graphFont1 set-font  ;
  \ XXX TODO use this proc instead of happyEnd?

variable option

: enterTreasureIsland  ( -- )

  \ XXX TODO finish the new interface

  cls
  sunnySky
  wipeIsland
  graphFont2 set-font
  green ink  yellow paper
  0 3 at-xy ."  5     6       45     6       5"
  black ink
  25 0 do
    i 3 + 3 at-xy .\" :\x7F"
    i 2+  4 at-xy .\" :\::\::\x7F"
    i 1+  5 at-xy .\" :\::\::\::\::\x7F"
    i     6 at-xy .\" :\::\::\::\::\::\::\x7F"
    \ XXX TODO -- adapt the graphics notation
  8 +loop
  textFont set-font  white ink  red paper
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
  textFont set-font
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
  textFont set-font
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
  12 7 at-xy ." 0  " villages 1- village$ type
  graphFont2 set-font
  green ink  27 5 at-xy .\" S\::T" 27 6 at-xy ." VUW"

  textFont set-font
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
  7 16  7 14  7 13
  success? if
    at-xy ." ¡Hemos encontrado"
    at-xy ." el oro,"
    at-xy ." capitán!"  treasureFound
  else
    at-xy ." ¡Nos hemos"
    at-xy ." equivocado"
    at-xy ." capitán!"
  then  2 seconds  graphFont1 set-font  ;
  \ XXX TODO -- use a window for the last message

  \ ============================================================
  cr .( Island graphics)  \ {{{1

: wipeIslandScenery  ( -- )
  [ yellow dup papery + ] literal colorSea  ;
  \ XXX TODO -- print spaces instead

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
  0 6 at-xy ." mn" 0 10 at-xy ." kl" 0 13 at-xy ." k"
  0 4 at-xy ." m" 1 8 at-xy ." l"
  \ graphFont2 set-font \ XXX TMP -- deactivated for debugging
  yellow ink  blue paper
  iPos @ 6 +
  \ XXX FIXME -- crash here:
  ~~ islandMap  \ XXX FIXME -- why `islandMap` crashes?!
  ~~ @ coast <> if  2  3 at-xy 'A' emit  then
  iPos @ 6 + islandMap @ coast =  if  2  4 at-xy 'A' emit  then
  iPos @ 6 - islandMap @ coast =  if  2 13 at-xy 'C' emit  then
  graphFont1 set-font  ;

: drawRightWaves  ( -- )
  white ink  blue paper
  16 3 do  30 i at-xy ."  "  loop
  white ink  blue paper
  30 6 at-xy ." mn" 30 10 at-xy ." kl" 31 13 at-xy ." k"
  30 4 at-xy ." m" 31 8 at-xy ." l"
  yellow ink  blue paper  graphFont2 set-font
  iPos @ 6 + islandMap @ coast =
  if    29  4 at-xy 'B' emit  then
  iPos @ 6 - islandMap @ coast =
  if    29 13 at-xy 'D'
  else  29  3 at-xy 'B'
  then  emit  graphFont1 set-font  ;

: drawVillage  ( -- )
  graphFont2 set-font  green ink  yellow paper
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
  26  6 at-xy ." Z"  graphFont1 set-font  ;

: drawNative  ( -- )
  black ink  yellow paper  8 10 at-xy ."  _ `"
                           8 11 at-xy ." }~.,"
                           8 12 at-xy ." {|\?"  ;

: drawAmmo  ( -- )
  black ink  yellow paper  14 12 at-xy ." hi"  ;

: drawSupplies  ( -- )
  graphFont2 set-font
  black ink  yellow paper 14 12 at-xy ." 90  9099 0009"
  graphFont1 set-font  ;
  \ XXX TODO draw graphics depending on the actual ammount

: drawSnake  ( -- )
  graphFont2 set-font
  black ink  yellow paper  14 12 at-xy ." xy"
  graphFont1 set-font  ;

: drawDubloons  ( n -- )
  graphFont2 set-font  black ink  yellow paper
  12 dup at-xy s" vw vw vw vw vw vw vw vw " drop swap 3 * type
  graphFont1 set-font  ;

: islandWaves  ( -- )
  iPos @ 6 - islandMap @ coast = if  drawBottomWaves   then
  iPos @ 6 + islandMap @ coast = if  drawHorizonWaves  then
  iPos @ 1-  islandMap @ coast = if  drawLeftWaves     then
  iPos @ 1+  islandMap @ coast = if  drawRightWaves    then  ;

: islandLocation  ( -- )
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

: islandScenery  ( -- )
  graphicWindow graphFont1 set-font
  wipeIslandScenery sunnySky islandWaves islandLocation  ;

  \ ============================================================
  cr .( Events on an island)  \ {{{1

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
  cr .( Enter island location)  \ {{{1

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

  graphFont1 set-font
  100 pause \ XXX OLD

  ;

  \ ============================================================
  cr .( Disembark)  \ {{{1

: disembarkingScene  ( -- )
  graphFont1 set-font  green ink  blue paper
  31  8 at-xy ." :"
  27  9 at-xy .\" HI :\::"
  25 10 at-xy .\" F\::\::\::\::\::\::"
  23 11 at-xy .\" JK\::\::\::\::\::\::\::"
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
  cr .( Storm)  \ {{{1

: rainDrops  ( c -- )
  white ink  cyan paper
  cloud0x @ 2 at-xy dup 4 ruler type
  cloud1x @ 2 at-xy     3 ruler type  3 pause  ;

: rain  ( -- )
  graphFont1 set-font  71 1 do
    ';' rainDrops  ']' rainDrops  '[' rainDrops
    3 random 0= if  redrawShip  then
  loop  ;

: -rain  ( -- )
  cloud0x @ 2 at-xy ."     " cloud1x @ 2 at-xy ."    "  ;

: storm  ( -- )
  wipePanel stormySky
  s" Se desata una tormenta"
  s"  que causa destrozos en el barco." s+ message
  rain  10 49 damaged  -rain
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

: toReef?  ( n -- f )  shipPos @ + reef?  ;
  \ Does the sea movement offset _n_ leads to a reef?

: seaMove  ( n -- )
  dup toReef? if    drop runAground
              else  shipPos +!  then  ;
  \ Move on the sea map, using offset _n_ from the current
  \ position.

: ?seaMoveNorth?  ( -- f )
  possibleNorth @ dup 0exit  seaMapCols seaMove  ;

: ?seaMoveSouth?  ( -- f )
  possibleSouth @ dup 0exit  seaMapCols negate seaMove  ;

: ?seaMoveEast?  ( -- f )
  possibleEast @ dup 0exit  1 seaMove  ;

: ?seaMoveWest?  ( -- f )
  possibleWest @ dup 0exit  -1 seaMove  ;

: shipCommand?  ( c -- f )
  ?break  \ XXX TMP --  for debugging
  dup 0exit  case  ( c )
  'N' key-up                or-of  ?seaMoveNorth?     endof
  'S' key-down              or-of  ?seaMoveSouth?     endof
  'E' key-right             or-of  ?seaMoveWest?      endof
  'O' key-left              or-of  ?seaMoveWest?      endof
  'I'                          of  mainReport   true  endof
  'A' feasibleAttack @ and     of  attackShip   true  endof
  'T'                          of  crewReport   true  endof
  'P'                          of  scoreReport  true  endof
  'D' feasibleDisembark @ and  of  disembark    true  endof
  'F'                          of  quitGame on  true  endof
  false swap  ( false c )  endcase  ( false )  ;
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

: redrawShip?  ( -- f )  frames@ drop 1024 mod 0=  ;

: ?redrawShip  ( -- )  redrawShip? if  redrawShip  then  ;

: storm?  ( -- f )  8912 random 0=  ;
  \ XXX TODO increase the probability of storm every day?

: ?storm  ( -- )  storm? if  storm  then  ;

: shipCommand  ( -- )
  begin  ?redrawShip ?storm  inkey upper shipCommand? until  ;

  \ ============================================================
  cr .( Misc commands on the island)  \ {{{1

: embark  ( -- )
  shipPos @ visited on  1 day +!  aboard on  ;

: toLand?  ( n -- f )  iPos @ + islandMap @ coast <>  ;
  \ Does the island movement offset _n_ leads to land?

: islandMove  ( n -- )
  dup toLand? if    iPos +!  enterIslandLocation
              else  drop  then  ;
  \ Move on the sea map, using offset _n_ from the current
  \ position, if possible.
  \
  \ XXX TODO -- make the movement impossible on the panel if it
  \ leads to the sea, or show a warning

  \ ============================================================
  cr .( Clues)  \ {{{1

: pathClue$  ( -- ca len )
  s" Tomar camino " path @ number$ s+ s" ." s+  ;

: treeClue$  ( -- ca len )
  s" Parar en árbol " tree @ number$ s+ s" ." s+  ;

: turnClue$  ( -- ca len )
  s" Ir a " turn @ hand$ s+ s"  en árbol." s+  ;

: villageClue$  ( -- ca len )
  s" Atravesar poblado " village @ village$ s+ s" ." s+  ;

: directionClue$  ( -- ca len )
  s" Ir " direction @ cardinal$ s+ s"  desde poblado." s+  ;

: stepsClue$  ( -- ca len )
  s" Dar " pace @ number$ s+ s"  paso" s+
  s" s " pace @ 1 > and s+ s" desde poblado." s+  ;

create clues  ( -- a )
] pathClue$    treeClue$      turnClue$
  villageClue$ directionClue$ stepsClue$ [

: clue$  ( -- ca len )  6 random cells clues + perform  ;

: nativeTellsClue  ( -- )
  s" Bien... Pista ser..." nativeSays
  2 seconds  clue$ nativeSays
  2 seconds  s" ¡Buen viaje a isla de tesoro!" nativeSays  ;

  \ ============================================================
  cr .( Trading)  \ {{{1

: nativeSpeechBalloon  ( -- )
  black ink
  100 100 plot  20 10 rdraw  0 30 rdraw  2 2 rdraw
  100 0 rdraw  2 -2 rdraw  0 -60 rdraw  -2 -2 rdraw
  -100 0 rdraw -2 2 rdraw  0 20 rdraw  -20 0 rdraw  ;

variable price  variable offer
  \ XXX TODO -- remove `offer`, use the stack instead

: makeOffer  ( -- )
  cash @ maxOffer min >r
  s" Tienes " cash @ coins$ s+
  s" . ¿Qué oferta le haces? (1-" s+ r@ u>str s+ ." )" s+
  message
  r> getDigit offer !
  beep .2,10
  s" Le ofreces " offer @ coins$ s+ s" ." s+ message  ;
  \ Ask the player for an offer.
  \ XXX TODO -- check the note about the allowed range
  \ XXX TODO -- remove `offer`, use the stack instead
  \ XXX TODO -- rename to `yourOffer`

: rejectedOffer  ( -- )
  2 seconds  s" ¡Tú insultar! ¡Fuera de isla mía!" nativeSays
  4 seconds  embark  ;

: acceptedOffer  ( -- )
  wipeMessage
  offer @ negate cash +!  200 score +!  1 trades +!
  nativeTellsClue  4 seconds  embark  ;

: newPrice  ( -- )
  3 8 random-range dup price ! coins$ 2dup uppers1
  s"  ser nuevo precio, blanco." s+ nativeSays  ;
  \ The native decides a new price.

: lowerPrice  ( -- )
  -3 -2 random-range price +!
  s" Bueno, tú darme... " price @ coins$ s+
  s"  y no hablar más." s+ nativeSays
  makeOffer offer @ price @ >=
  if  acceptedOffer  else  rejectedOffer  then  ;
  \ The native lowers the price by several dubloons.

: oneCoinLess  ( -- )
  makeOffer offer @ price @ 1- >=
  if    acceptedOffer
  else  offer @ price @ 1- < if  rejectedOffer  then
  then  lowerPrice  ;
  \ He accepts one dubloon less

: initTrade  ( -- )
  graphFont1 set-font  black ink  yellow paper
  16 3 do  0 i at-xy blankLine$ type  loop
    \ XXX TODO improve with `fill`
  4 4 palm2  drawNative nativeSpeechBalloon
  s" Un comerciante nativo te sale al encuentro." message  ;

: trade  ( -- )
  initTrade  s" Yo vender pista de tesoro a tú." nativeSays
  5 9 random-range price !
  s" Precio ser " price @ coins$ s+ s" ." s+ nativeSays
  \ XXX TODO pause or join:
  1 seconds  s" ¿Qué dar tú, blanco?" nativeSays  makeOffer
  offer @ price @ 1-  >= if  acceptedOffer exit  then
    \ One dubloon less is accepted.
  offer @ price @ 4 - <= if  rejectedOffer exit  then
    \ Too low offer is not accepted.

  \ You offered too few
  4 random case 0 of  lowerPrice           exit  endof
                1 of  newPrice oneCoinLess exit  endof  endcase

  -1 price +!
  s" ¡No! ¡Yo querer más! Tú darme " price @ coins$ s+ s" ." s+
  nativeSays  oneCoinLess  ;

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

  graphFont2 set-font  black ink  yellow paper yellow
  14 10 do  8 i at-xy ." t   "  loop
  black ink  yellow paper  8  9 at-xy ." u"
  white ink  black  paper  9 10 at-xy ." nop"
                           9 11 at-xy ." qrs"
  graphFont1 set-font

  label L6897

  4 iPos @ islandMap !
    \ XXX TODO -- constant for 4

  label L6898

  3 seconds  ;

  \ ============================================================
  cr .( Command dispatcher on the island)  \ {{{1

: ?islandMoveNorth?  ( -- f )
  possibleNorth @ dup 0exit  islandMapCols islandMove  ;

: ?islandMoveSouth?  ( -- f )
  possibleSouth @ dup 0exit  islandMapCols negate islandMove  ;

: ?islandMoveEast?  ( -- f )
  possibleEast @ dup 0exit  1 islandMove  ;

: ?islandMoveWest?  ( -- f )
  possibleWest @ dup 0exit  -1 islandMove  ;

: ?trade?  ( -- f )  feasibleTrade @ dup 0exit  trade  ;

: ?embark?  ( -- f )  feasibleEmbark @ dup 0exit  embark  ;

: ?attack?  ( -- f )  feasibleAttack @ dup 0exit  attack  ;

: islandCommand?  ( c -- f )
  ?break  \ XXX TMP --  for debugging
  case  ( c )
    'N' key-up    or-of  ?islandMoveNorth?  endof
    'S' key-down  or-of  ?islandMoveSouth?  endof
    'E' key-right or-of  ?islandMoveEast?   endof
    'O' key-left  or-of  ?islandMoveWest?   endof
    'C'              of  ?trade?            endof
    'B'              of  ?embark?           endof
    'I'              of  mainReport   true  endof
    'M'              of  ?attack?           endof
    'T'              of  crewReport   true  endof
    'P'              of  scoreReport  true  endof
    'F'              of  quitGame on  true  endof
  false swap  ( false c )  endcase  ( false )  ;
  \ If character _c_ is a valid command on the island, execute
  \ it and return true; else return false.

: islandCommand  ( -- )
  begin  inkey upper islandCommand?  until  ;

  \ ============================================================
  cr .( Setup)  \ {{{1

: initOnce  ( -- )  initScreen  ;

: initSeaRowReefs  ( n1 n0 -- )  ?do  reef i seaMap !  loop  ;

: initSeaNorthReefs  ( -- )  seaMapCols 0 initSeaRowReefs  ;

: initSeaSouthReefs  ( -- )
  [ seaMapRows 1- seaMapCols * dup seaMapCols + ]
  literal literal initSeaRowReefs  ;

: initSeaColReefs  ( n1 n0 -- )
  ?do  reef i seaMap !  seaMapCols +loop  ;

: initSeaEastReefs  ( -- )
  [ seaMapRows 2- seaMapCols * 1+ ] literal seaMapCols
  initSeaColReefs  ;

: initSeaWestReefs  ( -- )
  [ seaMapCols 2* 1-  /seaMap seaMapCols - ]
  literal literal initSeaColReefs  ;

: initSeaReefs  ( -- )
  initSeaNorthReefs initSeaSouthReefs
  initSeaEastReefs initSeaWestReefs  ;

: initSeaSceneries  ( -- )
  /seaMap seaMapCols - seaMapCols 1+ do
    i reef? 0= if  2 21 random-range i seaMap !  then
  loop
  treasureIsland 94 104 random-range seaMap !  ;
  \ XXX TODO -- 21 is shark; these are picture types

: emptySeaMap  ( -- )
  0 seaMap  /seaMap cells erase
  0 visited /seaMap cells erase  ;

: initSeaMap  ( -- )
  emptySeaMap initSeaReefs initSeaSceneries  ;

: initShip  ( -- )
  32 42 random-range shipPos !  9 shipY !  4 shipX !
  shipUp off  ;

: initClues  ( -- )
  1 3 random-range path !  \ XXX TODO -- check range 0..?
  1 3 random-range tree !  \ XXX TODO -- check range 0..?
  0 9 random-range village !
  0 1 random-range turn !
  0 3 random-range direction !
  1 9 random-range pace !  ;  \ XXX TODO -- check range 0..?
  \ XXX TODO -- use `random` for 0..x
  \ XXX TODO -- convert all ranges to 0..x

: initPlot  ( -- )
  initClues  aboard on  1 iPos !
  men alive !  2 ammo !  5 cash !  10 morale !  10 supplies !
  quitGame off  damage off  day off  foundClues off  score off
  sunkShips off  trades off  ;

: unusedName  ( -- n )
  0  begin  drop  0 [ stockNames 1- ] literal random-range
     \ dup . yellow border blue border  \ XXX INFORMER
     dup usedName @ 0= until  ;
  \ Return the random identifier _n_ of an unused name.

: initCrewName  ( n -- )
  \ ." initCrewName " dup .  \ XXX INFORMER
  unusedName  dup usedName on  stockName$ rot name 2!  ;
  \ Choose an unused name for crew member _n_.

: initCrewNames  ( -- )  men 0 do  i initCrewName  loop  ;
  \ Choose unused names for the crew members.

: initCrewStamina  ( -- )
  men 0 do  maxStamina i stamina !  loop  ;
  \ Set the stamina of the crew to its maximum.

: initCrew  ( -- )  initCrewNames initCrewStamina  ;

: init  ( -- )
  0 randomize0
  [ 2 attrLine ] literal [ 20 columns * ] literal erase
    \ XXX TODO -- check if needed
    \ XXX TODO -- use constant to define the zone
  white ink  black paper textFont set-font
  0 14 at-xy s" Preparando el viaje..." columns type-center
  initSeaMap initShip initCrew initPlot  ;

  \ ============================================================
  cr .( Game over)  \ {{{1

: reallyQuit  ( -- )
  \ Confirm the quit
  \ XXX TODO
  ;

: playAgain  ( -- )
  \ Play again?
  \ XXX TODO
  ;

: sadEnd  ( -- )
  textFont set-font  white ink  red paper
  0 3 at-xy s" FIN DEL JUEGO" columns type-center
  theEndWindow
  supplies @ 0 <= if
    s" - Las provisiones se han agotado." wtype wcr  then
  morale @ 0 <= if
    s" - La tripulación se ha amotinado." wtype wcr  then
  ammo @ 0 <= if
    s" - La munición se ha terminado." wtype wcr  then
  alive @ 0= if
    s" - Toda tu tripulación ha muerto." wtype wcr  then
  damage @ 100 = if
    s" - El barco está muy dañado y es imposible repararlo."
    wtype wcr
  then
  cash @ 0 <= if  s" - No te queda dinero." wtype  then  ;

: happyEnd  ( -- )
  s" Lo lograste, capitán." message  ;
  \ XXX TODO --

: theEnd  ( -- )
  black ink yellow paper wcls
  graphFont1 set-font  16 1 do  27 i palm2  1 i palm2  7 +loop
  success? if  happyEnd  else  sadEnd  then
  s" Pulsa una tecla para ver tus puntos" message
  0 pause beep .2,30 scoreReport  ;
  \ XXX TODO new graphic, based on the cause of the end

  \ ============================================================
  cr .( Intro)  \ {{{1

: skulls  ( -- )
  ."   nop  nop  nop  nop  nop  nop  "
  ."   qrs  qrs  qrs  qrs  qrs  qrs  "  ;
  \ Draw a row of six skulls.

: skullBorder  ( -- )
  graphFont2 set-font white ink  black paper  1 bright
  home skulls 0 22 at-xy skulls  graphFont1 set-font
  0 bright  ;
  \ Draw top and bottom borders of skulls.

: intro  ( -- )
  ink black paper cls
  skullBorder introWindow whome get-font textFont set-font
  s" Viejas leyendas hablan del tesoro "
  s" que esconde la perdida isla de " s+
  islandName$ s+ s" ." s+ wtype wcr wcr
  s" Los nativos del archipiélago recuerdan "
  s" las antiguas pistas que conducen al tesoro. " s+
  s" Deberás comerciar con ellos para que te las digan." s+
  wtype wcr wcr
  s" Visita todas las islas hasta encontrar la isla de "
  islandName$ s+
  s"  y sigue las pistas hasta el tesoro..." s+ wtype wcr wcr
  0 row 2+ at-xy s" Pulsa una tecla" columns type-center
  6000 pause set-font  ;

  \ ============================================================
  cr .( Main)  \ {{{1

: scenery  ( -- )
  aboard @ if  seaScenery  else  islandScenery  then  panel  ;

: command  ( -- )
  aboard @ if  shipCommand  else  islandCommand  then  ;

: game  ( -- )
  cls  screenRestored off
  begin
    screenRestored @ if    screenRestored off
                     else  scenery
                     then  command
  gameOver? until  ;

: run  ( -- )
  initOnce  begin  intro init game theEnd  again  ;

  \ ============================================================
  cr .( Debugging tools [2])  \ {{{1

variable invflag
  \ XXX TMP --

: showSea  ( -- )
  cls
  seaMapRows 0 do
    seaMapCols 0 do
      invflag @ inverse  j seaMapCols * i + seaMap @ 2 .r
      invflag @ 0= invflag !
    loop  cr
  loop  0 inverse  ;

: .chars  ( c1 c0 -- )  do  i emit  loop  ;

: .ascii  ( -- )  cr 128  32 .chars  ;

: .udg    ( -- )  cr 256 128 .chars  ;

: .udgau  ( -- )  cr 165 144 .chars  ;

: .font  ( a -- )
  textFont set-font cr ." font: " dup .  set-font .ascii
  textFont set-font  ;

: .graphs  ( -- )
  cls graphFont1 .font graphFont2 .font .udg  ;

: showDamages  ( -- )
  101 0 do
    cr i . damageIndex . damage$ type  key drop
  loop  ;

: ini  ( -- )  initOnce init  ;

: f  ( -- )  textFont set-font  ;
  \ XXX TMP for debugging after an error

  \ ============================================================
  cr .( Graphics)  \ {{{1

here 256 - dup /font + to graphFont2
                       to graphFont1
  \ Update the font pointers with addresses relative to the
  \ current data pointer, were the two graphic fonts are being
  \ compiled.

  \ vim: filetype:soloforth foldmethod=marker
