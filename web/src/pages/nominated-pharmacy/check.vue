<template>
  <div v-if="showTemplate"
       id="mainDiv">
    <div v-if="hasNoNominatedPharmacy">
      <no-nominated-pharmacy-warning/>
    </div>
    <div v-else
         :class="$style.info" data-purpose="info">
      <pharmacy-detail id="pharmacy-details"
                       :pharmacy="pharmacy"
                       :is-my-nominated-pharmacy="true"
                       :previous-path="currentPage"
                       :can-change-pharmacy="showChangePharmacyLink"
                       :display-change-my-nominated-pharmacy-button="false"
                       :show-instruction="false" />
    </div>

    <generic-button id="continue-button-found"
                    :button-classes="['nhsuk-button']"
                    @click.prevent="onContinueButtonClicked">
      {{ getContinueButtonText }}
    </generic-button>

    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            id="back-link"
                            :path="prescriptionsPath"
                            :button-text="'nominatedPharmacy.notFound.backButton'"
                            @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { PRESCRIPTIONS_PATH, PRESCRIPTION_REPEAT_COURSES_PATH, NOMINATED_PHARMACY_CHECK_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'NominatedPharmacyCheck',
  components: {
    GenericButton,
    PharmacyDetail,
    DesktopGenericBackLink,
    NoNominatedPharmacyWarning,
  },
  data() {
    return {
      currentPage: NOMINATED_PHARMACY_CHECK_PATH,
      pharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
    };
  },
  computed: {
    getContinueButtonText() {
      return this.hasNoNominatedPharmacy ?
        this.$t('nominatedPharmacy.notFound.continueButton') :
        this.$t('nominatedPharmacy.continueButton');
    },
    showChangePharmacyLink() {
      return (this.pharmacy.pharmacyType !== PharmacyType.P3);
    },
    prescriptionsPath() {
      return PRESCRIPTIONS_PATH;
    },
  },
  created() {
    if (this.$store.state.nominatedPharmacy.hasLoaded === false || !this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptionsPath);
    }
  },
  methods: {
    onContinueButtonClicked() {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES_PATH);
    },
    onBackButtonClicked() {
      redirectTo(this, this.prescriptionsPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/nominated-pharmacy-check";
</style>
