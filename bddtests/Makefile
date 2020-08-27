BUILD_SCRIPTS := $(sort $(wildcard buildscripts/*_build_*.sh))

build: $(BUILD_SCRIPTS)	## Build dev stubs

$(BUILD_SCRIPTS):
	$@

-include ../buildscripts/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
$(call expand_run_options_docker_image,run,STUBS)
run: run-deps	## Run the integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_tranche.sh

$(eval $(call expand_run_options_docker_images,run-accessibility))
$(call expand_run_options_docker_image,run-accessibility,STUBS)
run-accessibility: run-deps	## Run the accessibility integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_accessibility.sh

$(eval $(call expand_run_options_docker_images,run-others))
$(call expand_run_options_docker_image,run-others,STUBS)
run-others: run-deps	## Run the other integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_others.sh

$(eval $(call expand_run_options_docker_images,run-local))
$(call expand_run_options_docker_image,run-local,STUBS)
run-local: run-deps	## Run in docker with ports open
	RUN_LOCAL_BDD=1 ./buildscripts/03_integration_test_run_tranche.sh

$(eval $(call expand_run_options_docker_images,run-native-android))
$(call expand_run_options_docker_image,run-native-android,STUBS)
run-native-android: run-deps	## Run the nativesmoketests in BrowserStack against Android
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_browserstack_android.sh

$(eval $(call expand_run_options_docker_images,run-native-ios))
$(call expand_run_options_docker_image,run-native-ios,STUBS)
run-native-ios:	run-deps ## Run the nativesmoketests in BrowserStack against iOS; NATIVE_APP_PATH_IOS must be set to the path to the ipa to test
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_browserstack_ios.sh

run-deps:
	$(MAKE) -C .. run-deps

.PHONY: $(BUILD_SCRIPTS) build run run-dev-stubs run-others run-local run-native-android run-native-ios run-deps validate_local_secrets validate_local_images

-include ../buildscripts/util.make
