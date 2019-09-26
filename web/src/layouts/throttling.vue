<template>
  <div id="app">
    <main>
      <spinner />
      <nuxt />
    </main>
  </div>
</template>

<script>
import Spinner from '@/components/widgets/Spinner';
import NativeVersionSetup from '@/services/nativeVersionSetup';
import { findByName } from '@/lib/routes';

export default {
  components: {
    Spinner,
  },
  head() {
    const head = {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - ${this.$t('appTitle')}`,
      script: [],
    };
    if (this.$env.ANALYTICS_SCRIPT_URL !== 'NOT_SET') {
      head.script = [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ];
    }
    return head;
  },
  computed: {
    currentHelpUrl() {
      return findByName(this.$route.name).helpUrl;
    },
  },
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
    this.setHelpUrl(this.currentHelpUrl);
  },
};
</script>

<style lang="scss">
@import '../style/main';
@import '../style/pulltorefresh';
@import '../style/elements';
</style>

<style module lang='scss' scoped>
@import '../style/home';
@import '../style/throttling/throttling';
</style>
