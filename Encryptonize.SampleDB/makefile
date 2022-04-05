# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
# Change shell to bash to allow sourcing of env variables
SHELL := /bin/bash

##### Variables #####
proto_path = ../../../../../encryptonize-core/integrations/protobuf

##### Functions #####
# Check that given variables are set and all have non-empty values,
# die with an error otherwise.
#
# Params:
#   1. Variable name(s) to test.
#   2. (optional) Error message to print.
check_defined = \
    $(strip $(foreach 1,$1, \
        $(call __check_defined,$1,$(strip $(value 2)))))
__check_defined = \
    $(if $(value $1),, \
        $(error Undefined $1$(if $2, ($2))$(if $(value @), \
                required by target `$@')))

##### Build targets #####
.PHONY: build
build:
	$(MAKE) -C ../Encryptonize.EntityFramework build proto_path=$(proto_path)
	dotnet build

.PHONY: run
run:
	@:$(call check_defined, db_connection_string enc_url enc_user enc_pass)	
	ENCDB_CONNECTIONSTRINGS__DEFAULTCONNECTIONSTRING=$(db_connection_string) ENCDB_ENCRYPTONIZE__URL=$(enc_url) ENCDB_ENCRYPTONIZE__USERNAME=$(enc_user) ENCDB_ENCRYPTONIZE__PASSWORD=$(enc_pass) dotnet run

##### Cleanup targets #####
.PHONY: clean  ## Remove build artifacts
clean :
	dotnet clean
