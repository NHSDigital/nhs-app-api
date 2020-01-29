import getOr from 'lodash/fp/getOr';
// eslint-disable-next-line import/no-extraneous-dependencies
import Vue from 'vue';
import MedicationCourseStatus from '@/lib/medication-course-status';
import NativeCallbacks from '@/services/native-app';
import ResetPageFocusMixin from '@/plugins/mixinDefinitions/ResetPageFocus';
import Sources from '@/lib/sources';
import { redirectTo } from '@/lib/utils';
import { createUri } from '@/lib/noJs';
import { ACCOUNT_SIGNOUT, LOGIN, MYRECORD, GP_MEDICAL_RECORD, INDEX } from '@/lib/routes';

Vue.mixin(ResetPageFocusMixin);

Vue.mixin({
  computed: {
    showTemplate() {
      let hasConnectionError = false;
      if (process.client && !navigator.onLine) {
        this.$store.dispatch('errors/setConnectionProblem', true);
        hasConnectionError = true;
      }
      return !this.$store.getters['errors/showApiError'] && !hasConnectionError;
    },
  },
  methods: {
    getMedicationCourseStatus(medicationStatusId) {
      // eslint-disable-next-line no-restricted-syntax
      for (const [key, value] of Object.entries(MedicationCourseStatus)) {
        if (value === medicationStatusId) {
          return key;
        }
      }
      return null;
    },
    goToUrl(url, statusCode = undefined) {
      if (url === '') {
        this.$router.go();
        return;
      }

      if ((url === MYRECORD.path || url === GP_MEDICAL_RECORD.path) && statusCode === 504) {
        this.$store.dispatch('myRecord/load');
      }

      if ((url === LOGIN.path && this.$store.getters['session/isLoggedIn']())
        || url === ACCOUNT_SIGNOUT.path) {
        this.$store.dispatch('auth/logout');
        return;
      }

      if (url === LOGIN.path) {
        const sourceDevice = this.$store.state.device.source;
        if (Sources.isNative(sourceDevice)) {
          redirectTo(this, url, { source: sourceDevice });
          return;
        }
      }

      redirectTo(this, url);
    },
    correctUrl(url) {
      return url === LOGIN.path && this.$store.getters['session/isLoggedIn']()
        ? ACCOUNT_SIGNOUT.path
        : url;
    },
    configureWebContext(currentHelpUrl) {
      if (this.$store.state.device.isNativeApp) {
        let retryPath = '';

        if (this.$store.getters['session/isProxying']) {
          retryPath = createUri({
            path: INDEX.path,
            noJs: {
              flashMessage: {
                show: true,
                key: 'linkedProfiles.lossProxyError',
                type: 'error',
              },
            },
          });
        } else {
          retryPath = getOr('', 'state.errors.pageSettings.redirectUrl.default', this.$store);
        }
        NativeCallbacks.configureWebContext(currentHelpUrl, retryPath);
      } else {
        // TODO: Add code when help function is added to the web version (Jira ticket NHSO-6388)
      }
    },
  },
});
