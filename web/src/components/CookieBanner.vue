<template>
  <div v-if="showCookieBanner" :class="$style['cookie-banner']">
    <div :class="$style['cookie-banner-panel']" data-purpose="cookie-banner">
      <div :class="$style['cookie-caption']">
        <p><span>{{ $t('cookieBanner.caption.line1') }} </span>
          <a :class="$style['nhsuk-action-link__link']" :href="cookieBannerUrl" target="_blank">
            {{ $t('cookieBanner.caption.linkText') }}
          </a>
        </p>
      </div>
      <div :class="$style['cookie-close']">
        <no-js-form :value="formData">
          <generic-button aria-label="Close"
                          @click.prevent="onCookieBannerClicked">
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
    this.$store.dispatch('cookieBanner/sync');
  },
  methods: {
    onCookieBannerClicked() {
      this.$store.dispatch('cookieBanner/acknowledge');
    },
  },
};
</script>
<style lang="scss" module scoped>
  @import '../style/colours';
  @import '../style/textstyles';
  @import '../style/webshared';
  @import '../style/screensizes';

  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/action-link/action-link';

  .cookie-banner {
    background: $nhs_blue_med;
    word-wrap: break-word;

    .cookie-caption {
      width: 90%;
      padding: 1.2em;
      display: inline-block;
      a, p {
        color: #fff;
        //padding: 1.2em 0 0 20px;
        font-size: 0.813em;
        font-weight: 600;
        text-align: left;
        font-family: $default;
      }

      a {
        padding-left: 0;
        text-decoration: underline;
      }

      a:hover {
        text-decoration: none;
      }
    }

    .cookie-close {
      width: 10%;
      display: inline-block;
      button {
        position: absolute;
        padding: 0 20px 40px 20px;
        top: 5px;
        right: 20px;
        cursor: pointer;
        background: $nhs_blue_med url(~assets/icon-close-das.gif) center no-repeat;
        width: 11px;
        height: 11px;
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

        &:focus, &:hover {
          box-shadow: 0 0 0 4px $focus_highlight;
          outline: none;
        }
      }
    }

    .cookie-banner-panel {
      @include main-container-width;
      position: relative;
    }

    .close-caption {
      display: none;
    }


    @include tablet-and-above {
      .cookie-banner-panel {
        margin: 0 2em;
      }
    }

    @include desktop {
      .cookie-banner-panel  {
        margin: 0 auto;
      }
    }
  }
</style>
