#! /usr/bin/env bash
set -e

scriptPath=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

outputPath="${scriptPath}/.output"
libPath="${scriptPath}/lib"
templatePath="${scriptPath}/template"

function publishPomFile()
{
  local libraryName="$1"
  local libraryFile="${outputPath}/${libraryName}.aar"
  local pomFile="${outputPath}/${libraryName}.pom"

  echo "Publishing ${libraryName} pom file..."

  mvn deploy:deploy-file \
    -DrepositoryId="nhsapp" \
    -Durl="https://pkgs.dev.azure.com/nhsapp/_packaging/nhsapp/maven/v1" \
    -DpomFile="${pomFile}" \
    -Dfile="${libraryFile}"
}

function publishPomFiles()
{
  pushd "${outputPath}" &> /dev/null

  for lib in *.pom; do
    local libraryName="${lib%.*}"

    publishPomFile "${libraryName}"
  done

  popd &> /dev/null
}

function buildLibPomFile()
{
  local libraryName="$1"
  local version="$2"
  local paycassoVersion="$3"

  echo "Building ${libraryName} pom file..."

  local libVersion="${version}-${paycassoVersion}"
  local outFilePrefix="${outputPath}/${libraryName}-${libVersion}"
  local pomFile="${outFilePrefix}.pom"

  cp "${libPath}/${libraryName}.aar" "${outFilePrefix}.aar"
  cp "${templatePath}/${libraryName}.pom" "${pomFile}"

  sed -i '' "s/#PACKAGE_VERSION#/${libVersion}/g" "${pomFile}"
}

function buildLibPomFiles()
{
  local paycassoVersion="$1"
  local version=$(date +%Y.%-m.%-d)

  pushd "${libPath}" &> /dev/null

  for lib in *.aar; do
    libraryName="${lib%.*}"

    buildLibPomFile "${libraryName}" "${version}" "${paycassoVersion}"
  done

  popd &> /dev/null
}

function createOutputPath()
{
  rm -rf "${outputPath}"
  mkdir -p "${outputPath}"
}

function publish()
{
  local paycassoVersion=$(<"${scriptPath}/paycasso-version.txt")

  createOutputPath

  buildLibPomFiles "${paycassoVersion}"
  publishPomFiles
}

publish
