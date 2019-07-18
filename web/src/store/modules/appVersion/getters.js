import semver from 'semver';

const develop = 'develop';

function isStoreVersionAfterVersionToTest(versionInStore, versionToTest) {
  if (versionInStore === versionToTest) {
    return false;
  }
  const validVersionInStoreNumber = semver.valid(versionInStore) !== null;
  const validVersionToTestNumber = semver.valid(versionToTest) !== null;

  if (validVersionInStoreNumber && validVersionToTestNumber) {
    return semver.gt(versionInStore, versionToTest);
  }
  if (validVersionInStoreNumber) {
    return false;
  }
  if (validVersionToTestNumber) {
    return true;
  }
  if (versionInStore === develop) {
    return false;
  }
  if (versionToTest === develop) {
    return true;
  }
  return versionInStore > versionToTest;
}

export default {
  isNativeVersionAfter: state => toTest =>
    isStoreVersionAfterVersionToTest(state.nativeVersion, toTest),
  isWebVersionAfter: state => toTest =>
    isStoreVersionAfterVersionToTest(state.webVersion, toTest),
};

