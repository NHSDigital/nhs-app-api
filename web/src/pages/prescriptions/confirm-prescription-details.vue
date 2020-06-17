<template>
  <div v-if="showTemplate">
    <div data-purpose="info">
      <p class="nhsuk-body nhsuk-u-padding-bottom-3">
        {{ $t('rp04.subHeader') }}
      </p>
    </div>
    <CardGroup role="list" class="nhsuk-grid-row">
      <CardGroupItem class="nhsuk-grid-column-full">
        <Card>
          <div v-for="selectedPrescription in selectedPrescriptions"
               :key="selectedPrescription.courseId"
               data-purpose="selected-prescription">
            <strong class="nhsuk-u-margin-bottom-0" data-purpose="prescription-name">
              {{ selectedPrescription.name }}</strong>
            <p data-purpose="prescription-description">{{ selectedPrescription.details }}</p>
          </div>
          <hr>
          <div v-if="specialRequestNecessity !== 'NotAllowed'">
            <strong class="nhsuk-u-margin-bottom-0">{{ $t('rp04.specialRequestsLabel') }}</strong>
            <p v-if="specialRequest"
               id="specialRequestText" :class="$style.wrapContent">{{ specialRequest }}
            </p>
            <p v-else id="specialRequestText">
              {{ $t('rp03.noSpecialRequestDefaultText') }}
            </p>
          </div>
          <sjr-if journey="nominatedPharmacy">
            <div v-if="!hasNoNominatedPharmacy" id="my-nominated-pharmacy">
              <hr>
              <strong class="nhsuk-u-margin-bottom-0">{{ pharmacyHeader }}</strong>
              <pharmacy-summary id="pharmacy-summary"
                                :pharmacy="nominatedPharmacy"/>
            </div>
          </sjr-if>
        </Card>
      </CardGroupItem>
    </CardGroup>
    <div>
      <no-js-form :action="confirmPrescriptionsPath" :value="{}" method="post">
        <generic-button id="btn_confirm_and_order_prescription"
                        class="nhsuk-button"
                        click-delay="medium"
                        @click="onConfirmButtonClicked">
          {{ $t('rp04.confirmButton') }}
        </generic-button>
      </no-js-form>
    </div>
    <div class="nhsuk-body-m">
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              :path="prescriptionRepeatCoursesPath"
                              :button-text="'rp04.backButton'"
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
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';
import NoJsForm from '@/components/no-js/NoJsForm';

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
    Card,
    CardGroupItem,
    CardGroup,
    NoJsForm,
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
      return PRESCRIPTION_REPEAT_COURSES_PATH;
    },
    confirmPrescriptionsPath() {
      return PRESCRIPTION_CONFIRM_COURSES_PATH;
    },
    pharmacyHeader() {
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P3) {
        return this.$t('rp04.dispensingPracticeHeader');
      }
      return this.$t('rp04.nominatedPharmacyHeader');
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
  },
};
</script>

<style module lang="scss" scoped>
 hr {
  margin: 0.5em auto 0.5em;
 }
 .wrapContent {
   word-wrap: break-word;
 }
</style>
