# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Variables #####
release_name = d1db-sample

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
.PHONY: install
install: ## Install the sample application in a Kubernetes cluster
	@:$(call check_defined, registry_pass db_connectionstring enc_uid enc_pass enc_url)
	helm install $(release_name) D1DBSample \
	--set imageCredentials.registry=ghcr.io,imageCredentials.username=cybot-cc,imageCredentials.email=cybot@cyber-crypt.com \
	--set imageCredentials.password='$(registry_pass)',database.connectionstring="$(db_connectionstring)",d1.username='$(enc_uid)' \
	--set d1.password='$(enc_pass)',d1.url='$(enc_url)'

.PHONY: uninstall
uninstall: ## Uninstall the sample application
	helm uninstall $(release_name)
