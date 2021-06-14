ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
VALIDATION_SCRIPTS := $(sort $(wildcard buildscripts/*validate*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS)											## Build all backend components
validate: $(VALIDATION_SCRIPTS)							## Run backend validation scripts
test: $(TEST_SCRIPTS)												## Unit test all backend components

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@


-include ../buildscripts/util.make
