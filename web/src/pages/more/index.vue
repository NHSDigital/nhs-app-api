<template>
  <div v-if="showTemplate" id="mainDiv">
    <menu-item-list>
      <menu-item
        v-if="gpMessagingAvailable"
        id="btn_messages"
        header-tag="h2"
        data-purpose="text_link"
        :href="messagesPath"
        :text="$t('messages.messages')"
        :description="$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')"
        :show-indicator="hasUnreadMessages"
        :click-func="navigateToMessages"
        :aria-label="ariaLabel"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import sjrIf from '@/lib/sjrIf';
import { redirectTo } from '@/lib/utils';
import {
  MESSAGES_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
} from '@/router/paths';

export default {
  name: 'MorePage',
  components: {
    MenuItemList,
    MenuItem,
  },
  data() {
    return {
      hasUnreadMessages: false,
      appMessagingPath: HEALTH_INFORMATION_UPDATES_PATH,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      isNativeApp: this.$store.state.device.isNativeApp,
      isProxying: this.$store.getters['session/isProxying'],
      isProofLevel9: this.$store.getters['session/isProofLevel9'],
      gpMessagingAvailable: !this.$store.state.gpMessages.gpMessagingSessionUnavailable,
      messagesPath: MESSAGES_PATH,
    };
  },
  computed: {
    gpMessagesEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    onlyAppMessagesEnabled() {
      return this.appMessagingEnabled
        && !this.gpMessagesEnabled
        && (sjrIf({ $store: this.$store, journey: 'silverIntegrationMessages', disabled: true })
          || this.isProxying || !this.isProofLevel9);
    },
    ariaLabel() {
      if (this.onlyAppMessagesEnabled) {
        return (this.hasUnreadMessages) ?
          `${this.$t('messages.hub.healthInformationAndUpdates')}
            ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.
            ${this.$t('messages.youHaveUnreadMessages')}`
          : `${this.$t('messages.hub.healthInformationAndUpdates')}
            ${this.$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')}.`;
      }
      return (this.hasUnreadMessages) ?
        `${this.$t('messages.messages')}
          ${this.$t('messages.sendOrViewMessagesFromSurgeryOrHealthServices')}.
          ${this.$t('messages.youHaveUnreadMessages')}`
        : `${this.$t('messages.messages')}
          ${this.$t('messages.sendOrViewMessagesFromSurgeryOrHealthServices')}.`;
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

    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    navigateToAppMessages() {
      this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
      redirectTo(this, this.appMessagingPath);
    },
    navigateToMessages() {
      redirectTo(this, this.messagesPath);
    },
  },
};
</script>
