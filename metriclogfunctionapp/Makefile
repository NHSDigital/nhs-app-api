
.PHONY: run run-inttests

run: Database/update.sql	## Run function dependencies in docker
	./buildscripts/run_docker_compose.sh docker-compose.yml

run-inttests: Database/update.sql	## Run function and dependencies in docker for integration tests
	RUN_LOCAL=1 ./buildscripts/start_int_tests.sh

Database/update.sql: Database/merge_scripts.sh $(shell find Database/*/ -type f -name '*.sql')
	./Database/merge_scripts.sh

-include buildscripts/util.make
