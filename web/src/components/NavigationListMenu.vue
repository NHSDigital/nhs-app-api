<template>
  <menu-item-list data-sid="navigation-list-menu">
    <menu-item id="menu-item-symptoms"
               data-sid="symptoms-list-item"
               :href="symptomsPath"
               :text="$t('navigationMenuList.symptoms')"
               :aria-label="$t('navigationMenuList.symptoms')"
               :click-func="goToUrl"
               :click-param="symptomsPath"/>

    <menu-item id="menu-item-appointments"
               data-sid="appointments-menu-item"
               :href="appointmentsPath"
               :text="$t('navigationMenuList.appointments')"
               :aria-label="$t('navigationMenuList.appointments')"
               :click-func="goToUrl"
               :click-param="appointmentsPath"/>

    <menu-item id="menu-item-prescriptions"
               data-sid="prescriptions-menu-item"
               :href="prescriptionsPath"
               :text="$t('navigationMenuList.prescriptions')"
               :aria-label="$t('navigationMenuList.prescriptions')"
               :click-func="goToUrl"
               :click-param="prescriptionsPath"/>

    <menu-item id="menu-item-myRecord"
               data-sid="myrecord-menu-item"
               :href="myRecordPath"
               :text="$t('navigationMenuList.myRecord')"
               :aria-label="$t('navigationMenuList.myRecord')"
               :click-func="goToUrl"
               :click-param="myRecordPath"/>

    <organ-donation-link id="organ-donation-link"
                         data-sid="organ-donation-menu-item" />

    <menu-item v-if="hasLinkedProfiles()"
               id="menu-item-linkedProfiles"
               data-sid="linkedProfiles-menu-item"
               :href="linkedProfilesPath"
               :text="$t('navigationMenuList.linkedProfiles')"
               :aria-label="$t('navigationMenuList.linkedProfiles')"
               :click-func="goToUrl"
               :click-param="linkedProfilesPath"/>
  </menu-item-list>
</template>

<script>
/* eslint-disable import/extensions */
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { APPOINTMENTS, MYRECORD, PRESCRIPTIONS, SYMPTOMS, LINKED_PROFILES } from '@/lib/routes';

export default {
  name: 'NavigationListMenu',
  components: {
    MenuItem,
    MenuItemList,
    OrganDonationLink,
  },
  data() {
    return {
      organDonationUrl: this.$store.app.$env.ORGAN_DONATION_URL,
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
  },
  methods: {
    hasLinkedProfiles() {
      return this.$store.getters['serviceJourneyRules/hasLinkedAccountsEnabled'];
    },
  },
};
</script>

<style module lang="scss">
  @import '../style/colours';
  @import '../style/textstyles';
  @import '../style/fonts';
</style>
