export default {
  isPreForceUpdate() {
    // hideMenuBar was introduced to the native apps for 0.6.0 release.
    // Pre 0.6.0 will not have this function.
    return !window.nativeApp.hideMenuBar;
  },
};
