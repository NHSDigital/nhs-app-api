
-include ../build/expand_run_options_docker_images.make

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

$(eval $(call expand_run_options_docker_images,run-throttling))
run-throttling: run-deps	## Run the throttling integration tests
	./buildscripts/02_integration_test_code_analysis.sh
	./buildscripts/03_integration_test_run_throttling.sh

$(eval $(call expand_run_options_docker_images,run-local))
run-local: run-deps	## Run in docker with ports open
	./buildscripts/02_integration_test_code_analysis.sh
	RUN_LOCAL_BDD=1 ./buildscripts/03_integration_test_run_tranche.sh

$(eval $(call expand_run_options_docker_images,run-native-android))
run-native-android: run-deps	## Run the nativesmoketests in BrowserStack against Android; BROWSERSTACK_APPPATH must be set (e.g. ops20/AndroidTCv1.17.0)
	$(MAKE) -C ops run RUN_NATIVE=1 RUN_AS_DEVELOP=1 BROWSER=browserstack_android

$(eval $(call expand_run_options_docker_images,run-native-ios))
run-native-ios:	run-deps ## Run the nativesmoketests in BrowserStack against iOS; BROWSERSTACK_APPPATH must be set (e.g. ops20/iOSTCv1.17.0)
	$(MAKE) -C ops run RUN_NATIVE=1 RUN_AS_DEVELOP=1 BROWSER=browserstack_ios

run-deps:
	$(MAKE) -C .. run-deps

.PHONY: run run-others run-throttling run-local run-native-android run-native-ios run-deps validate_local_secrets validate_local_images

-include ../build/util.make
