ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
CLEAN_SCRIPTS := $(sort $(wildcard buildscripts/*clean*.sh))
ANDROID_SCRIPTS := $(sort $(wildcard buildscripts/*android*.sh))
IOS_SCRIPTS := $(sort $(wildcard buildscripts/*ios*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(ALL_SCRIPTS)))
ANDROID_BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(ANDROID_SCRIPTS)))
IOS_BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(IOS_SCRIPTS)))

build: build-deps ## Build xamarin clients
	$(BUILD_SCRIPTS)

build-android: build-deps	## Build android xamarin clients
	$(ANDROID_BUILD_SCRIPTS)

build-ios: build-deps	## Build ios xamarin clients
	$(IOS_BUILD_SCRIPTS)

clean: $(CLEAN_SCRIPTS)	## Clean xamarin clients

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@

build-deps:
	$(MAKE) -C .. validate_local_secrets

-include ../buildscripts/util.make
