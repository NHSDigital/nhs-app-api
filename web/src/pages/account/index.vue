<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <h2>Details</h2>
    <welcome-section :name="$store.state.session.user"
                     :date-of-birth="$store.state.session.dateOfBirth"
                     :nhs-number="$store.state.session.nhsNumber" />

    <h2>About us</h2>
    <ul :class="$style['list-menu']">
      <li>
        <analytics-tracked-tag id="btn_terms" :href="$store.app.$env.TERMS_AND_CONDITIONS_URL"
                               :text="$t('myAccount.termsAndConditions')"
                               tag="a" target="_blank">
          {{ $t('myAccount.termsAndConditions') }}
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag id="btn_privacy" :href="$store.app.$env.PRIVACY_POLICY_URL"
                               :text="$t('myAccount.privacyPolicy')"
                               tag="a" target="_blank">
          {{ $t('myAccount.privacyPolicy') }}
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag id="btn_cookies" :href="$store.app.$env.COOKIES_POLICY_URL"
                               :text="$t('myAccount.cookiesPolicy')"
                               tag="a" target="_blank">
          {{ $t('myAccount.cookiesPolicy') }}
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag id="btn_openSource" :href="$store.app.$env.OPEN_SOURCE_LICENCES_URL"
                               :text="$t('myAccount.openSourceLicences')"
                               tag="a" target="_blank">
          {{ $t('myAccount.openSourceLicences') }}
        </analytics-tracked-tag>
      </li>
      <li>
        <analytics-tracked-tag id="btn_help" :href="$store.app.$env.HELP_AND_SUPPORT_URL"
                               :text="$t('myAccount.helpAndSupport')"
                               tag="a" target="_blank">
          {{ $t('myAccount.helpAndSupport') }}
        </analytics-tracked-tag>
      </li>
    </ul>
    <analytics-tracked-tag :text="$t('signOutButton.signOut')" data-purpose="button">
      <floating-button-bottom id="signout-button" :button-classes="['grey']" @click="signout()">
        {{ $t('signOutButton.signOut') }}
      </floating-button-bottom>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import WelcomeSection from '@/components/WelcomeSection';

export default {
  components: {
    AnalyticsTrackedTag,
    FloatingButtonBottom,
    WelcomeSection,
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
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
  padding-bottom : 5em;
  h2 {
    margin-left: 0.7em;
    margin-top: 0.5em;
  }
}

</style>
