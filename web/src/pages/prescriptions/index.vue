<!-- eslint-disable vue/no-v-html -->
<!-- eslint-disable vue/no-template-shadow -->
<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content' ,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]" >
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
      <div v-for="(statusGroup, key) in prescriptionCoursesToDisplay"
           :key="key">
        <div v-if="getMedicationCourseStatus(key) != null">
          <h2>{{ getMedicationCourseStatus(key) }}</h2>
        </div>
        <div v-for="(prescriptionCourse, key) in statusGroup"
             :key="key"
             data-label="historic-prescription">
          <historic-prescription :prescription-course="prescriptionCourse" />
        </div>
      </div>
    </div>

    <form v-if="$store.state.device.isNativeApp" :action="repeatCoursesPath" method="get">
      <floating-button-bottom v-if="hasLoaded"
                              id="order-prescription-button"
                              @click="onRepeatPrescriptionButtonClicked">
        {{ $t('rp01.orderPrescriptionButton') }}
      </floating-button-bottom>
    </form>
  </div>
</template>

<script>
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import HistoricPrescription from '@/components/HistoricPrescription';
import GlossaryHeader from '@/components/GlossaryHeader';
import { PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import MedicationCourseStatus from '@/lib/medication-course-status';
import keys from 'lodash/fp/keys';
import each from 'lodash/fp/each';
import sortBy from 'lodash/fp/sortBy';
import isEmpty from 'lodash/fp/isEmpty';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    FloatingButtonBottom,
    HistoricPrescription,
    GlossaryHeader,
  },
  computed: {
    repeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES.path;
    },
    showNoPrescriptions() {
      const {
        hasLoaded,
        prescriptionCourses,
      } = this.$store.state.prescriptions;
      return hasLoaded && isEmpty(prescriptionCourses);
    },
    showPrescriptions() {
      const {
        hasLoaded,
        prescriptionCourses,
      } = this.$store.state.prescriptions;
      return hasLoaded && !isEmpty(prescriptionCourses);
    },
    prescriptionCoursesToDisplay() {
      const prescriptionKeys =
        sortBy(i =>
          this.statusDisplayPriority[i])(keys(this.$store.state.prescriptions.prescriptionCourses));

      const orderedMap = {};
      each((k) => {
        orderedMap[k] =
          this.$store.state.prescriptions.prescriptionCourses[k];
      })(prescriptionKeys);
      return orderedMap;
    },
    hasLoaded() {
      return this.$store.state.prescriptions.hasLoaded;
    },
  },
  async asyncData({ store }) {
    await store.dispatch('prescriptions/clear');
    await store.dispatch('prescriptions/load');
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Rejected]: 1,
        [MedicationCourseStatus.Requested]: 2,
        [MedicationCourseStatus.Approved]: 3,
      },
    };
  },
  created() {
  },
  methods: {
    onRepeatPrescriptionButtonClicked(e) {
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES.path, null);
      e.preventDefault();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/spacings";
@import "../../style/fonts";

.desktopWeb {
   font-family: $default-web;
   max-width: 540px + 64px; // 64px (2em) to compasensate padding apply to the div by main.scss
  p {
   font-family: $default-web;
   font-weight: lighter;
   max-width: 540px;
  }
 }
.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
</style>
