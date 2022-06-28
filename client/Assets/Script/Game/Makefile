include ../../../premake.mk

DLL := Game.dll
MDB := Game.dll.mdb
PDB := Game.pdb

SRC := -recurse:*.cs

LIBS := \
	-r:Core.dll \
	-r:Misc.dll \
	-r:Asset.dll \
	-r:Action.dll \
	-r:Model.dll \
	-r:XLua.dll \
	-r:protobuf-net.dll \
	-r:UnityEngine.dll \
	-r:UnityEngine.CoreModule.dll \
	-r:UnityEngine.ParticlesLegacyModule.dll \
	-r:UnityEngine.ParticleSystemModule.dll \
	-r:UnityEngine.TextRenderingModule.dll \
	-r:UnityEngine.AnimationModule.dll \
	-r:UnityEngine.UnityWebRequestWWWModule.dll

debug release: build
