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
            <b data-purpose="prescription-name">{{ selectedPrescription.name }}</b>
            <p data-purpose="prescription-description">{{ selectedPrescription.details }}</p>
          </div>
          <hr>
          <div v-if="specialRequestNecessity !== 'NotAllowed'">
            <b>{{ $t('rp04.specialRequestsLabel') }}</b>
            <p v-if="specialRequest"
               id="specialRequestText">{{ specialRequest }}
            </p>
            <p v-else id="specialRequestText">
              {{ $t('rp03.noSpecialRequestDefaultText') }}
            </p>
          </div>
          <sjr-if journey="nominatedPharmacy">
            <div v-if="!hasNoNominatedPharmacy" id="my-nominated-pharmacy">
              <hr>
              <b>{{ $t('rp04.nominatedPharmacyHeader') }}</b>
              <pharmacy-summary id="pharmacy-summary"
                                :pharmacy="nominatedPharmacy"
                                :pharmacy-name-as-header="false" />
            </div>
          </sjr-if>
        </Card>
      </CardGroupItem>
    </CardGroup>

    <div>
      <button id="btn_confirm_and_order_prescription"
              class="nhsuk-button"
              click-delay="medium"
              @click="onConfirmButtonClicked">
        {{ $t('rp04.confirmButton') }}
      </button>
    </div>
    <div>
      <generic-button v-if="$store.state.device.isNativeApp"
                      id="back-to-prescriptions"
                      :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                      @click="backToPrescriptionsClicked">
        {{ $t('rp04.backButton') }}
      </generic-button>
      <desktopGenericBackLink v-else
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
import { redirectTo } from '@/lib/utils';
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    GenericButton,
    PharmacySummary,
    SjrIf,
    Card,
    CardGroupItem,
    CardGroup,
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
 hr {
  margin: 0.5em auto 0.5em;
 }
</style>
