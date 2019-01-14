<template>
  <div id="app">
    <main>
      <div v-if="showErrorMessageContainer" :class="$style.errorMessageContainer">
        <connection-error />
        <api-error />
        <flash-message />
      </div>
      <nuxt />
    </main>
  </div>
</template>

<script>
/* eslint-disable no-underscore-dangle */
import Sources from '@/lib/sources';
import ApiError from '@/components/errors/ApiError';
import ConnectionError from '@/components/errors/ConnectionError';
import FlashMessage from '@/components/widgets/FlashMessage';
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';
import NativeVersionSetup from '../services/nativeVersionSetup';

export default {
  components: {
    ApiError,
    ConnectionError,
    FlashMessage,
  },
  mixins: [ErrorMessageMixin],
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.header.headerText} - ${this.$t('appTitle')}`,
      script: [
        {
          src: this.$env.ANALYTICS_SCRIPT_URL,
        },
      ],
    };
  },
  computed: {
    showErrorMessageContainer() {
      return this.hasConnectionError() || this.hasApiError();
    },
  },
  mounted() {
    NativeVersionSetup(this.$store, this.$route);
  },
  created() {
    const { source } = this.$route.query;
    if (Sources.isNative(source)) {
      this.$store.dispatch('device/updateIsNativeApp', true);
      this.$store.dispatch('device/setSourceDevice', source);
    }
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
