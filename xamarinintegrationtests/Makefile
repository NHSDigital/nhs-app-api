ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*run*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))

build: $(BUILD_SCRIPTS)	## Build all integration test components

.PHONY: $(ALL_SCRIPTS) run run-local run-deps
$(ALL_SCRIPTS):
	$@

-include ../buildscripts/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run the integration tests
	./buildscripts/04_run_integration_tests.sh

$(eval $(call expand_run_options_docker_images,run-local))
run-local: run-deps	## Start the app so the integration tests can be run locally
	RUN_LOCAL=1 ./buildscripts/04_run_integration_tests.sh

run-deps:
	$(MAKE) -C .. run-deps

-include ../buildscripts/util.make
