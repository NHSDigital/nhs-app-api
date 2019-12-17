ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))
SSH_CERT := ~/.nhsonline/local-development-certificate/local-development-https.crt

build: $(BUILD_SCRIPTS)											## Build all web components
test: $(TEST_SCRIPTS)												## Unit test all web components

-include ../build/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run in docker
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml

$(eval $(call expand_run_options_docker_images,run-biometrics))
run-biometrics: run-deps	## Run in docker with biometrics enabled
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.biometrics.yml

$(eval $(call expand_run_options_docker_images,run-https))
run-https: run-deps $(SSH_CERT)	## Run in docker with https enabled
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https.yml

$(eval $(call expand_run_options_docker_images,run-android))
run-android: run-deps $(SSH_CERT)	## Run in docker for android
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https-android.yml

$(eval $(call expand_run_options_docker_images,run-android-biometrics))
run-android-biometrics: run-deps $(SSH_CERT)	## Run in docker for android with biometrics enabled
	../build/run_docker_compose.sh  ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https-android.yml docker-compose.biometrics.yml

run-deps:
	$(MAKE) -C .. run-deps

$(SSH_CERT):
	$(error Could not find $(SSH_CERT); Please run web/create-certificate.sh)

.PHONY: $(ALL_SCRIPTS) build test run run-biometrics run-https run-android run-android-biometrics run-deps
$(ALL_SCRIPTS):
	$@

-include ../build/util.make
