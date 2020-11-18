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

          <third-party-jump-off-button v-if="showPkbTestResults && !isProxying"
                                       id="btn_pkb_test_results"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.testResults" />
          <third-party-jump-off-button v-if="showPkbCieTestResults && !isProxying"
                                       id="btn_pkb_cie_test_results"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.testResultsCie" />
          <third-party-jump-off-button v-if="showPkbCarePlans && !isProxying"
                                       id="btn_pkb_care_plans"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.carePlans" />
          <third-party-jump-off-button v-if="showPkbCieCarePlans && !isProxying"
                                       id="btn_pkb_cie_care_plans"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.carePlansCie" />
          <third-party-jump-off-button v-if="showPkbHealthTracker && !isProxying"
                                       id="btn_pkb_health_trackers"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.healthTrackers" />
          <third-party-jump-off-button v-if="showPkbCieHealthTracker && !isProxying"
                                       id="btn_pkb_cie_health_trackers"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.healthTrackersCie" />
          <third-party-jump-off-button v-if="showPkbSharedLinks && !isProxying"
                                       id="btn_pkb_shared_links"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.sharedLinks"/>
          <third-party-jump-off-button v-if="showPkbCieSharedLinks && !isProxying"
                                       id="btn_pkb_cie_shared_links"
                                       provider-id="pkb"
                                       :provider-configuration="thirdPartyProvider
                                         .pkb.sharedLinksCie" />
          <third-party-jump-off-button v-if="showPatientPackAccountAdmin && !isProxying"
                                       id="btn_substrakt_update_details"
                                       provider-id="substraktPatientPack"
                                       :provider-configuration="thirdPartyProvider
                                         .substraktPatientPack.accountAdmin" />
          <third-party-jump-off-button
            v-if="showGncrMessages && !isProxying"
            id="btn_gncr_messages_and_consultations"
            provider-id="gncr"
            :provider-configuration="thirdPartyProvider.gncr.correspondence" />

          <organ-donation-link
            id="btn_organ_donation"
            header-tag="h2"
            :display-description="true"
            :back-link-override="yourHealthPath"/>

          <menu-item
            id="btn_data_sharing"
            header-tag="h2"
            data-purpose="text_link"
            :href="dataSharingPath"
            :text="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared')"
            :description="$t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose')"
            :click-func="navigateToDataSharing"
            :aria-label="$t('dataSharing.chooseIfDataFromYourHealthRecordIsShared') |
              join($t('dataSharing.findOutHowTheNhsUsesYourInformationAndChoose') ,'. ')"/>
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
