<!-- eslint-disable vue/no-v-html -->
<!-- eslint-disable vue/no-template-shadow -->
<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content' ,
                                    !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <glossary-header v-if="hasLoaded"/>
    <ul :class="$style['list-menu-white']" role="list">
      <li role="link">
        <analytics-tracked-tag
          id="btn_choices" :click-func="onNominatedPharmacyDetailClicked"
          :class="$style['no-decoration']"
          :text="$t('rp01.nominatedPharmacy')"
          :aria-label="`${$t('rp01.nominatedPharmacy')}. ${$t('rp01.nominatedPharmacy')}`"
          tag="a">
          <h3 :aria-label="$t('rp01.nominatedPharmacy')">{{ $t('rp01.nominatedPharmacy') }}</h3>
          <p :class="!$store.state.device.isNativeApp && $style.desktopWeb">
            {{ nominatedPharmacyName }}
          </p>
        </analytics-tracked-tag>
      </li>
    </ul>
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

    <no-js-form v-if="$store.state.device.isNativeApp"
                :action="repeatCoursesPath" method="get"
                :value="{}">
      <floating-button-bottom v-if="hasLoaded"
                              id="order-prescription-button"
                              @click.stop.prevent="onOrderRepeatPrescriptionClicked">
        {{ $t('rp01.orderPrescriptionButton') }}
      </floating-button-bottom>
    </no-js-form>
  </div>
</template>

<script>
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import HistoricPrescription from '@/components/HistoricPrescription';
import GlossaryHeader from '@/components/GlossaryHeader';
import { PRESCRIPTION_REPEAT_COURSES, NOMINATED_PHARMACY } from '@/lib/routes';
import MedicationCourseStatus from '@/lib/medication-course-status';
import keys from 'lodash/fp/keys';
import each from 'lodash/fp/each';
import sortBy from 'lodash/fp/sortBy';
import isEmpty from 'lodash/fp/isEmpty';
import { redirectTo } from '@/lib/utils';
import NoJsForm from '@/components/no-js/NoJsForm';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  components: {
    FloatingButtonBottom,
    HistoricPrescription,
    GlossaryHeader,
    NoJsForm,
    AnalyticsTrackedTag,
  },
  async asyncData({ store }) {
    await store.dispatch('prescriptions/clear');
    await store.dispatch('nominatedPharmacy/clear');
    await store.dispatch('prescriptions/load');
    await store.dispatch('nominatedPharmacy/load');
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Rejected]: 1,
        [MedicationCourseStatus.Requested]: 2,
        [MedicationCourseStatus.Approved]: 3,
      },
      nominatedPharmacyName: store.state.nominatedPharmacy.pharmacy.pharmacyName,
    };
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
  methods: {
    onOrderRepeatPrescriptionClicked() {
      redirectTo(this, this.repeatCoursesPath, null);
    },
    onNominatedPharmacyDetailClicked() {
      redirectTo(this, NOMINATED_PHARMACY.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/spacings";
@import '../../style/listmenu';
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
