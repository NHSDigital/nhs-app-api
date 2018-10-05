/* eslint-disable */
import Vue from 'vue';
import MedicationCourseStatus from '@/lib/medication-course-status';

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
    }
  }
});
