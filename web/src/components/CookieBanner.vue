<template>
  <div v-if="showCookieBanner" :class="$style['cookie-banner']">
    <div :class="$style['cookie-banner-panel']">
      <div :class="$style['cookie-caption']">
        <p>{{ $t('cookieBanner.caption.line1') }}
          <a :href="cookieBannerUrl" tabindex="-2" target="_blank">
            {{ $t('cookieBanner.caption.linkText') }}
          </a>
        </p>
      </div>
      <div :class="$style['cookie-close']">
        <no-js-form :value="formData">
          <generic-button @click.prevent="onCookieBannerClicked">
            <span :class="$style['close-caption']">Close</span>
          </generic-button>
        </no-js-form>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';

export default {
  name: 'CookieBanner',
  components: {
    GenericButton,
    NoJsForm,
  },
  computed: {
    formData() {
      return {
        cookieBanner: {
          acknowledged: true,
        },
      };
    },
    showCookieBanner() {
      return !this.$store.state.cookieBanner.acknowledged && !this.$store.state.device.isNativeApp;
    },
    cookieBannerUrl() {
      return this.$store.app.$env.COOKIES_BANNER_URL;
    },
  },
  created() {
    if (process.server) {
      this.$store.dispatch('cookieBanner/sync');
    }
  },
  methods: {
    onCookieBannerClicked() {
      this.$store.dispatch('cookieBanner/acknowledge');
    },
  },
};
</script>
<style lang="scss" module scoped>
  @import "../style/cookiebanner";


</style>
