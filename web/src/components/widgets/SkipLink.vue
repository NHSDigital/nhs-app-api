<template>
  <div>
    <a v-if="!$store.state.device.isNativeApp"
       :class="$style['nhsuk-skip-link']" href="#maincontent"
       @click.prevent="setFocus()">{{ $t('generic.skipToMainContent') }}</a>
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
  @import "@/style/custom/skip-link";
</style>
