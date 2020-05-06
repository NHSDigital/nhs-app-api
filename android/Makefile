ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: build-deps $(BUILD_SCRIPTS)	## Build android client
test: $(TEST_SCRIPTS)	## Unit test android client

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@

build-deps:
	$(MAKE) -C .. validate_local_secrets

-include ../buildscripts/util.make
