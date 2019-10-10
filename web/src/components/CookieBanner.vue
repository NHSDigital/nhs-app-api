<template>
  <div v-if="showCookieBanner" :class="$style['nhsuk-cookie-banner']">
    <div data-purpose='cookie-banner'
         :class="$style['cookie-banner-panel']" style="display: block;">
      <div class="nhsuk-width-container">
        <p :class="$style['cookie-banner__message']">
          <span>{{ $t('cookieBanner.caption.line1') }} </span>
          <a :href="cookieBannerUrl" target="_blank">
            {{ $t('cookieBanner.caption.linkText') }}
          </a></p>
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

  .nhsuk-cookie-banner {
    background: $nhs_blue_med;

    @include fromTablet {
      .cookie-banner-panel {
        margin: 0 2em;
      }
    }

    @include fromDesktop {
      .cookie-banner-panel  {
        margin: 0 auto;
      }
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
  .close-caption {
    display: none;
  }
  a {
    display: inline-block;
    margin-top: -1px;
    color: $white;
  }
  .nhsuk-cookie-banner .cookie-banner-panel, .cookie-banner__message a {
    color: $white;
    position: relative;
  }

  .cookie-banner-panel {
    max-width: 960px;
    position: relative;
  }

  .cookie-banner__message {
    width: 90%;
    margin-bottom:-12px;
    padding-top:10px;
  }
</style>
