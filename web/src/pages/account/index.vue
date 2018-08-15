<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <h2>About us</h2>
    <ul :class="$style['list-menu']">
      <li>
        <a id="btn_terms" :href="termsAndConditionsURL" target="_blank">
          {{ $t('myAccount.termsAndConditions') }}
        </a>
      </li>
      <li>
        <a id="btn_privacy" :href="privacyPolicyURL" target="_blank">
          {{ $t('myAccount.privacyPolicy') }}
        </a>
      </li>
      <li>
        <a id="btn_cookies" :href="cookiesPolicyURL" target="_blank">
          {{ $t('myAccount.cookiesPolicy') }}
        </a>
      </li>
      <li>
        <a id="btn_openSource" :href="openSourceLicensesURL" target="_blank">
          {{ $t('myAccount.openSourceLicenses') }}
        </a>
      </li>
      <li>
        <a id="btn_help" :href="helpAndSupportURL" target="_blank">
          {{ $t('myAccount.helpAndSupport') }}
        </a>
      </li>
    </ul>
    <floating-button-bottom id="signout-button" :button-classes="['grey']" @on-click="signout()">
      {{ $t('signOutButton.signOut') }}
    </floating-button-bottom>
  </div>
</template>


<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';

export default {
  components: {
    FloatingButtonBottom,
  },
  data() {
    return {
      termsAndConditionsURL: process.env.TERMS_AND_CONDITIONS_URL,
      privacyPolicyURL: process.env.PRIVACY_POLICY_URL,
      cookiesPolicyURL: process.env.COOKIES_POLICY_URL,
      openSourceLicensesURL: process.env.OPEN_SOURCE_LICENSES_URL,
      helpAndSupportURL: process.env.HELP_AND_SUPPORT_URL,
    };
  },
  methods: {
    signout() {
      this.$store.dispatch('auth/logout');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/listmenu";
.no-padding {
  h2 {
    margin-left: 0.7em;
    margin-top: 0.5em;
  }
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
}

</style>
