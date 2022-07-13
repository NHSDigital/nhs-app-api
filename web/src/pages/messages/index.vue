<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list v-if="hasAnyAccess">
        <menu-item v-if="showGpMessagesLink"
                   id="btn_im1_messaging"
                   data-sid="im1-messaging-list-item"
                   header-tag="h2"
                   :href="im1MessagingPath"
                   :show-indicator="hasUnreadGPMessages"
                   :text="$t('messages.hub.gpSurgeryMessaging')"
                   :description="$t('messages.hub.sendOrViewMessagesFromYourSurgery')"
                   :aria-label="ariaLabelGpMessages()"
                   :click-func="navigateToGpMessages"/>
        <third-party-jump-off-button
          v-if="substraktEnabled"
          id="btn_substrakt_messages"
          provider-id="substraktPatientPack"
          :provider-configuration="thirdPartyProvider.substraktPatientPack.messages" />
        <third-party-jump-off-button v-if="accurxEnabled"
                                     id="btn_accurx_messages"
                                     provider-id="accurx"
                                     :provider-configuration="thirdPartyProvider.accurx.messages"/>
        <third-party-jump-off-button v-if="engageEnabled"
                                     id="btn_engage_messages"
                                     provider-id="engage"
                                     :provider-configuration="thirdPartyProvider.engage.messages" />
        <third-party-jump-off-button v-if="pkbEnabled"
                                     id="btn_pkb_messages_and_consultations"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider.pkb.messages" />
        <third-party-jump-off-button
          v-if="testProviderEnabled"
          id="btn_test_silver_messages"
          provider-id="silver_third_party_api_test"
          :provider-configuration="thirdPartyProvider['silver-third-party-api-test'].messages" />
        <menu-item v-if="appMessagingSjrEnabled"
                   id="btn_appMessaging"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="appMessagingPath"
                   :show-indicator="hasUnreadAppMessages"
                   :text="$t('messages.hub.yourHealthServiceMessages')"
                   :description="$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')"
                   :click-func="navigateToAppMessages"
                   :aria-label="ariaLabelAppMessages()"/>
      </menu-item-list>
      <p v-else data-purpose="no-messages-available">
        {{ $t('messages.youHaveNoMessages') }}
      </p>
    </div>
  </div>
</template>
<script>
import { get } from 'lodash/fp';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import MenuItemList from '@/components/MenuItemList';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import MenuItem from '@/components/MenuItem';
import { redirectTo } from '@/lib/utils';
import { GP_MESSAGES_PATH, HEALTH_INFORMATION_UPDATES_PATH } from '@/router/paths';

export default {
  name: 'MessagesPage',
  components: {
    MenuItem,
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      hasGpSession: this.$store.state.session.hasGpSession,
      hasUnreadAppMessages: false,
      hasUnreadGPMessages: false,
      isProxying: this.$store.getters['session/isProxying'],
      isProofLevel9: this.$store.getters['session/isProofLevel9'],
      im1MessagingPath: GP_MESSAGES_PATH,
      appMessagingPath: HEALTH_INFORMATION_UPDATES_PATH,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      hasEngageMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'messages',
        },
      }),
      hasPkbMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'messages',
        },
      }),
      hasTestProviderMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'testSilverThirdPartyProvider',
          serviceType: 'messages',
        },
      }),
      hasSubstraktMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'substraktPatientPack',
          serviceType: 'messages',
        },
      }),
      hasAccurxMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'accurx',
          serviceType: 'messages',
        },
      }),
    };
  },
  computed: {
    gpSessionApiError() {
      return this.$store.state.auth.gpSessionError;
    },
    hasAnyAccess() {
      return this.shouldLoadGpMessages ||
        this.appMessagingSjrEnabled ||
        this.engageEnabled ||
        this.pkbEnabled ||
        this.substraktEnabled ||
        this.accurxEnabled ||
        this.testProviderEnabled;
    },
    shouldLoadGpMessages() {
      return this.im1MessagingSjrEnabled && (
        this.$store.state.practiceSettings.im1MessagingEnabled !== false);
    },
    showGpMessagesLink() {
      return this.im1MessagingSjrEnabled && (
        this.$store.state.practiceSettings.im1MessagingEnabled === true);
    },
    engageEnabled() {
      return this.hasEngageMessages && !this.isProxying && this.isProofLevel9;
    },
    pkbEnabled() {
      return this.hasPkbMessages && !this.isProxying && this.isProofLevel9;
    },
    testProviderEnabled() {
      return this.hasTestProviderMessages && this.isProofLevel9;
    },
    substraktEnabled() {
      return this.hasSubstraktMessages && !this.isProxying && this.isProofLevel9;
    },
    accurxEnabled() {
      return this.hasAccurxMessages && !this.isProxying && this.isProofLevel9;
    },
  },
  async mounted() {
    if (this.shouldLoadGpMessages && !this.ignoreGpSessionError()) {
      await this.$store.dispatch('gpMessages/loadMessages', { ignoreError: true });
    }

    if (this.appMessagingSjrEnabled) {
      await this.$store.dispatch('messaging/load', { summary: true });
    }

    this.hasUnreadGPMessages = this.$store.state.gpMessages.hasUnread;
    this.hasUnreadAppMessages = this.$store.state.messaging.hasUnread;
  },
  methods: {
    ignoreGpSessionError() {
      const ignoreGpSessionError = get('gpSessionOnDemand.ignoreError', this.$route.meta);
      return this.gpSessionApiError && ignoreGpSessionError;
    },
    navigateToGpMessages() {
      redirectTo(this, this.im1MessagingPath);
    },
    navigateToAppMessages() {
      redirectTo(this, this.appMessagingPath);
    },
    ariaLabelGpMessages() {
      const { hasUnreadGPMessages } = this;
      return (hasUnreadGPMessages) ?
        `${this.$t('messages.hub.gpSurgeryMessaging')}
          ${this.$t('messages.hub.sendOrViewMessagesFromYourSurgery')}.
          ${this.$t('messages.youHaveUnreadMessages')}`
        : `${this.$t('messages.hub.gpSurgeryMessaging')}
          ${this.$t('messages.hub.sendOrViewMessagesFromYourSurgery')}.`;
    },
    ariaLabelAppMessages() {
      const { hasUnreadAppMessages } = this;
      return (hasUnreadAppMessages) ?
        `${this.$t('messages.hub.yourHealthServiceMessages')}
          ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.
          ${this.$t('messages.youHaveUnreadMessages')}`
        : `${this.$t('messages.hub.yourHealthServiceMessages')}
          ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.`;
    },
  },
};
</script>
