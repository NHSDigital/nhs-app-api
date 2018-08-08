<template>
  <div v-if="showTemplate" class="content">
    <main :class="$style['panel-title']">
      <div>
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
      </div>
      <floating-button-bottom id="signout-button" :button-classes="['gray']" @on-click="signout()">
        {{ $t('signOutButton.signOut') }}
      </floating-button-bottom>
    </main>
  </div>
</template>


<script>

/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom.';

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
      this.$store.dispatch('auth/logout', '/login');
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";


  ul.list-menu li a{
    box-sizing: border-box;
    background: transparent url('~/assets/icon_arrow_left.svg') no-repeat center right;
    background-position: right 1em center;
    position: relative;
    margin-bottom: 0em;
    margin-left: 0em;
    padding: 1em 3em 0.250em 1em;
    border-bottom: 1px #D8DDE0 solid;
    padding-left: 16px;
    display: block;
    font-weight: 700;
    font-size: 16px;
    line-height: 1.5em;
    color: #005EB8;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    text-decoration: none;
  }

  menu li h2, .form h2, .panel-title h2 {
    font-family: "FrutigerLTW01-65Bold", Arial, sans-serif;
    font-weight: 700;
    font-size: 1.375em;
    line-height: 1.375em;
    color: #425563;
    padding-bottom: 0.2em;
    padding-top: 0.5em;
    margin-bottom: 0;
    margin-left: 15.4px;
  }

  main.content.nopadding {
    padding: 3.125em 3em 3.125em 0em;
  }
</style>

