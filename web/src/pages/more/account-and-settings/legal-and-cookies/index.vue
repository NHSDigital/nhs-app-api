<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate">
    <menu-item-list data-purpose="legal-and-cookies-menu">
      <menu-item id="'manage-cookies'"
                 :key="'manage-cookies'"
                 header-tag="h2"
                 :target="manageCookiesPath"
                 :href="manageCookiesPath"
                 :text="$t('legalAndCookies.cookies.cookies')"
                 :aria-label="$t('legalAndCookies.cookies.cookies')"
                 :click-func="goToUrl"
                 :click-param="manageCookiesPath"/>
      <menu-item id="'terms-of-use'"
                 :key="'terms-of-use'"
                 header-tag="h2"
                 :target="termsOfUsePath"
                 :href="termsOfUsePath"
                 :text="$t('legalAndCookies.termsOfUse')"
                 :aria-label="$t('legalAndCookies.termsOfUse')"/>
      <menu-item id="'privacy-policy'"
                 :key="'privacy-policy'"
                 header-tag="h2"
                 :target="privacyPolicyPath"
                 :href="privacyPolicyPath"
                 :text="$t('legalAndCookies.privacyPolicy')"
                 :aria-label="$t('legalAndCookies.privacyPolicy')"/>
      <menu-item id="'accessibility-statement'"
                 :key="'accessibility-statement'"
                 header-tag="h2"
                 :target="accessibilityStatementPath"
                 :href="accessibilityStatementPath"
                 :text="$t('legalAndCookies.accessibilityStatement')"
                 :aria-label="$t('legalAndCookies.accessibilityStatement')"/>
      <menu-item :id="'open-source-licences'"
                 :key="'open-source-licences'"
                 header-tag="h2"
                 :target="openSourceLicencesPath"
                 :href="openSourceLicencesPath"
                 :text="$t('legalAndCookies.openSourceLicences')"
                 :aria-label="$t('legalAndCookies.openSourceLicences')"/>
    </menu-item-list>

    <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                               data-purpose="back-link"
                               :path="backPath"
                               :button-text="'generic.back'"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { MORE_ACCOUNTANDSETTINGS_PATH, MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_PATH } from '@/router/paths';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    MenuItem,
    MenuItemList,
  },
  data() {
    return {
      manageCookiesPath: MORE_ACCOUNTANDSETTINGS_LEGALANDCOOKIES_MANAGECOOKIES_PATH,
      termsOfUsePath: this.$store.$env.TERMS_AND_CONDITIONS_URL,
      privacyPolicyPath: this.$store.$env.PRIVACY_POLICY_URL,
      accessibilityStatementPath: this.$store.$env.ACCESSIBILITY_STATEMENT_URL,
      openSourceLicencesPath: this.$store.$env.OPEN_SOURCE_LICENCES_URL,
      backPath: MORE_ACCOUNTANDSETTINGS_PATH,
    };
  },
};
</script>
