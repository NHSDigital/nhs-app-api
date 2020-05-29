<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <menu-item-list v-if="hasAnyAccess">
        <menu-item v-if="gpMessagesEnabled"
                   id="btn_im1_messaging"
                   data-sid="im1-messaging-list-item"
                   header-tag="h2"
                   :href="im1MessagingPath"
                   :has-unread-messages="hasUnreadGPMessages"
                   :text="$t('messagesHub.im1Messaging.subheader')"
                   :description="$t('messagesHub.im1Messaging.body')"
                   :aria-label="ariaLabel('im1Messaging')"
                   :click-func="navigate"/>
        <third-party-jump-off-button v-if="pkbEnabled"
                                     id="btn_pkb_messages_and_consultations"
                                     provider-id="pkb"
                                     :jump-off-type="thirdPartyProvider.pkb.messages.type"
                                     :redirect-path="thirdPartyProvider
                                       .pkb.messages.redirectPath" />
        <third-party-jump-off-button v-if="testProviderEnabled"
                                     id="btn_test_silver_messages"
                                     provider-id="silver-third-party-api-test"
                                     :jump-off-type="thirdPartyProvider.testProvider.messages.type"
                                     :redirect-path="thirdPartyProvider
                                       .testProvider.messages.redirectPath" />
        <menu-item v-if="appMessagingSjrEnabled"
                   id="btn_appMessaging"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="appMessagingPath"
                   :has-unread-messages="hasUnreadAppMessages"
                   :text="$t('messagesHub.appMessaging.subHeader')"
                   :description="$t('messagesHub.appMessaging.body')"
                   :click-func="navigate"
                   :aria-label="ariaLabel('appMessaging')"/>
      </menu-item-list>
      <p v-else data-purpose="no-messages-available">
        {{ $t('messagesHub.noMessages') }}
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
import { GP_MESSAGES, HEALTH_INFORMATION_UPDATES } from '@/lib/routes';

export default {
  name: 'Messages',
  layout: 'nhsuk-layout',
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
      im1MessagingPath: GP_MESSAGES.path,
      appMessagingPath: HEALTH_INFORMATION_UPDATES.path,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
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
    };
  },
  computed: {
    hasAnyAccess() {
      return this.gpMessagesEnabled ||
        this.appMessagingSjrEnabled ||
        this.pkbEnabled ||
        this.testProviderEnabled;
    },
    gpMessagesEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    pkbEnabled() {
      return this.hasPkbMessages && !this.isProxying && this.isProofLevel9;
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
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
    ariaLabel(type) {
      const { hasUnreadGPMessages, hasUnreadAppMessages } = this;
      return (hasUnreadGPMessages || hasUnreadAppMessages) ?
        `${this.$t(`messagesHub.${type}.subheader`)}
          ${this.$t(`messagesHub.${type}.body`)}.
          ${this.$t('messagesHub.unreadMessages')}`
        : `${this.$t(`messagesHub.${type}.subheader`)}
          ${this.$t(`messagesHub.${type}.body`)}.`;
    },
  },
};
</script>
