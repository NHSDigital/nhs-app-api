<template>
  <div v-if="showTemplate">
    <p class="nhsuk-u-margin-bottom-2"><strong>
      {{ $t('prescriptions.confirmDetails.medicines') }}</strong></p>
    <div v-for="selectedPrescription in selectedPrescriptions"
         :key="selectedPrescription.courseId"
         data-purpose="selected-prescription">
      <p class="nhsuk-u-margin-bottom-0" data-purpose="prescription-name">
        {{ selectedPrescription.name }}</p>
      <p data-purpose="prescription-description">{{ selectedPrescription.details }}</p>
    </div>
    <p><a id="changeRepeatPrescription"
          :aria-label="$t('prescriptions.confirmDetails.changeMedicines')"
          :href="coursesPageRepeatPrescription"
          @click.prevent="changePrescriptions">
      {{ $t('prescriptions.confirmDetails.change') }}
    </a>
    </p>
    <hr>
    <div v-if="specialRequestNecessity !== 'NotAllowed'">
      <p class="nhsuk-u-margin-bottom-2"><strong>
        {{ $t('prescriptions.confirmDetails.specialRequestsRelating') }}</strong></p>
      <p v-if="specialRequest"
         id="specialRequestText" :class="$style.wrapContent">{{ specialRequest }}
      </p>
      <p v-else id="specialRequestText">
        {{ $t('prescriptions.confirmDetails.specialRequestsNone') }}
      </p>
    </div>
    <p><a id="changeSpecialRequest"
          :aria-label="$t('prescriptions.confirmDetails.changeNotes')"
          :href="coursesPageSpecialRequest"
          @click.prevent="changeSpecialRequest">
      {{ $t('prescriptions.confirmDetails.change') }}
    </a>
    </p>
    <sjr-if journey="nominatedPharmacy">
      <div v-if="!hasNoNominatedPharmacy && !isProxying" id="my-nominated-pharmacy">
        <hr>
        <p class="nhsuk-u-margin-bottom-0">
          <strong>{{ pharmacyHeader }}</strong>
        </p>
        <pharmacy-summary id="pharmacy-summary"
                          :pharmacy="nominatedPharmacy"/>
      </div>
    </sjr-if>
    <generic-button id="btn_confirm_and_order_prescription"
                    class="nhsuk-button"
                    click-delay="medium"
                    @click="onConfirmButtonClicked">
      {{ $t('prescriptions.confirmDetails.confirmAndOrder') }}
    </generic-button>
    <div class="nhsuk-body-m">
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              :path="prescriptionRepeatCoursesPath"
                              :button-text="'prescriptions.confirmDetails.back'"
                              @clickAndPrevent="backToPrescriptionsClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import SjrIf from '@/components/SjrIf';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { redirectTo } from '@/lib/utils';
import {
  PRESCRIPTIONS_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH,
  PRESCRIPTIONS_ORDER_SUCCESS_PATH,
} from '@/router/paths';

const onSubmit = async (store, selectedCourseIds, specialRequest) => {
  const repeatPrescriptionOrder = {
    CourseIds: selectedCourseIds,
    SpecialRequest: specialRequest,
  };

  await store.dispatch('repeatPrescriptionCourses/orderRepeatPrescription', repeatPrescriptionOrder);
};

export default {
  name: 'ConfirmPrescriptionDetails',
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
      coursesPageRepeatPrescription: `${PRESCRIPTION_REPEAT_COURSES_PATH}#repeatPrescriptions`,
      coursesPageSpecialRequest: `${PRESCRIPTION_REPEAT_COURSES_PATH}#specialRequest`,
      isProxying: this.$store.getters['session/isProxying'],
    };
  },
  computed: {
    specialRequestNecessity() {
      return this.$store.state.repeatPrescriptionCourses
        .specialRequestNecessity;
    },
    prescriptionRepeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES_PATH;
    },
    confirmPrescriptionsPath() {
      return PRESCRIPTION_CONFIRM_COURSES_PATH;
    },
    pharmacyHeader() {
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P3) {
        return this.$t('prescriptions.confirmDetails.yourDispensingPractice');
      }
      return this.$t('prescriptions.confirmDetails.yourNominatedPharmacy');
    },
  },
  created() {
    if (!this.$store.getters['errors/showApiError'] &&
      (!this.selectedPrescriptions || this.selectedPrescriptions.length === 0)) {
      redirectTo(this, PRESCRIPTIONS_PATH);
    }
  },
  methods: {
    async onConfirmButtonClicked() {
      try {
        await onSubmit(
          this.$store,
          this.selectedPrescriptions.map(x => x.id),
          this.specialRequest,
        );

        if (this.$store.state.repeatPrescriptionCourses.partialOrderResult) {
          redirectTo(this, PRESCRIPTIONS_REPEAT_PARTIAL_SUCCESS_PATH, null);
        } else {
          redirectTo(this, PRESCRIPTIONS_ORDER_SUCCESS_PATH, null);
        }
      } catch (error) {
        /*
        empty catch block as the
        ApiError.vue (component) handles and
        surfaces appropriate error content based on the http status code returned from the API
        */
      }
    },
    backToPrescriptionsClicked() {
      redirectTo(this, this.prescriptionRepeatCoursesPath);
    },
    changePrescriptions() {
      redirectTo(this, this.coursesPageRepeatPrescription);
    },
    changeSpecialRequest() {
      redirectTo(this, this.coursesPageSpecialRequest);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/prescriptions-confirm-prescription-details";
</style>
