#! /usr/bin/env gforth

\ twisty_font_es.fs

\ This file is part of Bandera Negra
\ http://programandala.net/

\ Last modified: 201702020151

\ Description:
\
\ Spanish characters designed for the twisity font.

\ Credit:
\
\ The twisty font was designed by Paul Howard for Alchemist PD,
\ 1995, and packed into a viewer called "Fontbox I".

\ ==============================================================

.( \ Twisty font Spanish characters UDG set) cr

include ../make/udg_stack_to_forth_c-comma.fs

2 base !

\ ¡
00000000
00001000
00000000
00001000
00001000
00001000
00001000
00010000
udg

\ ¿
00000000
00010000
00000000
00010000
00100000
01000000
00111100
00000010
udg

\ Á
00000100
00001000
00011100
00100010
01000010
01111110
01000010
10000000
udg

\ É
00000100
00001001
01111110
01000000
01111100
01000000
01111110
00000001
udg

\ Í
00000100
00001001
00111110
00001000
00001000
01001000
00111110
00000000
udg

\ Ñ
00011000
01000010
00100010
01010010
01001010
01000110
01000010
00100000
udg

\ Ó
00000100
00001000
00011100
00100010
01000010
01000010
00111100
00000000
udg

\ Ú
00000100
00101001
01000010
01000010
01000010
01000010
00111100
00000000
udg

\ Ü
00100010
00000000
01000010
01000010
01000010
01000010
00111100
00000000
udg

\ á
00001000
01010000
00111000
00000100
00111100
01000100
00111100
00000010
udg

\ é
00001000
01010000
00111000
01000100
01111000
01000000
00111100
00000010
udg

\ í
00001000
00010000
00000000
00110000
01010000
00010100
00111000
00000000
udg

\ ñ
00011000
10000000
01111000
01000100
01000100
01000100
01000100
10000010
udg

\ ó
00001000
00010000
00011000
00100100
01000100
01000100
00111000
00000000
udg

\ ú
00001000
00010000
01000100
01000100
01000100
01000100
00111010
00000000
udg

\ ü
00100100
00000000
01000100
01000100
01000100
01000100
00111010
00000000
udg

bye
