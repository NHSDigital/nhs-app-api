<template>
  <header-slim v-if="isSlim" :click-url="backLink">{{ headerText }}</header-slim>
  <div v-else :class="$style['header-container']">
    <header>
      <div :class="$style.spacer" />
      <nhs-logo :class="$style.nhsoLogo"/>
      <div :class="$style.spacer" />
    </header>
    <throttling-banner />
  </div>
</template>

<script>
import NhsLogo from '@/components/icons/NhsLogo';
import ThrottlingBanner from '@/components/ThrottlingBanner';
import HeaderSlim from '@/components/HeaderSlim';
import { GP_FINDER, GP_FINDER_RESULTS } from '@/lib/routes';

export default {
  components: {
    NhsLogo,
    ThrottlingBanner,
    HeaderSlim,
  },
  props: {
    pagesWithSlimHeaders: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {
      backLink: '#',
      headerText: '',
    };
  },
  computed: {
    isSlim() {
      return this.pagesWithSlimHeaders && this.pagesWithSlimHeaders.indexOf(this.$route.path) > -1;
    },
  },
  created() {
    switch (this.$route.path) {
      case GP_FINDER_RESULTS.path:
        this.backLink = GP_FINDER.path;
        this.headerText = this.$t('th03.header');
        break;
      default:
        break;
    }
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/headerslim";
@import "../style/throttling/throttlingheader";

.nhsoLogo {
  margin-top: 0 !important;
}

</style>
