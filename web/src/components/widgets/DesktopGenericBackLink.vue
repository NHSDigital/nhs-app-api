<template>
  <a :class="[$style['nhsuk-back-link__link']]"
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

 .nhsuk-back-link__link {
   display: inline-block;
 }

</style>
