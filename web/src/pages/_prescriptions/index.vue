<template>
  <main v-if="showTemplate" :class="$style.content">

    <div :class="$style['above-float-button']">

      <success-dialog v-if="justOrderedARepeatPrescription">
        <p>
          {{ $t('rp05.confirmationMessage') }}
        </p>
      </success-dialog>

      <div v-if="showNoPrescriptions" :class="$style.info" data-purpose="no-prescriptions-error">
        <h3>{{ $t('rp01.empty.subHeader') }}</h3>
        <p>
          {{ $t('rp01.empty.body') }}
        </p>
      </div>
      <ul v-if="showPrescriptions" data-purpose="prescriptions">
        <li
          v-for="(statusGroup, key) in prescriptionCoursesToDisplay"
          :key="key"
          :class="$style['prescription-course']">
          <div v-if="getMedicationCourseStatus(key) != null" :class="$style['panel-title']">
            <h2>{{ getMedicationCourseStatus(key) }}</h2>
          </div>
          <ul>
            <li
              v-for="(prescriptionCourse, key) in statusGroup"
              :key="key"
              :class="$style['prescription-course']"
              aria-label="historic-prescription">
              <historic-prescription :prescription-course="prescriptionCourse" />
            </li>
          </ul>
        </li>
      </ul>
    </div>

    <floating-button-bottom v-if="hasLoaded" @on-click="onRepeatPrescriptionButtonClicked">
      {{ $t('rp01.orderPrescriptionButton') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import SuccessDialog from '@/components/widgets/SuccessDialog';
import HistoricPrescription from '@/components/HistoricPrescription';
import { MedicationCourseStatus } from '@/lib/medication-course-status';
import _ from 'lodash';

export default {
  middleware: ['auth', 'meta'],
  components: {
    FloatingButtonBottom,
    SuccessDialog,
    HistoricPrescription,
  },
  data() {
    return {
      justOrderedARepeatPrescription: false,
      statusDisplayPriority: {
        [MedicationCourseStatus.Rejected]: 1,
        [MedicationCourseStatus.Requested]: 2,
        [MedicationCourseStatus.Approved]: 3,
      },
    };
  },
  computed: {
    showNoPrescriptions() {
      const { hasLoaded, prescriptionCourses } = this.$store.state.prescriptions;

      return hasLoaded
        && (prescriptionCourses === null || Object.keys(prescriptionCourses).length === 0);
    },
    showPrescriptions() {
      const { hasLoaded, prescriptionCourses } = this.$store.state.prescriptions;

      return hasLoaded && prescriptionCourses !== null
        && Object.keys(prescriptionCourses).length > 0;
    },
    prescriptionCoursesToDisplay() {
      const context = this;
      const keys = _.sortBy(
        _.keys(this.$store.state.prescriptions.prescriptionCourses),
        item => context.statusDisplayPriority[item],
      );

      const orderedMap = {};

      _.each(
        keys,
        (k) => {
          orderedMap[k] = context.$store.state.prescriptions.prescriptionCourses[k];
        },
      );

      return orderedMap;
    },
    hasLoaded() {
      return this.$store.state.prescriptions.hasLoaded;
    },
  },
  mounted() {
    this.$store.dispatch('prescriptions/clear');
    this.justOrderedARepeatPrescription =
      this.$store.state.repeatPrescriptionCourses.justOrderedARepeatPrescription;
    this.$store.dispatch('prescriptions/load', this.$config);

    this.$store.dispatch('errors/setApiErrorButtonPath', '');
  },
  methods: {
    onRepeatPrescriptionButtonClicked() {
      this.$router.push('/prescriptions/repeat-courses');
    },
  },
};
</script>

<style module lang="scss">
@import "../../style/html";
@import "../../style/elements";
@import "../../style/buttons";
@import "../../style/fonts";
@import "../../style/spacings";
@import "../../style/textstyles";
@import "../../style/colours";

.prescription-course {
  list-style: none;
  @include space(margin, bottom, $three);
}

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
</style>
