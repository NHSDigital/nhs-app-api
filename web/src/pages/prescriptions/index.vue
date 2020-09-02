<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
        <no-js-form :action="getContinueButtonPath()" method="get" :value="{}">
          <generic-button
            id="repeat-prescription-button"
            :button-classes="['nhsuk-button']"
            @click.stop.prevent="onOrderRepeatPrescriptionClicked">
            {{ $t('prescriptions.hub.orderARepeatPrescription') }}
          </generic-button>
        </no-js-form>
        <menu-item-list>
          <menu-item
            id="view-orders"
            :text="$t('prescriptions.hub.viewYourOrders')"
            :href="viewOrdersPath"
            :aria-label="ariaLabelCaption
              ($t('prescriptions.hub.viewYourOrders'),
               $t('prescriptions.hub.seeRepeatPrescriptionsYouHaveOrdered'))"
            :description="$t('prescriptions.hub.seeRepeatPrescriptionsYouHaveOrdered')"
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
          <third-party-jump-off-button
            v-if="showPkbMedicines"
            id="btn_pkb_medicines"
            provider-id="pkb"
            :provider-configuration="thirdPartyProvider.pkb.medicines" />
          <third-party-jump-off-button
            v-if="showPkbCieMedicines"
            id="btn_pkb_cie_medicines"
            provider-id="pkb"
            :provider-configuration="thirdPartyProvider.pkb.medicinesCie" />
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
import { NOMINATED_PHARMACY_PATH, NOMINATED_PHARMACY_INTERRUPT_PATH, PRESCRIPTIONS_VIEW_ORDERS_PATH } from '@/router/paths';
import ThirdPartyJumpOffButton from '@/components/ThirdPartyJumpOffButton';
import jumpOffProperties from '@/lib/third-party-providers/jump-off-configuration';
import sjrIf from '@/lib/sjrIf';

const loadData = async (store) => {
  store.dispatch('repeatPrescriptionCourses/init');
  if (sjrIf({ $store: store, journey: 'nominatedPharmacy' })) {
    store.dispatch('nominatedPharmacy/clearInterruptBackTo');

    if (store.state.nominatedPharmacy.hasLoaded === false) {
      store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  }
};

export default {
  name: 'PrescriptionIndexPage',
  components: {
    MenuItemList,
    MenuItem,
    NoJsForm,
    GenericButton,
    ThirdPartyJumpOffButton,
  },
  data() {
    return {
      thirdPartyProvider: jumpOffProperties.thirdPartyProvider,
      isProxying: this.$store.getters['session/isProxying'],
    };
  },
  computed: {
    nominatedPharmacyName() {
      return this.$store.getters['nominatedPharmacy/pharmacyName'];
    },
    nominatedPharmacyText() {
      if (this.nominatedPharmacyName === undefined) {
        return this.$t('prescriptions.hub.nominateAPharmacy');
      }
      return this.$t('prescriptions.hub.yourNominatedPharmacy');
    },
    nominatedPharmacyDescription() {
      if (this.nominatedPharmacyName === undefined) {
        return this.$t('prescriptions.hub.chooseAPharmacyForYourPrescriptions');
      }
      return this.nominatedPharmacyName;
    },
    showNominatedPharmacy() {
      return this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled'];
    },
    hasPkbMedicines() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkb',
          serviceType: 'medicines',
        },
      });
    },
    hasPkbCieMedicines() {
      return sjrIf({
        $store: this.$store,
        journey: 'silverIntegration',
        context: {
          provider: 'pkbCie',
          serviceType: 'medicines',
        },
      });
    },
    showPkbMedicines() {
      return !this.isProxying && this.hasPkbMedicines;
    },
    showPkbCieMedicines() {
      return !this.isProxying && this.hasPkbCieMedicines;
    },
    viewOrdersPath() {
      return PRESCRIPTIONS_VIEW_ORDERS_PATH;
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
  mounted() {
    loadData(this.$store);

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
      redirectTo(this, this.viewOrdersPath);
    },
    onNominatedPharmacyDetailClicked() {
      if (this.$store.state.nominatedPharmacy.pharmacy.pharmacyName === undefined) {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY_INTERRUPT_PATH, true);
        this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.PRESCRIPTIONS);
        redirectTo(this, NOMINATED_PHARMACY_INTERRUPT_PATH);
      } else {
        this.$store.app.$analytics.trackButtonClick(NOMINATED_PHARMACY_PATH, true);
        this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.NOMINATED_PHARMACY_SUMMARY);
        redirectTo(this, NOMINATED_PHARMACY_PATH);
      }
    },
    ariaLabelCaption(header, body) {
      return `${header}. ${body}`;
    },
  },
};
</script>
