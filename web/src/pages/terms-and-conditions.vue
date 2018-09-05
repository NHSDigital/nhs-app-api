<template>
  <div :class="[getHeaderState(), 'pull-content']">
    <header-slim> {{ $t('termsAndConditions.title') }} </header-slim>
    <body>
      <div>
        <terms-conditions/>
      </div>
    </body>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import HeaderSlim from '@/components/HeaderSlim';
import TermsConditions from '@/components/TermsConditions';

export default {
  components: {
    HeaderSlim,
    TermsConditions,
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
