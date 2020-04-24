<template>
  <div v-if="showTemplate">
    <sjr-if journey="nominatedPharmacy">
      <div v-if="showNominatedPharmacy" id="nominated-pharmacy-section">
        <div v-if="nominatedPharmacyName" id="nominated-pharmacy">
          <p id="pharmacy-description" class="nhsuk-u-margin-bottom-2">
            {{ $t('rp01.nominatedPharmacy') }} </p>
          <p id="pharmacy-name" class="nhsuk-u-margin-bottom-2"> {{ nominatedPharmacyName }}</p>
        </div>
        <p v-else id="no-nominated-pharmacy" class="nhsuk-u-margin-bottom-2">
          {{ $t('rp01.noNominatedPharmacy') }}
        </p>
        <p> <a id="change-link" href="#" @click="onNominatedPharmacyDetailClicked">
          {{ $t('rp01.changePharmacyLink') }} </a></p>
        <span v-if="nominatedPharmacyName" class="nhsuk-u-visually-hidden">
          {{ $t('rp01.hiddenText.nominatedPharmacy') }}
        </span>
        <span v-else class="nhsuk-u-visually-hidden">
          {{ $t('rp01.hiddenText.noNominatedPharmacy') }}
        </span>
      </div>
    </sjr-if>
    <div v-if="showNoPrescriptions"
         id="show-no-prescription"
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
          <h2 class="nhsuk-u-padding-top-0 nhsuk-u-margin-bottom-2">
            {{ getMedicationCourseStatus(key) }}
          </h2>
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
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import HistoricPrescription from '@/components/HistoricPrescription';
import MedicationCourseStatus from '@/lib/medication-course-status';
import SjrIf from '@/components/SjrIf';
import each from 'lodash/fp/each';
import isEmpty from 'lodash/fp/isEmpty';
import keys from 'lodash/fp/keys';
import sortBy from 'lodash/fp/sortBy';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_INTERRUPT } from '@/lib/routes';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';
import InterruptBackTo from '@/lib/pharmacy-detail/interrupt-back-to';

const loadData = async (store) => {
  store.dispatch('prescriptions/clear');
  await store.dispatch('prescriptions/load');

  if (store.getters['serviceJourneyRules/nominatedPharmacyEnabled']) {
    store.dispatch('nominatedPharmacy/clearInterruptBackTo');

    if (store.state.nominatedPharmacy.hasLoaded === false) {
      store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  }
};

export default {
  name: 'ViewOrders',
  layout: 'nhsuk-layout',
  components: {
    HistoricPrescription,
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
    nominatedPharmacyName() {
      return this.$store.getters['nominatedPharmacy/pharmacyName'];
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      loadData(this.$store);
    },
    hasLoaded() {
      if (this.hasLoaded) {
        this.$store.dispatch('flashMessage/show');
      }
    },
  },
  async fetch({ store }) {
    if (process.server) {
      await loadData(store);
    }
  },
  mounted() {
    if (process.client) {
      loadData(this.$store);
    }
    if (this.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
  },
  methods: {
    onNominatedPharmacyDetailClicked() {
      this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY_INTERRUPT.path, true);
      this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.PRESCRIPTIONS);
      redirectTo(this, NOMINATED_PHARMACY_INTERRUPT.path);
    },
    onOrderRepeatPrescriptionClicked() {
      const path = this.getContinueButtonPath();
      this.$store.app.$analytics.trackButtonClick(path, true);
      this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.NOMINATED_PHARMACY_CHECK);
      redirectTo(this, path);
    },
    getContinueButtonPath() {
      return GetNavigationPathFromPrescriptions(this.$store);
    },
    ariaLabelCaption(header, body) {
      return `${header}. ${body}`;
    },
  },
};
</script>
<style scoped>
  a {
    display: inline-block;
  }
</style>
