SHELL=/bin/bash -euo pipefail

install: install-node install-python install-hooks

install-python:
	poetry install

install-node:
	npm install --legacy-peer-deps
	cd sandbox && npm install --legacy-peer-deps

install-hooks:
	cp scripts/pre-commit .git/hooks/pre-commit

lint:
	npm run lint
	find . -name '*.py' | xargs poetry run flake8

clean:
	rm -rf build
	rm -rf dist

publish: clean
	mkdir -p build
	npm run publish 2> /dev/null

serve:
	npm run serve

check-licenses:
	npm run check-licenses
	scripts/check_python_licenses.sh

format:
	poetry run black **/*.py

sandbox: update-examples
	cd sandbox && npm run start

build-proxy:
	scripts/build_proxy.sh

release: clean publish build-proxy
	mkdir -p dist
	cp -r build/. dist
	cp -r tests dist
	cp ecs-proxies-deploy.yml dist/ecs-deploy-sandbox.yml
	cp ecs-proxies-deploy.yml dist/ecs-deploy-internal-qa-sandbox.yml
	cp ecs-proxies-deploy.yml dist/ecs-deploy-internal-dev-sandbox.yml

test:
	echo "TODO: add tests"
