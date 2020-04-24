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

    <menu-item v-if="isProofLevel9"
               id="menu-item-appointments"
               :header-tag="headerTag"
               data-sid="appointments-menu-item"
               :href="appointmentsPath"
               :text="$t('navigationMenuList.appointments')"
               :aria-label="$t('navigationMenuList.appointments')"
               :click-func="goToUrl"
               :click-param="appointmentsPath"/>

    <menu-item v-if="isProofLevel9"
               id="menu-item-prescriptions"
               :header-tag="headerTag"
               data-sid="prescriptions-menu-item"
               :href="prescriptionsPath"
               :text="$t('navigationMenuList.prescriptions')"
               :aria-label="$t('navigationMenuList.prescriptions')"
               :click-func="goToUrl"
               :click-param="prescriptionsPath"/>

    <menu-item v-if="isProofLevel9"
               id="menu-item-myRecord"
               :header-tag="headerTag"
               data-sid="myrecord-menu-item"
               :href="gpMedicalRecordPath"
               :text="$t('navigationMenuList.myRecord')"
               :aria-label="$t('navigationMenuList.myRecord')"
               :click-func="goToUrl"
               :click-param="gpMedicalRecordPath"/>

    <menu-item id="btn_messages"
               :header-tag="headerTag"
               data-purpose="messages-menu-item"
               :href="messagesPath"
               :text="$t('navigationMenuList.messages')"
               :aria-label="$t('navigationMenuList.messages')"
               :click-func="goToUrl"
               :click-param="messagesPath"/>

    <organ-donation-link v-if="isProofLevel9"
                         id="organ-donation-link"
                         data-sid="organ-donation-menu-item"
                         :back-link-override="indexPath"/>

    <menu-item v-if="hasLinkedProfiles && isProofLevel9"
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
import { APPOINTMENTS, GP_MEDICAL_RECORD, PRESCRIPTIONS, SYMPTOMS,
  LINKED_PROFILES, INDEX, MESSAGES } from '@/lib/routes';
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
      organDonationUrl: this.$store.app.$env.ORGAN_DONATION_URL,
      messagesPath: MESSAGES.path,
      symptomsPath: SYMPTOMS.path,
      appointmentsPath: APPOINTMENTS.path,
      prescriptionsPath: PRESCRIPTIONS.path,
      gpMedicalRecordPath: GP_MEDICAL_RECORD.path,
      linkedProfilesPath: LINKED_PROFILES.path,
      indexPath: INDEX.path,
    };
  },
  computed: {
    hasLinkedProfiles() {
      return this.$store.getters['linkedAccounts/hasLinkedAccounts'];
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
  },
  methods: {
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
