import getOr from 'lodash/fp/getOr';
// eslint-disable-next-line import/no-extraneous-dependencies
import MedicationCourseStatus from '@/lib/medication-course-status';
import NativeCallbacks from '@/services/native-app';
import { redirectTo } from '@/lib/utils';
import { createUri } from '@/lib/noJs';
import {
  GP_MEDICAL_RECORD_PATH,
  LOGOUT_PATH,
  LOGIN_PATH,
  INDEX_PATH,
} from '@/router/paths';

export default {
  name: 'defaultMixin',
  computed: {
    showTemplate() {
      return !this.$store.getters['errors/showApiError'] && !this.hasConnectionProblem();
    },
  },
  methods: {
    configureWebContext(currentHelpUrl) {
      if (this.$store.state.device.isNativeApp) {
        let retryPath = '';

        if (this.$store.getters['session/isProxying']) {
          retryPath = createUri({
            path: INDEX_PATH,
            noJs: {
              flashMessage: {
                show: true,
                key: 'profiles.itMayNotBePossibleToProxyRightNow',
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
    correctUrl(url) {
      return url === LOGIN_PATH && this.$store.getters['session/isLoggedIn']()
        ? LOGOUT_PATH
        : url;
    },
    hasConnectionProblem() {
      const hasConnectionError = !navigator.onLine;
      this.$store.dispatch('errors/setConnectionProblem', hasConnectionError);
      return hasConnectionError;
    },
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

      if (url === GP_MEDICAL_RECORD_PATH && statusCode === 504) {
        this.$store.dispatch('myRecord/load');
      }

      if ((url === LOGIN_PATH && this.$store.getters['session/isLoggedIn']())
        || url === LOGOUT_PATH) {
        this.$store.dispatch('auth/logout');
        return;
      }

      redirectTo(this, url);
    },
    reload() {
      redirectTo(this, this.$route.path);
    },
  },
};
