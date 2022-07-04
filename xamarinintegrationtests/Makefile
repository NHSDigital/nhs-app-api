ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*run*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

SECURE_STUBS_SSL_CERT := ~/.nhsonline/secure-stubs-certificate/secure-stubs.crt

build: $(BUILD_SCRIPTS)	## Build all integration test components

generate-flipbook: ## Runs flipbook tests, build and generates the flipbook (Flipbook journeys must be ran before this)
	FLIPBOOK_RUN='True' ./buildscripts/06_run_integration_tests.sh && \
	./buildscripts/09_run_build_flipbook.sh && ./buildscripts/10_run_flipbook_generate.sh

.PHONY: $(ALL_SCRIPTS) run run-local run-deps
$(ALL_SCRIPTS):
	$@

-include ../buildscripts/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps $(SECURE_STUBS_SSL_CERT) ## Run the xamarin integration tests
	./buildscripts/06_run_integration_tests.sh

$(eval $(call expand_run_options_docker_images,run-local))
run-local: run-deps	$(SECURE_STUBS_SSL_CERT) ## Start the app so the integration tests can be run locally
	RUN_LOCAL=1 ./buildscripts/06_run_integration_tests.sh

$(eval $(call expand_run_options_docker_images,run-local-upgrade))
run-local-upgrade: run-deps	## Start the app so the upgrade integration tests can be run locally
	RUN_LOCAL=1 USE_APP_UPGRADE_CONFIG='True' ./buildscripts/06_run_integration_tests.sh

run-deps:
	$(MAKE) -C .. run-deps

$(SECURE_STUBS_SSL_CERT):
	../buildscripts/create-secure-stub-certificate.sh

-include ../buildscripts/util.make
