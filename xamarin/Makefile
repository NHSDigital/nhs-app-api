ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
CLEAN_SCRIPTS := $(sort $(wildcard buildscripts/*clean*.sh))
ANDROID_SCRIPTS := $(sort $(wildcard buildscripts/*android*.sh))
IOS_SCRIPTS := $(sort $(wildcard buildscripts/*ios*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(ALL_SCRIPTS)))
ANDROID_BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(ANDROID_SCRIPTS)))
IOS_BUILD_SCRIPTS := $(sort $(filter-out $(CLEAN_SCRIPTS),$(IOS_SCRIPTS)))

build: build-deps $(BUILD_SCRIPTS) ## Build Xamarin clients

build-android: build-deps $(ANDROID_BUILD_SCRIPTS)	## Build Android Xamarin client

build-ios: build-deps $(IOS_BUILD_SCRIPTS)	## Build iOS Xamarin client

clean: $(CLEAN_SCRIPTS)	## Clean Xamarin clients

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@

build-deps:
	$(MAKE) -C .. validate_local_secrets

-include ../buildscripts/util.make
