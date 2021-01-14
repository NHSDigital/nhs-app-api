<template>
  <div v-if="showCookieBanner" :class="$style['nhsuk-cookie-banner']">
    <div :class="$style['cookie-banner-panel']">
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div data-purpose="cookie-banner" class="nhsuk-grid-column-full nhsuk-u-padding-bottom-3">
            <div :class="$style['cookie-close']">
              <generic-button id="btn_closeCookieBanner" aria-label="Close"
                              @click.prevent="onCookieBannerClicked"/>
            </div>
            <p>{{ $t('navigation.header.weHavePutCookiesOnYourDevice') }}</p>
            <p>
              {{ $t('navigation.header.weWillNotUseAnyOtherCookiesUnlessYouTurnThemOn') }}
              <a :href="cookieBannerUrl"
                 target="_blank"
                 rel="noopener noreferrer">
                {{ $t('navigation.header.cookiesPolicy') }}</a>.
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import {
  COOKIES_BANNER_URL,
} from '@/router/externalLinks';

export default {
  name: 'CookieBanner',
  components: {
    GenericButton,
  },
  data() {
    return {
      cookieBannerUrl: COOKIES_BANNER_URL,
    };
  },
  computed: {
    showCookieBanner() {
      return (
        !this.$store.state.cookieBanner.acknowledged &&
        !sessionStorage.getItem('hasAcknowledgedCookies') &&
        !this.$store.state.device.isNativeApp
      );
    },
  },
  methods: {
    onCookieBannerClicked() {
      this.$store.dispatch('cookieBanner/acknowledge');
      sessionStorage.setItem('hasAcknowledgedCookies', true);
    },
  },
};
</script>

<style lang="scss" module scoped>
  @import "@/style/custom/cookie-banner";
</style>
