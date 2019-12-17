
-include ../build/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run the BDD tests
	$(MAKE) -C ops run

$(eval $(call expand_run_options_docker_images,run-local))
run-local: run-deps	## Run in docker with ports open
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.ports.yml

$(eval $(call expand_run_options_docker_images,run-native-android))
run-native-android: run-deps	## Run the nativesmoketests in BrowserStack against Android; BROWSERSTACK_APPPATH must be set (e.g. ops20/AndroidTCv1.17.0)
	$(MAKE) -C ops run RUN_NATIVE=1 RUN_AS_DEVELOP=1 BROWSER=browserstack_android

$(eval $(call expand_run_options_docker_images,run-native-ios))
run-native-ios:	run-deps ## Run the nativesmoketests in BrowserStack against iOS; BROWSERSTACK_APPPATH must be set (e.g. ops20/iOSTCv1.17.0)
	$(MAKE) -C ops run RUN_NATIVE=1 RUN_AS_DEVELOP=1 BROWSER=browserstack_ios

run-deps:
	$(MAKE) -C .. run-deps

.PHONY: run run-local run-native-android run-native-ios run-deps validate_local_secrets validate_local_images

-include ../build/util.make
