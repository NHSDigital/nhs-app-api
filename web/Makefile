ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS) ## Build all web components
test: $(TEST_SCRIPTS) ## Unit test all web components

.PHONY: $(ALL_SCRIPTS) build test
$(ALL_SCRIPTS):
	$@

-include ../buildscripts/util.make
