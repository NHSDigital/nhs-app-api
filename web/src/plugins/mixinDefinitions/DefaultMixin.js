import getOr from 'lodash/fp/getOr';
// eslint-disable-next-line import/no-extraneous-dependencies
import MedicationCourseStatus from '@/lib/medication-course-status';
import NativeApp from '@/services/native-app';
import { generateContextualHelpLink, redirectTo } from '@/lib/utils';
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
      this.checkHasNetworkAccess();

      return !this.hasConnectionProblem;
    },
    currentHelpUrl() {
      return generateContextualHelpLink(this.$store, this.$route);
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    hasNetworkProblem() {
      return this.$store.state.errors.hasConnectionProblem;
    },
    hasConnectionProblem() {
      return this.hasNetworkProblem || this.hasApiError;
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      this.checkHasNetworkAccess();
    },
  },
  methods: {
    configureWebContext() {
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
        NativeApp.configureWebContext(this.currentHelpUrl, retryPath);
      } else {
        // TODO: Add code when help function is added to the web version (Jira ticket NHSO-6388)
      }
    },
    correctUrl(url) {
      return url === LOGIN_PATH && this.$store.getters['session/isLoggedIn']()
        ? LOGOUT_PATH
        : url;
    },
    checkHasNetworkAccess() {
      const hasNetworkAccess = navigator.onLine;
      this.$store.dispatch('errors/setConnectionProblem', !hasNetworkAccess);
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
      redirectTo(this, this.$route.path, this.$route.query);
    },
  },
};
