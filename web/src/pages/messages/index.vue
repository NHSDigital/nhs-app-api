<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list v-if="hasAnyAccess">
        <menu-item v-if="gpMessagesEnabled"
                   id="btn_im1_messaging"
                   data-sid="im1-messaging-list-item"
                   header-tag="h2"
                   :href="im1MessagingPath"
                   :show-indicator="hasUnreadGPMessages"
                   :text="$t('messages.hub.gpSurgeryMessages')"
                   :description="$t('messages.hub.sendOrViewMessagesFromYourSurgery')"
                   :aria-label="ariaLabelGpMessages()"
                   :click-func="navigateToGpMessages"/>
        <third-party-jump-off-button v-if="engageEnabled"
                                     id="btn_engage_messages"
                                     provider-id="engage"
                                     :provider-configuration="thirdPartyProvider.engage.messages" />
        <third-party-jump-off-button v-if="pkbEnabled"
                                     id="btn_pkb_messages_and_consultations"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider.pkb.messages" />
        <third-party-jump-off-button v-if="pkbCieEnabled"
                                     id="btn_pkb_cie_messages_and_consultations"
                                     provider-id="pkb"
                                     :provider-configuration="thirdPartyProvider.pkb.messagesCie" />
        <third-party-jump-off-button
          v-if="testProviderEnabled"
          id="btn_test_silver_messages"
          provider-id="silver-third-party-api-test"
          :provider-configuration="thirdPartyProvider.testProvider.messages" />
        <menu-item v-if="appMessagingSjrEnabled"
                   id="btn_appMessaging"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="appMessagingPath"
                   :show-indicator="hasUnreadAppMessages"
                   :text="$t('messages.hub.healthInformationAndUpdates')"
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
      hasPkbCieMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
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
    };
  },
  computed: {
    hasAnyAccess() {
      return this.gpMessagesEnabled ||
        this.appMessagingSjrEnabled ||
        this.engageEnabled ||
        this.pkbEnabled ||
        this.pkbCieEnabled ||
        this.testProviderEnabled;
    },
    onlyAppMessagingEnabled() {
      return !this.gpMessagesEnabled && this.appMessagingSjrEnabled &&
      !this.engageEnabled && !this.pkbEnabled && !this.testProviderEnabled;
    },
    gpMessagesEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    engageEnabled() {
      return this.hasEngageMessages && !this.isProxying && this.isProofLevel9;
    },
    pkbEnabled() {
      return this.hasPkbMessages && !this.isProxying && this.isProofLevel9;
    },
    pkbCieEnabled() {
      return this.hasPkbCieMessages && !this.isProxying && this.isProofLevel9;
    },
    testProviderEnabled() {
      return this.hasTestProviderMessages && this.isProofLevel9;
    },
  },
  async mounted() {
    const params = { ignoreError: true };

    if (this.gpMessagesEnabled) {
      await this.$store.dispatch('gpMessages/loadMessages', params);
    }

    if (this.appMessagingSjrEnabled) {
      await this.$store.dispatch('messaging/load', params);
    }

    this.hasUnreadGPMessages = this.$store.state.gpMessages.hasUnread;
    this.hasUnreadAppMessages = this.$store.state.messaging.hasUnread;
  },
  methods: {
    navigateToGpMessages() {
      redirectTo(this, this.im1MessagingPath);
    },
    navigateToAppMessages() {
      redirectTo(this, this.appMessagingPath);
    },
    ariaLabelGpMessages() {
      const { hasUnreadGPMessages } = this;
      return (hasUnreadGPMessages) ?
        `${this.$t('messages.hub.gpSurgeryMessages')}
          ${this.$t('messages.hub.sendOrViewMessagesFromYourSurgery')}.
          ${this.$t('messages.youHaveUnreadMessages')}`
        : `${this.$t('messages.hub.gpSurgeryMessages')}
          ${this.$t('messages.hub.sendOrViewMessagesFromYourSurgery')}.`;
    },
    ariaLabelAppMessages() {
      const { hasUnreadAppMessages } = this;
      return (hasUnreadAppMessages) ?
        `${this.$t('messages.hub.healthInformationAndUpdates')}
          ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.
          ${this.$t('messages.youHaveUnreadMessages')}`
        : `${this.$t('messages.hub.healthInformationAndUpdates')}
          ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.`;
    },
  },
};
</script>
