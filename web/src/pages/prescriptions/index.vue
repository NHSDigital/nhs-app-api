<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
        <no-js-form :action="getContinueButtonPath()" method="get" :value="{}">
          <generic-button
            id="repeat-prescription-button"
            :button-classes="['nhsuk-button']"
            @click.stop.prevent="onOrderRepeatPrescriptionClicked">
            {{ $t('gpPrescriptionsHub.menuOptions.orderRepeat') }}
          </generic-button>
        </no-js-form>
        <menu-item-list>
          <menu-item
            id="view-orders"
            :text="$t('gpPrescriptionsHub.menuOptions.viewOrders')"
            :aria-label="ariaLabelCaption
              ($t('gpPrescriptionsHub.menuOptions.viewOrders'),
               $t('gpPrescriptionsHub.menuOptions.viewOrdersHelpText'))"
            :description="$t('gpPrescriptionsHub.menuOptions.viewOrdersHelpText')"
            :click-func="onViewOrdersClicked"
            :header-tag="'h2'"
          />
          <menu-item
            v-if="showNominatedPharmacy && !isProxying"
            id="nominated-pharmacy"
            :text="nominatedPharmacyText"
            :aria-label="ariaLabelCaption
              (nominatedPharmacyText,
               nominatedPharmacyDescription)"
            :description="nominatedPharmacyDescription"
            :click-func="onNominatedPharmacyDetailClicked"
            :header-tag="'h2'"
          />
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import MenuItemList from '@/components/MenuItemList';
import NoJsForm from '@/components/no-js/NoJsForm';
import GenericButton from '@/components/widgets/GenericButton';
import MenuItem from '@/components/MenuItem';
import GetNavigationPathFromPrescriptions from '@/lib/prescriptions/navigation';
import InterruptBackTo from '@/lib/pharmacy-detail/interrupt-back-to';
import { redirectTo } from '@/lib/utils';
import { NOMINATED_PHARMACY, NOMINATED_PHARMACY_INTERRUPT, PRESCRIPTIONS_VIEW_ORDERS } from '@/lib/routes';

const loadData = async (store) => {
  store.dispatch('repeatPrescriptionCourses/init');
  if (store.getters['serviceJourneyRules/nominatedPharmacyEnabled']) {
    store.dispatch('nominatedPharmacy/clearInterruptBackTo');

    if (store.state.nominatedPharmacy.hasLoaded === false) {
      store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  }
};

export default {
  layout: 'nhsuk-layout',
  name: 'Prescriptions',
  components: {
    MenuItemList,
    MenuItem,
    NoJsForm,
    GenericButton,
  },
  computed: {
    nominatedPharmacyName() {
      return this.$store.getters['nominatedPharmacy/pharmacyName'];
    },
    isProxying() {
      return this.$store.getters['session/isProxying'];
    },
    nominatedPharmacyText() {
      if (this.nominatedPharmacyName === undefined) {
        return this.$t('gpPrescriptionsHub.menuOptions.nominatePharmacy');
      }
      return this.$t('gpPrescriptionsHub.menuOptions.yourNominatedPharmacy');
    },
    nominatedPharmacyDescription() {
      if (this.nominatedPharmacyName === undefined) {
        return this.$t('gpPrescriptionsHub.menuOptions.nominatePharmacyHelpText');
      }
      return this.nominatedPharmacyName;
    },
    showNominatedPharmacy() {
      return this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled'];
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
    onOrderRepeatPrescriptionClicked() {
      const path = this.getContinueButtonPath();
      this.$store.app.$analytics.trackButtonClick(path, true);
      this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.NOMINATED_PHARMACY_CHECK);
      redirectTo(this, path);
    },
    getContinueButtonPath() {
      return GetNavigationPathFromPrescriptions(this.$store);
    },
    onViewOrdersClicked() {
      redirectTo(this, PRESCRIPTIONS_VIEW_ORDERS.path);
    },
    onNominatedPharmacyDetailClicked() {
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY_INTERRUPT.path, true);
        this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.PRESCRIPTIONS);
        redirectTo(this, NOMINATED_PHARMACY_INTERRUPT.path);
      } else {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY.path, true);
        this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.NOMINATED_PHARMACY_SUMMARY);
        redirectTo(this, NOMINATED_PHARMACY.path);
      }
    },
    ariaLabelCaption(header, body) {
      return `${header}. ${body}`;
    },
  },
};
</script>
