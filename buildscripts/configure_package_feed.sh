#! /usr/bin/env bash
set -e

# Change current working directory to be the root, regardless of how this script is invoked
cd "$(dirname "${BASH_SOURCE[0]}")/.." || exit 1

echo -e "Personal Access Token\n
You will require a token to access the feeds, to generate a new one:\n
Login in Azure DevOps\n
* Go to User Settings (top-right beside help)\n
* Click Personal access tokens\n
* Click New Token\n
* Configure Name and Expiration\n
* Ensure Scopes is set to Custom defined\n
* Check Packaging > Read\n
* Click Create\n
* Add the new token to your password manager\n"

read -s -p "Please enter your Devops token:" USER_TOKEN
echo -e "\nPlease enter your HSCIC email address:"
read EMAIL_ADDRESS

ENCODED_USER_TOKEN=$(echo -n "$USER_TOKEN" | base64)

TOKEN_PLACE_HOLDER='<TOKEN>'
EMAIL_PLACE_HOLDER='<HSCIC_EMAIL>'

NUGET_CONFIG_FILE='NuGet.config'
NPM_CONFIG_FILE='.npmrc'
MAVEN_CONFIG_FILE='settings.xml'

TEMPLATE_FILE_LOCATION='buildscripts/package_feeds/'

TEMPLATE_NUGET_LOCATION="$TEMPLATE_FILE_LOCATION$NUGET_CONFIG_FILE"
TEMPLATE_NPM_LOCATION="$TEMPLATE_FILE_LOCATION$NPM_CONFIG_FILE"
TEMPLATE_MAVEN_LOCATION="$TEMPLATE_FILE_LOCATION$MAVEN_CONFIG_FILE"

case "$(uname -s)" in

   Darwin)
     echo 'Mac OS detected'
     NUGET_CONFIG_LOCATION="${HOME}/.nuget/NuGet/"
     NPM_CONFIG_LOCATION="${HOME}/"
     MAVEN_CONFIG_LOCATION="${HOME}/.m2/"
     ;;

   Linux)
     echo 'Linux detected'
     NUGET_CONFIG_LOCATION="${HOME}/.nuget/NuGet/"
     NPM_CONFIG_LOCATION="${HOME}/"
     MAVEN_CONFIG_LOCATION="${HOME}/.m2/"
     ;;

   CYGWIN*|MINGW32*|MSYS*|MINGW*)
     echo 'Windows detected'
     NUGET_CONFIG_LOCATION="$APPDATA/NuGet/"
     NPM_CONFIG_LOCATION="$USERPROFILE/"
     MAVEN_CONFIG_LOCATION="$USERPROFILE/.m2/"
     ;;
   *)
     echo 'Unsupported OS'
     exit 1
     ;;
esac

NUGET_CONFIG_LOCATION="$NUGET_CONFIG_LOCATION$NUGET_CONFIG_FILE"
NPM_CONFIG_LOCATION="$NPM_CONFIG_LOCATION$NPM_CONFIG_FILE"
MAVEN_CONFIG_LOCATION="$MAVEN_CONFIG_LOCATION$MAVEN_CONFIG_FILE"

function copy_and_replace_tokens {
  SOURCE_CONFIG=$1
  TARGET_CONFIG=$2
  TOKEN_VALUE=$3

  cat "$SOURCE_CONFIG" | sed -e "s/$TOKEN_PLACE_HOLDER/$TOKEN_VALUE/g" -e "s/$EMAIL_PLACE_HOLDER/$EMAIL_ADDRESS/g" > "$TARGET_CONFIG"
  echo "Template copied to $TARGET_CONFIG"
}

function setup_package_feed {
  SOURCE_CONFIG=$1
  TARGET_CONFIG=$2
  TOKEN_VALUE=$3

  if [ -e "$TARGET_CONFIG" ]; then
    echo "$TARGET_CONFIG exists do you want to overwrite? (y/n)"
    read CONTINUE
    if [ "$CONTINUE" == "y" ]; then
      cp -R "$TARGET_CONFIG" "$TARGET_CONFIG.old"
      echo "Copy of existing config made in $TARGET_CONFIG.old"
      copy_and_replace_tokens $SOURCE_CONFIG $TARGET_CONFIG $TOKEN_VALUE
    else
      echo -e "File has not been replaced or updated \n"
    fi
  else
    copy_and_replace_tokens $SOURCE_CONFIG $TARGET_CONFIG $TOKEN_VALUE
  fi
}

echo "Configuring NuGet Feed"
setup_package_feed "$TEMPLATE_NUGET_LOCATION" "$NUGET_CONFIG_LOCATION" "$USER_TOKEN"
echo "Configuring npm Feed"
setup_package_feed "$TEMPLATE_NPM_LOCATION" "$NPM_CONFIG_LOCATION" "$ENCODED_USER_TOKEN"
echo "Configuring maven Feed"
setup_package_feed "$TEMPLATE_MAVEN_LOCATION" "$MAVEN_CONFIG_LOCATION" "$USER_TOKEN"

echo 'Package feed configured'
