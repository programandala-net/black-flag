# Bandera Negra Makefile

# Author: Marcos Cruz (programandala.net), 2016, 2017

# This file is part of Black Flag
# http://programandala.net/en.program.black_flag.html

# Last modified 201705211943
# See change log at the end of the file

################################################################
# Requirements

# cat (from the GNU coreutils)

# fsb2 (by Marcos Cruz)
# 	http://programandala.net/en.program.fsb2.html

# Gforth (by Anton Erlt, Bernd Paysan et al.)
# 	http://gnu.org/software/gforth

# mkmgt (by Marcos Cruz)
# 	http://programandala.net/en.program.mkmgt.html

# vim (by Bram Moolenaar)
#   http://vim.org

################################################################
# Notes

# $^ list of all prerequisites
# $? list of prerequisites changed more recently than current target
# $< name of first prerequisite
# $@ name of current target

################################################################
# Config

VPATH = ./

MAKEFLAGS = --no-print-directory

.ONESHELL:

################################################################
# Main

.PHONY: all
all: disk_2_black_flag.mgt

.PHONY : clean
clean:
	rm -f tmp/*
	rm -f disk_2_black_flag.mgt

secondary_source_files=$(sort $(wildcard src/00*.fs))
library_source_files=$(sort $(wildcard src/lib/*.fs))

tmp/graph_font_1.fs: graphs/jolly_roger_graph_font_1.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/graph_font_2.fs: graphs/jolly_roger_graph_font_2.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/udg.fs: graphs/jolly_roger_udg.tap
	make/udg_tap_to_forth.fs $< > $@

tmp/sticks_font.fs: fonts/sticks_font.fs
	./$< > $@

tmp/twisty_font.fs: fonts/twisty_font.fs
	./$< > $@

tmp/sticks_font_es.fs: fonts/sticks_font_es.fs
	./$< > $@

tmp/twisty_font_es.fs: fonts/twisty_font_es.fs
	./$< > $@

tmp/black_flag.complete.fs: \
	src/black_flag.fs \
	tmp/graph_font_1.fs \
	tmp/graph_font_2.fs \
	tmp/sticks_font.fs \
	tmp/twisty_font.fs \
	tmp/sticks_font_es.fs \
	tmp/twisty_font_es.fs \
	tmp/udg.fs \
	src/black_flag_end-program.fs
	cat $^ > $@

tmp/black_flag.complete.converted.fs: tmp/black_flag.complete.fs
	vim -S ./make/utf8_to_udg.vim \
		-c "set fileencoding=latin1" \
		-c "saveas! $@" \
		-c "quit!" $<

tmp/black_flag.fba: tmp/black_flag.complete.converted.fs
	./make/fs2fba.sh $<
	mv $(basename $<).fba $@

tmp/library.fs: \
	$(secondary_source_files) \
	$(library_source_files)
	cat $^ > $@

tmp/library.fb: tmp/library.fs
	fsb2 $<

tmp/disk_2_black_flag.fb: \
	tmp/library.fb \
	tmp/black_flag.fba
	cat $^ > $@

disk_2_black_flag.mgt: tmp/disk_2_black_flag.fb
	cp $< $<.copy
	make/fb2mgt.sh tmp/disk_2_black_flag.fb
	mv tmp/disk_2_black_flag.mgt .
	mv $<.copy $<

################################################################
# Change log

# 2016-12-19: Start.
#
# 2016-12-21: Build fonts and UDG sources from the original TAP
# files.
#
# 2017-01-08: Reorganize graph fonts and add ordinary fonts.
#
# 2017-02-02: Add Spanish chars UDG files. Update the
# requirements.  Build the fonts from the Forth source files,
# using a FantomoUDG converter; the original TAP files are not
# needed anymore.
#
# 2017-02-04: Update after the new name of the project.
#
# 2017-03-05: Update filename extension after the changes in
# Solo Forth and fsb2.
#
# 2017-05-21: Update to changes in Solo Forth.

