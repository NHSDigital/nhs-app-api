<template>
  <div :class="[getHeaderState(), 'pull-content']">
    <header-slim> {{ $t('sy01.pageHeader') }}</header-slim>
    <body>
      <div>
        <symptoms-check/>
      </div>
    </body>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import NativeCallbacks from '@/services/native-app';
import HeaderSlim from '@/components/HeaderSlim';
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  components: {
    HeaderSlim,
    symptomsCheck,
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.showHeaderSlim();
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
    margin-top: -3.625em;
  }

  .nativeHeader {
    padding: 0em 0em 3.125em 2.0px;
  }

</style>
