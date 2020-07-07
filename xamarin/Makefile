ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
CLEAN_SCRIPTS := $(sort $(wildcard buildscripts/*clean*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(ALL_SCRIPTS)))

build: build-deps $(BUILD_SCRIPTS)	## Build xamarin clients
clean: $(CLEAN_SCRIPTS) ## Clean xamarin clients

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@

build-deps:
	$(MAKE) -C .. validate_local_secrets

-include ../buildscripts/util.make
