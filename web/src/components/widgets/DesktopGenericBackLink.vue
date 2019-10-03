<template>
  <a :class="[$style['nhsuk-back-link__link'], $style['desktopBackLink']]"
     :href="path"
     :target="target"
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
    target: {
      type: String,
      default: '',
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

 @import '~nhsuk-frontend/packages/core/all.scss';
 @import '~nhsuk-frontend/packages/components/back-link/back-link';

 .desktopBackLink {
  font-size: 1.125em;
  line-height: 1.125em;
  font-weight: normal;
  vertical-align: middle;
  padding-left: 0;
  margin: 0 0 40px 0;
 }

</style>
