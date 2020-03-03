

export IMAGE_NAMES=(nhsonline-web nhsonline-backendpfsapi nhsonline-backendcidapi nhsonline-backendservicejourneyrulesapi nhsonline-backendusersapi nhsonline-backenduserinfoapi nhsonline-backendmessagesapi)
export IMAGE_SETTING_NAMES=(WEB PFSAPI CIDAPI SJRCONFIG SJRAPI USERSAPI USERINFOAPI LOGAPI MESSAGESAPI)

export PFSAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.PfsApi/Properties/launchSettings.json
export PFSAPI_SERVICE_NAME=pfs.local.bitraft.io

export CIDAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.CidApi/Properties/launchSettings.json
export CIDAPI_SERVICE_NAME=cid.local.bitraft.io

export SJRAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.ServiceJourneyRulesApi/Properties/launchSettings.json
export SJRAPI_SERVICE_NAME=servicejourneyrulesapi.local.bitraft.io

export USERSAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.UsersApi/Properties/launchSettings.json
export USERSAPI_SERVICE_NAME=users.local.bitraft.io

export USERINFOAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.UserInfoApi/Properties/launchSettings.json
export USERINFOAPI_SERVICE_NAME=userinfo.local.bitraft.io

export LOGAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.LoggerApi/Properties/launchSettings.json
export LOGAPI_SERVICE_NAME=log.local.bitraft.io

export MESSAGESAPI_LAUNCH_SETTINGS=backendworker/NHSOnline.Backend.MessagesApi/Properties/launchSettings.json
export MESSAGESAPI_SERVICE_NAME=messages.local.bitraft.io

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." || exit 1; pwd)"
export REPO_ROOT
