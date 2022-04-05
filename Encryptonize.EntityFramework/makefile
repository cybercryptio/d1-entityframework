# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
# Change shell to bash to allow sourcing of env variables
SHELL := /bin/bash

##### Variables #####
proto_path = ../../../../../encryptonize-core/integrations/protobuf

##### Build targets #####
.PHONY: build
build:
	$(MAKE) -C ../../../clients/encryption-service/dotnet/Encryptonize.Client build proto_path=$(proto_path)
	dotnet build ./src

.PHONY: test
test: build
	dotnet test ./tests/Encryptonize.EntityFramework.Tests --no-build

.PHONY: pack
pack: build
	dotnet pack ./src --output ./artifacts/ --configuration Release

##### Cleanup targets #####
.PHONY: clean  ## Remove build artifacts
clean :
	dotnet clean ./src
	dotnet clean ./tests/Encryptonize.EntityFramework.Tests
	rm -rf ./artifacts/
