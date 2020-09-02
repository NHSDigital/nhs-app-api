ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS) ## Build all web components
test: $(TEST_SCRIPTS) ## Unit test all web components

dev-init: ## Initalise docker container and npm packages for local development
	run/dev_init.sh

dev-run:  ## Run dev docker container with local files mounted with hot reloading
	run/dev_run.sh

dev-audit:  ## Run audit in docker container
	run/dev_audit.sh

dev-bash: ## Start dev docker container at bash prompt to run arbitrary npm commands
	run/dev_bash.sh

.PHONY: $(ALL_SCRIPTS) build test dev-init dev-run dev-audit dev-bash
$(ALL_SCRIPTS):
	$@

-include ../buildscripts/util.make
