<template>
  <div v-if="showTemplate" :class="[$style['above-float-button'], 'pull-content',
                                    !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <sjr-if journey="nominatedPharmacy">
      <div v-if="showNominatedPharmacy" id="nominated-pharmacy-section">
        <ul :class="$style['list-menu-white']" role="list">
          <li :class="$style.link" role="link">
            <analytics-tracked-tag
              id="nominated-pharmacy"
              :click-func="onNominatedPharmacyDetailClicked"
              :class="$style['no-decoration']"
              :text="$t('rp01.nominatedPharmacy')"
              :aria-label="`${$t('rp01.nominatedPharmacy')}. ${$t('rp01.nominatedPharmacy')}`"
              tag="a">
              <h3 :aria-label="$t('rp01.nominatedPharmacy')">{{ $t('rp01.nominatedPharmacy') }}</h3>
              <p id="pharmacy-name" :class="!$store.state.device.isNativeApp
                && $style.desktopWeb">
                {{ pharmacyName }}
              </p>
            </analytics-tracked-tag>
          </li>
        </ul>
      </div>
    </sjr-if>
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
        <div v-for="(prescriptionCourse, index) in statusGroup"
             :key="index"
             data-label="historic-prescription">
          <historic-prescription :prescription-course="prescriptionCourse" />
        </div>
      </div>
    </div>

    <no-js-form v-if="$store.state.device.isNativeApp"
                :action="getContinueButtonPath()" method="get" :value="{}">
      <floating-button-bottom v-if="hasLoaded"
                              id="order-prescription-button"
                              @click.stop.prevent="onOrderRepeatPrescriptionClicked">
        {{ $t('rp01.orderPrescriptionButton') }}
      </floating-button-bottom>
    </no-js-form>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import FloatingButtonBottom from '@/components/widgets/FloatingButtonBottom';
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import HistoricPrescription from '@/components/HistoricPrescription';
import MedicationCourseStatus from '@/lib/medication-course-status';
import NoJsForm from '@/components/no-js/NoJsForm';
import SjrIf from '@/components/SjrIf';
import { each, isEmpty, keys, sortBy } from 'lodash/fp';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY } from '@/lib/routes';

export default {
  components: {
    AnalyticsTrackedTag,
    FloatingButtonBottom,
    HistoricPrescription,
    NoJsForm,
    SjrIf,
  },
  data() {
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Requested]: 1,
        [MedicationCourseStatus.Approved]: 2,
        [MedicationCourseStatus.Rejected]: 3,
      },
    };
  },
  computed: {
    pharmacyName() {
      if (this.nominatedPharmacyName === undefined) {
        return this.$t('nominatedPharmacyNotFound.noPharmacyButton');
      }
      return this.nominatedPharmacyName;
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
    showNominatedPharmacy() {
      return this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled'];
    },
  },
  async asyncData({ store }) {
    await store.dispatch('prescriptions/clear');
    await store.dispatch('nominatedPharmacy/clear');
    await store.dispatch('prescriptions/load');
    await store.dispatch('nominatedPharmacy/load');
    return {
      nominatedPharmacyName: store.state.nominatedPharmacy.pharmacy.pharmacyName,
    };
  },
  methods: {
    onNominatedPharmacyDetailClicked() {
      this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY.path, true);
      redirectTo(this, NOMINATED_PHARMACY.path, null);
    },
    onOrderRepeatPrescriptionClicked() {
      const path = this.getContinueButtonPath();
      this.$store.app.$analytics.trackButtonClick(path, true);
      redirectTo(this, path, null);
    },
    getContinueButtonPath() {
      return GetNavigationPathFromPrescriptions(this.$store);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/spacings";
@import '../../style/listmenu';
@import "../../style/fonts";

div {
  &.desktopWeb {
  max-width: 540px;
  p {
    font-family: $default_web;
    font-weight: normal;
    }
  }
}
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
