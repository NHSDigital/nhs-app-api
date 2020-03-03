ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS)											## Build all backend components
test: $(TEST_SCRIPTS)												## Unit test all backend components

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@


-include ../buildscripts/util.make
