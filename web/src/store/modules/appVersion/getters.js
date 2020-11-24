import semver from 'semver';

import { isBlankString } from '@/lib/utils';

const develop = 'develop';
const pullRequestRegex = RegExp(/^pr\d+$/);
const minorVersionIgnoreRegex = RegExp(/[.][xX]$/);

function isStoreVersionAfterVersionToTest(
  versionInStore,
  versionToTestRaw,
  isNative = false,
) {
  if (
    (!isNative && isBlankString(versionInStore))
    || isBlankString(versionToTestRaw)
    || versionInStore === versionToTestRaw
  ) {
    // bad data in store/parameter or identical versions
    return false;
  }

  if (versionInStore === develop) {
    // pass all version checks on develop version of native/web
    return true;
  }

  if (pullRequestRegex.test(versionInStore)) {
    // pass all version checks on PR native/web versions
    return true;
  }

  let versionToTest = versionToTestRaw;

  if (versionToTestRaw.toLowerCase().endsWith('.x')) {
    // call only wants to check major + minor version (ignore patch versions)
    versionToTest = versionToTest.replace(minorVersionIgnoreRegex, '.9999');
  }

  const validVersionInStoreNumber = semver.valid(versionInStore) !== null;
  const validVersionToTestNumber = semver.valid(versionToTest) !== null;

  if (isNative && !validVersionInStoreNumber) {
    // pass all version checks on a native check without a store version number
    // (this usually indicates we are running in a mock native context)
    return true;
  }

  if (!validVersionInStoreNumber || !validVersionToTestNumber) {
    return false;
  }

  const semVersionInStore = semver.coerce(versionInStore);
  const semVersionToTest = semver.coerce(versionToTest);

  // is the major version newer or same major version but minor version is newer
  // or same major and minor version but patch version is newer
  return semVersionInStore.major > semVersionToTest.major ||
    (
      semVersionInStore.major === semVersionToTest.major
      && semVersionInStore.minor > semVersionToTest.minor
    ) ||
    (
      semVersionInStore.major === semVersionToTest.major
      && semVersionInStore.minor === semVersionToTest.minor
      && semVersionInStore.patch > semVersionToTest.patch
    );
}

export default {
  isNativeVersionAfter: state => toTest =>
    isStoreVersionAfterVersionToTest(state.nativeVersion, toTest, true),
  isWebVersionAfter: state => toTest =>
    isStoreVersionAfterVersionToTest(state.webVersion, toTest),
};
