/* eslint-disable */
import Vue from 'vue';

Vue.directive('visible', (el, binding) => {
  el.style.visibility = !!binding.value ? 'visible' : 'hidden';
});
