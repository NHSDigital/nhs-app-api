<template>
  <div>
    <gp-appointment-gp-session-errors
      v-if="appointmentsOnDemandApiError"
      :error="gpSessionApiError" />
    <prescription-errors
      v-else-if="prescriptionOnDemandApiError"
      :error="gpSessionApiError"
      :reference-code="gpSessionApiError.serviceDeskReference"
      :try-again-route="tryAgainPath" />
    <health-record-errors
      v-else-if="healthRecordOnDemandApiError"
      :error="gpSessionApiError" />
    <linked-profile-errors
      v-else-if="linkedAccountsOnDemandApiError"
      :error="gpSessionApiError" />
    <organ-donation-errors
      v-else-if="organDonationOnDemandApiError"
      :error="gpSessionApiError"/>
    <generic-errors
      v-else-if="defaultOnDemandApiError"
      :try-again-route="tryAgainPath" />
  </div>
</template>

<script>
import { get, isEmpty } from 'lodash/fp';
import { TERMSANDCONDITIONS_PATH } from '@/router/paths';
import {
  REDIRECT_PARAMETER,
  GP_PRESCRIPTION_JOURNEY_NAME,
  GP_APPOINTMENT_JOURNEY_NAME,
  GP_LINKED_ACCOUNT_JOURNEY_NAME,
  GP_HEALTH_RECORD_JOURNEY_NAME,
  ORGAN_DONATION_JOURNEY_NAME,
} from '@/router/names';
import NativeReferrerSetup from '@/services/nativeReferrerSetup';
import { isBlankString, removeNhsAppHost } from '@/lib/utils';
import GpAppointmentGpSessionErrors from '@/components/errors/pages/appointments/GpAppointmentGpSessionErrors';
import PrescriptionErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import LinkedProfileErrors from '@/components/linked-profiles/LinkedProfileErrors';
import GenericErrors from '@/components/errors/pages/on-demand-generic/GenericErrors';
import HealthRecordErrors from '@/components/errors/pages/health-record/HealthRecordErrors';
import OrganDonationErrors from '@/components/errors/pages/organ-donation/OrganDonationErrors';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  name: 'OnDemandGpReturnPage',
  components: {
    GpAppointmentGpSessionErrors,
    PrescriptionErrors,
    LinkedProfileErrors,
    HealthRecordErrors,
    OrganDonationErrors,
    GenericErrors,
  },
  mixins: [OnUpdateTitleMixin],
  data() {
    return {
      prescriptionOnDemandApiError: false,
      appointmentsOnDemandApiError: false,
      linkedAccountsOnDemandApiError: false,
      healthRecordOnDemandApiError: false,
      organDonationOnDemandApiError: false,
      defaultOnDemandApiError: false,
    };
  },
  computed: {
    gpSessionApiError() {
      return this.$store.state.auth.gpSessionError;
    },
    tryAgainPath() {
      return this.$router.currentRoute.query.state;
    },
  },
  async mounted() {
    NativeReferrerSetup(this.$store);
    await this.initializeAppVersions();

    const route = this.$router.currentRoute;
    const routeDetails = this.$router.resolve(removeNhsAppHost(route.query.state));
    const routeMetaData = get('route.meta', routeDetails);
    EventBus.$emit(UPDATE_TITLE, routeMetaData);

    await this.$store.dispatch('auth/handleGpOnDemandResponse', route.query);

    const hasGpSessionError = !isEmpty(this.gpSessionApiError);
    const hasApiError = !isEmpty(this.$store.state.errors.apiErrors);
    const { deepLinkUrl } = route.query;

    if (!isBlankString(deepLinkUrl)) {
      if (!hasGpSessionError && !hasApiError) {
        this.setHasGpSession();
      }
      this.handleDeepLinkUrl(deepLinkUrl);
      return;
    }

    if (hasGpSessionError) {
      const ignoreGpSessionError = get('gpSessionOnDemand.ignoreError', routeMetaData);
      this.$store.dispatch('session/setGpSession', false);

      if (ignoreGpSessionError) {
        this.redirect(route);
      } else {
        this.displayJourneySpecificError(routeMetaData);
        EventBus.$emit(UPDATE_HEADER, routeMetaData);
      }
    } else if (!hasApiError) {
      this.setHasGpSession();
      this.redirect(route);
    }
  },
  methods: {
    async initializeAppVersions() {
      await this.$store.dispatch('appVersion/init');
      const appVersion = this.$store.$env.VERSION_TAG;
      if (appVersion) {
        this.$store.dispatch('appVersion/updateWebVersion', appVersion);
      }
    },
    setHasGpSession() {
      this.$store.dispatch('session/setGpSession', true);
      sessionStorage.removeItem('hasRetried');
    },
    handleDeepLinkUrl(deepLinkUrl) {
      this.$router.push({
        path: TERMSANDCONDITIONS_PATH,
        query: { [REDIRECT_PARAMETER]: deepLinkUrl },
      });
    },
    redirect(route) {
      const query = route.query.state.length > 1
        ? { [REDIRECT_PARAMETER]: route.query.state }
        : {};
      this.$router.push({ path: TERMSANDCONDITIONS_PATH, query });
    },
    displayJourneySpecificError(routeMetaData) {
      const gpSessionOnDemandJourney = get('gpSessionOnDemand.journey', routeMetaData);

      switch (gpSessionOnDemandJourney) {
        case GP_PRESCRIPTION_JOURNEY_NAME:
          this.prescriptionOnDemandApiError = true;
          break;
        case GP_APPOINTMENT_JOURNEY_NAME:
          this.appointmentsOnDemandApiError = true;
          break;
        case GP_LINKED_ACCOUNT_JOURNEY_NAME:
          this.linkedAccountsOnDemandApiError = true;
          break;
        case GP_HEALTH_RECORD_JOURNEY_NAME:
          this.healthRecordOnDemandApiError = true;
          break;
        case ORGAN_DONATION_JOURNEY_NAME:
          this.organDonationOnDemandApiError = true;
          break;
        default:
          this.defaultOnDemandApiError = true;
      }
    },
  },
};
</script>
