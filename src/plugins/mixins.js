/* eslint-disable */
import Vue from 'vue';

Vue.mixin({
  computed: {
    showTemplate() {
      return !this.$store.getters['errors/showApiError'];
    },
  },
});