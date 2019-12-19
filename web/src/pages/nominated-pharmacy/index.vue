<template>
  <div v-if="showTemplate">
    <div v-if="hasNoNominatedPharmacy" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <no-nominated-pharmacy-warning/>
      </div>
    </div>
    <div v-else class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <pharmacy-detail
          :pharmacy="nominatedPharmacy"
          :is-my-nominated-pharmacy="true"
          :previous-path="currentPage"
          :can-change-pharmacy="isCommunityPharmacy"
          :show-instruction="true"/>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="!isCommunityPharmacy"
                        id="warning-dialog-dispensing-practice"
                        message-type="warning"
                        :icon-text="$t('messageIconText.important')">
          <message-text id="warning-text-1" :class="$style.warningText">
            {{ $t('nominated_pharmacy.warning.changeDispensingPractice.line1') }}
          </message-text>
          <message-text id="warning-text-2" :class="$style.warningText">
            {{ $t('nominated_pharmacy.warning.changeDispensingPractice.line2') }}
          </message-text>
        </message-dialog>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <analytics-tracked-tag :text="$t('generic.backButton.text')"
                               :tabindex="-1">
          <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                  :id="'back-button'"
                                  :path="prescriptionsPath"
                                  :button-text="'generic.backButton.text'"
                                  @clickAndPrevent="backButtonClicked"/>
        </analytics-tracked-tag>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable global-require */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { PRESCRIPTIONS, NOMINATED_PHARMACY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    PharmacyDetail,
    DesktopGenericBackLink,
    NoNominatedPharmacyWarning,
    MessageDialog,
    MessageText,
  },
  data() {
    return {
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      currentPage: NOMINATED_PHARMACY.path,
      prescriptionsPath: PRESCRIPTIONS.path,
    };
  },
  computed: {
    isCommunityPharmacy() {
      return (
        this.$store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P1);
    },
  },
  async asyncData({ store }) {
    if (store.state.nominatedPharmacy.hasLoaded === false) {
      await store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  },
  created() {
    if (!this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptionsPath);
    }
  },
  mounted() {
    if (this.$store.state.nominatedPharmacy.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.prescriptionsPath);
    },
  },
};
</script>
