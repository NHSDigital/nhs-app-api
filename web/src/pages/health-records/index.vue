<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate">
        <menu-item-list>
          <div v-if="!isProxying">
            <third-party-jump-off-button v-if="showNetCompanyVaccineRecord"
                                         id="btn_netCompany_vaccine_record"
                                         provider-id="netCompany"
                                         :provider-configuration="thirdPartyProvider
                                           .netCompany.vaccineRecord" />
            <third-party-jump-off-button v-if="showNhsdVaccineRecord"
                                         id="btn_nhsd_vaccine_record"
                                         provider-id="nhsd"
                                         :provider-configuration="thirdPartyProvider
                                           .nhsd.vaccineRecord" />
          </div>

          <menu-item id="btn_gp_medical_record"
                     header-tag="h2"
                     data-purpose="text_link"
                     :href="gpMedicalRecordPath"
                     :click-func="redirectToMedicalRecord"
                     :text="$t('myRecord.hub.gpHealthRecord')"
                     :description="$t('myRecord.hub.viewAllergiesMedicinesAndMore')"
                     :aria-label="ariaLabelCaption(
                       'myRecord.hub.gpHealthRecord',
                       'myRecord.hub.viewAllergiesMedicinesAndMore')" />

          <div v-if="!isProxying">
            <third-party-jump-off-button v-if="showWellnessAndPrevention"
                                         id="btn_wellness_and_prevention"
                                         provider-id="wellnessAndPrevention"
                                         :provider-configuration="thirdPartyProvider
                                           .wellnessAndPrevention.healthTrackers" />
            <third-party-jump-off-button v-if="showPkbTestResults"
                                         id="btn_pkb_test_results"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.testResults" />
            <third-party-jump-off-button v-if="showPkbCarePlans"
                                         id="btn_pkb_care_plans"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.carePlans" />
            <third-party-jump-off-button v-if="showPkbHealthTracker"
                                         id="btn_pkb_health_trackers"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.healthTrackers" />
            <third-party-jump-off-button v-if="showPkbSharedLinks"
                                         id="btn_pkb_shared_links"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.sharedLinks"/>
            <third-party-jump-off-button v-if="showPatientPackAccountAdmin"
                                         id="btn_substrakt_update_details"
                                         provider-id="substraktPatientPack"
                                         :provider-configuration="thirdPartyProvider
                                           .substraktPatientPack.accountAdmin" />
            <third-party-jump-off-button v-if="showPkbRecordSharing"
                                         id="btn_pkb_record_sharing"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.recordSharing" />
            <third-party-jump-off-button
              v-if="showGncrMessages"
              id="btn_gncr_messages_and_consultations"
              provider-id="gncr"
              :provider-configuration="thirdPartyProvider.gncr.correspondence" />

            <organ-donation-link
              id="btn_organ_donation"
              header-tag="h2"
              :display-description="true"
              :back-link-override="yourHealthPath"/>

            <menu-item
              v-if="showDataSharing"
              id="btn_data_sharing"
              header-tag="h2"
              data-purpose="text_link"
              :href="dataSharingPath"
              :text="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared')"
              :description="$t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose')"
              :click-func="navigateToDataSharing"
              :aria-label="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared') |
                join($t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose') ,'. ')"/>
          </div>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>

import {
  GP_MEDICAL_RECORD_PATH,
  HEALTH_RECORDS_PATH,
  DATA_SHARING_OVERVIEW_PATH,
} from '@/router/paths';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import { redirectTo } from '@/lib/utils';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';

export default {
  components: {
    MenuItem,
    MenuItemList,
    ThirdPartyJumpOffButton,
    OrganDonationLink,
  },
  data() {
    return {
      gpMedicalRecordPath: GP_MEDICAL_RECORD_PATH,
      yourHealthPath: HEALTH_RECORDS_PATH,
      dataSharingPath: this.$store.state.device.isNativeApp
        ? DATA_SHARING_OVERVIEW_PATH
        : this.$store.$env.YOUR_NHS_DATA_MATTERS_URL,
      showWellnessAndPrevention: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'wellnessAndPrevention',
          serviceType: 'healthTrackers',
        },
      }),
      showPkbTestResults: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'testResults',
        },
      }),
      showPkbCarePlans: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'carePlans',
        },
      }),
      showPkbHealthTracker: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'healthTrackers',
        },
      }),
      showPkbSharedLinks: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'libraries',
        },
      }),
      showPatientPackAccountAdmin: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'substraktPatientPack',
          serviceType: 'accountAdmin',
        },
      }),
      showGncrMessages: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'gncr',
          serviceType: 'messages',
        },
      }),
      showNhsdVaccineRecord: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'nhsd',
          serviceType: 'vaccineRecord',
        },
      }),
      showNetCompanyVaccineRecord: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'netCompany',
          serviceType: 'vaccineRecord',
        },
      }),
      showPkbRecordSharing: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'recordSharing',
        },
      }),
      showDataSharing: sjrIf({
        $store: this.$store,
        journey: 'ndop',
      }),
      isProxying: this.$store.getters['session/isProxying'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  updated() {
    window.scrollTo(0, 0);
  },
  mounted() {
    this.$store.dispatch('navigation/setBackLinkOverride', HEALTH_RECORDS_PATH);
    this.$store.dispatch('device/unlockNavBar');
  },
  methods: {
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${this.$t(body)}`;
    },
    redirectToMedicalRecord() {
      redirectTo(this, this.gpMedicalRecordPath);
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

<style module lang="scss" scoped>
  @import "../../style/buttons";
</style>
