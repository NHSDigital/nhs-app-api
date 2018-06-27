/* eslint-disable */
import Vue from 'vue';
import { MedicationCourseStatus } from '@/lib/medication-course-status';

Vue.mixin({
  computed: {
    showTemplate() {
      return !this.$store.getters['errors/showApiError'];
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