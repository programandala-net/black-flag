( bandera-negra )

  \ Bandera negra
  \
  \ A simulation game
  \ Written in Forth for the ZX Spectrum 128

  \ This game is a translated and improved remake of
  \   "Jolly Roger"
  \   Copyright (C) 1984 Barry Jones / Video Vault ltd.

  \ Copyright (C) 2011,2014,2015 Marcos Cruz (programandala.net)

  \ Version 0.0.0+201612151459

  \ }}} ---------------------------------------------------------
  \ Functions {{{

: attrLine ( l -- a )  32 * attrAd +  ;
  \ First attribute address of a character line (mode 1).

: attr$ ( paper ink bright -- c )  64 * + swap 8 * +  ;
  \ XXX TODO -- rename

: attrLines$ ( line paper ink bright -- ca len )
  attr$  swap 32 *  string$  ;
  \ XXX TODO -- finish


: dubloons$ ( n -- ca len )
  s" dobl " rot 1 > if s" ones"  else  s" ón"  then s+  ;
  \ "doubloon" or "doubloons", depending on _n_

dim n$(11,6) \ numbers with letters
let \
  n$(1)="un",\
  n$(2)="dos",\
  n$(3)="tres",\
  n$(4)="cuatro",\
  n$(5)="cinco",\
  n$(6)="seis",\
  n$(7)="siete",\
  n$(8)="ocho",\
  n$(9)="nueve",\
  n$(10)="diez",\
  n$(11)="once"
  \ XXX TODO -- create a string table

: number$ ( n -- ca len )  n$(n)  ;
  \ Convert _n_ to letters in string _ca len_.
  \ XXX TODO --

: highlighted$ ( c -- ca len )
  chr$ 20+chr$ 1+c$+chr$ 20+chr$ 0  ;
  \ A highlighted char
  \ XXX TODO --

: activeOption$  ( ca len l -- )
  o$(to l-1)+fn highlighted$(o$(l))+o$(l+1 to)  ;
  \ An active option of the panel; l=pos of highlighted letter
  \ XXX TODO -- _ca len_ is o$

: option$ ( ca len l a)
  (o$ and not a)+(fn activeOption$(o$,l) and a)  ;
  \ A panel option; a=active?; l=pos of highlighted letter
  \ XXX TODO -- _ca len_ is o$

  \ XXX OLD
  \ deffn center$(l,t$)=\
  \   \ A text centered at line l
  \   chr$ 22+chr$ l+chr$ ((fn cpl-len t$)/2)+t$

: centered$  ( ca len -- )
  chr$ 23+chr$ ((fn cpl-len t$)/2)+chr$ 0+t$  ;
  \ A text centered on the current line
  \ (the cursor must be at the start of the line)
  \ XXX Why a chr$ 0 is needed? Without it, the first letter of
  \ the text is removed.
  \ XXX TODO --

: banner$ ( ca len -- )
  (string$((fn cpl-len t$)/2," ")+t$+string$(fn cpl," "))(to fn cpl)  ;
  \ A text centered on the current line
  \ (the cursor must be at the start of the line
  \ and the whole line is overwritten)
  \ XXX TODO --

: coins$ ( x -- ca len )
  dup >r n>letters s"  " s+ r> dubloons$ s+  ;
  \ Return the text "x doubloons", with letters.
  \ XXX TODO --

: upper1$ ( ca len -- ca len )  over c@ upper >r over r> swap c!  ;
  \ Change the first char of _ca len_ to uppercase.

: failure  ( -- f )
  alive @ 0=
  morale @ 1 < or
  damageIndex damageLevels = or
  supplies @ 1 < or
  cash @ 1 < or  ;
  \ Failed mission?

: success  ( -- f )  foundClues @ 6 =  ;
  \ Success?

: gameOver  ( -- f )  failure success quitGame or or  ;
  \ Game over?

: condition$ ( m -- ca len )  stamina$(stamina(m))  ;
  \ Physical condition of a crew member
  \ XXX TODO --

: blankLine$  ( -- ca len )  string$(fn cpl," ")  ;
  \ XXX TODO --

: damageIndex  ( -- n )  damage @ damageLevels @ 101 / 1+  ;

: damage$  ( -- ca len )  damage$(damageIndex)  ;
  \ Damage description
  \ XXX TODO -- `damage$()`  is the array

  \ }}} ---------------------------------------------------------
  \ Constants {{{

: islandName$  ( -- ca len )  s" Calavera"  ;
: shipName$  ( -- ca len )  s" Furioso"  ;

  \ Ids of sea cells
 1 constant reef
 2 constant coast
21 constant shark

  \ Ids of island cells
  \ XXX TODO complete
1 constant coast
2 constant dubloonsFound
3 constant nativeFights
5 constant snake
7 constant nativeSupplies
8 constant nativeAmmo
9 constant nativeVillage

  \ }}} ---------------------------------------------------------
  \ Main {{{

main

: main  ( -- )
  initOnce  begin  intro init game theEnd  repeat  ;

: game  ( -- )
  cls
  let screenRestored=false
  begin
    if not screenRestored then \
      \ XXX FIXME sometimes scenery is called here without reason
      \ XXX The logic is wrong.
      scenery
    else \
      let screenRestored=0
    command
  fn gameOver until
  ;

: scenery  ( -- )
  \ XXX FIXME useScreen2 and usesCreen2 cause the sea
  \ background is missing
  useScreen2
  aboard if  seaScenery  else  islandScenery  then
  panel
  useScreen1
  ;

: command  ( -- )
  aboard if  shipCommand  else  islandCommand  then  ;

  \ }}} ---------------------------------------------------------
  \ Command panel {{{

: panel  ( -- )

  wipePanel
  0 charset

  print #0;pen white;\
    at 0,0;fn option$("Información",1,1);\
    at 1,0;fn option$("Tripulación",1,1);\
    at 2,0;fn option$("Puntuación",1,1)

  aboard if
    \ XXX TODO possibleDisembarking only if no enemy ship is present
    let possibleDisembarking=(visited(shipPos)=false) or (seaMap(shipPos)=treasureIsland)
    print #0;at 0,16;fn option$("Desembarcar",1,possibleDisembarking)
  else
    let possibleEmbarking=true \ XXX TODO only if iPos is coast
    print #0;at 0,16;fn option$("emBarcar",3,possibleEmbarking)
  then

  \ XXX TODO check condition -- what about the enemy ship?
  \ XXX TODO several commands: attack ship/island/shark?
  let possibleAttacking=not (seaMap(shipPos)<13 or seaMap(shipPos)=shark or seaMap(shipPos)=treasureIsland)
  print #0;at 1,16;fn option$("Atacar",1,possibleAttacking)

  let possibleTrading=islandMap(iPos)=nativeVillage
  print #0;at 2,16;fn option$("Comerciar",1,possibleTrading)

  directionsMenu

  ;

: directionsMenu  ( -- )

  \ XXX TODO conditions
  let \
    possibleNorth=true,\
    possibleSouth=true,\
    possibleEast=true,\
    possibleWest=true
  print #0;paper black; pen white;\
    at 0,30;inverse possibleNorth;"N";inverse 0;\
    at 1,29;inverse possibleWest;"O";inverse 0;\
    at 1,31;inverse possibleEast;"E";inverse 0;\
    at 2,30;inverse possibleSouth;"S";inverse 0;\
    at 1,30;"+"
  \ XXX TODO use a modified  version of "+"?

  ;

: impossible  ( -- )
  s" Lo siento, capitán, no puede hacer eso." message
  seconds 2  ;
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
    if not fn between(0,80) then storm

  loop

  ;

: seaMove  ( offset -- )
  if seaMap(shipPos+offset)=reef then \
    runAground
  else \
    let shipPos=shipPos+offset
    \ XXX OLD
    #seaScenery
  ;

: disembark  ( -- )

  let supplies=supplies-fn between(1,2)
  wipeMessage
  seaAndSky

  \ Disembarking scene
  1 charset
  print pen green; paper blue;\
    at 8,31;":";\
    at 9,27;"HI :\::";\
    at 10,25;"F\::\::\::\::\::\::";\
    at 11,23;"JK\::\::\::\::\::\::\::"
  yellow ink blue paper
  21 0 do
    i 11 at-xy ." <>"  10 pause
  loop

  aboard off
  if seaMap(shipPos)=treasureIsland then \
    enterTreasureIsland
  else \
    newIslandMap
    enterIslandLocation

  ;

  \ }}} ---------------------------------------------------------
  \ Trading {{{

: trade  ( -- )

  1 charset
  \ XXX TODO factor out:
  black ink  yellow paper
  16 3 do
    0 i at-xy fn blankLine$ type
  loop
  drawNative
  nativeSpeechBalloon
  palm2 4,4

  s" Un comerciante nativo te sale al encuentro." message
  nativeSays "Yo vender pista de tesoro a tú."

  let price=fn between(5,9)
  nativeSays "Precio ser "+fn coins$(price)+"."
  \ XXX TODO pause or join:
  seconds 1
  nativeSays "¿Qué dar tú, blanco?"
  makeOffer
  \ One dubloon less is accepted:
  if offer>=(price-1) then \
    acceptedOffer:exit proc
  \ Too low offer is not accepted:
  if offer<=(price-4) then \
    rejectedOffer:exit proc

  \ You offered too few
  on fn between(1,4)
    goto lowerPrice
    goto newPrice

  \ He reduces the price by one dubloon
  let price=price-1
  nativeSays "¡No! ¡Yo querer más! Tú darme "+fn coins$(price)+"."

  label oneCoinLess
  \ He accepts one dubloon less
  makeOffer
  if offer>=(price-1)
    acceptedOffer
  else if offer<(price-1)
    rejectedOffer
  endif

  label lowerPrice
  \ He lowers the price by several dubloons
  let price=price-fn between(2,3)
  nativeSays "Bueno, tú darme... "+fn coins$(price)+" y no hablar más."
  makeOffer
  if offer>=price then \
    acceptedOffer
  else \
    rejectedOffer

  exit proc

  label newPrice
  let price=fn between(3,8)
  nativeSays fn upper1$(fn coins$(price))+" ser nuevo precio, blanco."
  goto oneCoinLess

  ;

: nativeSpeechBalloon  ( -- )
  pen black
  plot 100,100: draw 20,10: draw 0,30: draw 2,2
  draw 100,0: draw 2,-2: draw 0,-60: draw -2,-2: draw -100,0
  draw -2,2: draw 0,20: draw -20,0
  pen white
  ;

: makeOffer  ( -- )

  \ Ask the player for an offer

  local maxOffer
  let maxOffer=fn min(9,cash)
  message "Tienes "+fn coins$(cash)+". ¿Qué oferta le haces? (1-"+str$ maxOffer+")"
  digitTo offer,maxOffer
  beep .2,10
  message "Le ofreces "+fn coins$(offer)+"."

  ;

: rejectedOffer  ( -- )

  seconds 2
  nativeSays "¡Tú insultar! ¡Fuera de isla mía!"
  seconds 4
  embark

  ;

: acceptedOffer  ( -- )

  wipeMessage
  let \
    cash=cash-offer,\
    score=score+200,\
    trades=trades+1
  nativeTellsClue
  seconds 4
  embark

  ;

: nativeTellsClue  ( -- )

  local clue
  nativeSays "Bien... Pista ser..."
  seconds  2
  on fn between(1,6)
    nativeTellsClue1
    nativeTellsClue2
    nativeTellsClue3
    nativeTellsClue4
    nativeTellsClue5
    nativeTellsClue6
  seconds  2
  nativeSays "¡Buen viaje a isla de tesoro!"

  ;

: nativeTellsClue1  ( -- )
  nativeSays "Tomar camino "+trunc$ n$(path)+"."
  ;

: nativeTellsClue2  ( -- )
  nativeSays "Parar en árbol "+trunc$ n$(tree)+"."
  ;

: nativeTellsClue3  ( -- )
  nativeSays "Ir a "+hand$(turn)+" en árbol."
  ;

: nativeTellsClue4  ( -- )
  nativeSays "Atravesar poblado "+village$(village)+"."
  ;

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

  if islandMap(iPos+offset)<>coast then \
    let iPos=iPos+offset
    enterIslandLocation

  ;

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

  if islandMap(iPos)=snake \ XXX MasterBASIC BUG? "b not found"!
    manInjured
    message "Una serpiente ha mordido a "+name$(injured)+"."

  else if islandMap(iPos)=nativeFights
    manInjured
    message \
      "Un nativo intenta bloquear el paso y hiere a "+\
      name$(injured)+\
      ", que resulta "+fn condition$(injured)+"."

  else if islandMap(iPos)=dubloonsFound
    let dub=fn between(1,2)
    message "Encuentras "+fn coins$(dub)+"."
    let cash=cash+dub
    drawDubloons dub
    let islandMap(iPos)=4

  else if islandMap(iPos)=nativeAmmo
    s" Un nativo te da algo de munición." message
    let ammo=ammo+1
    let islandMap(iPos)=nativeFights

  else if islandMap(iPos)=nativeSupplies
    s" Un nativo te da provisiones." message
    \ XXX TODO random ammount
    let supplies=supplies+1
    let islandMap(iPos)=nativeFights

  else if islandMap(iPos)=nativeVillage
    s" Descubres un poblado nativo." message

  \ XXX TODO constants for these cases:
  else if islandMap(iPos)=4 or islandMap(iPos)=6
    islandEvents

  endif

  1 charset
  100 pause \ XXX OLD

  ;

  \ }}} ---------------------------------------------------------
  \ Events on an island {{{

: islandEvents  ( -- )
  on fn between(1,11)
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
  ;

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
  message "A "+name$(injured)+" le muerde una araña."
  ;

: event4  ( -- )
  manInjured
  message "A "+name$(injured)+" le pica un escorpión."
  ;

: event5  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulación está hambrienta." message
  let morale=morale-1
  ;

: event6  ( -- )
  \ XXX TODO only if supplies are not enough
  s" La tripulación está sedienta." message
  let morale=morale-1
  ;

: event7  ( -- )
  let dub=fn between(2,5)
  message "Encuentras "+fn coins$(dub)+"."
  let cash=cash+dub
  drawDubloons dub
  ;

: event8  ( -- )
  s" Sin novedad, capitán." message
  ;

: event9  ( -- )
  s" La costa está despejada, capitán." message
  ;

  \ }}} ---------------------------------------------------------
  \ Island graphics {{{

: islandScenery  ( -- )

  graphicWindow
  \ XXX OLD
  \   load "attr/zp6i6b0l13" code fn attrLine(3)
  poke fn attrLine(3),fn attrLines$(6,yellow,yellow,0)+fn attrLines$(7,yellow,yellow,0)
  sunnySky
  if islandMap(iPos-6)=coast then drawBottomWaves
  if islandMap(iPos+6)=coast then drawHorizontWaves
  if islandMap(iPos-1)=coast then drawLeftWaves
  if islandMap(iPos+1)=coast then drawRightWaves
  if islandMap(iPos)=nativeVillage
    drawVillage
  else if islandMap(iPos)=dubloonsFound
    palm2 8,4
    palm2 5,14
  else if islandMap(iPos)=nativeFights
    palm2 5,14
    palm2 8,25
    drawNative
  else if islandMap(iPos)=4 \ XXX TODO constant
    palm2 8,25
    palm2 8,4
    palm2 5,16
  else if islandMap(iPos)=snake
    palm2 5,13
    palm2 6,5
    palm2 8,18
    palm2 8,23
    drawSnake
  else if islandMap(iPos)=6 \ XXX TODO constant
    palm2 8,23
    palm2 5,17
    palm2 8,4
  else if islandMap(iPos)=nativeSupplies
    drawSupplies
    drawNative
    palm2 4,16
  else if islandMap(iPos)=nativeAmmo
    drawAmmo
    drawNative
    palm2 5,20
  endif

  ;

: drawHorizontWaves  ( -- )
  print at 3,0; pen white; paper blue;\
  "  kl  mn     nm    klk   nm nm n"
  ;

: drawBottomWaves  ( -- )
  print at 14,0; paper blue; pen white;\
  "  kl     mn  mn    kl    kl kl  m     mn      klmn   mn m  mn   "
  ;

: drawLeftWaves  ( -- )
  white ink blue paper
  16 3 do
    0 i at-xy ."  "
  loop
  print at 6,0; pen white; paper blue;"mn";at 10,0;"kl";at 13,0;"k";at 4,0;"m";at 8,1;"l"
  if islandMap(iPos+6)<>1 then \
    2 charset
    print at 3,2; pen yellow; paper blue;"A"
    1 charset
  if islandMap(iPos+6)=1 then \
    2 charset
    print at 4,2; pen yellow; paper blue;"A"
    1 charset
  if islandMap(iPos-6)=1 then \
    2 charset
    print at 13,2; pen yellow; paper blue;"C"
    1 charset
  ;

: drawRightWaves  ( -- )
  white ink  blue paper
  16 3 do
    30 i at-xy ."  "
  loop
  print at 6,30; pen white; paper blue;"mn";at 10,30;"kl";at 13,31;"k";at 4,30;"m";at 8,31;"l"
  2 charset
  if islandMap(iPos+6)=1 then \
    print at 4,29; pen yellow; paper blue;"B"
  if islandMap(iPos-6)=1 then \
    print at 13,29; pen yellow; paper blue;"D"
  if islandMap(iPos+6)<>1 then \
    print at 3,29; pen yellow; paper blue;"B"
  1 charset

  ;

: drawVillage  ( -- )

  2 charset

  print pen green;paper yellow;\
    at 5,6;" S\::T    ST   S\::T";\
    at 6,6;" VUW    78   VUW   4";\
    at 8,4;"S\::T   S\::T    S\::T S\::T  S\::T ";\
    at 9,4;"VUW   VUW  4 VUW VUW  VUW";\
    at 11,4;"S\::T    S\::T ST  S\::T S\::T";\
    at 12,4;"VUW  4 VUW 78  VUW VUW"

  print pen black;paper yellow;\
    at 12,7;"X";\
    at 12,17;"Y";\
    at 12,22;"Z";\
    at 12,26;"XY";\
    at 9,8;"ZZ";\
    at 9,13;"Y";\
    at 9,24;"ZX";\
    at 6,10;"XYZ";\
    at 6,17;"YX";\
    at 6,26;"Z"

  1 charset

  ;

: drawNative  ( -- )
  print pen black;paper yellow;\
    at 10,8;" _ `";\
    at 11,8;"}~.,";\
    at 12,8;"{|\?"
  ;

: drawAmmo  ( -- )
  print pen black;paper yellow;at 12,14; "hi"
  ;

: drawSupplies  ( -- )

  \ XXX TODO draw graphics depending on the actual ammount
  2 charset
  print at 12,14; pen black; paper yellow;"90  9099 0009"
  1 charset

  ;

: drawSnake  ( -- )
  2 charset
  print pen black;paper yellow;\
    at 12,14; "xy"
  1 charset
  ;

: drawDubloons  ( coins -- )

  2 charset
  print pen black;paper yellow;\
    at 12,12; "vw vw vw vw vw vw vw vw"(to coins*3)
  1 charset

  ;

: palm1  ( y,x -- )
  print pen green;paper blue;\
    at y,x;"OPQR";\
    at y+1,x;"S TU";\
    at y+1,x+1; pen yellow; "N";\
    at y+2,x+1;"M";\
    at y+3,x+1;"L"
  ;

: palm2  ( y,x -- )
  print pen green; paper yellow;\
    at y,x;"OPQR";\
    at y+1,x;"S TU";\
    at y+1,x+1; pen black;"N";\
    at y+2,x+1;"M";\
    at y+3,x+1;"L";\
    at y+4,x+1;"V"
  ;

  \ }}} ---------------------------------------------------------
  \ Ship battle {{{

: attackShip  ( -- )
  if not ammo
    noAmmoLeft
  else
    if seaMap(shipPos)>=13 and seaMap(shipPos)<=16 then \
      shipBattle
    else \
      attackOwnBoat
  endif
  ;

: attackOwnBoat  ( -- )

  if ammo
    doAttackOwnBoat
  else
    s" Por suerte no hay munición para disparar..." message
    3 pause
    s" Enseguida te das cuenta de que ibas a hundir uno de tus botes." message
    3 pause
    wipeMessage \ XXX needed?
  endif

  ;

: doAttackOwnBoat  ( -- )

  let ammo=ammo-1
  s" Disparas por error a uno de tus propios botes..." message
  seconds 5

  if fn between(0,2)
    s" Por suerte el disparo no ha dado en el blanco." message
  else
    \ XXX TODO inform about how many injured?
    s" La bala alcanza su objetivo. Esto desmoraliza a la tripulación." message
    let morale=morale-2
    2 3 between 1+ 1 ?do  manInjured  loop
  endif
  seconds 5
  wipeMessage

  ;

: shipBattle  ( -- )
  local done,k
  let done=false
  saveScreen
  battleScenery
  begin
    moveEnemyShip
    let k$=inkey$
    if instr("123",k$) then \
      on val k$
        fire 3
        fire 10
        fire 17
  done ammo 0= or until
  restoreScreen
  if not ammo then noAmmoLeft
  ;

: battleScenery  ( -- )

  window:paper blue:cls:0 charset:
  print at 21,10; pen white; paper red;" Munición = ";ammo

  black ink yellow paper
  22 0 do  0 i at-xy  ." ________ "  loop

  print at 2,0; pen black; paper white;"1";at 9,0;"2";at 16,0;"3"

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

: fire  ( y -- )

  local z

  let ammo=ammo-1
  0 charset
  print at 21,22; pen white; paper red;ammo
  1 charset
  print at y-1,9; pen yellow; paper blue;"+";at y+1,9;"-"
  moveEnemyShip
  print at y-1,9; pen yellow; paper blue;" ";at y+1,9;" "
  print at y,9; pen yellow; paper blue;" j"
  moveEnemyShip
  31 9 do
    \ XXX TODO -- `z` is the loop index:
    print at y,z; pen yellow; paper blue;" j"
    if m=y and z=n or m=y-1 and z=n or m=y-2 and z=n then sunk
    if m=y and z=n+1 or m=y-1 and z=n+1 or m=y-2 and z=n+1 then sunk
  loop
  print at y,30; paper blue;"  "

  ;

: noAmmoLeft  ( -- )
  \ XXX TODO the enemy wins; our ship sinks,
  \ or the money and part of the crew is captured
  s" Te quedaste sin munición." message
  seconds 4
  ;

: moveEnemyShip  ( -- )
  let \
    ship=fn between(1,5),\
    n=n+(ship=1 and n<28)-(ship=2 and n>18),\
    m=m+(ship=3 and m<17)-(ship=4 and m>1)
  print pen white; paper blue;\
    at m,n;" ab ";\
    at m+1,n;" 90 ";\
    at m+2,n-1;" 678 ";\
    at m-1,n;"   ";\
    at m+3,n;"   "
  if ship=5 then \
    drawWave
  ;

: drawWave  ( -- )
  print pen 5;\
    at fn between(1,20),fn between(11,30);"kl"
  ;

: sunk  ( -- )

  \ Sunk the enemy ship

  print pen white;paper blue;\
    at m,n;"   ";\
    at m+1,n;" ab";\
    at m+2,n;" 90";\
    at m,n;"   ";\
    at m+1,n;"   ";\
    at m+2,n;" ab";\
    at m,n;"   ";\
    at m+1,n;"   ";\
    at m+2,n;"   "
  seconds  2
  \ XXX TODO simpler and better
  \ XXX why this condition?:
  if seaMap(shipPos)>=13 and seaMap(shipPos)<=16 then \
    let \
      sunkShips=sunkShips+1,\
      score=score+1000,\
      done=true

  \ XXX --- original version:
  if seaMap(shipPos)=13:let seaMap(shipPos)=10
  else if seaMap(shipPos)=14:let seaMap(shipPos)=9
  else if seaMap(shipPos)=15:let seaMap(shipPos)=8
  else if seaMap(shipPos)=16:let seaMap(shipPos)=7
  endif
  \ XXX TODO deprecated, buggy alternative:
  \   on fn max(1,seaMap(shipPos)-12):\ \ 13~16
  \     let seaMap(shipPos)=10:\ \ 13
  \     let seaMap(shipPos)=9:\ \ 14
  \     let seaMap(shipPos)=8:\ \ 15
  \     let seaMap(shipPos)=7: \ 16

  ;

  \ }}} ---------------------------------------------------------
  \ Crew stamina {{{

: manInjured  ( -- )
  \ A man is injured
  \ Output: injured = his number
  begin
    let injured=fn between(1,men)
  stamina(injured) until
  let stamina(injured)=stamina(injured)-1,\
  alive=alive-not stamina(injured)
  ;

: manDead  ( -- )
  \ A man dies
  \ Output: dead = his number
  begin
    let dead=fn between(1,men)
  stamina(dead) until
  let \
    stamina(dead)=0,\
    alive=alive-1
  ;

  \ }}} ---------------------------------------------------------
  \ Attack {{{

: attack  ( -- )

  #!!!if islandMap(iPos)=2 or islandMap(iPos)=4 or islandMap(iPos)=6 then \
    #gosub @impossible
    #gosub @islandPanel
    #exit proc

  s" Atacas al nativo..." message \ XXX OLD
  100 pause

  \ XXX FIXME snake?!
  if islandMap(iPos)=5 then \
    manDead
    message \
      "Lo matas, pero la serpiente mata a "+\
      name$(dead)+"."
    goto L6897

  if islandMap(iPos)=9 then \
    manDead
    message \
      "Un poblado entero es un enemigo muy difícil."+\
      name$(dead)+" muere en el combate."
    goto L6898

  let kill=fn between(1,5)
  #let z=int (rnd*2)+2
  if kill=1
    manDead
    message \
      "El nativo muere, pero antes mata a "+name$(dead)+"."
  else if kill=2
    s" El nativo tiene provisiones escondidas en su taparrabos." message
    let supplies=supplies+1
  else if kill>=3
    let dub=fn between(2,3)
    message \
      "Encuentras "+fn coins$(dub)+\
      " en el cuerpo del nativo muerto."
    let cash=cash+dub
  endif

  2 charset  black ink  yellow paper yellow
  14 10 do  8 i at-xy ." t   "  loop
  print \
    at 9,8; paper yellow; pen black;"u";\
    at 10,9; paper black; pen white;"nop";at 11,9;"qrs"
  1 charset

  label L6897

  let islandMap(iPos)=4

  label L6898

  seconds 3

  ;

  \ }}} ---------------------------------------------------------
  \ Storm {{{

: storm  ( -- )

  \ XXX TODO make the enemy ship to move, if present
  \ (use the same graphic of the player ship)
  wipePanel
  stormySky
  damaged 10,49
  s" Se desata una tormenta que causa destrozos en el barco." message
  rain
  \ XXX TODO bright sky!
  print pen white; paper 5;\
    at 2,cloud0X;"    ";\
    at 2,cloud1X;"   "
  message "Tras la tormenta, el barco está "+fn damage$+"."
  panel

  ;

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
  print pen white; paper 5;\
    at 2,cloud0X;string$(4,c$);\
    at 2,cloud1X;string$(3,c$)
  3 pause \ XXX TODO use TICS instead?
  ;

  \ }}} ---------------------------------------------------------
  \ Sea graphics {{{

: seaScenery  ( -- )

  graphicWindow
  seaAndSky
  redrawShip

  \ XXX INFORMER
  \   0 charset
  \   print at 0,0;shipPos,seaMap(shipPos)
  \   1 charset

  seaPicture seaMap(shipPos)

  ;

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
  print at 13,18; pen white; paper blue;"\S"
  ;

  \ .............................................................
  \ Reefs

: drawReefs  ( -- )
  if seaMap(shipPos+15)=1 then drawFarIslands
  if seaMap(shipPos-15)=1 then bottomReef
  if seaMap(shipPos-1)=1 then leftReef
  if seaMap(shipPos+1)=1 then rightReef
  ;

: bottomReef  ( -- )
  \ XXX FIXME still "Off the screen" error!
  \ The reason is the window is changed
  print pen black; paper blue;\
    at 14,2;" A  HI   HI       HI  HI  A";\
    at 15,0;"WXY  :\::\::\#127     Z123     :\::\::\#127"
  ;

: leftReef  ( -- )
  print pen black;paper blue;\
    at 4,0;"A";\
    at 6,1;"HI";\
    at 8,0;"WXY";\
    at 11,1;"A";\
    at 13,0;"HI"
  ;

: rightReef  ( -- )
  print pen black;paper blue;\
    at 4,30;"HI";\
    at 6,28;"A";\
    at 7,29;"WXY";\
    at 9,31;"A"
  ;

  \ .............................................................
  \ Islands

: drawBigIsland5  ( -- )
  print  pen green; paper blue;\
    at 7,18;"HI A";\
    at 8,17;"G\::\::\::\::BC";\
    at 9,16;"F\::\::\::\::\::\::\::D";\
    at 10,14;"JK\::\::\::\::\::\::\::\::E";\
    at 11,13;"F\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawBigIsland4  ( -- )
  print pen green;paper blue;\
    at 7,16;"WXYA";\
    at 8,14;":\::\::\::\::\::\::C F\::\::D";\
    at 9,13;":\::\::\::\::\::\::\::\::B\::\::\::E";\
    at 10,12;"F\::\::\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawLittleIsland2  ( -- )
  print pen green; paper blue;\
    at 8,14;":\::\::C";\
    at 7,16;"A";\
    at 9,13;":\::\::\::\::D";\
    at 10,12;"F\::\::\::\::\::E"
  ;

: drawLittleIsland1  ( -- )
  print pen green;paper blue;\
    at 8,23;"JK\::C";\
    at 9,22;":\::\::\::\::D";\
    at 10,21;"F\::\::\::\::\::E"
  ;

: drawBigIsland3  ( -- )
  print pen green;paper blue;\
    at 7,21;"Z123";\
    at 8,19;":\::\::\::\::\::C";\
    at 9,18;":\::\::\::\::\::\::\::D";\
    at 10,15;"F\::B\::\::\::\::\::\::\::\::E";\
    at 11,13;"JK\::\::\::\::\::\::\::\::\::\::\::\::C"
  ;

: drawBigIsland2  ( -- )
  print pen green; paper blue;\
    at 7,17;"Z123";\
    at 8,14;"F\::B\::\::\::\::\::C";\
    at 9,13;"G\::\::\::\::\::\::\::\::\::D";\
    at 10,12;"F\::\::\::\::\::\::\::\::\::\::E;"
  ;

: drawBigIsland1  ( -- )
  print pen green;paper blue;\
    at 7,20;"HI A";\
    at 8,19;"G\::\::B\::\::\::C";\
    at 9,18;"F\::\::\::\::\::\::\::\::D";\
    at 10,16;"JK\::\::\::\::\::\::\::\::\::E"
  ;

: drawTwoLittleIslands  ( -- )
  print pen green;paper blue;\
  at 6,17;"WXY  A";\
  at 7,16;"A   A   F\::C";\
  at 8,15;":\::\#127 :\::\#127 G\::\::\::D";\
  at 9,14;"G\::\::\::D   F\::\::\::\::E";\
  at 10,13;"F\::\::\::\::E"
  ;

: drawFarIslands  ( -- )
  print pen green; paper 5;\
    at 2,0;"Z123 HI A Z123 HI A Z123 HI Z123"
  ;

: drawTreasureIsland  ( -- )

  1 charset
  print pen green;paper blue;\
    at 7,16;"A A   HI";\
    at 8,13;"F\::\::\::B\::\::\::B\::\::B\::\::\::C";\
    at 9,12;"G\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::D";\
    at 10,10;"JK\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::E"
    at 11,9;":\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::C";\
    at 12,8;"F\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::\::D"
  print pen blue;paper green;\
    at 13,8;" HI Z123  HI A  A A  A ";\
    at 14,20;"B\::\::\::\::B"
  print pen green;paper blue;\
    at 13,31;"E"
  palm1 4,19
  palm1 4,24
  palm1 4,14
  print at 9,22; pen black; paper green;"\T\U":\ \ the treasure
  if visited(shipPos) then \
    message "Llegas nuevamente a la isla de "+islandName$+"."
  else \
    message "Has encontrado la perdida isla de "+islandName$+"..."
  1 charset

  ;

: wipeIsland  ( -- )
  poke fn attrLine(3),fn attrLines$(5,6,6,0)
  ;

  \ .............................................................
  \ Ships

: redrawShip  ( -- )
  shipPicture @ if    drawShipDown shipPicture off
                else  drawShipUp   shipPicture on  then  ;

: drawShipUp  ( -- )
  print paper blue;pen white;\
    at shipY,shipX;"\A\B\C";\
    at shipY+1,shipX;"\D\E\F";\
    at shipY+2,shipX;"\G\H\I"
  ;

: drawShipDown  ( -- )
  print paper blue;pen white;\
    at shipY,shipX;"\J\K\L";\
    at shipY+1,shipX;"\M\N\O";\
    at shipY+2,shipX;"\P\Q\R"
  ;

: drawEnemyShip  ( -- )
  print pen yellow; paper blue;\
    at 4,11;" ab";\
    at 5,11;" 90";\
    at 6,11;"678"
  ;

: wipeEnemyShip  ( -- )
  print paper blue;\
    at 4,11;"   ";\
    at 5,11;"   ";\
    at 6,11;"   "
  ;

: drawBoat  ( -- )
  print at 7,11; pen yellow; paper blue;"<>"
  ;

  \ }}} ---------------------------------------------------------
  \ Reports {{{

: reportStart  ( -- )
  \ Common task at the start of all reports
  saveScreen
  cls
  window
  0 charset
  ;

: reportEnd  ( -- )
  \ Common task at the end of all reports
  1000 pause
  restoreScreen
  ;


: mainReport  ( -- )
  reportStart
  print \
    at 1,0;fn centered$("Informe de situación");\
    at 4,0;\
    "Días:",using$("##",day);''\
    "Barco:",fn upper1$(fn damage$)''\
    "Hombres:",using$("##",alive)'\
    "Moral:",using$("## ",morale)''\
    "Provisiones:",using$("##",supplies)'\
    "Doblones:",using$("##",cash)''\
    "Hundimientos:",using$("## ",sunkShips)'\
    "Munición:",using$("##",ammo)''
  reportEnd
  ;

: crewReport  ( -- )
  local nameCol,dataCol
  let nameCol=1,dataCol=20
  reportStart
  print \
    at 1,0;fn centered$("Informe de tripulación");\
    at 4,nameCol;"Nombre";\
    at 4,dataCol;"Condición"
  men 1+ 1 do
    \ XXX TODO -- `z` is the loop index:
    print \
      pen white;\
      at z+5,nameCol;name$(z);\
      pen staminaPen(stamina(z)+1);\
      paper staminaPap(stamina(z)+1);\
      bright staminaBri(stamina(z)+1);\
      at z+5,dataCol;fn upper1$(stamina$(stamina(z)+1))
  loop
  reportEnd
  ;

: scoreReport  ( -- )
  reportStart
  print \
    at 1,0;fn centered$("Informe de puntuación");\
    at 4,0;\
    "Días",using$("####",day);" x  200"'\
    "Hundimientos",using$("####",sunkShips);" x 1000"'\
    "Negocios",using$("####",trades);" x  200"'\
    "Pistas",using$("####",foundClues);" x 1000"
  if foundClues=6 then \
    let score=score+4000
    print "Tesoro",using$("####",4000)
  let score=score+(foundClues*1000)+(day*200)+(sunkShips*1000)+(trades*200)
  print '"Total","       ";using$("####",score)
  reportEnd
  ;

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

  print pen white;\
    at 8,14;"\A\B\C";\
    at 9,14;"\D\E\F";\
    at 10,14;"\G\H\I"
  print pen black; paper blue;\
    at 10,17;"WXY     A";\
    at 6,19;"A   Z123";\
    at 11,6;"A   HI";\
    at 4,5;"Z123    HI";\
    at 8,7;"H\..I  A"

  damaged 10,29
  \ XXX TODO improved message: "Por suerte, ..."
  message "¡Has encallado! El barco está "+damage$+"."
  \ XXX TODO print at the proper zone:
  if damage=100 then print at 20,7; pen 5; paper black;"TOTAL"
  print pen black; paper green;at 17,0;fn centered$("INFORME")
  \ XXX TODO choose more men, and inform about them
  manInjured
  manDead
  let morale=morale-fn between(1,4)
  seconds 3

  ;

: damaged  ( min,max -- )
  \ Increase the ship damage with random value in a range
  let damage=damage+fn between(min,max)
  if damage>100 then let damage=100
  ;


  \ }}} ---------------------------------------------------------
  \ Landscape graphics {{{

: stormySky  ( -- )
  load "attr/zp5i5b0l03" code fn attrLine(0)
  let noStorm=0
  sunAndClouds false
  ;

: seaWaves  ( -- )
  local z
  1 charset
  16 1 do
    print paper blue;pen cyan;\
      at fn between(4,graphicWinBottom),fn between(1,28);"kl";\
      at fn between(4,graphicWinBottom),fn between(1,28);"mn"
  loop  ;

: seaAndSky  ( -- )
  graphicWindow:\ \ XXX TMP needed, because the wipePanel before the calling
  wipeSea
  seaWaves
  sunnySky
  ;

: sunnySky  ( -- )
  load "attr/zp5i5b1l03" code fn attrLine(0)
  \ XXX OLD -- where is it?:
  #finnishTheSky 1
  ;

: sunAndClouds  ( sunny -- )

  default sunny=true
  2 charset
  print paper 5;bright sunny;\
    at 0,26; pen yellow; "AB";\
    at 1,26;"CD"
  let \
    cloud0X=fn between(1,9),\
    cloud1X=fn between(13,21)
  print pen white; paper 5;bright sunny;\
    at 0,cloud0X;"EFGH";\
    at 1,cloud0X;"IJKL";\
    at 0,cloud1X;"MNO";\
    at 1,cloud1X;"PQR"
  1 charset

  ;

: wipeSea  ( -- )
  load "attr/zp1i1b0l13" code fn attrLine(3)
  ;

  \ }}} ---------------------------------------------------------
  \ Setup {{{

: initOnce  ( -- )  initScreen  initUDG  ;

variable aboard      \ flag
variable alive       \ counter
variable ammo        \ counter
variable cash        \ counter
variable damage      \ counter
variable day         \ counter
variable foundClues  \ counter
variable morale      \ counter
variable score       \ counter
variable sunkShips   \ counter
variable supplies    \ counter
variable trades      \ counter
variable quitGame    \ flag

variable shipPicture \ flag

: init  ( -- )

  local i,i$

  randomize
  #load "attr/zp0i0b0l20" code fn attrLine(2)
  print pen white; paper black; flash 1;\
    at peek UWBOT-introWinTop-1,0;fn banner$("Preparando el viaje...")

  \ The sea map has 135 cells (9 rows, 15 columns)
  let locations=135
  dim seaMap(locations)
  dim visited(locations) \ flags for islands


  \ Reefs around the sea map
  \ XXX TODO -- `i` is the loop index:
  17 1 do  let seaMap(i)=reef  loop  \ north
  locations 1+ 120 do  let seaMap(i)=reef  loop  \ south
  106 30 do step 15 let seaMap(i)=reef 15 +loop  \ east
  107 32 do  let seaMap(i)=reef 15 +loop \ west

  \ Normal islands
  120 17 do
    \ XXX TODO -- `i` is the loop index:
    if seaMap(i)<>reef then \
      let seaMap(i)=fn between (2,21)  \ random type
      \ XXX 21 is shark; these are picture types
  loop

  \ Treasure island
  let \
    treasureIsland=22
    seaMap(fn between(94,104))=treasureIsland

  \ Ship position
  let shipPos=fn between (32,42)

  \ Ship coordinates
  let shipY=9,shipX=4

  \ Panel lines
  let panelTop=17,panelBottom=21

  initCrew

  \ Ship damage labels
  let \
    damageLevels=0
    damageMaxLen=0
  restore damageData
  do
    read i$
    let i=len i$
    exit if not i
    let \
      damageLevels=damageLevels+1
      damageMaxLen=fn max(damageMaxLen,i)
  loop
  dim damage$(damageLevels,damageMaxLen)
  restore damageData
  damageLevels +1 do
    \ XXX TODO -- `i` is the loop index:
    read damage$(i)
  loop

  \ Island map
  dim islandMap(30)
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
  let path=fn between(1,3)
  let tree=fn between(1,3)
  let village=fn between(1,10)
  let turn=fn between(1,2)
  let direction=fn between(1,4)
  let pace=fn between(1,9)

  \ Villages
  restore villageNamesData
  dim village$(10,9)
  11 1 do
    \ XXX TODO -- `i` is the loop index:
    read village$(i)
  loop

  \ Cardinal points
  dim cardinal$(4,5)
  let \
    cardinal$(1)="norte",\
    cardinal$(2)="sur",\
    cardinal$(3)="este",\
    cardinal$(4)="oeste"

  \ Left and right
  dim hand$(2,9)
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
    nameMaxLen=0
  restore menNamesData
  do
    read i$
    let i=len i$
    exit if not i
    let \
      names=names+1,\
      nameMaxLen=fn max(nameMaxLen,i)
  loop
  dim names$(names,nameMaxLen)
  restore menNamesData
  names 1+ 1 do
    \ XXX TODO -- `i` is the loop index:
    read names$(i)
  loop
  dim name$(men,nameMaxLen)
  men 1+ 1 do
    \ XXX TODO -- `man` is the loop index:
    begin
      let \
        name=fn between(1,names),\
        i$=names$(name)
    len trunc$ i$ until
    let \
      name$(man)=i$,\
      names$(name)=""
  loop

  ;

: initCrewStamina  ( -- )

  local i

  \ XXX TODO stamina levels = array indexes
  let minStamina=0
  let maxStamina=4
  dim stamina(men)
  men 1+ 1 do
    \ XXX TODO -- `i` is the loop index:
    let stamina(i)=maxStamina
  loop

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
  \ Village names

  \ (They are Esperanto compound words with funny sounds and meanings)

label villageNamesData
  \ XXX TODO translate
data \
  "Mislongo",\ \ mis-long-o = "wrong lenght"
  "Ombreto",\ \ ombr-et-o = "little shadow"
  "Figokesto",\ \ fig-o-kest-o
  "Misedukota",\ \ mis-eduk-ot-a = "the one that will be wrongly educated"
  "Topikega",\ \ topik-eg-a =
  "Fibaloto",\ \ fi-balot-o
  "Pomotruko",\ \ pom-o-truk-o
  "Putotombo",\ \ put-o-tomb-o
  "Ursorelo",\ \ urs-orel-o = "ear of bear"
  "Kukumemo" \ kukum-em-o

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
  \ Ship damage descriptions

label damageData
data "impecable" \ best: perfect
data "casi como nuevo"
data "muy poco dañado"
data "algo dañado"
data "muy dañado"
data "gravemente dañado"
data "casi destrozado"
data "destrozado"
data "haciendo agua"
data "a punto de hundirse"
data "hundiéndose" \ worst: sinking
data "" \ end of data

  \ .............................
  \ Crewmen names

  \ (They are pun funny names in Spanish)

label menNamesData
data "Alfredo Minguero"
data "Armando Bronca"
data "Borja Monserrano"
data "Clemente Cato"
data "César Pullido"
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
data "Néstor Nillo"
data "Néstor Tilla"
data "Paco Tilla"
data "Pascual Baricoque"
data "Pedro Medario"
data "Policarpio Nero"
data "Ramiro Inoveo"
data "Ricardo Borriquero"
data "Roberto Mate"
data "Rodrigo Minolas"
data "Ulises Cocido"
data "Ulises Tantería"
data "Vicente Rador"
data "Víctor Nillo"
data "Víctor Tilla"
data "Zacarías Queroso"
data "Óscar Romato"
data "Óscar Terista"
data ""

  \ .............................
  \ Islands

label islandData
data 1,2,3,4,5,6,7,12,13,18,19,24,25,26,27,28,29,30

  \ }}} ---------------------------------------------------------
  \ Island map {{{

: newIslandMap  ( -- )

  local w,z

  \ XXX TMP erase the map -- do better
  31 1 do
    \ XXX TODO -- `z` is the loop index:
    let islandMap(z)=0
  loop

  restore islandData
  19 1 do
    read w:let islandMap(w)=coast
  loop

  24 8 do
    \ XXX TODO -- `z` is the loop index:
    if islandMap(z)<>coast then let islandMap(z)=fn between(2,5)
  loop

  let \
    islandMap(fn between(20,23))=nativeVillage,\
    islandMap(fn between(14,17))=nativeAmmo,\
    islandMap(fn between(8,11))=nativeSupplies,\
    iPos=fn between(8,11) \ player position on the island

  ;

  \ }}}----------------------------------------------------------
  \ On the treasure island {{{

: enterTreasureIsland  ( -- )

  \ XXX TODO finish the new interface

  cls
  sunnySky
  wipeIsland
  2 charset
  print pen green; paper yellow;\
    at 3,0; " 5     6       45     6       5"
  25 0 do
    \ XXX TODO -- `z` is the loop index:
    print pen black; paper yellow;\
      at 3,z+3;":\#127";\
      at 4,z+2;":\::\::\#127";\
      at 5,z+1;":\::\::\::\::\#127";\
      at 6,z;":\::\::\::\::\::\::\#127"
  8 +loop
  0 charset
  print at 7,0; pen white; paper red;"   1       2       3       4    "

  \ XXX TODO improve with LOAD or POKE
  22 8 do
    \ XXX TODO -- `z` is the loop index:
    print at z,0; pen white; paper black;fn blankLine$
  loop

  sailorAndCaptain

  sailorSays "¿Qué camino, capitán?"
  print at 15,23;"?" \ XXX TODO better, in all cases
  digitTo option
  print at 15,23; paper black;option: beep .2,30
  seconds  2
  if option=path then let foundClues=foundClues+1

  sailorSays "¿Qué árbol, capitán?"
  print at 15,23;"? "
  digitTo option: 0 charset: print at 15,23; paper black;option: beep .2,30
  trees
  seconds  2:
  if option=tree then let foundClues=foundClues+1

  \ XXX TODO better, with letters
  print at 14,7; paper black;"Izquierda Derecha";at 16,8;"I=1  D=2 ";at 15,23;"? "
  digitTo option
  0 charset
  print at 15,23; paper black;option: beep .2,30
  seconds  2
  if option=turn then let foundClues=foundClues+1

  wipeIsland
  8 3 do
    \ XXX TODO -- `z` is the loop index:
    print paper yellow;pen black;\
      at z,1;z-2;"  ";village$(z-2);\
      at z,12;z+3;"  ";village$(z+3)
  loop
  print at 7,12; pen black; paper yellow;"0  ";village$(10)
  2 charset
  print at 5,27; pen green; paper yellow;"S\::T";at 6,27;"VUW"

  0 charset
  print at 14,7; paper black;" Poblado  ";at 13,7;"¿Cuál";at 16,8;" capitán.";at 15,23;"? "
  digitTo option
  print at 15,23; paper black;option: beep .2,30
  seconds  2
  if option=village then let foundClues=foundClues+1

  \ XXX TODO better, with letters
  print at 13,7; paper black;"¿Qué camino";at 14,7;"capitán?";at 16,7;"1N 2S 3E 4O";at 15,23;"? "
  digitTo option: print at 15,23; paper black;option: beep .2,30
  seconds  2: if option=direction then let foundClues=foundClues+1

  print at 13,7; paper black;"¿Cuántos";at 14,7;"pasos,";at 16,7;"capitán?";at 15,23;"? "
  digitTo option: print at 15,23; paper black;option: beep .2,30
  seconds  2: if option=pace then let foundClues=foundClues+1

  \ XXX TODO use tellZone
  if foundClues=6 then \
    print paper black;\
      at 13,7;"¡Hemos encontrado";\
      at 14,7;"el oro,";\
      at 16,7;"capitán!"
      treasureFound
  else \
    print paper black;\
      at 13,7;"¡Nos hemos";\
      at 14,7;" equivocado ";\
      at 16,7;"capitán!"
  seconds  2
  1 charset

  ;

: sailorAndCaptain  ( -- )
  1 charset
  print pen cyan; paper black;\
    at 17,0;\
    " xy";tab 28;"pq"'\
    " vs";tab 28;"rs"'\
    " wu";tab 28;"tu"
  sailorSpeechBalloon
  captainSpeechBalloon
  ;

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
  print pen black;paper yellow;\
    at 7,0;" 1       2       3       4"
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

  paper yellow
  pen black
  cls 1
  \ XXX OLD
  \   for z=0 to 21
  \     print at z,0; paper yellow; pen black;fn blankLine$: beep .001,z+9
  \   next z

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
  print pen white; paper red;at 3,0;fn centered$("FIN DEL JUEGO")
  window 5,26,2,21 \ XXX TODO
  if supplies<=0 then \
  s" Las provisiones se han agotado." tell
  if morale<=0 then \
  s" La tripulación se ha amotinado." tell
  if ammo<=0 then \
  s" La munición se ha terminado." tell
  if not alive then \
  s" Toda tu tripulación ha muerto." tell
  if damage=100 then \
  s" El barco está muy dañado y es imposible repararlo." tell
  if cash<=0 then \
  s" No te queda dinero." tell
  window

  ;

: treasureFound  ( -- )

  \ XXX TODO use this proc instead of happyEnd?

  local z

  seconds 2
  load "attr/zp5i5b1l03" code fn attrLine(0)
  load "attr/zp6i6b0l18" code fn attrLine(4)
  sunnySky
  23 7 do
    \ XXX TODO -- `z` is the loop index:
    palm2 5,z
  5 +loop
  palm2 7,3:palm2 7,26
  \ Cofre del tesoro:
  print at 13,8; pen black; paper yellow;\
  "pq          xy                  rs          vs                  tu      ";\
  "\T\U    wu"
  palm2 11,28:palm2 11,0
  2 charset
  print at 17,13; pen blue; paper yellow;"l\::m"
  s" ¡Capitán, somos ricos!" message
  seconds 4
  1 charset

  ;

: happyEnd  ( -- )
  s" Lo lograste, capitán." message
  ;

  \ }}} ---------------------------------------------------------
  \ Intro {{{

: intro  ( -- )
  window
  cls
  skullBorder
  introWindow
  s" Viejas leyendas hablan del tesoro que esconde la perdida isla de "
  islandName$ s+ s" ." s+ tellCR
  s" Los nativos del archipiélago recuerdan las antiguas pistas" tellCR
  s" que conducen al tesoro." tell
  s" Deberás comerciar con ellos para que te las digan." tell
  s" Visita todas las islas hasta encontrar la isla de" tellCR
  islandName$ tell
  s" y sigue las pistas hasta el tesoro..." tell
  print at peek UWBOT-introWinTop-1,0;fn centered$("Pulsa una tecla")
  6000 pause  ;

: skullBorder  ( -- )

  \ Draw top and bottom borders of skulls.

  2 charset
  skulls 2
  skulls 0
  1 charset

  ;

: skulls  ( channel -- )

  \ Draw a row of skulls at the given row.

  print #channel;paper black;pen white;bright 1;\
    at 0,0;\
    "  nop  nop  nop  nop  nop  nop  "'\
    "  qrs  qrs  qrs  qrs  qrs  qrs  "

  ;

  \ }}} ---------------------------------------------------------
  \ Text output {{{

: cpl  ( -- n )  32  ;
  \ Characters per line of the current upper window
  \ XXX TODO --

: tell  ( ca len -- )
  0 charset
  begin  dup cpl >  while
    \ for char=cpl to 1 step -1 \ XXX OLD
    0 cpl do
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
  cls 1
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
        print at row,col;text$(to char-1)
        let text$=text$(char+1 to)
        let row=row+1
        exit for
    -1 +loop
  repeat
  print at row,col;text$

  ;

  \ }}} ---------------------------------------------------------
  \ Screen {{{

: initScreen  ( -- )

  let attrAd=scrad+6144
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
  window 0,31,0,20
  ;

: graphicWindow  ( -- )
  \ Zone where graphics are shown
  window graphicWinLeft,graphicWinRight,graphicWinTop,graphicWinBottom
  1 charset \ default
  ;

: introWindow  ( -- )
  \ Zone where intro text is shown
  window 2,29,introWinTop,21
  ;

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

  \ XXX OLD
  #load "attr/zp0i0b0l06" code fn attrLine(panelTop-1)
  messageWindow
  paper black:pen white:cls 1

  ;

: saveScreen  ( -- )
  copy screen 1 to 2
  ;

: restoreScreen  ( -- )
  copy screen 2 to 1
  let screenRestored=true
  ;

: useScreen2  ( -- )
  saveScreen
  screen 2
  ;

: useScreen1  ( -- )
  restoreScreen
  screen 1
  ;

  \ }}} ---------------------------------------------------------
  \ Meta {{{

: showSea  ( -- )
  local x,y,invflag
  cls
  \ for y=0 to 8*2 step 2
  17 0 do
    \ XXX TODO -- `y` is the outer loop index:
    \ for x=0 to 14
    15 0 do
      \ XXX TODO -- `x` is the inner loop index:
      invflag inverse
      print using$("##",seaMap(1+y*9+x));
      let invflag=not invflag
    loop
    cr
  loop
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
    print damage,fn damageIndex;" ";damage$
  loop  ;

  \ vim: set filetype:soloforth
