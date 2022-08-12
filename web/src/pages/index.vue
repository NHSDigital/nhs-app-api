<template>
  <div v-if="showTemplate">
    <public-health-notification
      v-for="publicHealthNotification in publicHealthNotifications"
      :id="`public-health-notification-${publicHealthNotification.id}`"
      :key="`public-health-notification-${publicHealthNotification.id}`"
      :type="publicHealthNotification.type"
      :urgency="publicHealthNotification.urgency"
      :title="publicHealthNotification.title"
      :body="publicHealthNotification.body"/>
    <navigation-list-menu v-if="!isProxying" />
    <nhs-arrow-banner id="nhs-arrow-right-icon"
                      class="nhsuk-u-margin-top-3"
                      :banner-text="$t('home.findNearestService')"
                      :click-action="findServicesUrl"/>
    <proof-level-uplift-banner v-if="!isProofLevel9" id="upliftBlueBanner"/>
  </div>
</template>

<script>
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import get from 'lodash/fp/get';
import NavigationListMenu from '@/components/NavigationListMenu';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner.vue';
import ProofLevelUpliftBanner from '@/components/uplift/ProofLevelUpliftBanner';
import ProxyWelcomeSection from '@/components/ProxyWelcomeSection';
import PublicHealthNotification from '@/components/widgets/PublicHealthNotification';
import WelcomeSection from '@/components/WelcomeSection';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'IndexPage',
  components: {
    NavigationListMenu,
    ProofLevelUpliftBanner,
    ProxyWelcomeSection,
    PublicHealthNotification,
    WelcomeSection,
    NhsArrowBanner,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  data() {
    return {
      publicHealthNotifications: get('publicHealthNotifications', this.$store.state.serviceJourneyRules.rules.homeScreen),
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      findServicesUrl: this.$store.$env.FIND_NEAREST_SERVICE_URL,
    };
  },
  computed: {
    currentProfile() {
      if (this.isProxying) {
        return this.$store.state.linkedAccounts.actingAsUser;
      }
      return this.$store.getters['session/currentProfile'];
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
    if (this.$store.$env.GP_SESSION_ON_DEMAND_ENABLED === false) {
      if (this.gpMessagesEnabled) {
        await this.$store.dispatch('gpMessages/loadMessages', { ignoreError: true });
      }
    }

    if (this.appMessagingEnabled) {
      await this.$store.dispatch('messaging/load', { summary: true });
    }

    window.scrollTo(0, 0);

    this.$store.dispatch('device/pageLoadComplete');
  },
};
</script>
