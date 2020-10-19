<template>
  <div v-if="showTemplate">
    <h2 data-purpose="greeting" data-hj-suppress class="nhsuk-u-margin-bottom-0">
      {{ greetingMessage }}
    </h2>
    <welcome-section v-if="!isProxying" :date-of-birth="currentProfile.dateOfBirth"
                     :nhs-number="currentProfile.nhsNumber" />
    <proxy-welcome-section v-else :proxy-age="proxyAge" :proxy-details="currentProfile"/>
    <public-health-notification
      v-for="publicHealthNotification in publicHealthNotifications"
      :id="`public-health-notification-${publicHealthNotification.id}`"
      :key="`public-health-notification-${publicHealthNotification.id}`"
      :type="publicHealthNotification.type"
      :urgency="publicHealthNotification.urgency"
      :title="publicHealthNotification.title"
      :body="publicHealthNotification.body"/>
    <biometric-banner v-if="!isProxying" />
    <navigation-list-menu v-if="!isProxying"
                          :has-message-indicator="hasUnreadMessages"/>
    <proof-level-uplift-banner v-if="!isProofLevel9" id="upliftBlueBanner"/>
  </div>
</template>

<script>
import BiometricBanner from '@/components/widgets/BiometricBanner';
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import get from 'lodash/fp/get';
import NavigationListMenu from '@/components/NavigationListMenu';
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';
import ProxyWelcomeSection from '@/components/ProxyWelcomeSection';
import PublicHealthNotification from '@/components/widgets/PublicHealthNotification';
import WelcomeSection from '@/components/WelcomeSection';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'IndexPage',
  components: {
    BiometricBanner,
    NavigationListMenu,
    ProofLevelUpliftBanner,
    ProxyWelcomeSection,
    PublicHealthNotification,
    WelcomeSection,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  data() {
    return {
      publicHealthNotifications: get('publicHealthNotifications', this.$store.state.serviceJourneyRules.rules.homeScreen),
      hasUnreadMessages: false,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
    };
  },
  computed: {
    currentProfile() {
      if (this.isProxying) {
        return this.$store.state.linkedAccounts.actingAsUser;
      }
      return this.$store.getters['session/currentProfile'];
    },
    greetingMessage() {
      const message = this.$t('home.welcome');
      if (this.isProxying) {
        const { fullName } = this.currentProfile;
        return `${message}, ${fullName}`;
      }
      const { name } = this.currentProfile;
      return `${message}, ${name}`;
    },
    proxyAge() {
      return this.getDisplayedAgeText(this.currentProfile);
    },
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
    gpMessagesEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
  },
  async mounted() {
    const params = { ignoreError: true };
    if (this.gpMessagesEnabled) {
      await this.$store.dispatch('gpMessages/loadMessages', params);
    }

    if (this.appMessagingEnabled) {
      await this.$store.dispatch('messaging/load', params);
    }

    this.hasUnreadMessages =
      this.$store.state.gpMessages.hasUnread ||
      this.$store.state.messaging.hasUnread;

    window.scrollTo(0, 0);
  },
};
</script>
