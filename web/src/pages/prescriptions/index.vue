<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <no-js-form :action="getContinueButtonPath()" method="get" :value="{}">
          <button v-if="hasLoaded"
                  id="order-prescription-button"
                  class="nhsuk-button"
                  @click.stop.prevent="onOrderRepeatPrescriptionClicked">
            {{ $t('rp01.orderPrescriptionButton') }}
          </button>
        </no-js-form>
      </div>
    </div>
    <sjr-if journey="nominatedPharmacy">
      <div v-if="showNominatedPharmacy" id="nominated-pharmacy-section">
        <ul :class="$style['list-menu-white']" role="list">
          <li :class="$style.link" role="link">
            <analytics-tracked-tag
              id="nominated-pharmacy"
              :click-func="onNominatedPharmacyDetailClicked"
              :text="$t('rp01.nominatedPharmacy')"
              :aria-label="`${$t('rp01.nominatedPharmacy')}. ${$t('rp01.nominatedPharmacy')}`"
              tag="a">
              <h3 :aria-label="$t('rp01.nominatedPharmacy')">{{ $t('rp01.nominatedPharmacy') }}</h3>
              <p id="pharmacy-name">
                {{ pharmacyName }}
              </p>
            </analytics-tracked-tag>
          </li>
        </ul>
      </div>
    </sjr-if>
    <div v-if="showNoPrescriptions"
         data-purpose="no-prescriptions-error"
         class="nhsuk-u-padding-bottom-6">
      <h2>{{ $t('rp01.empty.subHeader') }}</h2>
      <p class="nhsuk-u-padding-bottom-2">
        {{ $t('rp01.empty.line1') }}
      </p>
      <p class="nhsuk-u-padding-bottom-2">
        {{ $t('rp01.empty.line2') }}
      </p>
    </div>
    <div v-if="showPrescriptions" data-purpose="prescriptions">
      <div v-for="(statusGroup, key) in prescriptionCoursesToDisplay"
           :key="key">
        <div v-if="getMedicationCourseStatus(key) != null">
          <h2>{{ getMedicationCourseStatus(key) }}</h2>
        </div>

        <CardGroup v-for="(prescriptionCourse, index) in statusGroup"
                   :key="index" role="list" class="nhsuk-grid-row">
          <CardGroupItem class="nhsuk-grid-column-full">
            <Card data-label="historic-prescription">
              <historic-prescription :prescription-course="prescriptionCourse" />
            </Card>
          </CardGroupItem>
        </CardGroup>

      </div>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import HistoricPrescription from '@/components/HistoricPrescription';
import MedicationCourseStatus from '@/lib/medication-course-status';
import NoJsForm from '@/components/no-js/NoJsForm';
import SjrIf from '@/components/SjrIf';
import { each, isEmpty, keys, sortBy } from 'lodash/fp';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY } from '@/lib/routes';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    HistoricPrescription,
    NoJsForm,
    SjrIf,
    Card,
    CardGroupItem,
    CardGroup,
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
  mounted() {
    if (this.$store.state.prescriptions.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
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
@import '../../style/listmenu';
</style>
