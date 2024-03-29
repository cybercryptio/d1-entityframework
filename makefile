# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
SHELL := /bin/bash

publishDir = "./artifacts/publish"
library = "$(publishDir)/CyberCrypt.D1.EntityFramework.dll"
apiDocsDir = "./documentation/api"

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
	dotnet build
	$(MAKE) apidocs

.PHONY: tests
tests: build ## Run the tests
	dotnet test ./tests/CyberCrypt.D1.EntityFramework.Tests

.PHONY: nuget-pack
nuget-pack: ## Pack the NuGet package
	$(call check_defined, VERSION)
	dotnet pack ./src --output ./artifacts/ --configuration Release /p:Version=${VERSION}

.PHONY: nuget-publish
nuget-publish: nuget-pack ## Publish the NuGet package
	$(call check_defined, PACKAGE_SOURCE, API_KEY)
	dotnet nuget push ./artifacts/CyberCrypt.D1.EntityFramework.${VERSION}.nupkg --source "${PACKAGE_SOURCE}" --api-key "${API_KEY}"

.PHONY: publish
publish: ## Publish the library and its dependencies to a local directory
	dotnet publish -o $(publishDir) --configuration Release ./src/

.PHONY: apidocs
apidocs: publish ## Generate API documentation
	rm -rf $(apiDocsDir)
	xmldocmd $(library) $(apiDocsDir)

.PHONY: apidocs-verify
apidocs-verify: publish ## Verify API documentation is up-to-date
	git ls-files --other --modified --deleted --exclude-standard -- $(apiDocsDir)  | sed q1

##### Cleanup targets #####
.PHONY: clean
clean: ## Remove build artifacts
	dotnet clean ./src
	dotnet clean ./tests/CyberCrypt.D1.EntityFramework.Tests
	rm -rf ./artifacts/
