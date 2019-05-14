<template>
  <a :class="[$style['nhsuk-back-link__link'], $style['desktopBackLink']]"
     :href="path"
     data-purpose="main-back-button"
     @click.prevent="$emit('clickAndPrevent', $event)">
    {{ $t(getBackButtonText) }}</a>
</template>
<script>
export default {
  name: 'DesktopGenericBackButton',
  props: {
    path: {
      type: String,
      default: undefined,
    },
    buttonText: {
      type: String,
      default: 'generic.backButton.text',
    },
  },
  computed: {
    getBackButtonText() {
      return this.buttonText;
    },
  },
};
</script>
<style module lang="scss" scoped>
 /*@import "../../style/textstyles";*/
 /*@import "../../style/fonts";*/
 /*@import "../../style/colours";*/

 @import '~nhsuk-frontend/packages/core/all.scss';
 @import '~nhsuk-frontend/packages/components/back-link/back-link';

 .desktopBackLink {
  font-size: 1.125em;
  line-height: 1.125em;
  font-weight: normal;
  vertical-align: middle;
  padding-left: 0;
  margin: 40px 0;
 }

</style>
