<template>
  <div v-if="showTemplate" id="mainDiv">
    <menu-item-list>
      <third-party-jump-off-button v-if="showPkbMessages && isNativeApp && !isProxying"
                                   id="btn_pkb_messages_and_consultations"
                                   provider-id="pkb"
                                   :jump-off-type="thirdPartyProvider.pkb.messages.type"
                                   :redirect-path="thirdPartyProvider.pkb.messages.redirectPath" />
      <menu-item v-if="messagingEnabled"
                 id="btn_messaging"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="messagingPath"
                 :text="$t('sc04.messaging.subheader')"
                 :description="$t('sc04.messaging.body')"
                 :click-func="navigateToMessaging"
                 :aria-label="$t('sc04.messaging.subheader') |
                   join($t('sc04.messaging.body') ,'. ')"/>

      <menu-item v-if="adminHelpEnabled"
                 id="btn_gp_help"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="adminHelpPath"
                 :text="$t('sc04.requestGpHelp.subheader')"
                 :description="$t('sc04.requestGpHelp.body')"
                 :click-func="navigateToAdminHelp"
                 :aria-label="$t('sc04.requestGpHelp.subheader') |
                   join($t('sc04.requestGpHelp.body') ,'. ')"/>

      <organ-donation-link id="btn_organ_donation"
                           header-tag="h2"
                           :display-description="true"
                           :back-link-override="morePath"/>

      <menu-item id="btn_data_sharing"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="dataSharingPath"
                 :text="$t('sc04.dataSharing.subheader')"
                 :description="$t('sc04.dataSharing.body')"
                 :click-func="navigateToDataSharing"
                 :aria-label="$t('sc04.dataSharing.subheader') |
                   join($t('sc04.dataSharing.body') ,'. ')"/>

    </menu-item-list>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import {
  APPOINTMENT_ADMIN_HELP,
  DATA_SHARING_PREFERENCES,
  MESSAGING,
  MORE,
  PATIENT_PRACTICE_MESSAGING,
} from '@/lib/routes';
import sjrIf from '@/lib/sjrIf';
import { createUri } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
    OrganDonationLink,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      adminHelpEnabled: sjrIf({ $store: this.$store, journey: 'cdssAdmin' }),
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      showPkbMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'messages',
        },
      }),
      isProxying: this.$store.getters['session/isProxying'],
      isNativeApp: this.$store.state.device.isNativeApp,
      patientPracticeMessagingPath: PATIENT_PRACTICE_MESSAGING.path,
      appMessagingPath: MESSAGING.path,
      morePath: MORE.path,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      adminHelpPath: createUri({
        path: APPOINTMENT_ADMIN_HELP.path,
        noJs: { onlineConsultations: { previousRoute: MORE.path } },
      }),
    };
  },
  computed: {
    dataSharingPath() {
      return this.$store.state.device.isNativeApp
        ? DATA_SHARING_PREFERENCES.path
        : this.$store.app.$env.YOUR_NHS_DATA_MATTERS_URL;
    },
    patientPracticeMessagingEnabled() {
      return this.im1MessagingSjrEnabled && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    // patientpracticemessaging should be shown on desktop & native if enabled
    // appMessaging should only be shown on native devices if enabled
    messagingEnabled() {
      return this.patientPracticeMessagingEnabled ||
      (this.appMessagingEnabled && this.$store.state.device.isNativeApp);
    },
    messagingPath() {
      return this.patientPracticeMessagingEnabled
        ? this.patientPracticeMessagingPath
        : this.appMessagingPath;
    },
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    navigateToAdminHelp(event) {
      this.navigate(event);
      this.$store.dispatch('navigation/setNewMenuItem', 4);
      this.$store.dispatch('onlineConsultations/setPreviousRoute', this.morePath);
      this.$store.dispatch('navigation/setBackLinkOverride', this.morePath);
      this.$store.dispatch('navigation/setRouteCrumb', 'moreCrumb');
    },
    navigateToMessaging(event) {
      this.$store.dispatch('navigation/setBackLinkOverride', this.morePath);
      this.navigate(event);
    },
    navigateToDataSharing(event) {
      if (this.$store.state.device.isNativeApp) {
        this.navigate(event);
      } else {
        window.open(this.dataSharingPath, '_blank');
      }
    },
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
  },
};
</script>
