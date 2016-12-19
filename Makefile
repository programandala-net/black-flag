# Bandera Negra Makefile

# Author: Marcos Cruz (programandala.net), 2016

# This file is part of Bandera Negra
# http://programandala.net/

################################################################
# Requirements

# cat (from the GNU coreutils)

# fsb2 (by Marcos Cruz)
# 	http://programandala.net/en.program.fsb2.html

# mkmgt (by Marcos Cruz)
# 	http://programandala.net/en.program.mkmgt.html

################################################################
# History

# 2016-12-19: Start.

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

tmp/bandera_negra.fba: src/bandera_negra.fs
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
