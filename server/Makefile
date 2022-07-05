export GOARCH=amd64
export GOOS=linux

build:
	rm -rf ./build && mkdir build
	cd build && mkdir lobby
	cd build && mkdir match
	cd build && mkdir game

	cd ./lobby/run && go build -o ../../build/lobby/lobby

	cd ./match/run && go build -o ../../build/match/match

	cd ./game/run && go build -o ../../build/game/game

.PHONY: clean
clean:
	rm -rf ./build
