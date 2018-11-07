<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <h2>{{ $t('myAccount.detailsHeading') }}</h2>
    <welcome-section :name="$store.state.session.user"
                     :date-of-birth="$store.state.session.dateOfBirth"
                     :nhs-number="$store.state.session.nhsNumber" />
    <div v-if="shouldShowBiometrics">
      <h2>{{ $t('myAccount.accountSettingsHeading') }}</h2>
      <ul :class="$style['list-menu']">
        <li @click="goToBiometrics()">
          <analytics-tracked-tag id="btn_fingerprintId"
                                 :text="$t('myAccount.fingerprintID')"
                                 tag="a">
            {{ $t('myAccount.fingerprintID') }}
          </analytics-tracked-tag>
        </li>
      </ul>
    </div>
    <h2>{{ $t('myAccount.aboutUsHeading') }}</h2>
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
      <form action="account/signout" method="post">
        <floating-button-bottom id="signout-button" :button-classes="['grey']" type="submit"
                                @click="signout">
          {{ $t('signOutButton.signOut') }}
        </floating-button-bottom>
      </form>
    </analytics-tracked-tag>
    <p>
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </p>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import WelcomeSection from '@/components/WelcomeSection';
import NativeCallbacks from '@/services/native-app';

export default {
  components: {
    AnalyticsTrackedTag,
    FloatingButtonBottom,
    WelcomeSection,
  },
  computed: {
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.$store.state.device.source === 'android';
    },
  },
  methods: {
    signout(event) {
      event.preventDefault();
      this.$store.dispatch('auth/logout');
    },
    goToBiometrics() {
      NativeCallbacks.goToBiometrics();
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
  p, h2 {
    margin-left: 0.7em;
    margin-top: 0.5em;
  }
}
</style>
