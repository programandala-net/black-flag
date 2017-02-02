\ jolly_roger_graph_font_2_sprites.fs

\ This file is part of Bandera Negra
\ http://programandala.net/

\ XXX UNDER DEVELOPMENT

\ Last modified: 201702021217

\ ==============================================================
\ Description

\ The second graphic font in the game.

\ ==============================================================
\ Credit

\ This is the second graphic font included in the original game
\ "Jolly Roger", written by Barry Jones and published by Video
\ Vault ltd. in 1984.

\ ==============================================================
\ History

\ 2017-01-08: Create this file from the original TAP using a
\ FantomoUDG converter
\ (http://programandala.net/en.program.fantomoudg.html).  Edit
\ it to start combining the original graphic characters into
\ blocks and sprites.  The final goal is to create a single
\ source of graphics, easy to edit as text and to convert
\ automatically to Forth source.
\
\ 2017-01: Combine UDG into blocks and describe them.
\
\ 2017-02-02: Update file header.

\ ==============================================================

\ character 32 (' ')
00000000
00000000
00000000
00000000
00000000
00000000
00000000
00000000

\ character 33 ('!')
\ XXX REMARK -- not used
00000000
00010000
00010000
00010000
00010000
00000000
00010000
00000000

\ character 34 ('"')
\ XXX REMARK -- not used
00000000
00100100
00100100
00000000
00000000
00000000
00000000
00000000

\ character 35 ('#')
\ XXX REMARK -- not used
00000000
00100100
01111110
00100100
00100100
01111110
00100100
00000000

\ character 36 ('$')
\ XXX REMARK -- not used
00000000
00001000
00111110
00101000
00111110
00001010
00111110
00001000

\ character 37 ('%')
\ XXX REMARK -- not used
00000000
01100010
01100100
00001000
00010000
00100110
01000110
00000000

\ character 38 ('&')
\ XXX REMARK -- not used
00000000
00010000
00101000
00010000
00101010
01000100
00111010
00000000

\ character 39 (''')
\ XXX REMARK -- not used
00000000
00001000
00010000
00000000
00000000
00000000
00000000
00000000

\ character 40 ('(')
\ XXX REMARK -- not used
00000000
00000100
00001000
00001000
00001000
00001000
00000100
00000000

\ character 41 (')')
\ XXX REMARK -- not used
00000000
00100000
00010000
00010000
00010000
00010000
00100000
00000000

\ character 42 ('*')
\ XXX REMARK -- not used
00000000
00000000
00010100
00001000
00111110
00001000
00010100
00000000

\ character 43 ('+')
\ XXX REMARK -- not used
00000000
00000000
00001000
00001000
00111110
00001000
00001000
00000000

\ character 44 (',')
\ XXX REMARK -- not used
00000000
00000000
00000000
00000000
00000000
00001000
00001000
00010000

\ character 45 ('-')
\ XXX REMARK -- not used
00000000
00000000
00000000
00000000
00111110
00000000
00000000
00000000

\ character 46 ('.')
\ XXX REMARK -- not used
00000000
00000000
00000000
00000000
00000000
00011000
00011000
00000000

\ character 47 ('/')
\ XXX REMARK -- not used
00000000
00000000
00000010
00000100
00001000
00010000
00100000
00000000

\ character 48 ('0')
00000000
00111111
01111101
11111001
10001001
10001010
10001100
11111000

\ character 49 ('1')
00000000
00000000
00000000
01110000
11111000
11011100
11111100
01111000

\ character 50 ('2')
00110000
01111000
11111000
11111111
11111111
01111000
01111000
01111000

\ character 51 ('3')
01111100
01101100
01101110
01100110
01100110
01100110
01110111
01110111

\ character 52 ('4')
\ Small hut
00111100
01111110
11111111
01010010
01010010
01000010
01111110
01111110

\ character 53 ('5')
00011000
00111100
01111110
11111111
01111110
01001110
01111010
01111010

\ character 54 ('6')
00011000
00111100
01111110
11111111
01111110
01111110
01100110
01100110

\ character 55 ('7')
\ Left part of the door of the middle huts
00111111
00111111
00111111
00111110
00111100
00111100
00111100
00111100

\ character 56 ('8')
\ Right part of the door of the middle huts
11111100
11111100
11111100
01111100
00111100
00111100
00111100
00111100

\ character 57 ('9')
00111100
01000010
10111101
10000001
10000001
10000001
01000010
00111100

\ character 58 (':')
00000000
00000000
00000000
00010000
00000000
00000000
00010000
00000000

\ character 59 (';')
00000000
00000000
00010000
00000000
00000000
00010000
00010000
00100000

\ character 60 ('<')
00000011
00000101
00000101
00001001
00001001
00010001
00010001
00100001

\ character 61 ('=')
11000000
10100000
10100000
10010000
10010000
10001000
10001000
10000100

\ character 62 ('>')
11111111
00000000
00000000
00000000
00000000
00000000
00000000
00000000

\ character 63 ('?')
\ XXX REMARK -- not used
00000000
00111100
01000010
00000100
00001000
00000000
00001000
00000000

\ character 64 ('@')
\ XXX REMARK -- not used
00000000
00111100
01001010
01010110
01011110
01000000
00111100
00000000

\ character 65 ('A')

\ Sun:
\ AB
\ CD
\
\ Also, the individual characters are used as corners of the
\ island.

0000001111000000
0000111111110000
0011111111111100
0011111111111100
0111111111111110
0111111111111110
1111111111111111
1111111111111111
1111111111111111
1111111111111111
0111111111111110
0111111111111110
0011111111111100
0011111111111100
0000111111110000
0000001111000000

\ character 69 ('E')
\ Cloud 0:
\ EFGH
\ IJKL
00111000000000011100000000001110
01111100001100111110011100111111
11111110011111111111111111111111
11111111111111111111111111111111
01111111111111111111111111111110
00111111111111111111111111111110
00111111111111111111111111111100
01111111111111111111111111111100
11111111111111111111111111111110
11111111111111111111111111111111
01111111111111111111111111111111
00111111111111111111111111111110
00111111110111111111111111110100
01111111100111111101110111110000
01111110000011111001100111111000
00111000000001110000100011110000

\ character 77 ('M')
\ Cloud 1:
\ MNO
\ PQR
001111000000111000000110
011111110001111100001111
011111111001111110011111
111111111111111111011110
111111111111111111111100
111111111111111111111110
011111111111111111111110
011111111111111111111111
001111111111111111111111
001111111111111111111110
011111111111111111111100
011111111111111111111110
111111111111111011111111
111111111111110011111111
011111001111100001111110
001110000111000000001100

\ character 83 ('S')
\ Left roof of the big and middle huts
00000001
00000011
00000111
00001111
00011111
00111111
01111111
11111111

\ character 84 ('T')
\ Right roof of the big and middle huts
10000000
11000000
11100000
11110000
11111000
11111100
11111110
11111111

\ character 85 ('U')
\ Door of the big huts
11111111
11111111
11111111
11100111
11000011
11000011
11000011
11000011

\ character 86 ('V')
\ Left window of the big huts
00011111
00011111
00011001
00011001
00011001
00011111
00011111
00011111

\ character 87 ('W')
\ Right window of the big huts
11111000
11111000
10011000
10011000
10011000
11111000
11111000
11111000

\ character 88 ('X')
\ Native villager with vertical lance
00000010
00110010
00110010
01111110
10110010
00110010
01001010
01001010

\ character 89 ('Y')
\ Native villager with diagonal lance
00001000
00110100
00110010
01111101
10110000
00110000
01001000
01001000

\ character 90 ('Z')
\ Native villager without lance
00000000
00011000
00011000
00111100
01011010
00011000
00100100
00100100

\ character 91 ('[')
\ XXX REMARK -- not used
00000000
00001110
00001000
00001000
00001000
00001000
00001110
00000000

\ character 92 ('\')
\ XXX REMARK -- not used
00000000
00000000
01000000
00100000
00010000
00001000
00000100
00000000

\ character 93 (']')
\ XXX REMARK -- not used
00000000
01110000
00010000
00010000
00010000
00010000
01110000
00000000

\ character 94 ('^')
\ XXX REMARK -- not used
00000000
00010000
00111000
01010100
00010000
00010000
00010000
00000000

\ character 95 ('_')
\ XXX REMARK -- not used
00000000
00000000
00000000
00000000
00000000
00000000
00000000
11111111

\ character 96 ('`') (GPB sign in ZX Spectrum)
\ XXX REMARK -- not used
00000000
00011100
00100010
01111000
00100000
00100000
01111110
00000000

\ character 97 ('a')
00000000
00000000
00000011
11100100
00011111
00011111
11100100
00001100

\ character 98 ('b')
00000110
00001111
00001111
00011111
00000101
00000111
00000110
00000011

\ character 99 ('c')
01100000
11110000
11111000
11110000
10100000
11100000
01100000
11000000

\ character 100 ('d')
00110001
11111111
11101111
11100111
11100111
01100111
01100111
00100111

\ character 101 ('e')
10000000
11111100
01111110
11100110
01100110
11100110
01100110
11100010

\ character 102 ('f')
00100111
00100110
00100110
00100110
00100000
00100000
00100000
00100000

\ character 103 ('g')
11100000
01100000
01100000
01100000
01100000
01100000
01111000
01011000

\ character 104 ('h')
11100000
01100000
01100000
01100000
01111000
01011000
00000000
00000000

\ character 105 ('i')
00100111
00100110
00100110
00100110
00100000
00100000
00000000
00000000

\ character 106 ('j')
00000000
00011000
00111100
00111100
00011000
00011000
00011000
00011000

\ character 107 ('k')
11111111
11111111
00000000
00000000
00000000
00000000
00000000
00000000

\ character 108 ('l')
00000000
00000111
00011111
00111111
00111111
00011111
00000111
00000000

\ character 109 ('m')
00000000
11100000
11111000
11111100
11111100
11111000
11100000
00000000

\ character 110 ('n')
01100000
11100000
11100000
00010001
00001001
00000101
00000000
00000000

\ character 111 ('o')
00111100
01111110
11111111
11111111
10011001
10011001
11111111
11111111

\ character 112 ('p')
00000110
00000111
00000111
10001000
10010000
10100000
00000000
00000000

\ character 113 ('q')
00000010
00000100
00001000
00010000
11100000
11100000
01100000
00000000

\ character 114 ('r')
01111110
00000000
00111100
00111100
00011000
00000000
00000000
00000000

\ character 115 ('s')
01000000
00100000
00010000
00001000
00000111
00000111
00000110
00000000

\ character 116 ('t')
00000010
00000010
00000010
00000010
00000010
00000010
00000010
00000010

\ character 117 ('u')
00000000
00000000
00000000
00000000
00000010
00000111
00000010
00000010

\ "vw" = dubloon
0000001111000000
0001110000111000
0010000000000100
0010000110000100
0011100000011100
0001111111111000
0000011111100000
0000000000000000

\ character 120 ('x')
00000000
00000000
10000000
11011000
01100110
00000001
00000000
00000000

\ character 121 ('y')
00000000
00000000
00000000
00110000
01001100
10000011
00000000
00000000

\ character 122 ('z')
00000000
00000000
11000000
00100111
11111000
11111000
00100111
00110000

\ character 123 ('{')
\ XXX REMARK -- not used
00000000
00001110
00001000
00110000
00001000
00001000
00001110
00000000

\ character 124 ('|')
\ XXX REMARK -- not used
00000000
00001000
00001000
00001000
00001000
00001000
00001000
00000000

\ character 125 ('}')
\ XXX REMARK -- not used
00000000
01110000
00010000
00001100
00010000
00010000
01110000
00000000

\ character 126 ('~')
\ XXX REMARK -- not used
00000000
00010100
00101000
00000000
00000000
00000000
00000000
00000000

\ character 127 (Copyright sign in ZX Spectrum)
\ XXX REMARK -- not used
00111100
01000010
10011001
10100001
10100001
10011001
01000010
00111100

