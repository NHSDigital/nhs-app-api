<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate"
       :class="[$style['no-padding'],
                'pull-content', !$store.state.device.isNativeApp && $style.desktopWeb]">
    <h2>{{ $t('myAccount.detailsHeading') }}</h2>
    <div :class="$style.welcomeSectionContainer">
      <welcome-section :name="$store.state.session.user"
                       :date-of-birth="$store.state.session.dateOfBirth"
                       :nhs-number="$store.state.session.nhsNumber" />
    </div>
    <div v-if="shouldShowBiometrics">
      <h2>{{ $t('myAccount.accountSettingsHeading') }}</h2>
      <ul :class="$style['list-menu']">
        <li @click="goToLoginOptions()">
          <analytics-tracked-tag id="btn_passwordOptions"
                                 :text="$t('myAccount.passwordOptions')"
                                 tag="a">
            {{ $t('myAccount.passwordOptions') }}
          </analytics-tracked-tag>
        </li>
      </ul>
    </div>
    <h2>{{ $t('myAccount.aboutUsHeading') }}</h2>
    <ul :class="[$style['list-menu'], $style.myAccountList]">
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_terms" :href="$store.app.$env.TERMS_AND_CONDITIONS_URL"
                               :text="$t('myAccount.termsAndConditions')"
                               tag="a" target="_blank">
          {{ $t('myAccount.termsAndConditions') }}
        </analytics-tracked-tag>
      </li>
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_privacy" :href="$store.app.$env.PRIVACY_POLICY_URL"
                               :text="$t('myAccount.privacyPolicy')"
                               tag="a" target="_blank">
          {{ $t('myAccount.privacyPolicy') }}
        </analytics-tracked-tag>
      </li>
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_cookies" :href="$store.app.$env.COOKIES_POLICY_URL"
                               :text="$t('myAccount.cookiesPolicy')"
                               tag="a" target="_blank">
          {{ $t('myAccount.cookiesPolicy') }}
        </analytics-tracked-tag>
      </li>
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_openSource" :href="$store.app.$env.OPEN_SOURCE_LICENCES_URL"
                               :text="$t('myAccount.openSourceLicences')"
                               tag="a" target="_blank">
          {{ $t('myAccount.openSourceLicences') }}
        </analytics-tracked-tag>
      </li>
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_help" :href="$store.app.$env.HELP_AND_SUPPORT_URL"
                               :text="$t('myAccount.helpAndSupport')"
                               tag="a" target="_blank">
          {{ $t('myAccount.helpAndSupport') }}
        </analytics-tracked-tag>
      </li>
      <li :class="$style.listMenuItem">
        <analytics-tracked-tag id="btn_accessibility"
                               :href="$store.app.$env.ACCESSIBILITY_STATEMENT"
                               :text="$t('myAccount.accessibilityStatement')"
                               tag="a" target="_blank">
          {{ $t('myAccount.accessibilityStatement') }}
        </analytics-tracked-tag>
      </li>
    </ul>
    <p>
      Version {{ this.$store.state.appVersion.webVersion }}
      <span v-if="this.$store.state.appVersion.nativeVersion">
        ({{ this.$store.state.appVersion.nativeVersion }})
      </span>
    </p>
    <p v-if="this.$store.app.$env.CE_MARK_ENABLED">
      <ce-mark-icon/>
    </p>
    <analytics-tracked-tag :text="$t('signOutButton.signOut')"
                           data-purpose="button">
      <form action="account/signout" method="post">
        <floating-button-bottom
          v-if="$store.state.device.isNativeApp"
          id="signout-button" type="submit"
          @click="signout">
          {{ $t('signOutButton.signOut') }}
        </floating-button-bottom>
      </form>
    </analytics-tracked-tag>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import WelcomeSection from '@/components/WelcomeSection';
import NativeCallbacks from '@/services/native-app';
import CeMarkIcon from '@/components/icons/CeMarkIcon';

export default {
  components: {
    AnalyticsTrackedTag,
    FloatingButtonBottom,
    WelcomeSection,
    CeMarkIcon,
  },
  data() {
    return {
      nativeLoginOptionsMethodExists: true,
    };
  },
  computed: {
    shouldShowBiometrics() {
      return this.$env.BIOMETRICS_ENABLED && this.nativeLoginOptionsMethodExists &&
      (this.$store.state.device.source === 'android' || this.$store.state.device.source === 'ios');
    },
  },
  mounted() {
    this.nativeLoginOptionsMethodExists = NativeCallbacks.goToLoginOptionsExists();
  },
  methods: {
    signout(event) {
      event.preventDefault();
      this.$store.dispatch('auth/logout');
    },
    goToLoginOptions() {
      NativeCallbacks.goToLoginOptions();
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
@import "../../style/listmenu";
@import "../../style/colours";
  @import "../../style/webshared";
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

.welcomeSectionContainer{
 margin-left: 1em;
}

.myAccountList {
  @include inner-container-width;

  .listMenuItem {
      font-family: $default-web;
      font-weight: lighter;
      a {
        @extend .focusBorder;
      }
  }
}

</style>
