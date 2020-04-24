<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p>{{ $t('nominated_pharmacy.changeSuccess.header') }}</p>
        <div v-if="pharmacy !== null">
          <div v-if="isHighStreetSelected">
            <pharmacy-change-success-details id="pharmacy-summary"
                                             :pharmacy="pharmacy" />
          </div>
          <div v-else-if="isOnlineOnlySelected">
            <online-only-pharmacy-detail id="online-pharmacy-summary"
                                         :pharmacy="pharmacy"/>

            <h2 id="what-happens-next">
              {{ $t('nominated_pharmacy.changeSuccess.whatHappensNext') }}</h2>
            <p v-if="pharmacy.url" id="registrationWarningWithUrl">
              {{ $t('nominated_pharmacy.changeSuccess.registrationWarning',
                    {'pharmacyName': pharmacy.pharmacyName}) }}
              <analytics-tracked-tag v-if="pharmacy.url"
                                     id="pharmacy-url"
                                     :href="hrefForUrl"
                                     :text="displayUrl"
                                     tag="a" target="_blank"
                                     style="vertical-align: baseline; display: inline;">
                {{ displayUrl }}</analytics-tracked-tag>.
            </p>
            <p v-else id="registrationWarningWithNoUrl">
              {{ $t('nominated_pharmacy.changeSuccess.registrationWarningWithNoUrl',
                    {'pharmacyName': pharmacy.pharmacyName}) }}</p>
            <p id="postal-warning">{{ $t('nominated_pharmacy.changeSuccess.postalWarning',
                                         {'pharmacyName': pharmacy.pharmacyName,
                                          'pharmacyNameRepeated': pharmacy.pharmacyName}) }}</p>
          </div>
        </div>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink
          id="to-prescriptions-link"
          :path="prescriptionsOrdersPath"
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
import { PRESCRIPTIONS_VIEW_ORDERS, NOMINATED_PHARMACY_CHANGE_SUCCESS } from '@/lib/routes';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import PharmacyTypeChoice from '@/lib/pharmacy-detail/pharmacy-type-choice';
import AnalyticsTrackedTag from '../../components/widgets/AnalyticsTrackedTag';
import { redirectTo, hrefForURL, displayedURL } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    OnlineOnlyPharmacyDetail,
    PharmacyChangeSuccessDetails,
    DesktopGenericBackLink,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      pharmacy: null,
      currentPage: NOMINATED_PHARMACY_CHANGE_SUCCESS.path,
      prescriptionsOrdersPath: PRESCRIPTIONS_VIEW_ORDERS.path,
      isHighStreetSelected:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.HIGH_STREET_PHARMACY,
      isOnlineOnlySelected:
        this.$store.state.nominatedPharmacy.chosenType === PharmacyTypeChoice.ONLINE_PHARMACY,
    };
  },
  computed: {
    displayUrl() {
      return displayedURL(this.pharmacy.url);
    },
    hrefForUrl() {
      return hrefForURL(this.pharmacy.url);
    },
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
      redirectTo(this, this.prescriptionsOrdersPath);
    },
  },
};
</script>
