<template>
  <div>
    <a v-if="!$store.state.device.isNativeApp"
       :class="$style['nhsuk-skip-link']" href="#maincontent"
       @click.prevent="setFocus()">{{ $t('skipLink.linkText') }}</a>
  </div>
</template>

<script>
export default {
  name: 'SkipLink',
  methods: {
    setFocus() {
      if (this.$parent.$parent !== undefined) {
        this.$parent.$parent.$refs.mainContent.focus();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/skip-link/_skip-link.scss';

  .nhsuk-skip-link:active, .nhsuk-skip-link:focus
  {
    z-index: 999;
  }
</style>
