<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">

    <div :class="$style.info" data-purpose="info">
      <p>
        {{ $t('rp04.subHeader') }}
      </p>
    </div>
    <div :class="$style.panel">
      <div v-for="selectedPrescription in selectedPrescriptions"
           :key="selectedPrescription.courseId"
           data-purpose="selected-prescription">
        <b data-purpose="prescription-name">{{ selectedPrescription.name }}</b>
        <p data-purpose="prescription-description">{{ selectedPrescription.details }}</p>
      </div>
      <hr>
      <div v-if="specialRequestNecessity !== 'NotAllowed'">
        <b>{{ $t('rp04.specialRequestsLabel') }}</b>
        <p v-if="specialRequest"
           id="specialRequestText"
           :class="$style['keep-line-breaks']">{{ specialRequest }}
        </p>
        <p v-else id="specialRequestText">
          {{ $t('rp03.noSpecialRequestDefaultText') }}
        </p>
      </div>
      <sjr-if journey="nominatedPharmacy">
        <div v-if="!hasNoNominatedPharmacy" id="my-nominated-pharmacy">
          <hr>
          <b :class="$style.pharmacyHeader">{{ $t('rp04.nominatedPharmacyHeader') }}</b>
          <pharmacy-summary id="pharmacy-summary"
                            :pharmacy="nominatedPharmacy"
                            :pharmacy-name-as-header="false" />
        </div>
      </sjr-if>
    </div>

    <generic-button id="btn_confirm_and_order_prescription"
                    :button-classes="['button' , 'green',
                                      !$store.state.device.isNativeApp && 'medium']"
                    click-delay="medium"
                    @click="onConfirmButtonClicked">
      {{ $t('rp04.confirmButton') }}
    </generic-button>

    <generic-button v-if="$store.state.device.isNativeApp"
                    id="back-to-prescriptions"
                    :button-classes="['button' , 'grey']"
                    @click="backToPrescriptionsClicked">
      {{ $t('rp04.backButton') }}
    </generic-button>
    <desktopGenericBackLink v-else
                            :path="prescriptionRepeatCoursesPath"
                            :button-text="'rp04.backButton'"
                            @clickAndPrevent="backToPrescriptionsClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import SjrIf from '@/components/SjrIf';
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';

export default {
  components: {
    DesktopGenericBackLink,
    GenericButton,
    PharmacySummary,
    SjrIf,
  },
  data() {
    return {
      selectedPrescriptions: this.$store.getters['repeatPrescriptionCourses/selectedPrescriptions'],
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest,
      nominatedPharmacy: this.$store.state.nominatedPharmacy.pharmacy,
    };
  },
  computed: {
    specialRequestNecessity() {
      return this.$store.state.repeatPrescriptionCourses
        .specialRequestNecessity;
    },
    prescriptionRepeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES.path;
    },
  },
  created() {
    if (this.selectedPrescriptions === null || this.selectedPrescriptions.length === 0) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    async onConfirmButtonClicked() {
      const repeatPrescriptionOrder = {
        CourseIds: this.selectedPrescriptions.map(x => x.id),
        SpecialRequest: this.specialRequest,
      };
      try {
        await this.$store.dispatch('repeatPrescriptionCourses/orderRepeatPrescription', repeatPrescriptionOrder);
        this.$store.dispatch('flashMessage/addSuccess', this.$t('rp05.confirmationMessage'));
        redirectTo(this, PRESCRIPTIONS.path, null);
      } catch (error) {
        /*
        empty catch block as the
        ApiError.vue (component) handles and
        surfaces appropriate error content based on the http status code returned from the API
        */
      }
    },
    backToPrescriptionsClicked() {
      redirectTo(this, this.prescriptionRepeatCoursesPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/forms";
  @import "../../style/info";
  @import "../../style/panels";

.pull-content {
    &.desktopWeb {
      font-family: $frutiger-light;
      &>* {
        max-width: 540px;
      }
    }
    .panel {
      margin-bottom: 1em;
    }
    .pharmacyHeader {
      font-size: 1.2em;
    }
    hr {
      opacity: unset;
    }
    .keep-line-breaks {
    white-space: pre-line;
  }
  }

</style>
