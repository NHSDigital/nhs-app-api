<template>
  <div v-if="showTemplate" :class="[$style.webHeader, 'pull-content']">
    <header-slim :show-in-native="true"> {{ $t('termsAndConditions.title') }} </header-slim>
    <terms-conditions />
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import HeaderSlim from '@/components/HeaderSlim';
import TermsConditions from '@/components/TermsConditions';
import NativeCallbacks from '@/services/native-app';

export default {
  layout: 'termsAndConditions',
  components: {
    HeaderSlim,
    TermsConditions,
  },
  mounted() {
    this.version050compatibility();
  },
  methods: {
    getHeaderState() {
      return !this.$store.state.device.isNativeApp
        ? this.$style.webHeader : this.$style.nativeHeader;
    },
    version050compatibility() {
      if (this.$store.state.device.isNativeApp && this.$store.getters['appVersion/isPreForceUpdate']) {
        NativeCallbacks.hideHeader();
        NativeCallbacks.hideWhiteScreen();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  .webHeader {
    padding: 3.625em 0em 3.125em 2.0px;
  }

</style>
