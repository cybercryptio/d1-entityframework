# Copyright 2020-2022 CYBERCRYPT

##### Help message #####
help:  ## Display this help
	@awk 'BEGIN {FS = ":.*##"; printf "\nUsage:\n  make <target> \033[36m\033[0m\n\nTargets:\n"} /^[a-zA-Z0-9_-]+:.*?##/ { printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2 }' $(MAKEFILE_LIST)

##### Config #####
SHELL := /bin/bash

##### Build targets #####
.PHONY: build
build: ## Build the library
	@make -C D1DB.Sample build

##### Cleanup targets #####
.PHONY: clean
clean: ## Remove build artifacts
	@make -C D1DB.Sample build

.PHONY: docker-up
docker-up:
	docker-compose up --detach

.PHONY: docker-down
docker-down:
	docker-compose down -v

.PHONY: run
run:
	D1DB_CONNECTIONSTRINGS__DEFAULTCONNECTIONSTRING='Server=tcp:localhost,1433;Initial Catalog=SampleDB;Persist Security Info=False;User ID=sa;Password=yourStrong(!)Password;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30' \
	D1DB_D1GENERIC__URL='http://localhost:9000' \
	D1DB_D1GENERIC__OIDC__AUTHORIZATIONENDPOINT='http://7f0d22.iam.encryptonize.com/realms/master/protocol/openid-connect/auth' \
	D1DB_D1GENERIC__OIDC__CLIENTID='d1-sandbox' \
	dotnet run --project ./D1DB.Sample/D1DB.Sample.csproj
