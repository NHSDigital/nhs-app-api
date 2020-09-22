<template>
  <div v-if="showTemplate" id="mainDiv">
    <menu-item-list>
      <menu-item
        v-if="onlyAppMessagesEnabled"
        id="btn_appMessaging"
        header-tag="h2"
        data-purpose="text_link"
        :href="appMessagingPath"
        :has-unread-messages="hasUnreadMessages"
        :text="$t('messages.hub.healthInformationAndUpdates')"
        :description="$t('messages.hub.viewMessagesFromHealthServicesAndTheApp')"
        :click-func="navigateToAppMessages"
        :aria-label="ariaLabel"/>

      <menu-item
        v-else
        id="btn_messages"
        header-tag="h2"
        data-purpose="text_link"
        :href="messagesPath"
        :text="$t('messages.messages')"
        :description="$t('messages.sendOrViewMessagesFromSurgeryOrHealthServices')"
        :has-unread-messages="hasUnreadMessages"
        :click-func="navigateToMessages"
        :aria-label="ariaLabel"/>

      <menu-item
        v-if="adminHelpEnabled"
        id="btn_gp_help"
        header-tag="h2"
        data-purpose="text_link"
        :href="adminHelpPath"
        :text="$t('appointments.guidance.additionalGpServices.additionalGpServices')"
        :description="$t('appointments.guidance.additionalGpServices.getSickNotesAndLetters')"
        :click-func="navigateToAdminHelp"
        :aria-label="$t('appointments.guidance.additionalGpServices.additionalGpServices') |
          join($t('appointments.guidance.additionalGpServices.getSickNotesAndLetters') ,'. ')"/>

      <third-party-jump-off-button
        v-if="showEngageAdmin"
        id="btn_engage_admin"
        provider-id="engage"
        :provider-configuration="thirdPartyProvider.engage.admin" />

      <organ-donation-link id="btn_organ_donation"
                           header-tag="h2"
                           :display-description="true"
                           :back-link-override="morePath"/>

      <third-party-jump-off-button
        v-if="showPkbSharedLinks"
        id="btn_pkb_shared_links"
        provider-id="pkb"
        :provider-configuration="thirdPartyProvider.pkb.sharedLinks" />

      <third-party-jump-off-button
        v-if="showPkbCieSharedLinks"
        id="btn_pkb_cie_shared_links"
        provider-id="pkb"
        :provider-configuration="thirdPartyProvider.pkb.sharedLinksCie" />

      <menu-item
        id="btn_data_sharing"
        header-tag="h2"
        data-purpose="text_link"
        :href="dataSharingPath"
        :text="$t('dataSharing.findOutWhyYourDataMatters')"
        :description="$t('dataSharing.findOutHowTheNhsUsesYourInfomrationAndChoose')"
        :click-func="navigateToDataSharing"
        :aria-label="$t('dataSharing.findOutWhyYourDataMatters') |
          join($t('dataSharing.findOutHowTheNhsUsesYourInfomrationAndChoose') ,'. ')"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import sjrIf from '@/lib/sjrIf';
import { redirectTo } from '@/lib/utils';
import {
  APPOINTMENT_ADMIN_HELP_PATH,
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
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      hasUnreadMessages: false,
      adminHelpEnabled: sjrIf({ $store: this.$store, journey: 'cdssAdmin' }),
      appMessagingPath: HEALTH_INFORMATION_UPDATES_PATH,
      adminHelpPath: APPOINTMENT_ADMIN_HELP_PATH,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      isNativeApp: this.$store.state.device.isNativeApp,
      isProxying: this.$store.getters['session/isProxying'],
      isProofLevel9: this.$store.getters['session/isProofLevel9'],
      morePath: MORE_PATH,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      messagesPath: MESSAGES_PATH,
    };
  },
  computed: {
    dataSharingPath() {
      return this.$store.state.device.isNativeApp
        ? DATA_SHARING_OVERVIEW_PATH
        : YOUR_NHS_DATA_MATTERS_URL;
    },
    hasEngageAdmin() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'engage',
          serviceType: 'consultationsAdmin',
        },
      });
    },
    hasPkbSharedLinks() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'libraries',
        },
      });
    },
    hasPkbCieSharedLinks() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'libraries',
        },
      });
    },
    showEngageAdmin() {
      return this.hasEngageAdmin && !this.isProxying;
    },
    showPkbSharedLinks() {
      return this.hasPkbSharedLinks && !this.isProxying;
    },
    showPkbCieSharedLinks() {
      return this.hasPkbCieSharedLinks && !this.isProxying;
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
      this.$store.dispatch('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
      redirectTo(this, this.appMessagingPath);
    },
    navigateToMessages() {
      redirectTo(this, this.messagesPath);
    },
    navigateToAdminHelp() {
      this.$store.dispatch('navigation/setNewMenuItem', 4);
      this.$store.dispatch('onlineConsultations/setPreviousRoute', this.morePath);
      this.$store.dispatch('navigation/setBackLinkOverride', this.morePath);
      this.$store.dispatch('navigation/setRouteCrumb', 'moreCrumb');
      redirectTo(this, this.adminHelpPath);
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
