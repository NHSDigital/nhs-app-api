define AdditionalHelp
@echo
@echo 'For run* targets the following options can be used to customise the version of the docker images used:'
@echo
@echo '* TAG: Set the default tag for all images, e.g. set TAG=develop to run the images pushed by the develop CI build. Defaults to latest local build.'
@echo '* WEB, PFSAPI, CIDAPI, SJRCONFIG, SJRAPI, USERSAPI, USERINFOAPI, LOGAPI, MESSAGESAPI: Override the tag used for specific containers.'
@echo '    Set to "host" to route all traffic for that container to the host machine so the process can be run and debugged on the host'
endef

.PHONY: build test clean localbdd run run-biometrics run-https run-android run-android-biometrics run-localbdd run-smoketests run-deps validate_local_secrets validate_local_images

build:	## Build everything (run make from a subdirectory for more granularity)
	$(MAKE) -C backendworker build
	$(MAKE) -C web build

test:	## Unit test everything
	$(MAKE) -C backendworker test
	$(MAKE) -C web test

clean:	## Delete all docker containers and volumes
	./build/clean.sh

localbdd: build	run-localbdd	## Build everything and start containers ready to BDD tests locally

-include build/expand_run_options_docker_images.make

$(eval $(call expand_run_options_docker_images,run))
run: run-deps	## Run in docker
	./build/run_docker_compose.sh docker-compose.yml docker-compose.ports.yml

run-biometrics:	## Run in docker with biometrics
	$(MAKE) -C web run-biometrics

run-https:	## Run in docker with https
	$(MAKE) -C web run-https

run-android:	## Run in docker with https for android
	$(MAKE) -C web run-android

run-android-biometrics:	## Run in docker for android with biometrics
	$(MAKE) -C web run-android-biometrics

run-localbdd:	## Run in docker with stubs so BDD tests can be run locally
	$(MAKE) -C bddtests run-local

run-bdd:	## Run the BDD tests as they are run in the CI build
	$(MAKE) -C bddtests run

run-deps: validate_local_secrets validate_local_images

validate_local_secrets:
	./build/validate_local_secrets.sh

$(eval $(call expand_run_options_docker_images,validate_local_images))
validate_local_images:
	./build/validate_local_images.sh

-include build/util.make
