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
      <div class="nhsuk-u-margin-bottom-0" :class="$style['nhs-app-panel-heading']">
        <h2 class="nhsuk-heading-l">{{ $t('rp02.ordersTitle') }}</h2>
      </div>
      <div v-for="(prescriptionCourse, index) in prescriptionCoursesToDisplay"
           :key="index" :class="$style['list-menu']" data-label="historic-prescription">
        <historic-prescription :prescription-course="prescriptionCourse"
                               :class="$style['nhs-app-message']"
                               class="nhsuk-u-margin-bottom-0"/>
      </div>
    </div>
  </div></template>

<script>
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import HistoricPrescription from '@/components/HistoricPrescription';
import MedicationCourseStatus from '@/lib/medication-course-status';
import SjrIf from '@/components/SjrIf';
import isEmpty from 'lodash/fp/isEmpty';
import orderBy from 'lodash/fp/orderBy';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY_INTERRUPT } from '@/lib/routes';
import InterruptBackTo from '@/lib/pharmacy-detail/interrupt-back-to';
import sjrIf from '@/lib/sjrIf';

const loadData = async (store) => {
  store.dispatch('prescriptions/clear');
  await store.dispatch('prescriptions/load');

  if (sjrIf({ $store: store, journey: 'nominatedPharmacy' })) {
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
  },
  data() {
    return {
      statusDisplayPriority: {
        [MedicationCourseStatus.Requested]: 3,
        [MedicationCourseStatus.Approved]: 2,
        [MedicationCourseStatus.Rejected]: 1,
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
      const courses = this.$store.state.prescriptions.prescriptionCourses;
      courses.forEach((e) => {
        e.statusDisplayPriority = this.statusDisplayPriority[e.status];
      });
      return orderBy(['orderDate', 'statusDisplayPriority'], ['desc', 'asc'])(courses);
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
<style module lang="scss" scoped>
  @import '../../style/arrow';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/typography';
  @import '~nhsuk-frontend/packages/core/tools/typography';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';

  a {
    display: inline-block;
  }

  .list-menu {
    border-top: 1px #D8DDE0 solid;
    list-style: none;
    padding-left: 0;
    margin-bottom: .2em;
  }

  .list menu p {
    margin-bottom: 0;
    padding-right: 3em;
    color: #212b32;
  }

  .nhs-app-message {
    list-style: none;
    margin-bottom: nhsuk-spacing(3);
    border-top: 1px $nhsuk-border-color solid;
    @include govuk-media-query($until: desktop) {
      margin-left: (-$nhsuk-gutter-half);
      margin-right: (-$nhsuk-gutter-half);
    }
  }

  .nhs-app-message p {
    padding-top: 2px;
    padding-bottom: 0;
    color: $dark_grey;
  }

  .nhs-app-panel-heading {
    margin-bottom: 0;
    margin-top: 1em;

    @include mq($until: desktop) {
      margin: 1em -1em 0;
    }

    h1, h2, h3, h4, h5 {
      background-color: $color_nhsuk-white;
      padding: 0.5em 16px 0.5em 16px;
      border-top: 1px solid #d8dde0;
      margin-bottom: 0;
    }
  }
</style>
