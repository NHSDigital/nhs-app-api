<template>
  <menu-item-list data-sid="navigation-list-menu">
    <menu-item id="menu-item-symptoms"
               :header-tag="headerTag"
               data-sid="symptoms-list-item"
               :href="symptomsPath"
               :text="$t('navigationMenuList.symptoms')"
               :aria-label="$t('navigationMenuList.symptoms')"
               :click-func="goToUrl"
               :click-param="symptomsPath"/>

    <menu-item id="menu-item-appointments"
               :header-tag="headerTag"
               data-sid="appointments-menu-item"
               :href="appointmentsPath"
               :text="$t('navigationMenuList.appointments')"
               :aria-label="$t('navigationMenuList.appointments')"
               :click-func="goToUrl"
               :click-param="appointmentsPath"/>

    <menu-item id="menu-item-prescriptions"
               :header-tag="headerTag"
               data-sid="prescriptions-menu-item"
               :href="prescriptionsPath"
               :text="$t('navigationMenuList.prescriptions')"
               :aria-label="$t('navigationMenuList.prescriptions')"
               :click-func="goToUrl"
               :click-param="prescriptionsPath"/>

    <menu-item id="menu-item-myRecord"
               :header-tag="headerTag"
               data-sid="myrecord-menu-item"
               :href="myRecordPath"
               :text="$t('navigationMenuList.myRecord')"
               :aria-label="$t('navigationMenuList.myRecord')"
               :click-func="goToUrl"
               :click-param="myRecordPath"/>

    <menu-item v-if="messagingEnabled"
               id="btn_messaging"
               header-tag="h2"
               data-purpose="text_link"
               :href="messagingPath"
               :text="$t('navigationMenuList.messaging')"
               :click-func="navigate"
               :aria-label="$t('navigationMenuList.messaging')"/>

    <organ-donation-link id="organ-donation-link"
                         data-sid="organ-donation-menu-item"
                         :back-link-override="indexPath"/>

    <menu-item v-if="hasLinkedProfiles()"
               id="menu-item-linkedProfiles"
               :header-tag="headerTag"
               data-sid="linkedProfile-menu-item"
               :href="linkedProfilesPath"
               :text="$t('navigationMenuList.linkedProfiles')"
               :aria-label="$t('navigationMenuList.linkedProfiles')"
               :click-func="goToUrl"
               :click-param="linkedProfilesPath"/>

  </menu-item-list>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { APPOINTMENTS, MYRECORD, PRESCRIPTIONS, SYMPTOMS, LINKED_PROFILES, INDEX,
  PATIENT_PRACTICE_MESSAGING, MESSAGING } from '@/lib/routes';
import sjrIf from '@/lib/sjrIf';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'NavigationListMenu',
  components: {
    MenuItem,
    MenuItemList,
    OrganDonationLink,
  },
  props: {
    headerTag: {
      type: String,
      default: 'h2',
    },
  },
  data() {
    return {
      appMessagingEnabled: sjrIf({ $store: this.$store, journey: 'messaging' }),
      patientPracticeMessagingPath: PATIENT_PRACTICE_MESSAGING.path,
      appMessagingPath: MESSAGING.path,
      organDonationUrl: this.$store.app.$env.ORGAN_DONATION_URL,
      im1MessagingSjrEnabled: sjrIf({ $store: this.$store, journey: 'im1Messaging' }),
      im1MessagingPracticeEnabled: this.$store.state.practiceSettings.im1MessagingEnabled,
    };
  },
  computed: {
    symptomsPath() {
      return SYMPTOMS.path;
    },
    appointmentsPath() {
      return APPOINTMENTS.path;
    },
    prescriptionsPath() {
      return PRESCRIPTIONS.path;
    },
    myRecordPath() {
      return MYRECORD.path;
    },
    linkedProfilesPath() {
      return LINKED_PROFILES.path;
    },
    indexPath() {
      return INDEX.path;
    },
    patientPracticeMessagingEnabled() {
      return this.im1MessagingSjrEnabled && this.im1MessagingPracticeEnabled;
    },
    // patientpracticemessaging should be shown on desktop & native if enabled
    // appMessaging should only be shown on native devices if enabled
    messagingEnabled() {
      return this.patientPracticeMessagingEnabled ||
      (this.appMessagingEnabled && this.$store.state.device.isNativeApp);
    },
    messagingPath() {
      return (this.patientPracticeMessagingEnabled)
        ? this.patientPracticeMessagingPath
        : this.appMessagingPath;
    },
  },
  methods: {
    hasLinkedProfiles() {
      return this.$store.getters['linkedAccounts/hasLinkedAccounts'];
    },
    navigate(event) {
      redirectTo(this, event.currentTarget.pathname);
      event.preventDefault();
    },
  },
};
</script>

<style module lang="scss">
  @import '../style/colours';

  a:visited {
    color: $nhs_blue;
  }
</style>
