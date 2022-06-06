<template>
  <p :class="clazz">
    <a :class="[$style['inline-link']]"
       :href="path"
       :target="target"
       data-purpose="main-back-button"
       @click.prevent="goBack">
      {{ $te(getBackButtonText) ? $t(getBackButtonText) : getBackButtonText }}</a>
  </p>
</template>
<script>
import { redirectTo } from '@/lib/utils';

export default {
  name: 'DesktopGenericBackButton',
  props: {
    path: {
      type: String,
      required: true,
      validator: value => (value !== undefined && value !== ''),
    },
    clazz: {
      type: String,
      default: undefined,
    },
    buttonText: {
      type: String,
      default: 'generic.back',
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
  methods: {
    goBack(event) {
      if (this.$listeners.clickAndPrevent) {
        this.$emit('clickAndPrevent', event);
        return;
      }
      redirectTo(this, this.path);
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import "@/style/custom/inline-link";
</style>

