  \ 001_loader.fs
  \
  \ This file is part of Black Flag
  \ http://programandala.net/

( Black Flag -- load block  )

[defined] need
?\ 2 load
  \ Compile the `need` utility, which must be in block 2.

  \ need decode need words need order need dump
  \ XXX TMP -- for debugging

need load-program

load-program black-flag

  \ Compile the game.

  \ vim: filetype=soloforth
