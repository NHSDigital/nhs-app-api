
define expand_run_options_docker_images

$(call expand_run_options_docker_image,$(1),WEB)
$(call expand_run_options_docker_image,$(1),PFSAPI)
$(call expand_run_options_docker_image,$(1),CIDAPI)
$(call expand_run_options_docker_image,$(1),SJRCONFIG)
$(call expand_run_options_docker_image,$(1),SJRAPI)
$(call expand_run_options_docker_image,$(1),USERSAPI)
$(call expand_run_options_docker_image,$(1),USERINFOAPI)
$(call expand_run_options_docker_image,$(1),LOGAPI)
$(call expand_run_options_docker_image,$(1),MESSAGESAPI)

endef

define expand_run_options_docker_image

$(call expand_run_options_docker_images_tag, \
	$(1), \
	$(2), \
	$(if $(filter $(flavor $(2)),undefined), \
		$(TAG), \
		$($(2))))

endef

define expand_run_options_docker_images_tag

# If arg is exactly "local" then set registry/tag to be local/latest
$(if $(filter $(3),local),\
	$(1): export $(2)_DOCKER_REGISTRY=local)
$(if $(filter $(3),local),\
	$(1): export $(2)_DOCKER_TAG=latest)

# If arg is exactly "host" then set port file to be host
$(if $(filter $(3),host), \
	$(1): export $(2)_DOCKER_PORTS=docker-compose.ports-$(strip $(2))-host.yml, \
	$(1): export $(2)_DOCKER_PORTS=docker-compose.ports-$(strip $(2))-container.yml)

# If arg is anything else then set tag to be the arg value
$(if $(filter-out local,$(3)),\
	$(1): export $(2)_DOCKER_REGISTRY=nhsapp.azurecr.io)
$(if $(filter-out local,$(3)),\
	$(1): export $(2)_DOCKER_TAG=$(3))

$(1): export $(2)=$(3)

endef
