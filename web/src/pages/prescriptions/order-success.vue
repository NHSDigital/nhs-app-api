<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p v-if="isProxying" class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0"
         data-purpose="pre-text">
        {{ t('youHaveOrderedOnBehalfOf', { givenName }) }}
      </p>

      <template v-else>
        <p class="nhsuk-u-margin-top-3" data-purpose="pre-text">{{ t('youHaveOrdered') }}</p>

        <card id="prescription-order-success-summary" data-purpose="order-success-summary"
              class="nhsuk-u-margin-bottom-3">
          <div v-for="(prescription, index) in selectedPrescriptions" :id="prescription.id"
               :key="prescription.id" data-purpose="prescription-summary">
            <p class="nhsuk-u-padding-top-1 nhsuk-u-margin-bottom-0"
               data-purpose="prescription-name">{{ prescription.name }}</p>

            <p class="nhsuk-u-padding-bottom-1 prescription-description"
               data-purpose="prescription-details">{{ prescription.details }}</p>

            <hr v-if="index !== selectedPrescriptions.length - 1"
                class="nhsuk-section-break nhsuk-section-break--s
                      nhsuk-u-margin-top-2 nhsuk-u-margin-bottom-2"
                aria-hidden="true">
          </div>
        </card>
      </template>

      <h2 class="what-happens-next-heading nhsuk-u-margin-bottom-3">{{ t('whatHappensNext') }}</h2>

      <div v-if="isProxying" data-purpose="what-happens-next-proxy">
        <p>{{ t('theProxyOrderStatusWillBeUpdated', { givenName }) }}</p>

        <switch-profile-button />
      </div>

      <div v-else data-purpose="what-happens-next">
        <p>{{ t('prescriptionSentToSurgery') }}</p>

        <template v-if="epsAvailable">
          <p v-if="hasNoNominatedPharmacy" data-purpose="what-happens-next-no-nom-pharm">
            {{ t('needToCollectFromSurgery') }}
          </p>

          <div v-else-if="hasHighStreetPharmacy" data-purpose="what-happens-next-high-street-pharm">
            <p>{{ t('nomPharmOnceApproved', { pharmacyName }) }}</p>
            <pharmacy-summary :pharmacy="pharmacy" :pharmacy-name-as-header="false"/>
          </div>

          <div v-else-if="hasOnlineOnlyPharmacy" data-purpose="what-happens-next-online-only-pharm">
            <p>{{ t('nomPharmOnceApproved', { pharmacyName }) }}</p>

            <p v-if="pharmacyHasUrl && pharmacyHasPhone">
              {{ t('registerByUrlOrPhone.beforeUrl', { pharmacyName }) }}
              <analytics-tracked-tag id="pharmacy-url"
                                     :href="hrefPharmacyUrl"
                                     :text="displayPharmacyUrl"
                                     tag="a"
                                     target="_blank">
                {{ displayPharmacyUrl }}</analytics-tracked-tag>
              {{ t('registerByUrlOrPhone.afterUrl', { pharmacyPhone }) }}
            </p>
            <p v-else-if="pharmacyHasUrl">
              {{ t('registerAtUrl', { pharmacyName }) }}
              <analytics-tracked-tag id="pharmacy-url"
                                     :href="hrefPharmacyUrl"
                                     :text="displayPharmacyUrl"
                                     tag="a"
                                     target="_blank">
                {{ displayPharmacyUrl }}</analytics-tracked-tag>.
            </p>
            <p v-else>{{ t('registerByCalling', { pharmacyName, pharmacyPhone }) }}</p>

            <p>{{ t('whenThePharmacistsHaveChecked', { pharmacyName }) }}</p>
          </div>
        </template>

        <p v-else data-purpose="what-happens-next-default">
          {{ t('theOrderStatusWillBeUpdated') }}
        </p>

        <desktopGenericBackLink
          :path="viewOrdersPath"
          button-text="prescriptions.orderSuccess.goToYourPrescriptionOrders"
          @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import Card from '@/components/widgets/card/Card';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import PharmacySummary from '@/components/nominatedPharmacy/PharmacySummary';
import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import { PRESCRIPTIONS_VIEW_ORDERS_PATH } from '@/router/paths';
import { InternetPharmacy, CommunityPharmacy } from '@/lib/pharmacy-detail/pharmacy-sub-types';
import { redirectTo, displayedURL, hrefForURL, isBlankString } from '@/lib/utils';
import sjrIf from '@/lib/sjrIf';

export default {
  name: 'PrescriptionOrderSuccess',
  components: {
    AnalyticsTrackedTag,
    Card,
    DesktopGenericBackLink,
    PharmacySummary,
    SwitchProfileButton,
  },
  data() {
    const { pharmacy = {} } = this.$store.state.nominatedPharmacy;
    const pharmacyUrl = pharmacy.url;
    const pharmacyPhone = pharmacy.telephoneNumber;
    const nominatedPharmacyEnabled = this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled'];
    const sjrNominatedPharmacyEnabled = sjrIf({ $store: this.$store, journey: 'nominatedPharmacy' });

    return {
      givenName: get('$store.state.linkedAccounts.actingAsUser.givenName', this),
      viewOrdersPath: PRESCRIPTIONS_VIEW_ORDERS_PATH,
      selectedPrescriptions: this.$store.getters['repeatPrescriptionCourses/selectedPrescriptions'],
      isProxying: this.$store.getters['session/isProxying'],
      hasNoNominatedPharmacy: this.$store.getters['nominatedPharmacy/hasNoNominatedPharmacy'],
      epsAvailable: nominatedPharmacyEnabled && sjrNominatedPharmacyEnabled,
      pharmacy,
      pharmacyUrl,
      pharmacyPhone,
      pharmacyName: pharmacy.pharmacyName,
      hasOnlineOnlyPharmacy: pharmacy.pharmacySubType === InternetPharmacy,
      hasHighStreetPharmacy: pharmacy.pharmacySubType === CommunityPharmacy,
      hrefPharmacyUrl: hrefForURL(pharmacyUrl),
      displayPharmacyUrl: displayedURL(pharmacyUrl),
      pharmacyHasUrl: !isBlankString(pharmacyUrl),
      pharmacyHasPhone: !isBlankString(pharmacyPhone),
    };
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
    this.$store.dispatch('repeatPrescriptionCourses/completeOrderJourney');
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.viewOrdersPath, null);
    },
    t(key, options) {
      return this.$t(`prescriptions.orderSuccess.${key}`, options);
    },
  },
};
</script>

<style lang="scss">
@import '~nhsuk-frontend/packages/core/settings/colours';

#pharmacy-url {
  vertical-align: baseline;
  display: inline;
}

h2.what-happens-next-heading {
  margin: 0;
}

#prescription-order-success-summary {
  p {
    &:first-of-type {
      padding-top: 0;
    }

    &:last-of-type {
      margin-bottom: 0;
      padding-bottom: 0;
    }
  }

  .prescription-description {
    color: $nhsuk-secondary-text-color;
  }
}
</style>
