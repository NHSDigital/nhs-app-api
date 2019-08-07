ALL_SCRIPTS := $(sort $(wildcard buildscripts/*.sh))
TEST_SCRIPTS := $(sort $(wildcard buildscripts/*test*.sh))
BUILD_SCRIPTS := $(sort $(filter-out $(TEST_SCRIPTS),$(ALL_SCRIPTS)))
SSH_CERT := ~/.nhsonline/local-development-certificate/local-development-https.crt

build: $(BUILD_SCRIPTS)											## Build all web components
test: $(TEST_SCRIPTS)												## Unit test all web components

run:	## Run in docker
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml

run-biometrics:	## Run in docker with biometrics enabled
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.biometrics.yml

run-https: $(SSH_CERT)	## Run in docker with https enabled
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https.yml

run-android: $(SSH_CERT)	## Run in docker for android
	../build/run_docker_compose.sh ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https-android.yml

run-android-biometrics: $(SSH_CERT)	## Run in docker for android with biometrics enabled
	../build/run_docker_compose.sh  ../docker-compose.yml ../docker-compose.ports.yml docker-compose.yml docker-compose.https-android.yml docker-compose.biometrics.yml

$(SSH_CERT):
	$(error Could not find $(SSH_CERT); Please run web/create-certificate.sh)

.PHONY: $(ALL_SCRIPTS)
$(ALL_SCRIPTS):
	$@

-include ../build/util.make
