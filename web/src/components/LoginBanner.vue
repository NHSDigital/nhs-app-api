<template>
  <div :class="dynamicStyle('symptomBanner')">
    <h2>{{ $t('loginBanner.alreadyHaveNHSLogin') }}</h2>
    <generic-button id="btn_has_login"
                    :class="[$style.white, ...dynamicStyle('button')]"
                    @click.prevent="hasAnAccountLinkClicked()">
      <NHS-Arrow-Circle />
      {{ $t('loginBanner.loginLink') }}
    </generic-button>
  </div>
</template>
<script>

import GenericButton from '@/components/widgets/GenericButton';
import NHSArrowCircle from '@/components/icons/NHSArrowCircle';
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';
import { LOGIN } from '@/lib/routes';
import { setCookie } from '@/lib/cookie-manager';
import NativeCallbacks from '@/services/native-app';
import moment from 'moment';


export default {
  name: 'LoginBanner',
  components: {
    GenericButton,
    NHSArrowCircle,
  },
  methods: {
    hasAnAccountLinkClicked() {
      setCookie({
        cookies: this.$store.app.$cookies,
        key: 'BetaCookie',
        value: {
          Skipped: true,
        },
        options: {
          maxAge: moment.duration(1, 'y').asSeconds(),
          secure: this.$store.app.$env.SECURE_COOKIES,
        },
      });
      if (process.client) {
        NativeCallbacks.storeBetaCookie();
      }

      this.goToUrl(LOGIN.path);
    },
    dynamicStyle(...args) {
      return getDynamicStyle(this, args, exchangeStyle({ button: 'btn_home_symptoms-desktop' }));
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../style/buttons';
  @import "../style/elements";
  @import "../style/home";


  .button-desktop {
    outline: none;
  }

  .btn_home_symptoms-desktop {
    @include default_text;
    color: $nhs_blue;
    font-size: 1em;
    line-height: 1em;
    font-weight: bold;
    text-decoration: none;
    vertical-align: middle;
    cursor: pointer;
    display: inline-block;
    border: none;
    background: none;
    margin: 0;
    outline: none;

    h2 {
      color: $nhs_blue;
    }

    &:hover {
      background: $hover_highlight;
      box-shadow: 0 0 0 4px $hover_highlight;
      color: $black;
      text-decoration: none;
    }

    &:focus {
      background: $focus_highlight;
      box-shadow: 0 0 0 4px $focus_highlight;
      color: $black;
      outline: 0;
    }

    &:visited {
      color: #330072;
    }
  }

</style>
