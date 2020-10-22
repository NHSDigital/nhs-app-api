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

      <organ-donation-link id="btn_organ_donation"
                           header-tag="h2"
                           :display-description="true"
                           :back-link-override="morePath"/>

      <menu-item
        id="btn_data_sharing"
        header-tag="h2"
        data-purpose="text_link"
        :href="dataSharingPath"
        :text="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared')"
        :description="$t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose')"
        :click-func="navigateToDataSharing"
        :aria-label="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared') |
          join($t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose') ,'. ')"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import sjrIf from '@/lib/sjrIf';
import { redirectTo } from '@/lib/utils';
import {
  MORE_PATH,
  MESSAGES_PATH,
  DATA_SHARING_OVERVIEW_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
} from '@/router/paths';
import {
  YOUR_NHS_DATA_MATTERS_URL,
} from '@/router/externalLinks';

export default {
  name: 'MorePage',
  components: {
    MenuItemList,
    MenuItem,
    OrganDonationLink,
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
      morePath: MORE_PATH,
      messagesPath: MESSAGES_PATH,
    };
  },
  computed: {
    dataSharingPath() {
      return this.$store.state.device.isNativeApp
        ? DATA_SHARING_OVERVIEW_PATH
        : YOUR_NHS_DATA_MATTERS_URL;
    },
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
    navigateToDataSharing() {
      if (this.$store.state.device.isNativeApp) {
        redirectTo(this, this.dataSharingPath);
      } else {
        window.open(this.dataSharingPath, '_blank');
      }
    },
  },
};
</script>
