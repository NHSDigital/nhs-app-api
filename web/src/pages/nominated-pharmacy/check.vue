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
                       :displayChangeMyNominatedPharmacyButton="false"
                       :show-instruction="false" />
    </div>

    <generic-button id="continue-button-found"
                    :button-classes="['nhsuk-button']"
                    @click.prevent="onContinueButtonClicked">
      {{ getContinueButtonText }}
    </generic-button>

    <generic-button v-if="$store.state.device.isNativeApp"
                    id="back-button"
                    :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                    tabindex="0" @click.prevent="onBackButtonClicked">
      {{ $t('nominatedPharmacyNotFound.backButton') }}
    </generic-button>
    <desktopGenericBackLink v-else
                            id="back-link"
                            :path="prescriptionsPath"
                            :button-text="'nominatedPharmacyNotFound.backButton'"
                            @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_CHECK } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    PharmacyDetail,
    DesktopGenericBackLink,
    NoNominatedPharmacyWarning,
  },
  data() {
    return {
      currentPage: NOMINATED_PHARMACY_CHECK.path,
      pharmacy: this.$store.state.nominatedPharmacy.pharmacy,
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
    };
  },
  computed: {
    getContinueButtonText() {
      return this.hasNoNominatedPharmacy ?
        this.$t('nominatedPharmacyNotFound.continueButton') :
        this.$t('nominated_pharmacy.continueButton');
    },
    showChangePharmacyLink() {
      return (this.pharmacy.pharmacyType !== PharmacyType.P3);
    },
    prescriptionsPath() {
      return PRESCRIPTIONS.path;
    },
  },
  created() {
    if (this.$store.state.nominatedPharmacy.hasLoaded === false || !this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, this.prescriptionsPath);
    }
  },
  methods: {
    onContinueButtonClicked() {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES.path);
    },
    onBackButtonClicked() {
      redirectTo(this, this.prescriptionsPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/fonts';
  @import '../../style/buttons';
  @import '../../style/textstyles';
  @import "../../style/panels";
  @import '../../style/listmenu';
  @import "../../style/home";

div {
  &.desktopWeb {
  max-width: 540px;

  .warningText {
    font-family: $default_web;
    font-weight: normal;
  }

  li {
    font-family: $default_web;
    font-weight: normal;
  }

  p {
    font-family: $default_web;
    font-weight: normal;
    }
  }
}
</style>
