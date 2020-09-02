<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate" >
    <p class="nhsuk-body nhsuk-u-padding-bottom-3">{{ $t('myAccount.cookies.p1') }}</p>
    <p class="nhsuk-body nhsuk-u-padding-bottom-3">{{ $t('myAccount.cookies.p2') }}</p>

    <menu-item-list data-purpose="cookie-policy">
      <menu-item :id="'cookies-policy'"
                 :key="'cookies-policy'"
                 header-tag="h2"
                 :target="cookieLink"
                 :href="cookieLink"
                 :text="$t('myAccount.cookiesPolicy')"
                 :aria-label="$t('myAccount.cookiesPolicy')"/>
    </menu-item-list>

    <labelled-toggle
      v-model="registered"
      data-purpose="cookie-toggle"
      checkbox-id="allow_cookies"
      :is-waiting="false"
      :label="$t('myAccount.cookies.toggleLabel')"
      :hint-text="$t('myAccount.cookies.toggleHint')"/>

    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.back'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import { ACCOUNT_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import LabelledToggle from '@/components/widgets/LabelledToggle';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  COOKIES_POLICY_URL,
} from '@/router/externalLinks';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    LabelledToggle,
    MenuItem,
    MenuItemList,
  },

  data() {
    return {
      backPath: ACCOUNT_PATH,
      cookieLink: COOKIES_POLICY_URL,
      dataToggle: this.registered,
    };
  },

  computed: {
    registered: {
      get() {
        return this.$store.state.termsAndConditions.analyticsCookieAccepted;
      },
      async set() {
        const userConsent = !!this.$store.state.termsAndConditions.analyticsCookieAccepted;
        await this.removeFromCosmos(!userConsent);
        if (userConsent) {
          await this.removeCookies();
        }
        this.dataToggle = !this.dataToggle;
        window.location.reload(true);
      },
    },
    toggleLabel() {
      return this.$t('myAccount.cookies.toggleLabel') + this.$t('myAccount.cookies.toggleHint');
    },
  },

  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath, null);
    },
    async removeFromCosmos(consent) {
      const analyticsCookieConsent = {
        analyticsCookieAccepted: consent,
      };
      await this.$store.dispatch('termsAndConditions/toggleAnalyticsCookieConsent', { analyticsCookieConsent });
    },
    async removeCookies() {
      const { hostname } = window.location;
      const hosts = hostname.split('.');
      let subDomain = '';
      for (let i = hosts.length - 1; i >= 0; i -= 1) {
        subDomain = `.${hosts[i]}${subDomain}`;
        this.removeCookie(subDomain);
      }
    },
    removeCookie(subDomain) {
      const subDomainString = `domain=${subDomain};`;
      const expiresString = 'expires=Thu, 01 Jan 1970 00:00:01 UTC;';
      const pathString = 'path=/';
      const whitelistedPrefix = 'nhso.';
      document.cookie.split(';').forEach((cookie) => {
        if (!cookie.trim().toLowerCase().startsWith(whitelistedPrefix)) {
          const cookieName = cookie.trim().split('=')[0];
          const cookieString = `${cookieName}=;`;
          const fullLineString = cookieString + subDomainString + expiresString + pathString;
          document.cookie = fullLineString;
        }
      });
    },
  },
};
</script>
