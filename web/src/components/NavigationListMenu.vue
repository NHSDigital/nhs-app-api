<template>
  <menu-item-list data-sid="navigation-list-menu">
    <menu-item id="btn_messages"
               :header-tag="headerTag"
               data-purpose="messages-menu-item"
               :href="messagesPath"
               :show-indicator="hasMessageIndicator"
               :text="messagesItemText"
               :aria-label="ariaLabel"
               :click-func="navigateToMessages"/>

    <menu-item id="menu-item-advice"
               :header-tag="headerTag"
               data-sid="advice-list-item"
               :href="advicePath"
               :text="$t('navigation.getHealthAdvice')"
               :aria-label="$t('navigation.getHealthAdvice')"
               :click-func="goToUrl"
               :click-param="advicePath"/>

    <menu-item v-if="isProofLevel9"
               id="menu-item-appointments"
               :header-tag="headerTag"
               data-sid="appointments-menu-item"
               :href="appointmentsPath"
               :text="$t('navigation.bookAndManageAppointments')"
               :aria-label="$t('navigation.bookAndManageAppointments')"
               :click-func="goToUrl"
               :click-param="appointmentsPath"/>

    <menu-item v-if="isProofLevel9"
               id="menu-item-prescriptions"
               :header-tag="headerTag"
               data-sid="prescriptions-menu-item"
               :href="prescriptionsPath"
               :text="$t('navigation.orderARepeatPrescription')"
               :aria-label="$t('navigation.orderARepeatPrescription')"
               :click-func="goToUrl"
               :click-param="prescriptionsPath"/>

    <menu-item v-if="isProofLevel9 && !isAny3rdPartyHealthServiceEnabled"
               id="menu-item-myRecord"
               :header-tag="headerTag"
               data-sid="myrecord-menu-item"
               :href="gpMedicalRecordPath"
               :text="$t('navigation.viewYourGpHealthRecord')"
               :aria-label="$t('navigation.viewYourGpHealthRecord')"
               :click-func="goToUrl"
               :click-param="gpMedicalRecordPath"/>

    <menu-item v-if="isProofLevel9 && isAny3rdPartyHealthServiceEnabled"
               id="menu-item-health-record-hub"
               :header-tag="headerTag"
               data-sid="health-record-hub-menu-item"
               :href="healthRecordsHubPath"
               :text="$t('navigation.viewYourHealthRecords')"
               :aria-label="$t('navigation.viewYourHealthRecords')"
               :click-func="goToUrl"
               :click-param="healthRecordsHubPath"/>

    <organ-donation-link v-if="isProofLevel9"
                         id="organ-donation-link"
                         data-sid="organ-donation-menu-item"
                         :back-link-override="indexPath"/>

    <menu-item v-if="hasLinkedProfiles && isProofLevel9"
               id="menu-item-linkedProfiles"
               :header-tag="headerTag"
               data-sid="linkedProfile-menu-item"
               :href="linkedProfilesPath"
               :text="$t('navigation.linkedProfiles')"
               :aria-label="$t('navigation.linkedProfiles')"
               :click-func="goToUrl"
               :click-param="linkedProfilesPath"/>

  </menu-item-list>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { ORGAN_DONATION_URL } from '@/router/externalLinks';
import {
  APPOINTMENTS_PATH,
  MESSAGES_PATH,
  GP_MEDICAL_RECORD_PATH,
  HEALTH_RECORDS_PATH,
  PRESCRIPTIONS_PATH,
  ADVICE_PATH,
  LINKED_PROFILES_PATH,
  INDEX_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import sjrIf from '@/lib/sjrIf';

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
    hasMessageIndicator: {
      type: Boolean,
      default: false,
    },
    linkToAppMessages: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      organDonationUrl: ORGAN_DONATION_URL,
      advicePath: ADVICE_PATH,
      appointmentsPath: APPOINTMENTS_PATH,
      prescriptionsPath: PRESCRIPTIONS_PATH,
      gpMedicalRecordPath: GP_MEDICAL_RECORD_PATH,
      healthRecordsHubPath: HEALTH_RECORDS_PATH,
      linkedProfilesPath: LINKED_PROFILES_PATH,
      indexPath: INDEX_PATH,
    };
  },
  computed: {
    isAny3rdPartyHealthServiceEnabled() {
      return (this.is3rdPartyHealthServiceEnabled('pkb', 'testResults') ||
        this.is3rdPartyHealthServiceEnabled('pkbCie', 'testResults') ||
        this.is3rdPartyHealthServiceEnabled('pkb', 'healthTrackers') ||
        this.is3rdPartyHealthServiceEnabled('pkb', 'carePlans') ||
        this.is3rdPartyHealthServiceEnabled('pkbCie', 'healthTrackers') ||
        this.is3rdPartyHealthServiceEnabled('pkbCie', 'carePlans') ||
        this.is3rdPartyHealthServiceEnabled('substraktPatientPack', 'carePlans'));
    },
    hasLinkedProfiles() {
      return this.$store.getters['linkedAccounts/hasLinkedAccounts'];
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
    messagesItemText() {
      return (this.linkToAppMessages)
        ? this.$t('navigation.viewHealthInformationAndUpdates')
        : this.$t('navigation.viewYourMessages');
    },
    messagesPath() {
      return (this.linkToAppMessages) ? HEALTH_INFORMATION_UPDATES_PATH : MESSAGES_PATH;
    },
    ariaLabel() {
      return (this.hasMessageIndicator) ?
        `${this.messagesItemText}
          ${this.$t('navigation.youHaveUnreadMessages')}` :
        this.messagesItemText;
    },
  },
  methods: {
    is3rdPartyHealthServiceEnabled(provider, serviceType) {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider,
          serviceType,
        },
      });
    },
    navigateToMessages() {
      if (this.linkToAppMessages) {
        this.$store.dispatch('navigation/setRouteCrumb', 'appMessagesOnlyCrumb');
      }
      redirectTo(this, this.messagesPath);
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
