<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate">
        <menu-item-list>
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
            <third-party-jump-off-button v-if="showPkbTestResults"
                                         id="btn_pkb_test_results"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.testResults" />
            <third-party-jump-off-button v-if="showPkbCieTestResults"
                                         id="btn_pkb_cie_test_results"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.testResultsCie" />
            <third-party-jump-off-button v-if="showPkbSecondaryCareTestResults"
                                         id="btn_pkb_secondary_care_test_results"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.testResultsPkbSecondaryCare" />
            <third-party-jump-off-button v-if="showPkbMyCareViewTestResults"
                                         id="btn_pkb_my_care_view_test_results"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.testResultsPkbMyCareView" />
            <third-party-jump-off-button v-if="showPkbCarePlans"
                                         id="btn_pkb_care_plans"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.carePlans" />
            <third-party-jump-off-button v-if="showPkbCieCarePlans"
                                         id="btn_pkb_cie_care_plans"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.carePlansCie" />
            <third-party-jump-off-button v-if="showPkbSecondaryCareCarePlans"
                                         id="btn_pkb_secondary_care_care_plans"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.carePlansPkbSecondaryCare" />
            <third-party-jump-off-button v-if="showPkbMyCareViewCarePlans"
                                         id="btn_pkb_my_care_view_care_plans"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.carePlansPkbMyCareView" />
            <third-party-jump-off-button v-if="showPkbHealthTracker"
                                         id="btn_pkb_health_trackers"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.healthTrackers" />
            <third-party-jump-off-button v-if="showPkbCieHealthTracker"
                                         id="btn_pkb_cie_health_trackers"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.healthTrackersCie" />
            <third-party-jump-off-button v-if="showPkbSecondaryCareHealthTracker"
                                         id="btn_pkb_secondary_care_health_trackers"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.healthTrackersPkbSecondaryCare" />
            <third-party-jump-off-button v-if="showPkbMyCareViewHealthTracker"
                                         id="btn_pkb_my_care_view_health_trackers"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.healthTrackersPkbMyCareView" />
            <third-party-jump-off-button v-if="showPkbSharedLinks"
                                         id="btn_pkb_shared_links"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.sharedLinks"/>
            <third-party-jump-off-button v-if="showPkbCieSharedLinks"
                                         id="btn_pkb_cie_shared_links"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.sharedLinksCie" />
            <third-party-jump-off-button v-if="showPkbSecondaryCareSharedLinks"
                                         id="btn_pkb_secondary_care_shared_links"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.sharedLinksPkbSecondaryCare"/>
            <third-party-jump-off-button v-if="showPkbMyCareViewSharedLinks"
                                         id="btn_pkb_my_care_view_shared_links"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.sharedLinksPkbMyCareView" />
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
            <third-party-jump-off-button v-if="showPkbCieRecordSharing"
                                         id="btn_pkb_cie_record_sharing"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.recordSharingCie" />
            <third-party-jump-off-button v-if="showPkbSecondaryCareRecordSharing"
                                         id="btn_pkb_secondary_care_record_sharing"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.recordSharingPkbSecondaryCare" />
            <third-party-jump-off-button v-if="showPkbMyCareViewRecordSharing"
                                         id="btn_pkb_my_care_view_record_sharing"
                                         provider-id="pkb"
                                         :provider-configuration="thirdPartyProvider
                                           .pkb.recordSharingPkbMyCareView" />

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
import {
  YOUR_NHS_DATA_MATTERS_URL,
} from '@/router/externalLinks';
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
        : YOUR_NHS_DATA_MATTERS_URL,
      showPkbTestResults: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'testResults',
        },
      }),
      showPkbCieTestResults: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'testResults',
        },
      }),
      showPkbSecondaryCareTestResults: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'testResults',
        },
      }),
      showPkbMyCareViewTestResults: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
      showPkbCieCarePlans: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'carePlans',
        },
      }),
      showPkbSecondaryCareCarePlans: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'carePlans',
        },
      }),
      showPkbMyCareViewCarePlans: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
      showPkbCieHealthTracker: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'healthTrackers',
        },
      }),
      showPkbSecondaryCareHealthTracker: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'healthTrackers',
        },
      }),
      showPkbMyCareViewHealthTracker: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
      showPkbCieSharedLinks: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'libraries',
        },
      }),
      showPkbSecondaryCareSharedLinks: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'libraries',
        },
      }),
      showPkbMyCareViewSharedLinks: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
      showPkbCieRecordSharing: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'recordSharing',
        },
      }),
      showPkbSecondaryCareRecordSharing: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbSecondaryCare',
          serviceType: 'recordSharing',
        },
      }),
      showPkbMyCareViewRecordSharing: sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbMyCareView',
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
