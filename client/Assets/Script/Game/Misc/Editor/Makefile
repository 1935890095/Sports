include ../../../../premake.mk

DLL := Misc.Editor.dll
MDB := Misc.Editor.dll.mdb
PDB := Misc.Editor.pdb

SRC := *.cs \
	UI/*.cs

LIBS := \
	-r:Misc.dll \
	-r:UnityEngine.dll \
	-r:UnityEditor.dll \
	-r:UnityEngine.CoreModule.dll \
	-r:UnityEngine.IMGUIModule.dll \
	-r:UnityEngine.UnityWebRequestModule.dll

debug: build_editor