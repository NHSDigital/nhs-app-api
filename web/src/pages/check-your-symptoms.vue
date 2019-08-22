<template>
  <div :class="[getHeaderState(), 'pull-content', $store.state.device.isNativeApp && $style.web]">
    <header-slim v-if="$store.state.device.isNativeApp"> {{ $t('sy01.pageHeader') }}</header-slim>
    <div>
      <symptoms-check/>
    </div>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import NativeCallbacks from '@/services/native-app';
import HeaderSlim from '@/components/HeaderSlim';
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  layout: 'nhsuk-layout',
  components: {
    HeaderSlim,
    symptomsCheck,
  },
  mounted() {
    if (this.$store.state.device.isNativeApp) {
      NativeCallbacks.showHeaderSlim();
      NativeCallbacks.hideWhiteScreen();
    } else {
      window.scrollTo(0, 0);
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
   &.web {
    margin-top: -3.625em;
   }
  }

  .nativeHeader {
    padding: 0 0 3.125em 2.0px;
  }

</style>
