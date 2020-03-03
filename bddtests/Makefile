
-include ../buildscripts/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run the integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_tranche.sh

$(eval $(call expand_run_options_docker_images,run-accessibility))
run-accessibility: run-deps	## Run the accessibility integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_accessibility.sh

$(eval $(call expand_run_options_docker_images,run-cosmos))
run-cosmos: run-deps	## Run the cosmos integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_cosmos.sh

$(eval $(call expand_run_options_docker_images,run-onlineconsultations))
run-onlineconsultations: run-deps	## Run the online consultations integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_onlineconsultations.sh

$(eval $(call expand_run_options_docker_images,run-others))
run-others: run-deps	## Run the other integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_others.sh

$(eval $(call expand_run_options_docker_images,run-local))
run-local: run-deps	## Run in docker with ports open
	./buildscripts/02_integration_test_code_analysis.sh
	RUN_LOCAL_BDD=1 ./buildscripts/03_integration_test_run_tranche.sh

$(eval $(call expand_run_options_docker_images,run-native-android))
run-native-android: run-deps	## Run the nativesmoketests in BrowserStack against Android
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_browserstack_android.sh

$(eval $(call expand_run_options_docker_images,run-native-ios))
run-native-ios:	run-deps ## Run the nativesmoketests in BrowserStack against iOS; NATIVE_APP_PATH_IOS must be set to the path to the ipa to test
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_browserstack_ios.sh

run-deps:
	$(MAKE) -C .. run-deps

.PHONY: run run-others run-local run-native-android run-native-ios run-deps validate_local_secrets validate_local_images

-include ../buildscripts/util.make
