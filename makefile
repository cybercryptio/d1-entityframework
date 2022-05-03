# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
SHELL := /bin/bash

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
      $(error Undefined $1$(if $2, ($2))))

##### Build targets #####
.PHONY: build
build: ## Build the library
	dotnet build ./src

.PHONY: build-examples
build-examples: ## Build the samples
	@make -C examples build

.PHONY: tests
tests: build ## Run the tests
	dotnet test ./tests/Encryptonize.EntityFramework.Tests

.PHONY: pack
pack: ## Pack the NuGet package
	$(call check_defined, VERSION)
	dotnet pack ./src --output ./artifacts/ --configuration Release /p:Version=${VERSION}

.PHONY: publish
publish: pack ## Publish the NuGet package
	$(call check_defined, PACKAGE_SOURCE, API_KEY)
	dotnet nuget push ./artifacts/Encryptonize.Client.${VERSION}.nupkg --source "${PACKAGE_SOURCE}" --api-key "${API_KEY}"

##### Cleanup targets #####
.PHONY: clean
clean: ## Remove build artifacts
	dotnet clean ./src
	dotnet clean ./tests/Encryptonize.EntityFramework.Tests
	rm -rf ./artifacts/
