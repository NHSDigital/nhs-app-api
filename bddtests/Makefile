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

run-deps:
	$(MAKE) -C .. run-deps

.PHONY: $(BUILD_SCRIPTS) build run run-dev-stubs run-others run-local run-deps validate_local_secrets validate_local_images

-include ../buildscripts/util.make
