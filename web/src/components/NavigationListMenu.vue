<template>
  <div>
    <menu-item-list-header id="nav-popular-services-list-header"
                           header-tag="h2"
                           :widen-on-tablet="true"
                           :text="$t('navigation.popularServices')"/>
    <menu-item-list data-sid="navigation-list-menu" class="nhsuk-u-margin-top-0">
      <third-party-jump-off-button v-if="showNetCompanyVaccineRecord"
                                   id="btn_netCompany_vaccine_record"
                                   provider-id="netCompany"
                                   :provider-configuration="thirdPartyProvider.netCompany.
                                     vaccineRecord" />
      <third-party-jump-off-button v-if="showNetCompanyP5VaccineRecord"
                                   id="btn_netCompanyP5_vaccine_record"
                                   provider-id="netCompany"
                                   :provider-configuration="thirdPartyProvider.netCompany.
                                     vaccineRecordP5" />
      <menu-item id="btn_messages"
                 :header-tag="headerTag"
                 data-purpose="messages-menu-item"
                 :href="messagesPath"
                 :text="messagesLabel"
                 :aria-label="messagesLabelWithUnreadCount"
                 :count="unreadMessagesCount"
                 :click-func="navigateToMessages"
                 :is-messaging="true"/>
      <menu-item v-if="supportsLinkedProfiles && isProofLevel9"
                 id="linked-profiles-link"
                 :header-tag="headerTag"
                 :href="linkedProfilesPath"
                 :text="$t('navigation.pages.headers.linkedProfiles')"
                 :aria-label="$t('navigation.pages.headers.linkedProfiles')"
                 :click-func="navigateToLinkedProfiles"
                 :is-messaging="true"/>
      <menu-item v-if="isProofLevel9"
                 id="menu-item-myRecord"
                 :header-tag="headerTag"
                 data-sid="your-health-menu-item"
                 :href="gpMedicalRecordPath"
                 :text="$t('navigation.viewYourGpHealthRecord')"
                 :aria-label="$t('navigation.viewYourGpHealthRecord')"
                 :click-func="goToUrl"
                 :click-param="gpMedicalRecordPath"
                 :is-messaging="true"/>
      <menu-item v-if="isProofLevel9"
                 id="menu-item-prescriptions"
                 :header-tag="headerTag"
                 data-sid="prescriptions-menu-item"
                 :href="prescriptionsPath"
                 :text="$t('navigation.orderAPrescription')"
                 :aria-label="$t('navigation.orderAPrescription')"
                 :click-func="goToUrl"
                 :click-param="prescriptionsPath"
                 :is-messaging="true"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import MenuItemListHeader from '@/components/MenuItemListHeader';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import {
  MESSAGES_PATH,
  GP_MEDICAL_RECORD_PATH,
  PRESCRIPTIONS_PATH,
  LINKED_PROFILES_PATH,
  INDEX_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';

export default {
  name: 'NavigationListMenu',
  components: {
    MenuItem,
    MenuItemList,
    MenuItemListHeader,
    ThirdPartyJumpOffButton,
  },
  props: {
    headerTag: {
      type: String,
      default: 'h2',
    },
  },
  data() {
    return {
      isProxying: this.$store.getters['session/isProxying'],
      prescriptionsPath: PRESCRIPTIONS_PATH,
      gpMedicalRecordPath: GP_MEDICAL_RECORD_PATH,
      messagesPath: MESSAGES_PATH,
      linkedProfilesPath: LINKED_PROFILES_PATH,
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  computed: {
    hasNetCompanyVaccineRecord() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'netCompany',
          serviceType: 'vaccineRecord',
        },
      });
    },
    hasNetCompanyP5VaccineRecord() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'netCompanyP5',
          serviceType: 'vaccineRecord',
        },
      });
    },
    isProofLevel9() {
      return this.$store.getters['session/isProofLevel9'];
    },
    messagesLabel() {
      return this.$t('navigation.viewYourMessages');
    },
    messagesLabelWithUnreadCount() {
      let label = `${this.messagesLabel}. `;

      if (this.unreadMessagesCount && this.unreadMessagesCount > 0) {
        label += this.$t('navigation.youHaveCountUnreadMessagePlural', {
          count: this.unreadMessagesCount,
          plural: this.unreadMessagesCount > 1 ? 's' : '',
        });
      }

      return label;
    },
    supportsLinkedProfiles() {
      return this.$store.state.serviceJourneyRules.rules.supportsLinkedProfiles;
    },
    showNetCompanyVaccineRecord() {
      return this.hasNetCompanyVaccineRecord && !this.isProxying && this.isProofLevel9;
    },
    showNetCompanyP5VaccineRecord() {
      return this.hasNetCompanyP5VaccineRecord && !this.isProxying && !this.isProofLevel9;
    },
    unreadMessagesCount() {
      return this.$store.state.messaging.totalUnreadMessageCount || undefined;
    },
  },
  created() {
    this.$store.dispatch('navigation/clearBackLinkOverride');
  },
  methods: {
    navigateToMessages() {
      redirectTo(this, this.messagesPath);
    },
    navigateToLinkedProfiles() {
      this.$store.dispatch('navigation/setRouteCrumb', 'defaultCrumb');
      this.$store.dispatch('navigation/setBackLinkOverride', INDEX_PATH);
      this.goToUrl(this.linkedProfilesPath);
    },
  },
};
</script>

<style module lang="scss">
  @import "@/style/custom/navigation-list-menu";
</style>
