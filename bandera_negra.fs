( bandera-negra )

  \ Bandera negra
  \
  \ A simulation game
  \ Written in Forth for the ZX Spectrum 128

  \ This game is a translated and improved remake of
  \   "Jolly Roger"
  \   Copyright (C) 1984 Barry Jones / Video Vault ltd.

  \ Copyright (C) 2011,2014,2015,2016 Marcos Cruz (programandala.net)

  \ Version 0.0.0+201612161632

  \ }}} ---------------------------------------------------------
  \ Requirements {{{

only forth definitions

need chars>string  need string/  need columns  need inverse
need seconds  need random-range  need at-x     need row
need ruler    need avariable  need /sconstants need case

  \ XXX TODO -- make a version of `seconds` that can be
  \ interrupted with a key press

wordlist dup constant black-flag  dup >order  set-current

  \ }}} ---------------------------------------------------------
  \ Constants {{{

135 constant /seaMap
  \ cells of the sea map (15 columns x 9 rows)

30 constant /islandMap
  \ cells of the island map

0 constant minStamina
4 constant maxStamina

: islandName$  ( -- ca len )  s" Calavera"  ;

: shipName$  ( -- ca len )  s" Furioso"  ;

  \ Ids of sea and island cells
  \ XXX TODO complete
 1 constant reef
 1 constant coast
21 constant shark

  \ Ids of island cells
  \ XXX TODO complete
2 constant dubloonsFound
3 constant nativeFights
\ 4 constant \ XXX TODO --
5 constant snake
7 constant nativeSupplies
8 constant nativeAmmo
9 constant nativeVillage

  \ }}} ---------------------------------------------------------
  \ Variables {{{

variable aboard           \ flag
variable alive            \ counter
variable ammo             \ counter
variable cash             \ counter
variable damage           \ counter
variable day              \ counter
variable foundClues       \ counter
variable morale           \ counter
variable score            \ counter
variable sunkShips        \ counter
variable supplies         \ counter
variable trades           \ counter
variable quitGame         \ flag

variable shipPicture      \ flag

variable screenRestored   \ flag

variable iPos             \ player position on the island

  \ Clues

variable path
variable tree
variable village
variable turn
variable direction
variable pace

  \ }}} ---------------------------------------------------------
  \ Arrays {{{

  \ ............................
  \ Maps

/seaMap     avariable seaMap
/islandMap  avariable islandMap
/seaMap     avariable visited    \ flags for islands

  \ ............................
  \ Crew

men avariable stamina()

  \ ............................
  \ Ship damage descriptions

0
  here ," hundi�ndose"            \ worst: sinking
  here ," a punto de hundirse"
  here ," haciendo agua"
  here ," destrozado"
  here ," casi destrozado"
  here ," gravemente da�ado"
  here ," muy da�ado"
  here ," algo da�ado"
  here ," muy poco da�ado"
  here ," casi como nuevo"
  here ," impecable"            \ best: perfect
/sconstants damageLevel$  ( n -- ca len )
            constast damageLevels

  \ ............................
  \ Village names

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

  \ }}} ---------------------------------------------------------
  \ Functions {{{

22526 constant attributes
  \ Address of the screen attributes (768 bytes)

: attrLine ( l -- a )  columns * attributes +  ;
  \ First attribute address of a character line.

: >attr ( paper ink bright -- c )  64 * + swap 8 * +  ;
  \ Convert _paper_, _ink_ and _bright_ to an attribute byte
  \ _c_.

: attrLines$ ( lines paper ink bright -- ca len )
  >attr  swap columns *  ruler  ;
  \ Convert _paper_, _ink_ and _bright_ to a string _ca len_
  \ that holds as many equivalent attribute bytes to cover
  \ _lines_ lines of the screen.

: dubloons$ ( n -- ca len )
  s" dobl " rot 1 > if s" ones"  else  s" �n"  then s+  ;
  \ Return string "doubloon" or "doubloons", depending on _n_

: number$ ( n -- ca len )
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

: highlighted$ ( c -- ca len )  0 20 rot 1 20 5 chars>string  ;
  \ Convert _c_ to a string to print _c_ as a highlighted char.

: activeOption$  ( ca1 len1 n -- ca2 len2 )
  >r 2dup r@ 1- string/
  2over drop r@ + c@ highlighted$ s+
  2swap r> 1+ /string s+  ;
  \ Convert menu option _ca len_ to an active menu option
  \ with character at position _n_ highlighted with control
  \ characters.

: option$ ( ca1 len1 n f -- ca1 len1 | ca2 len2 )
  if  activeOption$  then  ;
  \ Prepare a panel option _ca1 len1_.  If the option is
  \ active, _f_ is true and _n_ is the position of its
  \ highlighted letter.

: coins$ ( x -- ca len )
  dup >r number$ s"  " s+ r> dubloons$ s+  ;
  \ Return the text "x doubloons", with letters.

: uppers1 ( ca len -- )  drop 1 uppers  ;
  \ Change the first char of _ca len_ to uppercase.
  \ XXX TODO -- move to the library of Solo Forth

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

: condition$ ( m -- ca len )  m stamina() @ stamina$() 2@  ;
  \ Physical condition of a crew member
  \ XXX TODO --

: blankLine$  ( -- ca len )  string$(columns," ")  ;
  \ XXX TODO --

: damage$  ( -- ca len )  damageIndex damageLevel$  ;
  \ Damage description

  \ }}} ---------------------------------------------------------
  \ Main {{{

main

: main  ( -- )
  initOnce  begin  intro init game theEnd  repeat  ;

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

: scenery  ( -- )
  useScreen2
  aboard if  seaScenery  else  islandScenery  then
  panel useScreen1  ;
  \ XXX FIXME useScreen2 and usesCreen2 cause the sea
  \ background is missing

: command  ( -- )
  aboard if  shipCommand  else  islandCommand  then  ;

  \ }}} ---------------------------------------------------------
  \ Command panel {{{

22 constant panel-y

variable possibleDisembarking   \ flag
variable possibleEmbarking      \ flag
variable possibleAttacking      \ flag

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
  0 panel-y at-xy s" Informaci�n" 1 true option$ type cr
                  s" Tripulaci�n" 1 true option$ type cr
                  s" Puntuaci�n"  1 true option$ type cr

  aboard if

    \ XXX TODO possibleDisembarking only if no enemy ship is present

    shipPos visited @ 0=  shipPas seaMap @ treasureIsland =  or
    possibleDisembanking !

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
  s" Lo siento, capit�n, no puede hacer eso." message
  2 seconds  ;
  \ XXX not used yet

  \ }}} ---------------------------------------------------------
  \ Commands on the ship {{{

: shipCommand  ( -- )

  \ XXX TODO simpler, with searchable string of keys and ON
  local k,w

  do

    81 1 do
      let k=code inkey$
      if k=110 or k=11 \ "n" or up -- north
        if possibleNorth then \
         seaMove 15:exit do
      else if k=115 or k=10 \ "s" or down -- south
        if possibleSouth then \
          seaMove -15:exit do
      else if k=101 or k=9 \ "e" or right -- east
        if possibleEast then \
          seaMove 1:exit do
      else if k=111 or k=8 \ "o" or left -- west
        if possibleWest then \
          seaMove -1:exit do
      else if k=105:mainReport:exit do \ "i"
      else if k=97 \ "a"
        if possibleAttacking then \
          attackShip:exit do
      else if k=116:crewReport:exit do \ "t"
      else if k=112:scoreReport:exit do \ "p"
      else if k=100 \ "d"
        if possibleDisembarking then \
          disembark:exit do
      else if k=70:let quitGame=true:exit do \ "F" XXX TODO lowercase
      endif

      \ if not (tics mod 5) then redrawShip  \ XXX OLD
      i 40 = i 80 = or if  redrawShip  then

    loop

    \ XXX TODO increase the probability every day?
    0 80 random-range 0= if  storm  then

  loop

  ;

: seaMove  ( offset -- )
  dup shipPos + seaMap @ reef = if    drop runAground
                                else  shipPos +!
                                then  drop  ;

: disembark  ( -- )

  -2 -1 random-range supplies +!
  wipeMessage
  seaAndSky

  \ Disembarking scene
  1 charset  green ink  blue paper
  31  8 at-xy ." :"
  37  9 at-xy ." HI :\::"
  25 10 at-xy ." F\::\::\::\::\::\::"
  23 11 at-xy ." JK\::\::\::\::\::\::\::"
  yellow ink blue paper
  21 0 do  i 11 at-xy ." <>" 10 pause  loop
  aboard off
  shipPos @ seaMap @ treasureIsland =
  if    enterTreasureIsland
  else  newIslandMap enterIslandLocation  then  ;

  \ }}} ---------------------------------------------------------
  \ Trading {{{

: trade  ( -- )

  1 charset
  \ XXX TODO factor out:
  black ink  yellow paper
  16 3 do
    0 i at-xy blankLine$ type
  loop
  drawNative
  nativeSpeechBalloon
  palm2 4,4

  s" Un comerciante nativo te sale al encuentro." message
  nativeSays "Yo vender pista de tesoro a t�."

  5 9 random-range price !
  nativeSays "Precio ser "+coins$(price)+"."
  \ XXX TODO pause or join:
  1 seconds
  nativeSays "�Qu� dar t�, blanco?"
  makeOffer
  \ One dubloon less is accepted:
  if offer>=(price-1) then \
    acceptedOffer:exit proc
  \ Too low offer is not accepted:
  if offer<=(price-4) then \
    rejectedOffer:exit proc

  \ You offered too few
  1 4 random-range case
    1 of  lowerPrice  endof
    2 of  newPrice  endof
  endcase
  \ XXX TODO -- the original does a `goto`, see:
  \ on fn between(1,4)
  \   goto lowerPrice
  \   goto newPrice

  \ He reduces the price by one dubloon
  let price=price-1
  nativeSays "�No! �Yo querer m�s! T� darme "+coins$(price)+"."

  label oneCoinLess
  \ He accepts one dubloon less
  makeOffer
  if offer>=(price-1)
    acceptedOffer
  else if offer<(price-1)
    rejectedOffer
  endif

  label lowerPrice
  \ XXX TODO -- factor out
  \ He lowers the price by several dubloons
  -3 -2 random-range price +!
  nativeSays "Bueno, t� darme... "+coins$(price)+" y no hablar m�s."
  makeOffer
  if offer>=price then \
    acceptedOffer
  else \
    rejectedOffer

  exit proc

  label newPrice
  \ XXX TODO -- factor out
  3 8 random-range dup price ! coins$ 2dup uppers1
  s"  ser nuevo precio, blanco." s+ nativeSays
  goto oneCoinLess

  ;

: nativeSpeechBalloon  ( -- )
  black ink
  plot 100,100: draw 20,10: draw 0,30: draw 2,2
  draw 100,0: draw 2,-2: draw 0,-60: draw -2,-2: draw -100,0
  draw -2,2: draw 0,20: draw -20,0
  white ink  ;

: makeOffer  ( -- )

  \ Ask the player for an offer

  local maxOffer
  9 cash @ min maxOffer !
  message "Tienes "+coins$(cash)+". �Qu� oferta le haces? (1-"+str$ maxOffer+")"
  digitTo offer,maxOffer
  beep .2,10
  message "Le ofreces "+coins$(offer)+"."

  ;

: rejectedOffer  ( -- )

  2 seconds
  nativeSays "�T� insultar! �Fuera de isla m�a!"
  4 seconds
  embark

  ;

: acceptedOffer  ( -- )

  wipeMessage
  let \
    cash=cash-offer,\
    score=score+200,\
    trades=trades+1
  nativeTellsClue
  4 seconds
  embark

  ;

create nativeTellsClues  ( -- a )
]
nativeTellsClue1
nativeTellsClue2
nativeTellsClue3
nativeTellsClue4
nativeTellsClue5
nativeTellsClue6
[

: nativeTellsClue  ( -- )
  nativeSays "Bien... Pista ser..."
  2 seconds
  0 5 random-range cells nativeTellsClues + perform
  2 seconds
  nativeSays "�Buen viaje a isla de tesoro!"
  ;

: nativeTellsClue1  ( -- )
  nativeSays "Tomar camino "+trunc$ n$(path)+"."
  ;

: nativeTellsClue2  ( -- )
  nativeSays "Parar en �rbol "+trunc$ n$(tree)+"."
  ;

: nativeTellsClue3  ( -- )
  nativeSays "Ir a "+hand$(turn)+" en �rbol."
  ;

: nativeTellsClue4  ( -- )
  s" Atravesar poblado " village @ village$ s+ s" ." s+
  nativeSays  ;

: nativeTellsClue5  ( -- )
  nativeSays "Ir "+cardinal$(direction)+" desde poblado."
  ;

: nativeTellsClue6  ( -- )
  nativeSays "Dar "+trunc$ n$(pace)+" paso"+("s" and (pace>1))+" desde poblado."
  ;

  \ }}} ---------------------------------------------------------
  \ Commands on the island {{{

: islandCommand  ( -- )

  \ XXX TODO simpler, with searchable string of keys and ON

  do
    let k=code inkey$
    if k=110 or k=11 \ "n" or up -- north
      if possibleNorth then \
        islandMove 6:exit do
    else if k=115 or k=10 \ "s" or down -- south
      if possibleSouth then \
        islandMove -6:exit do
    else if k=101 or k=9 \ "e" or right -- east
      if possibleEast then \
        islandMove 1:exit do
    else if k=111 or k=8 \ "o" or left -- west
      if possibleWest then \
        islandMove -1:exit do
    else if k=99 \ "c"
      if possibleTrading then \
        trade:exit do
    else if k=98 \ "b"
      if possibleEmbarking then \
        embark:exit do
    else if k=105:mainReport:exit do \ "i"
    else if k=109 \ "m"
      if possibleAttacking then \
        attack:exit do
    else if k=116:crewReport:exit do \ "t"
    else if k=112:scoreReport:exit do \ "p"
    else if k=70:let quitGame=true:exit do \ "F" XXX TODO lowercase
    endif
  loop
  ;

: islandMove  ( offset -- )
  dup iPos @ + islandMap @ coast <>
  if    iPos +!  enterIslandLocation
  else  drop
  then  ;

: embark  ( -- )
  let visited(shipPos)=true
  let day=day+1
  aboard on
  ;

  \ }}} ---------------------------------------------------------
  \ Enter island location {{{

: enterIslandLocation  ( -- )

  wipeMessage:\ \ XXX TODO needed?
  islandScenery

  iPos @ islandMap @ case

  snake of
    manInjured
    message "Una serpiente ha mordido a "+name$(injured)+"."
  endof

  nativeFights of
    manInjured
    message \
      "Un nativo intenta bloquear el paso y hiere a "+\
      name$(injured)+\
      ", que resulta "+condition$(injured)+"."
  endof

  dubloonsFound of
    1 2 random-range dub !
    message "Encuentras "+coins$(dub)+"."
    dub @ cash +!
    drawDubloons dub
    4 iPos @ islandMap !
      \ XXX TODO -- constant for 4
  endof

  nativeAmmo of
    s" Un nativo te da algo de munici�n." message
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

  \ }}} ---------------------------------------------------------
  \ Events on an island {{{

: event1  ( -- )
  manDead
  message name$(dead)+" se hunde en arenas movedizas."
  ;

: event2  ( -- )
  manDead
  message name$(dead)+" se hunde en un pantano."
  ;

: event3  ( -- )
  manInjured
  message "A "+name$(injured)+" le muerde una ara�a."
  ;

: event4  ( -- )
  manInjured
  message "A "+name$(injured)+" le pica un escorpi�n."
  ;

: event5  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulaci�n est� hambrienta." message
  let morale=morale-1
  ;

: event6  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulaci�n est� sedienta." message
  let morale=morale-1
  ;

: event7  ( -- )
  2 5 random-range dub !
  message "Encuentras "+coins$(dub)+"."
  let cash=cash+dub
  drawDubloons dub
  ;

: event8  ( -- )
  s" Sin novedad, capit�n." message
  ;

: event9  ( -- )
  s" La costa est� despejada, capit�n." message
  ;

create islandEvents>  ( -- a )
]
    event1
    event2
    event3
    event4
    event5
    event6
    event7
    event8
    event8
    event9
    event9
    noop
    noop
[

: islandEvents  ( -- )
  0 10 random-range cells islandEvents> + perform  ;

  \ }}} ---------------------------------------------------------
  \ Island graphics {{{

: islandScenery  ( -- )

  graphicWindow
  \ XXX OLD
  \   load "attr/zp6i6b0l13" code attrLine(3)
  poke attrLine(3),attrLines$(6,yellow,yellow,0)+attrLines$(7,yellow,yellow,0)
  \ XXX TODO -- adapt

  sunnySky

  iPos @ 6 - islandMap @ coast = if  drawBottomWaves   then
  iPos @ 6 + islandMap @ coast = if  drawHorizonWaves  then
  iPos @ 1-  islandMap @ coast = if  drawLeftWaves     then
  iPos @ 1+  islandMap @ coast = if  drawRightWaves    then

  iPos @ islandMap @ case
    nativeVillage of
      drawVillage
    endof
    dubloonsFound of
      palm2 8,4
      palm2 5,14
    endof
    nativeFights of
      palm2 5,14
      palm2 8,25
      drawNative
    endof
    4 of \ XXX TODO constant
      palm2 8,25
      palm2 8,4
      palm2 5,16
    endof
    snake of
      palm2 5,13
      palm2 6,5
      palm2 8,18
      palm2 8,23
      drawSnake
    endof
    6 of \ XXX TODO constant
      palm2 8,23
      palm2 5,17
      palm2 8,4
    endof
    nativeSupplies of
      drawSupplies
      drawNative
      palm2 4,16
    endof
    nativeAmmo of
      drawAmmo
      drawNative
      palm2 5,20
    endof
  endcase  ;

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

  2 charset

  green ink  yellow paper
  print \
  6  5 at-xy ."  S\::T    ST   S\::T"
  6  6 at-xy ."  VUW    78   VUW   4"
  4  8 at-xy ." S\::T   S\::T    S\::T S\::T  S\::T "
  4  9 at-xy ." VUW   VUW  4 VUW VUW  VUW"
  4 11 at-xy ." S\::T    S\::T ST  S\::T S\::T"
  4 12 at-xy ." VUW  4 VUW 78  VUW VUW"

  black ink  yellow paper
  print \
  7 12 at-xy ." X"
  17 12 at-xy ." Y"
  22 12 at-xy ." Z"
  26 12 at-xy ." XY"
  8 9 at-xy ." ZZ"
  13 9 at-xy ." Y"
  24 9 at-xy ." ZX"
  10 6 at-xy ." XYZ"
  17 6 at-xy ." YX"
  26 6 at-xy ." Z"

  1 charset

  ;

: drawNative  ( -- )
  black ink  yellow paper
  print \
  8 10 at-xy ."  _ `"
  8 11 at-xy ." }~.,"
  8 12 at-xy ." {|\?"
  ;

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

: drawDubloons  ( coins -- )
  2 charset  black ink  yellow paper
  12 12 at-xy s" vw vw vw vw vw vw vw vw" drop coins @ 3 * type
  1 charset  ;

: palm1  ( y,x -- )
  green ink  blue paper
  x y at-xy ." OPQR"
  x y+1 at-xy ." S TU"
  yellow ink
  x+1 y+1 at-xy ." N"
  x+1 y+2 at-xy ." M"
  x+1 y+3 at-xy ." L"  ;

: palm2  ( y,x -- )
  green ink  yellow paper
  print \
  x y at-xy ." OPQR"
  x y+1 at-xy ." S TU"
  black ink
  x+1 y+1 at-xy ." N"
  x+1 y+2 at-xy ." M"
  x+1 y+3 at-xy ." L"
  x+1 y+4 at-xy ." V"
  ;

  \ }}} ---------------------------------------------------------
  \ Ship battle {{{

: attackShip  ( -- )
  ammo @ 0=
  if  noAmmoLeft
  else
    shipPos @ seaMap @ 13 >=
    shipPos @ seaMap @ 16 <= and
      \ XXX TODO -- improve the expression with `between`
    if  shipBattle  else  attackOwnBoat  then
  then  ;

: attackOwnBoat  ( -- )

  if ammo
    doAttackOwnBoat
  else
    s" Por suerte no hay munici�n para disparar..." message
    3 pause
    s" Enseguida te das cuenta de que ibas a hundir uno de tus botes." message
    3 pause
    wipeMessage \ XXX needed?
  endif

  ;

: doAttackOwnBoat  ( -- )

  let ammo=ammo-1
  s" Disparas por error a uno de tus propios botes..." message
  5 seconds

  3 random if
    s" Por suerte el disparo no ha dado en el blanco." message
  else
    \ XXX TODO inform about how many injured?
    s" La bala alcanza su objetivo. Esto desmoraliza a la tripulaci�n." message
    -2 morale +!
    3 4 random-range 1 ?do  manInjured  loop
  then
  5 seconds
  wipeMessage

  ;

: shipBattle  ( -- )
  local done,k
  let done=false
  saveScreen battleScenery
  begin
    moveEnemyShip
    let k$=inkey$
    instr("123",k$) if
      on val k$
        fire 3
        fire 10
        fire 17
    then
  done ammo 0= or until
  restoreScreen
  ammo @ 0= if  noAmmoLeft  then  ;

: battleScenery  ( -- )
  window  blue paper cls  0 charset
  white ink  red paper  10 21 at-xy ." Munici�n = " ammo ?

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

  let m=6: let n=20
  31 1 do  drawWave  loop  ;

: fire  ( row -- )
  \ XXX TODO -- store _row_ in a variable, as local
  -1 ammo +!
  0 charset white ink  red paper 22 21 at-xy ammo ?
  1 charset
  yellow ink  blue paper
  dup 1- 9 swap at-xy ." +" dup 1+ 9 swap at-xy ." -"
  moveEnemyShip
  dup 1- 9 swap at-xy space dup 1+ 9 swap at-xy space
  9 over at-xy ."  j"
  moveEnemyShip
  31 9 do
    \ XXX TODO -- `z` is the loop index:
    dup i swap at-xy ."  j"
    m=y and i=n or m=y-1 and i=n or m=y-2 and i=n
      \ XXX TODO -- convert expression
    if  sunk  then
    m=y and i=n+1 or m=y-1 and i=n+1 or m=y-2 and i=n+1
      \ XXX TODO -- convert expression
    if  sunk  then
  loop
  blue paper 30 swap at-xy ."  "  ;

: noAmmoLeft  ( -- )
  s" Te quedaste sin munici�n." message  4 seconds  ;
  \ XXX TODO the enemy wins; our ship sinks,
  \ or the money and part of the crew is captured

: moveEnemyShip  ( -- )
  1 5 random-range ship !
  let n=n+(ship=1 and n<28)-(ship=2 and n>18)
  let m=m+(ship=3 and m<17)-(ship=4 and m>1)
  white ink  blue paper
  print
  n m at-xy ."  ab "
  n m+1 at-xy ."  90 "
  n-1 m+2 at-xy ."  678 "
  n m-1 at-xy ."    "
  n m+3 at-xy ."    "
  if ship=5 then \
    drawWave
  ;

: drawWave  ( -- )
  cyan ink 11 30 random-range 1 20 random-range at-xy ." kl"  ;

: sunk  ( -- )

  \ Sunk the enemy ship

  white ink  blue paper
  print \
  n m at-xy ."    "
  n m+1 at-xy ."  ab"
  n m+2 at-xy ."  90"
  n m at-xy ."    "
  n m+1 at-xy ."    "
  n m+2 at-xy ."  ab"
  n m at-xy ."    "
  n m+1 at-xy ."    "
  n m+2 at-xy ."    "
  2 seconds
  \ XXX TODO simpler and better
  \ XXX why this condition?:
  shipPos @ seaMap @ 13 >=
  shipPos @ seaMap @ 16 <= and
    \ XXX TODO -- simplify the condition and factor out
  if  1 sunkShips +!  1000 score +!  done on  then

  shipPos @ seaMap @ case
    13 of  10  endof
    14 of   9  endof
    15 of   8  endof
    16 of   7  endof
  endcase  shipPos @ seaMap !  ;
  \ XXX TODO -- use a calculation instead

  \ }}} ---------------------------------------------------------
  \ Crew stamina {{{

: manInjured  ( -- )
  begin
    1 men random-range dup injured !
  stamina() @ until
  -1 injured @ stamina() +!
  injured @ stamina() @ 0<> alive +!  ;
  \ A man is injured.
  \ Output: `injured` = his number

: manDead  ( -- )
  \ A man dies
  \ Output: dead = his number
  begin
    1 men random-range dup dead !
  stamina() @ until
  dead stamina() off
  -1 alive +!  ;

  \ }}} ---------------------------------------------------------
  \ Attack {{{

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
    message \
      "Lo matas, pero la serpiente mata a "+\
      name$(dead)+"."
    goto L6897
  then

  iPos @ islandMap @ nativeVillage = if
    manDead
    message \
      "Un poblado entero es un enemigo muy dif�cil."+\
      name$(dead)+" muere en el combate."
    goto L6898
  then

  1 5 random-range kill !
  \ let z=int (rnd*2)+2
  if kill=1
    manDead
    message \
      "El nativo muere, pero antes mata a "+name$(dead)+"."
  else if kill=2
    s" El nativo tiene provisiones escondidas en su taparrabos." message
    let supplies=supplies+1
  else if kill>=3
    2 3 random-range dub !
    message \
      "Encuentras "+coins$(dub)+\
      " en el cuerpo del nativo muerto."
    let cash=cash+dub
  endif

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

  \ }}} ---------------------------------------------------------
  \ Storm {{{

: damaged  ( min max -- )
  random-range damage +!  damage @ 100 min damage !  ;
  \ Increase the ship damage with random value in a range

: storm  ( -- )

  \ XXX TODO make the enemy ship to move, if present
  \ (use the same graphic of the player ship)
  wipePanel
  stormySky
  10 49 damaged
  s" Se desata una tormenta que causa destrozos en el barco." message
  rain
  \ XXX TODO bright sky!
  white ink  cyan paper
  cloud0X 2 at-xy ."     " cloud1X 2 at-xy ."    "
  message "Tras la tormenta, el barco est� "+damage$+"."
  panel  ;

: rain  ( -- )
  local z
  1 charset
  71 1 do
    rainDrops ";"
    rainDrops "]"
    rainDrops "["
    if not rnd(3) then redrawShip
  loop  ;

: rainDrops  ( c$ -- )
  white ink  cyan paper
  cloud0X @ 2 at-xy string$(4,c$) type
  cloud1X @ 2 at-xy string$(3,c$) type
  3 pause  ;

  \ }}} ---------------------------------------------------------
  \ Sea graphics {{{

: seaScenery  ( -- )
  graphicWindow
  seaAndSky redrawShip  shipPos @ seaMap @ seaPicture  ;

: seaPicture  ( n -- )

  if n=2
    drawBigIsland5
    palm1 4,19
  else if n=3
    drawBigIsland4
    palm1 4,14
    palm1 4,19
    palm1 4,24
    drawShark
  else if n=4
    drawLittleIsland2
    palm1 4,14
  else if n=5
    drawLittleIsland1
    palm1 4,24
  else if n=6
    drawLittleIsland1
    palm1 4,24
    drawLittleIsland2
    palm1 4,14
  else if n=7
    drawBigIsland3
    palm1 4,19
  else if n=8
    drawBigIsland2
    palm1 4,14
    drawShark
  else if n=9
    drawBigIsland1
    palm1 4,24
  else if n=10
    palm1 4,24
    drawTwoLittleIslands
  else if n=11
    drawShark
  #else if n=12:\ \ XXX not in the original
  else if n=13
    palm1 4,24
    drawTwoLittleIslands
    drawEnemyShip
  else if n=14
    drawBigIsland1
    palm1 4,24
    drawEnemyShip
  else if n=15
    drawBigIsland2
    palm1 4,14
    drawEnemyShip
  else if n=16
    drawBigIsland3
    palm1 4,19
    drawEnemyShip
  else if n=17
    drawLittleIsland2
    palm1 4,14
    drawBoat
    drawLittleIsland1
    palm1 4,24
  else if n=18
    drawLittleIsland1
    palm1 4,24
    drawBoat
  else if n=19
    drawBigIsland4
    palm1 4,14
    palm1 4,19
    palm1 4,24
    drawBoat
    drawShark
  else if n=20
    drawBigIsland5
    palm1 4,19
    drawBoat
  else if n=shark:\ \ XXX TODO needed?
    drawShark
  endif

  drawReefs

  if n=treasureIsland then \
    drawTreasureIsland

  ;

: drawShark  ( -- )
  white ink  blue paper  18 13 at-xy ." \S"  ;
  \ XXX TODO -- adapt the UDG notation

  \ .............................................................
  \ Reefs

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

  \ .............................................................
  \ Islands

: drawBigIsland5  ( -- )
  green ink  blue paper
  print
  18 7 at-xy ." HI A"
  17 8 at-xy ." G\::\::\::\::BC"
  16 9 at-xy ." F\::\::\::\::\::\::\::D"
  14 10 at-xy ." JK\::\::\::\::\::\::\::\::E"
  13 11 at-xy ." F\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawBigIsland4  ( -- )
  green ink  blue paper
  print \
  16 7 at-xy ." WXYA"
  14 8 at-xy ." :\::\::\::\::\::\::C F\::\::D"
  13 9 at-xy ." :\::\::\::\::\::\::\::\::B\::\::\::E"
  12 10 at-xy ." F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawLittleIsland2  ( -- )
  green ink  blue paper
  print
  14 8 at-xy ." :\::\::C"
  16 7 at-xy ." A"
  13 9 at-xy ." :\::\::\::\::D"
  12 10 at-xy ." F\::\::\::\::\::E"
  ;

: drawLittleIsland1  ( -- )
  green ink  blue paper
  print \
  23 8 at-xy ." JK\::C"
  22 9 at-xy ." :\::\::\::\::D"
  21 10 at-xy ." F\::\::\::\::\::E"
  ;

: drawBigIsland3  ( -- )
  green ink  blue paper
  print \
  21 7 at-xy ." Z123"
  19 8 at-xy ." :\::\::\::\::\::C"
  18 9 at-xy ." :\::\::\::\::\::\::\::D"
  15 10 at-xy ." F\::B\::\::\::\::\::\::\::\::E"
  13 11 at-xy ." JK\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawBigIsland2  ( -- )
  green ink  blue paper
  print
  17 7 at-xy ." Z123"
  14 8 at-xy ." F\::B\::\::\::\::\::C"
  13 9 at-xy ." G\::\::\::\::\::\::\::\::\::D"
  12 10 at-xy ." F\::\::\::\::\::\::\::\::\::\::E;"
  ;

: drawBigIsland1  ( -- )
  green ink  blue paper
  print \
  20 7 at-xy ." HI A"
  19 8 at-xy ." G\::\::B\::\::\::C"
  18 9 at-xy ." F\::\::\::\::\::\::\::\::D"
  16 10 at-xy ." JK\::\::\::\::\::\::\::\::\::E"
  ;

: drawTwoLittleIslands  ( -- )
  green ink  blue paper
  print \
17 6 at-xy ." WXY  A"
16 7 at-xy ." A   A   F\::C"
15 8 at-xy ." :\::\#127 :\::\#127 G\::\::\::D"
14 9 at-xy ." G\::\::\::D   F\::\::\::\::E"
13 10 at-xy ." F\::\::\::\::E"
  ;

: drawFarIslands  ( -- )
  green ink  cyan paper
  print
  0 2 at-xy ." Z123 HI A Z123 HI A Z123 HI Z123"
  ;

: drawTreasureIsland  ( -- )

  1 charset
  green ink  blue paper
  print \
  16 7 at-xy ." A A   HI"
  13 8 at-xy ." F\::\::\::B\::\::\::B\::\::B\::\::\::C"
  12 9 at-xy ." G\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::D"
  10 10 at-xy ." JK\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::E"
  9 11 at-xy ." :\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  8 12 at-xy ." F\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::D"
  blue ink  green paper
  print \
  8 13 at-xy ."  HI Z123  HI A  A A  A "
  20 14 at-xy ." B\::\::\::\::B"
  green ink  blue paper
  print \
  31 13 at-xy ." E"
  palm1 4,19
  palm1 4,24
  palm1 4,14
  black ink  green paper
  22 9 at-xy ." \T\U":\ \ the treasure
  if visited(shipPos) then \
    message "Llegas nuevamente a la isla de "+islandName$+"."
  else \
    message "Has encontrado la perdida isla de "+islandName$+"..."
  1 charset

  ;

: wipeIsland  ( -- )
  poke attrLine(3),attrLines$(5,6,6,0)  ;
  \ XXX TODO -- use `fill`

  \ .............................................................
  \ Ships

: redrawShip  ( -- )
  shipPicture @ if    drawShipDown shipPicture off
                else  drawShipUp   shipPicture on  then  ;

: drawShipUp  ( -- )
  white ink  blue paper
  print
  shipX shipY at-xy ." \A\B\C"
  shipX shipY+1 at-xy ." \D\E\F"
  shipX shipY+2 at-xy ." \G\H\I"
  ;

: drawShipDown  ( -- )
  white ink  blue paper
  print
  shipX shipY at-xy ." \J\K\L"
  shipX shipY+1 at-xy ." \M\N\O"
  shipX shipY+2 at-xy ." \P\Q\R"
  ;

: drawEnemyShip  ( -- )
  yellow ink  blue paper
  print
  11 4 at-xy ."  ab"
  11 5 at-xy ."  90"
  11 6 at-xy ." 678"
  ;

: wipeEnemyShip  ( -- )
  blue paper
  print
  11 4 at-xy ."    "
  11 5 at-xy ."    "
  11 6 at-xy ."    "
  ;

: drawBoat  ( -- )
  yellow ink  blue paper  11 7 at-xy ." <>"  ;

  \ }}} ---------------------------------------------------------
  \ Reports {{{

: reportStart  ( -- )  saveScreen cls window 0 charset  ;
  \ Common task at the start of all reports.

: reportEnd  ( -- )  1000 pause restoreScreen  ;
  \ Common task at the end of all reports.

: .##  ( u -- )  s>d <# # # #> type  ;
  \ Print _u_ with two digits.

: .####  ( u -- )  s>d <# # # # # #> type  ;
  \ Print _u_ with four digits.

: mainReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de situaci�n" columns type-center
  0 4 at-xy
  ." D�as:"         tab day @ .## cr cr
  ." Barco:"        tab damage$ 2dup uppers1 type cr cr
  ." Hombres:"      tab alive @ .## cr
  ." Moral:"        tab using$("## ",morale) cr cr
  ." Provisiones:"  tab supplies @ .## cr
  ." Doblones:"     tab cash @ .## cr cr
  ." Hundimientos:" tab using$("## ",sunkShips) cr
  ." Munici�n:"     tab ammo @ .## cr cr
  reportEnd  ;

: crewReport  ( -- )
  local nameCol,dataCol
  let nameCol=1,dataCol=20
  reportStart
  0 1 at-xy s" Informe de tripulaci�n" columns type-center
  4 nameCol at-xy ." Nombre"
  4 dataCol at-xy ." Condici�n"
  men 1+ 1 do
    white ink
    print \
    nameCol i 5 + at-xy name$(i) type
      \ XXX TODO -- convert array
    i stamina() @ 1+ staminaPen() @ ink
    i stamina() @ 1+ staminaPap() @ paper
    i stamina() @ 1+ staminaBri() @ bright
    dataCol i 5 + at-xy
    i stamina() @ 1+ stamina$() 2@ 2dup uppers1 type
      \ XXX TODO -- convert array
  loop
  reportEnd
  ;

: updateScore  ( -- )
  foundClues @ 1000 *
  day        @  200 * +
  sunkShips  @ 1000 * +
  trades     @  200 * +
               4000 success? and +
             score +!  ;

: scoreReport  ( -- )
  reportStart
  0 1 at-xy s" Informe de puntuaci�n" columns type-center
  0 4 at-xy
    "D�as",using$("####",day);" x  200" cr
    "Hundimientos",using$("####",sunkShips);" x 1000" cr
    "Negocios",using$("####",trades);" x  200" cr
    "Pistas",using$("####",foundClues);" x 1000" cr
    "Tesoro",using$("####",4000)
  updateScore
  print '"Total","       ";using$("####",score)
  reportEnd  ;

  \ .............................................................
  \ Run aground

: runAground  ( -- )

  wipeMessage:\ \ XXX TODO remove?
  1 charset
  wipeSea
  drawFarIslands
  bottomReef
  leftReef
  rightReef

  white ink
  print
  14 8 at-xy ." \A\B\C"
  14 9 at-xy ." \D\E\F"
  14 10 at-xy ." \G\H\I"
  black ink  blue paper
  print
  17 10 at-xy ." WXY     A"
  19 6 at-xy ." A   Z123"
  6 11 at-xy ." A   HI"
  5 4 at-xy ." Z123    HI"
  7 8 at-xy ." H\..I  A"

  10 29 damaged
  \ XXX TODO improved message: "Por suerte, ..."
  message "�Has encallado! El barco est� "+damage$+"."
  \ XXX TODO print at the proper zone:
  damage @ 100 =
  if  cyan ink  black paper  7 20 at-xy ." TOTAL"  then
  black ink  green paper
  0 17 at-xy s" INFORME" columns type-center
  \ XXX TODO choose more men, and inform about them
  manInjured manDead
  -4 -1 random-range morale +!  3 seconds  ;

  \ }}} ---------------------------------------------------------
  \ Landscape graphics {{{

: stormySky  ( -- )
  \ load "attr/zp5i5b0l03" code attrLine(0) \ XXX TODO --
  let noStorm=0
  false sunAndClouds  ;

: seaWaves  ( -- )
  local z
  1 charset
  cyan ink  blue paper
  16 1 do
    1 28 random-range 4 graphicWinBottom @ random-range
    at-xy ." kl"
    1 28 random-range 4 graphicWinBottom @ random-range
    at-xy ." mn"
  loop  ;

: seaAndSky  ( -- )
  graphicWindow:\ \ XXX TMP needed, because the wipePanel before the calling
  wipeSea
  seaWaves
  sunnySky
  ;

: sunnySky  ( -- )
  \ load "attr/zp5i5b1l03" code attrLine(0)
  \ XXX OLD -- where is it?:
  #finnishTheSky 1
  ;

: sunAndClouds  ( f -- )
  2 charset  bright  yellow ink  cyan paper
  26 0 at-xy ." AB"  1 26 at-xy ." CD"  white ink
  1 9 random-range dup cloud0X !
  dup 0 at-xy ." EFGH" 1 at-xy ." IJKL"
  13 21 random-range dup cloud1X !
  dup 0 at-xy ." MNO"  1 at-xy ." PQR"
  1 charset  0 bright  ;

: wipeSea  ( -- )
  \ load "attr/zp1i1b0l13" code attrLine(3) \ XXX TODO --
  ;

  \ }}} ---------------------------------------------------------
  \ Setup {{{

: initOnce  ( -- )  initScreen  initUDG  ;

: init  ( -- )

  local i,i$

  randomize
  \ load "attr/zp0i0b0l20" code attrLine(2) \ XXX TODO --
  white ink  black paper  1 flash
  0 14 at-xy s" Preparando el viaje..." columns type-center

  0 seaMap  /seaMap cells erase
  0 visited /seaMap cells erase

  \ Reefs around the sea map
  17 1 do  reef i seaMap !  loop  \ north
  /seaMap 1+ 120 do  reef i seaMap !  loop  \ south
  106 30 do  reef i seaMap !  15 +loop  \ east
  107 32 do  reef i seaMap !  15 +loop \ west

  \ Normal islands
  120 17 do
    i seaMap @ reef <> if
      2 21 random-range i seaMap !  \ random type
      \ XXX TODO -- 21 is shark; these are picture types
    then
  loop

  \ Treasure island
  22 treasureIsland !
  treasureIsland @ 94 104 random-range seaMap !

  \ Ship position
  32 42 random-range shipPos !

  \ Ship coordinates
  9 shipY !  4 shipX !

  \ Panel lines
  let panelTop=17,panelBottom=21

  initCrew

  let iPos=1 \ player position on the island

  initClues

  shipPicture off

  \ Plot variables
  aboard on
  men alive !
  2 ammo !
  5 cash !
  0 damage !
  0 day !
  0 foundClues !
  10 morale !
  0 score !
  0 sunkShips !
  10 supplies !
  0 trades !
  quitGame off

  ;

: initClues  ( -- )

  \ Clues
  1 3 random-range path !
  1 3 random-range tree !
  1 10 random-range village !
  1 2 random-range turn !
  1 4 random-range direction !
  1 9 random-range pace !

  \ Cardinal points
  dim cardinal$(4)
  let \
    cardinal$(1)="norte",\
    cardinal$(2)="sur",\
    cardinal$(3)="este",\
    cardinal$(4)="oeste"

  \ Left and right
  dim hand$(2)
  let \
    hand$(1)="izquierda",\
    hand$(2)="derecha"

  ;

: initCrew  ( -- )

  let men=10
  initCrewNames
  initCrewStamina

  ;

: initCrewNames  ( -- )

  local man,i,i$,names,name

  let \
    names=0,\
  restore menNamesData
  do
    read i$
    let i=len i$
    exit if not i
    let \
      names=names+1
  loop
  dim names$(names)
  restore menNamesData
  names 1+ 1 do
    \ XXX TODO -- `i` is the loop index:
    read names$(i)
  loop
  dim name$(men)
  men 1+ 1 do
    \ XXX TODO -- `man` is the loop index:
    begin
      1 names random-range name !
      let i$=names$(name)
    len trunc$ i$ until
    let \
      name$(man)=i$,\
      names$(name)=""
  loop

  ;

: initCrewStamina  ( -- )

  \ XXX TODO stamina levels = array indexes
  men 1+ 1 do  maxStamina i stamina() !  loop

  \ Stamina labels (1-5)
  dim \
    stamina$(5,13),\
    staminaPen(5),\
    staminaPap(5),\
    staminaBri(5)
  restore staminaData
  6 1 do
    \ XXX TODO -- `i` is the loop index:
    read stamina$(i),staminaPen(i),staminaPap(i),staminaBri(i)
  loop

  \ Stamina colors (one string char per level)
  let \
    staminaPap$=chr$ white+chr$ black+chr$ black+chr$ black+chr$ black,\
    staminaPen$=chr$ black+chr$ red+chr$ red+chr$ yellow+chr$ green,\
    staminaBri$=chr$ 0+chr$ 1+chr$ 0+chr$ 0+chr$ 0

  ;

  \ }}} ---------------------------------------------------------
  \ Data {{{

  \ .............................
  \ Crew stamina descriptions

label staminaData
  \ Data: label,pen,paper,bright
data "muerto",black,white,0
data "herido grave",red,black,1
data "herido leve",red,black,0
data "magullado",yellow,black,0
data "en forma",green,black,0

  \ .............................
  \ Crewmen names

  \ (They are pun funny names in Spanish)

label menNamesData
data "Alfredo Minguero"
data "Armando Bronca"
data "Borja Monserrano"
data "Clemente Cato"
data "C�sar Pullido"
data "Enrique Sitos"
data "Erasmo Coso"
data "Felipe Llejo"
data "Javi Oneta"
data "Javier Nesnoche"
data "Jorge Neral"
data "Jorge Ranio"
data "Lope Lotilla"
data "Manolo Pillo"
data "Marcos Tilla"
data "Melchor Icete"
data "N�stor Nillo"
data "N�stor Tilla"
data "Paco Tilla"
data "Pascual Baricoque"
data "Pedro Medario"
data "Policarpio Nero"
data "Ramiro Inoveo"
data "Ricardo Borriquero"
data "Roberto Mate"
data "Rodrigo Minolas"
data "Ulises Cocido"
data "Ulises Tanter�a"
data "Vicente Rador"
data "V�ctor Nillo"
data "V�ctor Tilla"
data "Zacar�as Queroso"
data "�scar Romato"
data "�scar Terista"
data ""

  \ }}} ---------------------------------------------------------
  \ Island map {{{

: newIslandMap  ( -- )

  local w,z

  0 islandMap /islandMap cells erase

  coast  0 islandMap !
  coast  1 islandMap !
  coast  2 islandMap !
  coast  3 islandMap !
  coast  4 islandMap !
  coast  5 islandMap !
  coast  6 islandMap !
  coast 11 islandMap !
  coast 12 islandMap !
  coast 17 islandMap !
  coast 18 islandMap !
  coast 23 islandMap !
  coast 24 islandMap !
  coast 25 islandMap !
  coast 26 islandMap !
  coast 27 islandMap !
  coast 28 islandMap !
  coast 29 islandMap !

  23 7 do
    i islandMap @ coast <>
    if  2 5 random-range i islandMap !  then
      \ XXX TODO -- use constant instead of `2 5`
  loop

  nativeVillage 20 23 random-range islandMap !
  nativeAmmo 14 17 random-range islandMap !
  nativeSupplies 8 11 random-range islandMap !
  8 11 random-range iPos !  ;

  \ }}}----------------------------------------------------------
  \ On the treasure island {{{

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

  sailorSays "�Qu� camino, capit�n?"
  23 15 at-xy ." ?" \ XXX TODO better, in all cases
  digitTo option
  black paper
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  if option=path then let foundClues=foundClues+1

  sailorSays "�Qu� �rbol, capit�n?"
  23 15 at-xy ." ? "
  digitTo option
  0 charset
  black paper  23 15 at-xy option ?  beep .2,30
    \ XXX TODO -- factor out
  trees
  2 seconds
  if option=tree then let foundClues=foundClues+1

  \ XXX TODO better, with letters
  black paper
  7 14 at-xy ." Izquierda Derecha"
  8 16 at-xy ." I=1  D=2 "
  23 15 at-xy ." ? "
  digitTo option
  0 charset
  23 15 at-xy option ?
  beep .2,30
  2 seconds
  if option=turn then let foundClues=foundClues+1

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
  7 14 at-xy ."  Poblado  " 7 13 at-xy ." �Cu�l"
  8 16 at-xy ."  capit�n." 23 15 at-xy ." ? "
  digitTo option
  23 15 at-xy option  \ XXX TODO --
  beep .2,30
  2 seconds
  option village @ = if  1 foundClues +!  then  \ XXX TODO --

  \ XXX TODO better, with letters
  7 13 at-xy ." �Qu� camino"
  7 14 at-xy ." capit�n?"
  7 16 at-xy ." 1N 2S 3E 4O"
  23 15 at-xy ." ? "
  digitTo option \ XXX TODO -- adapt
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option direction = if  1 foundClues +!  then  \ XXX TODO --

  7 13 at-xy ." �Cu�ntos"
  7 14 at-xy ." pasos,"
  7 16 at-xy ." capit�n?"
  23 15 at-xy ." ? "
  digitTo option
  23 15 at-xy option . \ XXX TODO -- adapt
  beep .2,30
  2 seconds
  option pace = if  1 foundClues +!  then  \ XXX TODO --

  \ XXX TODO use tellZone
  black paper
  if foundClues=6 then \
    print
    7 13 at-xy ." �Hemos encontrado"
    7 14 at-xy ." el oro,"
    7 16 at-xy ." capit�n!"
      treasureFound
  else \
    print
    7 13 at-xy ." �Nos hemos"
    7 14 at-xy ."  equivocado "
    7 16 at-xy ." capit�n!"
  2 seconds  1 charset  ;

: sailorAndCaptain  ( -- )
  1 charset  cyan ink  black paper
  0 17 at-xy ."  xy" 28 at-x ." pq" cr
             ."  vs" 28 at-x ." rs" cr
             ."  wu" 28 at-x ." tu"
  sailorSpeechBalloon captainSpeechBalloon  ;

: sailorSpeechBalloon  ( -- )
  plot 25,44
  draw 20,10:draw 0,30:draw 2,2:draw 100,0
  draw 2,-2:draw 0,-60:draw -2,-2:draw -100,0
  draw -2,2:draw 0,19:draw -20,0
  ;

: captainSpeechBalloon  ( -- )
  plot 220,44
  draw -15,5:draw 0,20:draw -2,2:draw -30,0
  draw -2,-2:draw 0,-40:draw 2,-2:draw 30,0:draw 2,2
  draw 0,14:draw 15,0
  ;

: sailorSays  ( text$ -- )
  \ XXX TODO use window instead
  wipeSailorSpeech
  tellZone text$,12,12,6
  ;

: wipeSailorSpeech  ( -- )
  19 12 do
    6 i at-xy ."            "
  loop  ;

: trees  ( -- )
  local z
  wipeIsland
  black ink  yellow paper
  print \
  0 7 at-xy ."  1       2       3       4"
  1 charset
  27 2 do
    \ XXX TODO -- `z` is the loop index:
    palm2 3,z
  8 +loop  ;

  \ }}} ---------------------------------------------------------
  \ User input {{{

: seconds  ( n -- )  50 * pause  ;

: digitTo  \ XXX TODO -- parameters: ref answer,max

  \ Return the digit number pressed by the player

  default max=9

  do
    0 pause
    let answer=code inkey$-code "0"
    if answer<1 or answer>max then beep .1,10
  \ loop until answer>0 and answer<=max  \ XXX OLD
  answer @ 0> answer @ max <= and until  ;

  \ }}} ---------------------------------------------------------
  \ UDGs and charsets {{{

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

  \ }}} ---------------------------------------------------------
  \ Game over{{{

: theEnd  ( -- )

  local z

  black ink yellow paper cls1

  \ XXX TODO new graphic, based on the cause of the end
  1 charset
  #for z=1 to 15 step 7
  16 1 do
    \ XXX TODO -- `z` is the loop index:
    palm2 z,27:palm2 z,1
  7 +loop

  if foundClues=6 then happyEnd:else sadEnd

  s" Pulsa una tecla para ver tus puntos" message
  0 pause
  beep .2,30
  scoreReport

  ;

: reallyQuit  ( -- )
  \ Confirm the quit
  \ XXX TODO
  ;

: playAgain  ( -- )
  \ Play again?
  \ XXX TODO
  ;

: sadEnd  ( -- )

  \ XXX TODO uset TellZone
  0 charset
  white ink  red paper
  0 3 at-xy s" FIN DEL JUEGO" columns type-center
  window 5,26,2,21 \ XXX TODO
  if supplies<=0 then \
  s" Las provisiones se han agotado." tell
  if morale<=0 then \
  s" La tripulaci�n se ha amotinado." tell
  if ammo<=0 then \
  s" La munici�n se ha terminado." tell
  if not alive then \
  s" Toda tu tripulaci�n ha muerto." tell
  if damage=100 then \
  s" El barco est� muy da�ado y es imposible repararlo." tell
  if cash<=0 then \
  s" No te queda dinero." tell
  window

  ;

: treasureFound  ( -- )

  \ XXX TODO use this proc instead of happyEnd?

  local z

  2 seconds
  \ load "attr/zp5i5b1l03" code attrLine(0)
  \ load "attr/zp6i6b0l18" code attrLine(4)
    \ XXX TODO --
  sunnySky
  23 7 do
    \ XXX TODO -- `z` is the loop index:
    palm2 5,z
  5 +loop
  palm2 7,3:palm2 7,26
  \ Cofre del tesoro:
  black ink  yellow paper
  print
  8 13 at-xy
  ." pq          xy                  "
  ." rs          vs                  tu      "
  ." \T\U    wu"
  palm2 11,28:palm2 11,0
  2 charset
  blue ink  yellow paper
  print
  13 17 at-xy ." l\::m"
  s" �Capit�n, somos ricos!" message
  4 seconds
  1 charset

  ;

: happyEnd  ( -- )
  s" Lo lograste, capit�n." message
  ;

  \ }}} ---------------------------------------------------------
  \ Intro {{{

: intro  ( -- )
  cls
  skullBorder
  introWindow
  s" Viejas leyendas hablan del tesoro" tell
  s" que esconde la perdida isla de" tell
  islandName$ s" ." s+ tellCR
  s" Los nativos del archipi�lago recuerdan" tell
  s" las antiguas pistas que conducen al tesoro." tell
  s" Deber�s comerciar con ellos para que te las digan." tellCR
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

  \ }}} ---------------------------------------------------------
  \ Text output {{{

: tell  ( ca len -- )
  0 charset
  begin  dup columns >  while
    \ for char=cpl to 1 step -1 \ XXX OLD
    0 columns do
      over i + c@ bl = if
        2dup drop i 1- type
        i 1+ string/ \ XXX OLD: let text$=text$(char+1 to)
                     \ XXX OLD -- `char` was the loop index
        unloop leave
      then
    -1 +loop
  repeat  type  ;

: tellCR  ( ca len -- )  tell cr  ;

: nativeSays  ( ca len -- )
  nativeWindow
  cls1
  tell
  \ XXX OLD
  #wipeNativeWords
  #tellZone text$,12,6,16
  ;

: message  ( ca len -- )
  0 charset  wipeMessage  messageWindow  tell  graphicWindow  ;

: tellZone  ( text$,width,row,col -- )

  \ XXX OLD
  \ XXX TODO use WINDOW instead

  local char

  0 charset
  begin len text$<=width while
    \ for char=width to 1 step -1
    0 width do
      \ XXX TODO -- `char` is the loop index:
      if text$(char)=" " then \
        col row at-xy
          \ XXX TODO -- adapt
        text$(to char-1) type
          \ XXX TODO -- adapt
        let text$=text$(char+1 to)
        let row=row+1
        exit for
    -1 +loop
  repeat
  col row at-xy text$ type  ;
  \ XXX TODO -- adapt

  \ }}} ---------------------------------------------------------
  \ Screen {{{

: initScreen  ( -- )

  cls

  \ Some window parameters
  let introWinTop=3
  let \
    graphicWinTop=0,\
    graphicWinBottom=15,\
    graphicWinLeft=0,\
    graphicWinRight=31,\
    graphicWinWidth=graphicWinRight-graphicWinLeft+1,\
    graphicWinHeight=graphicWinBottom-graphicWinTop+1,\
    graphicWinChars=graphicWinWidth*graphicWinHeight
  let \
    lowWinTop=21,\
    lowWinBottom=23,\
    lowWinLeft=0,\
    lowWinRight=31,\
    lowWinWidth=lowWinRight-lowWinLeft+1,\
    lowWinHeight=lowWinBottom-lowWinTop+1,\
    lowWinChars=lowWinWidth*lowWinHeight
  let \
    messageWinTop=17,\
    messageWinBottom=19,\
    messageWinLeft=1,\
    messageWinRight=30,\
    messageWinWidth=messageWinRight-messageWinLeft+1,\
    messageWinHeight=messageWinBottom-messageWinTop+1,\
    messageWinChars=messageWinWidth*messageWinHeight

  graphicWindow
  commandWindow

  ;

: wholeWindow  ( -- )
  window 0,31,0,20  ;

: graphicWindow  ( -- )
  window graphicWinLeft,graphicWinRight,graphicWinTop,graphicWinBottom
  1 charset  ;
  \ Zone where graphics are shown

: introWindow  ( -- )
  window 2,29,introWinTop,21  ;
  \ Zone where intro text is shown

: messageWindow  ( -- )
  window messageWinLeft,messageWinRight,messageWinTop,messageWinBottom
  ;

: commandWindow  ( -- )
  lowWindow lowWinLeft,lowWinRight,lowWinTop,lowWinBottom
  ;

: nativeWindow  ( -- )
  \ Window for native's speech
  window 16,26,6,9
  ;

: lowWindow  ( left,right,top,bottom -- )
  poke LWRHS,right,left,top,bottom
  ;

: wipePanel  ( -- )
  print #0;paper black;at 0,0;string$(lowWinChars," ");
  ;

: wipeMessage  ( -- )
  \ load "attr/zp0i0b0l06" code attrLine(panelTop-1)
  \ XXX OLD
  messageWindow
  white ink  black paper  cls1  ;

: saveScreen  ( -- )
  \ copy screen 1 to 2
  ;
  \ XXX TODO --

: restoreScreen  ( -- )
  \ copy screen 2 to 1
  screenRestored on  ;
  \ XXX TODO --

: useScreen2  ( -- )  saveScreen 2 screen  ;

: useScreen1  ( -- )  restoreScreen 1 screen  ;

  \ }}} ---------------------------------------------------------
  \ Meta {{{

variable invflag
  \ XXX TMP --

: showSea  ( -- )
  local x,y
  cls
  \ for y=0 to 8*2 step 2 \ XXX OLD
  17 0 do
    \ XXX TODO -- `y` is the outer loop index:
    \ for x=0 to 14 \ XXX OLD
    15 0 do
      \ XXX TODO -- `x` is the inner loop index:
      invflag @ inverse
      y 9 * x + 1+ seaMap @ .##
      invflag @ 0= invflag !
    loop
    cr
  2 +loop
  mode 1  0 inverse  ;

: showCharsets  ( -- )
  cls  3 0 do  i showCharset  loop
       0 charset cr ." UDG" showUdg  ;

: showCharset  ( n -- )
  0 charset cr ." charset " n .
  n charset showASCII  ;

: showASCII  ( -- )  128 32 do  i emit  loop  ;

: showUDG  ( -- )  256 128 do  i emit  loop  ;

: showDamages  ( -- )
  101 0 do
    \ XXX TODO -- `i` is the loop index:
    let damage=i
    print damage,damageIndex;" ";damage$
  loop  ;

  \ vim: set filetype:soloforth
