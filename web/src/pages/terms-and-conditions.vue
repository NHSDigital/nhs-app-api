<template>
  <div v-if="showTemplate" :class="[$style.webHeader, 'pull-content']">
    <header-slim :show-in-native="true"> {{ $t('termsAndConditions.title') }} </header-slim>
    <terms-conditions/>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import NativeCallbacks from '@/services/native-app';
import HeaderSlim from '@/components/HeaderSlim';
import TermsConditions from '@/components/TermsConditions';

export default {
  layout: 'termsAndConditions',
  components: {
    HeaderSlim,
    TermsConditions,
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.hideHeader();
      NativeCallbacks.hideWhiteScreen();
    }
  },
  methods: {
    getHeaderState() {
      return !this.$store.state.device.isNativeApp
        ? this.$style.webHeader : this.$style.nativeHeader;
    },
  },
};
</script>

<style module lang="scss" scoped>
  .webHeader {
    padding: 3.625em 0em 3.125em 2.0px;
  }

</style>
