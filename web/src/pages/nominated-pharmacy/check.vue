<template>
  <div v-if="showTemplate"
       id="mainDiv"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div v-if="hasNoNominatedPharmacy">
      <no-nominated-pharmacy-warning/>
    </div>
    <div v-else
         :class="$style.info" data-purpose="info">
      <pharmacy-detail id="pharmacy-details"
                       :pharmacy="pharmacy"
                       :is-my-nominated-pharmacy="true"
                       :previous-path="currentPage"
                       :can-change-pharmacy="showChangePharmacyLink" />
    </div>

    <generic-button id="continue-button-found"
                    :button-classes="['green', 'button']"
                    @click.prevent="onContinueButtonClicked">
      {{ getContinueButtonText }}
    </generic-button>

    <generic-button id="back-button"
                    :button-classes="['grey', 'button']" :class="$style.back"
                    tabindex="0" @click.prevent="onBackButtonClicked">
      {{ $t('nominatedPharmacyNotFound.backButton') }}
    </generic-button>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import PharmacyDetail from '@/components/nominatedPharmacy/PharmacyDetail';
import NoNominatedPharmacyWarning from '@/components/nominatedPharmacy/NoNominatedPharmacyWarning';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY_CHECK } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    PharmacyDetail,
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
        this.$t('nominatedPharmacy.continueButton');
    },
    showChangePharmacyLink() {
      if (this.pharmacy.pharmacyType !== PharmacyType.P3) {
        return true;
      }
      return false;
    },
  },
  created() {
    if (this.$store.state.nominatedPharmacy.hasLoaded === false || !this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled']) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    onContinueButtonClicked() {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES.path, null);
    },
    onBackButtonClicked() {
      redirectTo(this, PRESCRIPTIONS.path, null);
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
