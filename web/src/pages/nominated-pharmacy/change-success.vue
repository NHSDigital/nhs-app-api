<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>{{ $t('nominated_pharmacy.changeSuccess.header') }}</p>

        <pharmacy-change-success-details v-if="pharmacy !== null"
                                         id="pharmacy-summary"
                                         :pharmacy="pharmacy" />
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink id="to-prescriptions-link"
                                :path="prescriptionsPath"
                                :button-text="'nominated_pharmacy.changeSuccess.linkLabel'"
                                @clickAndPrevent="prescriptionsLinkClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable global-require */
import PharmacyChangeSuccessDetails from '@/components/nominatedPharmacy/PharmacyChangeSuccessDetails';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { PRESCRIPTIONS, NOMINATED_PHARMACY_CHANGE_SUCCESS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    PharmacyChangeSuccessDetails,
    DesktopGenericBackLink,
  },
  data() {
    return {
      pharmacy: null,
      currentPage: NOMINATED_PHARMACY_CHANGE_SUCCESS.path,
      prescriptionsPath: PRESCRIPTIONS.path,
    };
  },
  async mounted() {
    await this.$store.dispatch('nominatedPharmacy/clear');
    await this.$store.dispatch('nominatedPharmacy/load');
    this.pharmacy = this.$store.state.nominatedPharmacy.pharmacy;
    this.$store.dispatch('header/updateHeaderText', this.$t('pageHeaders.nominatedPharmacyChangeSuccess', { name: this.pharmacy.pharmacyName }));
    this.$store.dispatch('pageTitle/updatePageTitle', this.$t('pageTitles.nominatedPharmacyChangeSuccess', { name: this.pharmacy.pharmacyName }));
  },
  methods: {
    prescriptionsLinkClicked() {
      redirectTo(this, this.prescriptionsPath);
    },
  },
};
</script>
