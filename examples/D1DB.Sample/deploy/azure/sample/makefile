# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

SHELL := /bin/bash # To allow source

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

##### Targets #####

.PHONY: deploy
deploy: ## Deploy the infrastructure
	@:$(call check_defined, resource_group adminUser adminPass serverName)
	./scripts/deploy-infra.sh -u $(adminUser) -p '$(adminPass)' -r $(resource_group) -s $(serverName)

.PHONY: clean
clean: ## Delete the infrastructure
	@:$(call check_defined, resource_group adminUser adminPass serverName)
	./scripts/delete-infra.sh -r $(resource_group) -n $(serverName)
