<template>
  <svg :class="dynamicStyle('nhsuk-icon__arrow-right-circle')" xmlns="http://www.w3.org/2000/svg"
       viewBox="0 0 24 24" aria-hidden="true">
    <path d="M0 0h24v24H0z" fill="none"/>
    <path d="
          M12 2a10 10 0 0 0-9.95 9h11.64L9.74 7.05a1 1 0 0 1 1.41-1.41l5.66 5.65a1 1 0 0
           1 0 1.42l-5.66 5.65a1 1 0 0 1-1.41 0 1 1 0 0 1 0-1.41L13.69 13H2.05A10 10 0 1 0 12 2z"/>
  </svg>
</template>

<script>
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';

export default {
  name: 'NHSArrowCircle',
  props: {
    selected: {
      type: Boolean,
      default: false,
    },
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args, exchangeStyle({ button: 'btn' }));
    },
  },
};
</script>

<style lang="scss" module scoped>
@import "../../style/colours";
@import "../../style/navmenuicons";
/** Links */
.nhsuk-icon__arrow-right-circle-desktop {
  display: inline-block;
  vertical-align: middle;
  fill: #007f3b;
  height: 1.2em;
  left: -3px;
  top: -6px;
  width: 1.2em;
}

.nhsuk-icon, .nhsuk-icon__arrow-right-circle {
  display: none;
}

</style>
