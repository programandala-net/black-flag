#! /usr/bin/env gforth

\ twisty_font.fs

\ This file is part of Black Flag
\ http://programandala.net/en.program.black_flag.html

\ Last modified: 201704180050

\ ==============================================================
\ Description

\ Twisty font, used for the ordinary texts.

\ ==============================================================
\ Credit

\ The twisty font was designed by Paul Howard for Alchemist PD,
\ 1995, and packed into a viewer called "Fontbox I".

\ ==============================================================
\ History

\ 2017-01-08: Create this file from the original TAP using a
\ FantomoUDG converter
\ (http://programandala.net/en.program.fantomoudg.html).
\
\ 2017-02-02: Make the file executable for Gforth and prepare it
\ to convert the UDG data to bytes stored with `c,`, which is
\ the compact format used in the final source. Redesign
\ character 96 (the GBP sign in the ZX Spectrum character set)
\ as a backtick, its original shape in ASCII.

\ ==============================================================

.( \ Twisty font ) cr

include ../make/udg_stack_to_forth_c-comma.fs

2 base !

\ character 32 (' ')
00000000
00000000
00000000
00000000
00000000
00000000
00000000
00000000
udg

\ character 33 ('!')
00001000
00010000
00010000
00010000
00010000
00000000
00010000
00000000
udg

\ character 34 ('"')
00000000
00100100
00100100
01000010
00000000
00000000
00000000
00000000
udg

\ character 35 ('#')
00000000
01010010
00111110
00100100
00100100
01111110
00100101
00000000
udg

\ character 36 ('$')
00000000
00000101
00011110
00101000
00011100
01001010
00111100
00001000
udg

\ character 37 ('%')
00000000
00110010
01100101
00001000
00010000
00100110
00100110
00000000
udg

\ character 38 ('&')
00000000
00001100
00010100
00011000
00101010
01000100
00111010
00000001
udg

\ character 39 (''')
00000000
00000100
00011000
00000000
00000000
00000000
00000000
00000000
udg

\ character 40 ('(')
00000000
00000010
00000100
00001000
00001000
00001000
00000100
00000000
udg

\ character 41 (')')
00000000
00010000
00010000
00010000
00010000
00010000
00100000
00000000
udg

\ character 42 ('*')
00000000
00010000
00001010
00001100
00011110
00101100
00001010
00000000
udg

\ character 43 ('+')
00000000
00000000
00000100
00001000
00111110
01001000
00001000
00000000
udg

\ character 44 (',')
00000000
00000000
00000000
00000000
00000000
00001100
00001000
00010000
udg

\ character 45 ('-')
00000000
00000000
00000000
00100000
00011110
00000000
00000000
00000000
udg

\ character 46 ('.')
00000000
00000000
00000000
00000000
00000000
00001100
00011000
00000000
udg

\ character 47 ('/')
00000000
00000000
00000010
00000100
00001000
00110000
01000000
00000000
udg

\ character 48 ('0')
00000000
00011110
00100110
01001010
01010010
01100010
00011100
00000000
udg

\ character 49 ('1')
00100000
00011000
00001000
00001000
00001000
00001000
00111110
00000000
udg

\ character 50 ('2')
01100000
00011100
00000010
00000010
00111100
01000000
01111110
00000000
udg

\ character 51 ('3')
00100000
00011100
00000010
00001100
00100010
01000010
00111100
00000000
udg

\ character 52 ('4')
00000000
00001100
00010100
00100100
01001000
01111110
00001000
00000000
udg

\ character 53 ('5')
00000001
00011110
00100000
01111100
00000010
01000010
00111100
00000000
udg

\ character 54 ('6')
00000010
00111100
01000000
01111100
01000010
01000010
00111100
00000000
udg

\ character 55 ('7')
00000000
00111110
01000010
00100100
00001000
00010000
00010000
00000000
udg

\ character 56 ('8')
00000000
00001100
01010010
00111100
01000010
01000010
00111100
00000000
udg

\ character 57 ('9')
00000000
00011100
00100010
01000010
00111110
00000010
00111100
01000000
udg

\ character 58 (':')
00000000
00000000
00000000
00011000
00000000
00000000
00011000
00000000
udg

\ character 59 (';')
00000000
00000000
00011000
00000000
00000000
00011000
00011000
00110000
udg

\ character 60 ('<')
00000000
00000100
00001000
00001000
00010000
00001000
00000100
00000000
udg

\ character 61 ('=')
00000000
00000000
00100000
00011110
00000000
00111100
00000010
00000000
udg

\ character 62 ('>')
00000000
00000100
00001000
00001000
00000100
00001000
00010000
00000000
udg

\ character 63 ('?')
01000000
00111100
00000010
00000100
00001000
00000000
00001000
00000000
udg

\ character 64 ('@')
00000000
00111100
01001010
01010110
01001100
01000000
00111100
00000010
udg

\ character 65 ('A')
00000000
00011100
00100010
01000010
01111110
01000010
01000010
10000000
udg

\ character 66 ('B')
00000000
01111100
01000010
01111100
01000010
01000100
01111000
10000000
udg

\ character 67 ('C')
00000000
00111100
01000010
01000100
01000000
01000000
00111100
00000010
udg

\ character 68 ('D')
00000000
00011000
00100100
00100010
01000010
01000100
01111000
10000000
udg

\ character 69 ('E')
00000001
01111110
01000000
01111100
01000010
01000000
01111110
00000001
udg

\ character 70 ('F')
00000000
01111110
01000001
01111100
01000010
01000000
01000000
00100000
udg

\ character 71 ('G')
00000010
00111100
01000000
01010000
01001110
01000010
00111100
00000000
udg

\ character 72 ('H')
00000100
01001010
01000010
01111110
01000010
01000010
01000010
00100000
udg

\ character 73 ('I')
00000001
00111110
00001000
00001000
00001000
01001000
00111110
00000000
udg

\ character 74 ('J')
00000000
00000110
00000010
00100010
01010010
01000010
00111100
00000000
udg

\ character 75 ('K')
00100000
01000000
01001100
01110010
01001000
01000100
01000010
00001100
udg

\ character 76 ('L')
00100000
01000000
01000000
01000000
01000000
01000000
01111110
00000001
udg

\ character 77 ('M')
00000000
01000010
01110110
01001010
01000010
01000010
01010010
00100001
udg

\ character 78 ('N')
00000100
00101010
00100010
01010010
01001010
01000110
01000010
00100000
udg

\ character 79 ('O')
00000000
00011100
00100010
00100010
01000010
01000010
00111100
00000000
udg

\ character 80 ('P')
00000000
00011100
00100010
01000010
01111100
01000000
01000000
10000000
udg

\ character 81 ('Q')
00000000
00011100
00100010
01000010
01010010
01001010
00111100
00000011
udg

\ character 82 ('R')
00000000
00011100
00100010
01000010
01111100
01000100
01000010
10000001
udg

\ character 83 ('S')
00000010
00111100
01000000
00111100
00000010
00000010
00111100
01000000
udg

\ character 84 ('T')
00000000
11111110
00010001
00010000
00010000
00010000
00010000
00001000
udg

\ character 85 ('U')
00100001
01010010
01000010
01000010
01000010
01000010
00111100
00000000
udg

\ character 86 ('V')
10000000
01000010
01000010
01000010
01000010
00100100
00011000
00000000
udg

\ character 87 ('W')
00010000
00100001
00100010
01000010
01000010
01011010
00100100
00000000
udg

\ character 88 ('X')
00100000
01000010
00100101
00011000
00011000
10100100
01000010
00000100
udg

\ character 89 ('Y')
00000100
10000010
01000100
00101000
00010000
00010000
00010000
00001000
udg

\ character 90 ('Z')
01000000
00111110
00000100
00001000
00010000
00100001
01111110
00000000
udg

\ character 91 ('[')
00000000
00001110
00001000
00001000
00010000
00010000
00011000
00000100
udg

\ character 92 ('\')
00000000
00100000
00100000
00100000
00010000
00001000
00000100
00000000
udg

\ character 93 (']')
00100000
00011000
00001000
00001000
00010000
00010000
01110000
00000000
udg

\ character 94 ('^')
00100000
00010000
00111000
01010100
00010000
00010000
00010000
00001000
udg

\ character 95 ('_')
00000000
00000000
00000000
00000000
00000000
00000000
10000000
01111111
udg

\ character 96 ('`')
00000000
00100000
00011000
00000000
00000000
00000000
00000000
00000000
udg

\ character 97 ('a')
00000000
01000000
00111000
00000100
00111100
01000100
00111100
00000010
udg

\ character 98 ('b')
01000000
00100000
00100000
00111000
00100100
00100010
00111100
00000000
udg

\ character 99 ('c')
00000000
00000010
00011100
00100000
00100000
00100010
00011101
00000000
udg

\ character 100 ('d')
00001000
00000100
00000100
00111100
01000100
00100100
00011100
00000010
udg

\ character 101 ('e')
00000000
01000000
00111000
01000100
01111000
01000000
00111100
00000010
udg

\ character 102 ('f')
00000000
00001100
00010000
00011000
00010100
00010000
00010000
00100000
udg

\ character 103 ('g')
00000000
00000010
00011100
00100100
01000100
00111100
10000100
01111000
udg

\ character 104 ('h')
00100000
01000000
01000000
01111000
01000100
01000100
01000100
00000010
udg

\ character 105 ('i')
00000000
00010000
00000000
00110000
01010000
00010100
00111000
00000000
udg

\ character 106 ('j')
00000000
00000100
00000000
00001000
00000100
00010100
00100100
00011000
udg

\ character 107 ('k')
01000000
00100000
00101100
00110000
00110000
00101010
00100100
00000000
udg

\ character 108 ('l')
00100000
00010000
00010000
00010000
00010000
00010000
00001100
00000010
udg

\ character 109 ('m')
00000000
10000000
01101000
01010100
01010100
01010100
01010100
00000010
udg

\ character 110 ('n')
00000000
10000000
01111000
01000100
01000100
01000100
01000100
10000010
udg

\ character 111 ('o')
00000000
00000100
00011000
00100100
01000100
01000100
00111000
00000000
udg

\ character 112 ('p')
00000000
10000000
01110000
01001000
01000100
01111000
01000000
00100000
udg

\ character 113 ('q')
00000000
00000010
00011100
00100100
01000100
00111100
00000101
00000110
udg

\ character 114 ('r')
00000000
00000010
01011100
00100000
00100000
00100000
00100000
00000000
udg

\ character 115 ('s')
00000000
00000100
00111000
01000000
00111000
00000100
01111000
10000000
udg

\ character 116 ('t')
00000000
00010000
00111000
01010000
00010000
00010000
00001100
00000010
udg

\ character 117 ('u')
00000000
00100000
01000100
01000100
01000100
01000100
00111010
00000000
udg

\ character 118 ('v')
00000000
00000010
01000100
01000100
00101000
00101000
00010000
00000000
udg

\ character 119 ('w')
00000000
10000010
01000101
01010100
01010100
01010100
00101000
00000000
udg

\ character 120 ('x')
00000000
01000000
00100100
00101000
00010000
00101010
01000100
00000000
udg

\ character 121 ('y')
00000000
10000010
01000100
01000100
01000100
00111100
10000100
01111000
udg

\ character 122 ('z')
00000000
10000000
01111100
00001000
00110000
01000010
01111100
00000000
udg

\ character 123 ('{')
00000000
00001100
00001010
00010000
00101000
00001000
00001110
00000000
udg

\ character 124 ('|')
00000000
00000100
00000100
00000100
00001000
00001000
00001000
00000000
udg

\ character 125 ('}')
00000000
01110000
00010100
00001000
00010000
01010000
00110000
00000000
udg

\ character 126 ('~')
00000000
00110010
01001100
00000000
00000000
00000000
00000000
00000000
udg

\ character 127
00011100
00100010
01011001
10100101
10100001
10011010
01000010
00111100
udg

bye
