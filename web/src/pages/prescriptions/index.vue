<template>

  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content']" >

    <glossary-header v-if="hasLoaded"/>

    <div v-if="showNoPrescriptions" :class="$style.info" data-purpose="no-prescriptions-error">
      <h2>{{ $t('rp01.empty.subHeader') }}</h2>
      <p>
        {{ $t('rp01.empty.line1') }}
      </p>
      <p>
        {{ $t('rp01.empty.line2') }}
      </p>
    </div>
    <div v-if="showPrescriptions" data-purpose="prescriptions">
      <div
        v-for="(statusGroup, key) in prescriptionCoursesToDisplay"
        :key="key">
        <div v-if="getMedicationCourseStatus(key) != null">
          <h2>{{ getMedicationCourseStatus(key) }}</h2>
        </div>
        <div
          v-for="(prescriptionCourse, key) in statusGroup"
          :key="key"
          data-label="historic-prescription">
          <historic-prescription :prescription-course="prescriptionCourse" />
        </div>
      </div>
    </div>

    <floating-button-bottom
      v-if="hasLoaded"
      id="order-prescription-button"
      @click="onRepeatPrescriptionButtonClicked">
      {{ $t('rp01.orderPrescriptionButton') }}
    </floating-button-bottom>
  </div>
</template>

<script>
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import HistoricPrescription from '@/components/HistoricPrescription';
import MedicationCourseStatus from '@/lib/medication-course-status';
import GlossaryHeader from '@/components/GlossaryHeader';
import _ from 'lodash';

export default {
  components: {
    FloatingButtonBottom,
    HistoricPrescription,
    GlossaryHeader,
  },
  data() {
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Rejected]: 1,
        [MedicationCourseStatus.Requested]: 2,
        [MedicationCourseStatus.Approved]: 3,
      },
    };
  },
  computed: {
    showNoPrescriptions() {
      const {
        hasLoaded,
        prescriptionCourses,
      } = this.$store.state.prescriptions;

      return (
        hasLoaded &&
        (prescriptionCourses === null ||
          Object.keys(prescriptionCourses).length === 0)
      );
    },
    showPrescriptions() {
      const {
        hasLoaded,
        prescriptionCourses,
      } = this.$store.state.prescriptions;

      return (
        hasLoaded &&
        prescriptionCourses !== null &&
        Object.keys(prescriptionCourses).length > 0
      );
    },
    prescriptionCoursesToDisplay() {
      const context = this;
      const keys = _.sortBy(
        _.keys(this.$store.state.prescriptions.prescriptionCourses),
        item => context.statusDisplayPriority[item],
      );

      const orderedMap = {};

      _.each(keys, (k) => {
        orderedMap[k] =
          context.$store.state.prescriptions.prescriptionCourses[k];
      });

      return orderedMap;
    },
    hasLoaded() {
      return this.$store.state.prescriptions.hasLoaded;
    },
  },
  mounted() {
    this.$store.dispatch('prescriptions/clear');
    this.$store.dispatch('prescriptions/load', this.$config);
  },
  methods: {
    onRepeatPrescriptionButtonClicked() {
      this.$router.push('/prescriptions/repeat-courses');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/spacings";

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
</style>
