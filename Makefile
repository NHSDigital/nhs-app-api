define AdditionalHelp
@echo
@echo 'For run* targets the following options can be used to customise the version of the docker images used:'
@echo
@echo '* TAG: Set the default tag for all images, e.g. set TAG=develop to run the images pushed by the develop CI build. Defaults to latest local build.'
@echo '* WEB, PFSAPI, CIDAPI, SJRCONFIG, SJRAPI, USERSAPI, USERINFOAPI, LOGAPI, MESSAGESAPI: Override the tag used for specific containers.'
@echo '    Set to "host" to route all traffic for that container to the host machine so the process can be run and debugged on the host'
endef

SSH_CERT := ~/.nhsonline/local-development-certificate/local-development-https.crt

build:	## Build web and backend worker
	$(MAKE) -C backendworker build
	$(MAKE) -C web build

build-all: build	## Build everything (run make from a subdirectory for more granularity)
	$(MAKE) -C android build
	$(MAKE) -C bddtests build

test:	## Unit test web and backend worker
	$(MAKE) -C backendworker test
	$(MAKE) -C web test

test-all: test	## Unit test everything
	$(MAKE) -C android test

clean:	## Delete all docker containers and volumes
	./buildscripts/clean.sh

localbdd: build	run-localbdd	## Build everything and start containers ready to BDD tests locally

twistlock:	# Run twistlock security scan
	./buildscripts/run_twistlock_security_scan.sh

.PHONY: build test clean localbdd twistlock

-include buildscripts/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run in docker
	./buildscripts/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml

run-dev-stubs:	## Run in docker with dev stubs
	$(MAKE) -C bddtests run-dev-stubs

$(eval $(call expand_run_options_docker_images,run-perf-stubs))
run-perf-stubs: $(SSH_CERT)	 ## Run performance stubs in docker
	./buildscripts/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml docker/performance/docker-compose.yml

$(eval $(call expand_run_options_docker_images,run-https))
run-https: $(SSH_CERT)	## Run in docker with https
	./buildscripts/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml docker/https/docker-compose.yml

$(eval $(call expand_run_options_docker_images,run-android))
run-android:	## Run in docker for android
	./buildscripts/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml docker/android/docker-compose.yml

$(eval $(call expand_run_options_docker_images,run-android-https))
run-android-https: $(SSH_CERT)	## Run in docker with https for android
	./buildscripts/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml docker/https/docker-compose.yml docker/android/docker-compose.yml docker/android/docker-compose.https.yml

run-localbdd:	## Run in docker with stubs so BDD tests can be run locally
	$(MAKE) -C bddtests run-local

run-bdd:	## Run the BDD tests as they are run in the CI build
	$(MAKE) -C bddtests run

run-deps: validate_local_secrets validate_local_images

validate_local_secrets:
	./buildscripts/validate_local_secrets.sh

$(eval $(call expand_run_options_docker_images,validate_local_images))
validate_local_images:
	./buildscripts/validate_local_images.sh

$(SSH_CERT):
	./buildscripts/create-certificate.sh

.PHONY: run run-dev-stubs run-https run-android run-android-https run-localbdd run-bdd run-deps validate_local_secrets validate_local_images

-include buildscripts/util.make
