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
            <p>{{ $t('cookieBanner.caption.line1') }}</p>
            <p>
              {{ $t('cookieBanner.caption.line2') }}
              <a :href="cookieBannerUrl"
                 target="_blank"
                 rel="noopener noreferrer">
                {{ $t('cookieBanner.caption.linkText') }}</a>.
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
  computed: {
    cookieBannerUrl() {
      return COOKIES_BANNER_URL;
    },
    showCookieBanner() {
      return (
        (this.$route.query || {}).acknowledged !== 'true' &&
        !this.$store.state.device.isNativeApp
      );
    },
  },
  methods: {
    onCookieBannerClicked() {
      this.$router.push({ query: { ...(this.$route.query || {}), acknowledged: 'true' } });
    },
  },
};
</script>

<style>
.cookieBannerDisplay {
  display: none;
}
</style>

<style lang="scss" module scoped>
@import "../style/colours";
@import "../style/textstyles";
@import "../style/webshared";
@import "../style/screensizes";

@import "~nhsuk-frontend/packages/core/all.scss";

.nhsuk-cookie-banner {
  background: white;
  position: relative;
  box-shadow: 0 0 4px 0 #212b32;
  padding: 24px 0 0px;
  width: 100%;
  z-index: 1;
}

.cookie-close {
  margin-top: -20px;
  float: right;
  button {
    padding: 0px 15px 30px 15px;
    top: 0px;
    right: 5px;
    cursor: pointer;
    background: white url(~@/assets/icon-close.svg) center no-repeat;
    border: none;

    -webkit-box-shadow: none;
    -moz-box-shadow: none;
    box-shadow: none;
    outline: none;

    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -khtml-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;

    &:focus,
    &:hover {
      box-shadow: 0 0 0 4px $focus_highlight;
      outline: none;
    }
  }
}
a {
  display: inline-block;
  padding-bottom: 3px;
}

p {
  padding-right: 10px;
}
</style>
