# Bandera Negra Makefile

# Author: Marcos Cruz (programandala.net), 2016

# This file is part of Bandera Negra
# http://programandala.net/

# Last modified 201702021204

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
# History

# 2016-12-19: Start.
#
# 2016-12-21: Build fonts and UDG sources from the original TAP files.
#
# 2017-01-08: Reorganize graph fonts and add ordinary fonts.
#
# 2017-02-02: Add Spanish chars UDG files. Update the requirements.

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
all: disk_2_bandera_negra.mgt

.PHONY : clean
clean:
	rm -f tmp/*
	rm -f disk_2_bandera_negra.mgt

secondary_source_files=$(sort $(wildcard src/00*.fsb))
library_source_files=$(sort $(wildcard src/lib/*.fsb))

tmp/bandera_negra_graph_font_1.fs: graphs/jolly_roger_graph_font_1.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/bandera_negra_graph_font_2.fs: graphs/jolly_roger_graph_font_2.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/bandera_negra_udg.fs: graphs/jolly_roger_udg.tap
	make/udg_tap_to_forth.fs $< > $@

tmp/bandera_negra_sticks_font.fs: fonts/sticks_font.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/bandera_negra_twisty_font.fs: fonts/twisty_font.tap
	make/udg_tap_to_forth_c-comma.fs $< > $@

tmp/bandera_negra_sticks_font_es.fs: fonts/sticks_font_es.fs
	./$< > $@

tmp/bandera_negra_twisty_font_es.fs: fonts/twisty_font_es.fs
	./$< > $@

tmp/bandera_negra.complete.fs: \
	src/bandera_negra.fs \
	tmp/bandera_negra_graph_font_1.fs \
	tmp/bandera_negra_graph_font_2.fs \
	tmp/bandera_negra_sticks_font.fs \
	tmp/bandera_negra_twisty_font.fs \
	tmp/bandera_negra_sticks_font_es.fs \
	tmp/bandera_negra_twisty_font_es.fs \
	tmp/bandera_negra_udg.fs \
	src/bandera_negra_end-app.fs
	cat $^ > $@

tmp/bandera_negra.complete.converted.fs: tmp/bandera_negra.complete.fs
	vim -S ./make/utf8_to_udg.vim \
		-c "set fileencoding=latin1" \
		-c "saveas! $@" \
		-c "quit!" $<

tmp/bandera_negra.fba: tmp/bandera_negra.complete.converted.fs
	./make/fs2fba.sh $<
	mv $(basename $<).fba $@

tmp/library.fsb: \
	$(secondary_source_files) \
	$(library_source_files)
	cat $^ > $@

tmp/library.fb: tmp/library.fsb
	fsb2 $<
	mv $<.fb $@

tmp/disk_2_bandera_negra.fb: \
	tmp/library.fb \
	tmp/bandera_negra.fba
	cat $^ > $@

disk_2_bandera_negra.mgt: tmp/disk_2_bandera_negra.fb
	cp $< $<.copy
	make/fb2mgt.sh tmp/disk_2_bandera_negra.fb
	mv tmp/disk_2_bandera_negra.mgt .
	mv $<.copy $<
