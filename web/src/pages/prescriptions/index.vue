<template>
  <div v-if="showTemplate && hasLoaded">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
        <no-js-form :action="getContinueButtonPath()" method="get" :value="{}">
          <generic-button
            id="order-prescription-button"
            :button-classes="['nhsuk-button']"
            @click.stop.prevent="onOrderRepeatPrescriptionClicked">
            {{ $t('rp01.orderPrescriptionButton') }}
          </generic-button>
        </no-js-form>
      </div>
    </div>
    <sjr-if journey="nominatedPharmacy">
      <div v-if="showNominatedPharmacy" id="nominated-pharmacy-section">
        <menu-item-list>
          <menu-item id="nominated-pharmacy"
                     header-tag="h2"
                     data-purpose="text_link"
                     :text="pharmacyWidgetText"
                     :description="pharmacyName"
                     :click-func="onNominatedPharmacyDetailClicked"
                     :aria-label="ariaLabelCaption(
                       pharmacyWidgetText,
                       pharmacyName)"/>
        </menu-item-list>
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
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import NoJsForm from '@/components/no-js/NoJsForm';
import SjrIf from '@/components/SjrIf';
import { each, isEmpty, keys, sortBy } from 'lodash/fp';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY, NOMINATED_PHARMACY_SEARCH } from '@/lib/routes';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import GenericButton from '@/components/widgets/GenericButton';

const loadData = async (store) => {
  store.dispatch('prescriptions/clear');
  await store.dispatch('prescriptions/load');

  if (store.getters['serviceJourneyRules/nominatedPharmacyEnabled']) {
    store.dispatch('nominatedPharmacy/clear');
    await store.dispatch('nominatedPharmacy/load');
  }
};

export default {
  layout: 'nhsuk-layout',
  components: {
    HistoricPrescription,
    MenuItem,
    MenuItemList,
    NoJsForm,
    SjrIf,
    Card,
    CardGroupItem,
    CardGroup,
    GenericButton,
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
    nominatedPharmacyName() {
      return this.$store.getters['nominatedPharmacy/pharmacyName'];
    },
    pharmacyWidgetText() {
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyType === PharmacyType.P3) {
        return this.$t('rp04.dispensingPracticeHeader');
      }
      return this.$t('rp04.nominatedPharmacyHeader');
    },
  },
  watch: {
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
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY_SEARCH.path, true);
        redirectTo(this, NOMINATED_PHARMACY_SEARCH.path);
      } else {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY.path, true);
        redirectTo(this, NOMINATED_PHARMACY.path);
      }
    },
    onOrderRepeatPrescriptionClicked() {
      const path = this.getContinueButtonPath();
      this.$store.app.$analytics.trackButtonClick(path, true);
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
