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
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
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
