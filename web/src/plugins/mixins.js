/* eslint-disable */
import Vue from 'vue';
import MedicationCourseStatus from '@/lib/medication-course-status';
import { LOGIN, ACCOUNT_SIGNOUT } from '@/lib/routes';
import Sources from '@/lib/sources';

Vue.mixin({
  computed: {
    showTemplate() {
      const hasConnectionError = this.$store.state.errors.hasConnectionProblem;
      return !this.$store.getters['errors/showApiError'] && !hasConnectionError;
    },
  },
  methods: {
    getMedicationCourseStatus(medicationStatusId) {
      for (const [key, value] of Object.entries(MedicationCourseStatus)) {
        if (value === medicationStatusId) {
          return key;
        }
      }
      return null;
    },
    goToUrl(url) {
      if (url === '') {
        this.$router.go();
        return;
      }

      if ((url === LOGIN.path && this.$store.getters['session/isLoggedIn']())
        || url == ACCOUNT_SIGNOUT.path) {
        this.$store.dispatch('auth/logout');
        return;
      }

      if (url === LOGIN.path) {
        const sourceDevice = this.$store.state.device.source;
        if (Sources.isNative(sourceDevice)) {
          this.$router.push({
            path: url,
            query: { source: sourceDevice }
          });
          return;
        }
      }

      this.$router.push(url);
    },
    correctUrl(url) {
      if (url === LOGIN.path && this.$store.getters['session/isLoggedIn']()) {
        url = ACCOUNT_SIGNOUT.path;
      }
      return url;
    },
  },
});
