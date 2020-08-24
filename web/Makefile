ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS) ## Build all web components
test: $(TEST_SCRIPTS) ## Unit test all web components

dev-init:
	run/dev_init.sh

dev-run:
	run/dev_run.sh

dev-audit:
	run/dev_audit.sh

.PHONY: $(ALL_SCRIPTS) build test dev-init dev-run dev-audit
$(ALL_SCRIPTS):
	$@

-include ../buildscripts/util.make
