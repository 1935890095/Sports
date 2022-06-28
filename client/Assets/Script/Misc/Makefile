include ../../../premake.mk

DLL := Misc.dll
PDB := Misc.pdb
MDB := Misc.dll.mdb

SRC = \
	*.cs \
	Http/*.cs \
	UI/*.cs

LIBS := \
	-r:Core.dll \
	-r:UnityEngine.dll \
	-r:UnityEngine.CoreModule.dll \
	-r:UnityEngine.IMGUIModule.dll \
	-r:UnityEngine.UnityWebRequestModule.dll

debug release: build

.PHONY: editor
editor: 
	$(MAKE) -C Editor