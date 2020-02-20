<template>
  <div v-if="showTemplate" id="mainDiv">
    <menu-item-list>

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
                 :click-func="navigate"
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
import srjIf from '@/lib/sjrIf';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import {
  APPOINTMENT_ADMIN_HELP,
  DATA_SHARING_PREFERENCES,
  MESSAGING,
  MORE,
  PATIENT_PRACTICE_MESSAGING,
} from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { redirectTo, isTruthy } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
    OrganDonationLink,
  },
  data() {
    return {
      appMessagingEnabled: srjIf({ $store: this.$store, journey: 'messaging' }),
      adminHelpEnabled: srjIf({ $store: this.$store, journey: 'cdssAdmin' }),
      patientPracticeMessagingPath: PATIENT_PRACTICE_MESSAGING.path,
      appMessagingPath: MESSAGING.path,
      morePath: MORE.path,
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
      return isTruthy(this.$store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED)
        && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    // patientpracticemessaging should be shown on desktop & native if enabled
    // appMessaging should only be shown on native devices if enabled
    messagingEnabled() {
      if (this.$store.state.device.isNativeApp) {
        return this.patientPracticeMessagingEnabled || this.appMessagingEnabled;
      }
      return this.patientPracticeMessagingEnabled;
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
