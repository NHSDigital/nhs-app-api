<template>
  <div id="app">
    <throttling-header v-if="showHeader" :class="getHeaderClasses"
                       :pages-with-slim-headers="pagesWithSlimHeader"/>
    <main :class="[this.$style.homeMain, 'throttlingContent']">
      <connection-error />
      <api-error />
      <flash-message />
      <nuxt />
    </main>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import ThrottlingHeader from '@/components/ThrottlingHeader';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import { GP_FINDER_RESULTS } from '@/lib/routes';

export default {
  components: {
    ApiError,
    ConnectionError,
    FlashMessage,
    ThrottlingHeader,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText}`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
  },
  data() {
    return {
      pagesWithSlimHeader: [
        GP_FINDER_RESULTS.path,
      ],
    };
  },
  computed: {
    showHeader() {
      return this.showTemplate;
    },
    getHeaderClasses() {
      const classes = ['throttlingHeader'];
      if (this.pagesWithSlimHeader.indexOf(this.$route.path) === -1) {
        classes.push('notSlim');
      }
      return classes;
    },
  },
};
</script>

<style lang="scss">
@import '../style/main';
@import '../style/pulltorefresh';
@import '../style/elements';
@import '../style/throttling/throttling';
</style>

<style module lang='scss' scoped>
@import '../style/home';
</style>
