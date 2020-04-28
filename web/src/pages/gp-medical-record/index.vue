<template>
  <div class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate">
        <menu-item-list>
          <menu-item id="btn_gp_medical_record"
                     header-tag="h2"
                     role="link"
                     data-purpose="text_link"
                     :href="gpMedicalRecordPath"
                     :click-func="redirectToMedicalRecord"
                     :description="$t('healthRecordHubPage.gpMedicalRecord.body')"
                     :text="$t('healthRecordHubPage.gpMedicalRecord.subheader')"
                     :aria-label="ariaLabelCaption(
                       'healthRecordHubPage.gpMedicalRecord.subheader',
                       'healthRecordHubPage.gpMedicalRecord.body')"
                     :prevent-default="preventDefault()"/>
          <third-party-jump-off-button v-if="showPkbCarePlans && !isProxying && isNativeApp"
                                       id="btn_care_plans"
                                       provider-id="pkb"
                                       :jump-off-type="thirdPartyProvider.pkb.carePlans.type"
                                       :redirect-path="thirdPartyProvider
                                         .pkb.carePlans.redirectPath" />
          <third-party-jump-off-button v-if="showPkbHealthTracker && !isProxying && isNativeApp"
                                       id="btn_health_trackers"
                                       provider-id="pkb"
                                       :jump-off-type="thirdPartyProvider.pkb.healthTrackers.type"
                                       :redirect-path="thirdPartyProvider.pkb
                                         .healthTrackers.redirectPath" />
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>

import { GP_MEDICAL_RECORD } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import sjrIf from '@/lib/sjrIf';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import agreedToMedicalWarning from '@/lib/sessionStorage';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
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
      isNativeApp: this.$store.state.device.isNativeApp,
      isProxying: this.$store.getters['session/isProxying'],
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
    };
  },
  computed: {
    gpMedicalRecordPath() {
      return GP_MEDICAL_RECORD.path;
    },
    hasAgreedToMedicalWarning() {
      return agreedToMedicalWarning('agreedToMedicalWarning');
    },
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
      this.$router.push(this.gpMedicalRecordPath);
    },
    preventDefault() {
      return true;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/buttons";
</style>
