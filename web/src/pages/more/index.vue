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
                 :click-func="navigate"
                 :aria-label="ariaLabelCaption(
                   'sc04.messaging.subheader',
                   'sc04.messaging.body')"/>

      <menu-item v-if="adminHelpEnabled"
                 id="btn_gp_help"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="adminHelpPath"
                 :text="$t('sc04.requestGpHelp.subheader')"
                 :description="$t('sc04.requestGpHelp.body')"
                 :click-func="navigate"
                 :aria-label="ariaLabelCaption(
                   'sc04.requestGpHelp.subheader',
                   'sc04.requestGpHelp.body')"/>

      <organ-donation-link id="btn_organ_donation"
                           header-tag="h2"
                           :description="$t('sc04.organDonation.body')"
                           :back-link-override="morePath"/>

      <menu-item id="btn_data_sharing"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="dataSharingPath"
                 :text="$t('sc04.dataSharing.subheader')"
                 :description="$t('sc04.dataSharing.body')"
                 :click-func="navigate"
                 :aria-label="ariaLabelCaption(
                   'sc04.dataSharing.subheader',
                   'sc04.dataSharing.body')"/>

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
import { redirectTo } from '@/lib/utils';

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
      dataSharingPath: DATA_SHARING_PREFERENCES.path,
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
    patientPracticeMessagingEnabled() {
      return this.$store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED
        && this.$store.state.practiceSettings.im1MessagingEnabled;
    },
    messagingEnabled() {
      return this.patientPracticeMessagingEnabled || this.appMessagingEnabled;
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
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();

      if (event.currentTarget.pathname === this.requestAdminHelpPath) {
        this.$store.dispatch('navigation/setNewMenuItem', 4);
        this.$store.dispatch('onlineConsultations/setPreviousRoute', this.morePath);
      }
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
  },
};
</script>
