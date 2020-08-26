<template>
  <div v-if="showTemplate && available !== undefined" class="nhsuk-grid-row econsult-content">
    <div class="nhsuk-grid-column-full">
      <online-consultations-unavailable v-if="!available"/>
      <message-dialog v-else-if="isError" role="alert">
        <message-text data-purpose="error-heading"
                      :is-header="true">
          {{ $t('appointments.adminHelp.weAreExperiencingTechnicalDifficulties') }}
        </message-text>
        <message-text data-purpose="reason-error"
                      :aria-label="$t('appointments.adminHelp.ifTheProblemPersistsOneOneOne')">
          {{ $t('appointments.adminHelp.ifTheProblemPersists111') }}
        </message-text>
      </message-dialog>
      <template v-else>
        <demographics-question v-if="!demographicsQuestionAnswered"
                               :provider="provider"
                               :provider-name="providerName"
                               :service-definition-id="serviceDefinitionId">
          <p>{{ $t('appointments.adminHelp.useThisServiceToContactYourSurgery') }}</p>
          <p>{{ $t('appointments.adminHelp.itTakesAroundFiveMinutes') }}</p>
          <p>{{ $t('appointments.adminHelp.toSaveYouTypingIn') }}</p>
        </demographics-question>
        <orchestrator v-else :provider="provider" :service-definition-id="serviceDefinitionId"/>
      </template>
    </div>
  </div>
</template>

<script>
import OnlineConsultationsUnavailable from '@/components/online-consultations/OnlineConsultationsUnavailable';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { LOGIN_PATH } from '@/router/paths';

export default {
  name: 'GpAppointmentsAdminHelpPage',
  components: {
    MessageDialog,
    MessageText,
    OnlineConsultationsUnavailable,
    Orchestrator,
    DemographicsQuestion,
  },
  data() {
    return {
      provider: this.$store.state.serviceJourneyRules.rules.cdssAdmin.provider,
      serviceDefinitionId: this.$store.state.serviceJourneyRules.rules.cdssAdmin.serviceDefinition,
      available: undefined,
    };
  },
  computed: {
    demographicsQuestionAnswered() {
      return this.$store.state.onlineConsultations.demographicsQuestionAnswered;
    },
    isError() {
      return this.$store.state.onlineConsultations.error;
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    providerName() {
      return this.$store.state.onlineConsultations.adminProviderName;
    },
  },
  async created() {
    await this.$store.dispatch('onlineConsultations/serviceDefinitionIsValid', this.provider);

    if (this.$store.state.onlineConsultations.available === undefined) {
      // unable to get available status due to API error
      return;
    }

    if (this.$store.state.onlineConsultations.available === false) {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);

      EventBus.$emit(UPDATE_HEADER, {
        headerKey: 'appointments.adminHelp.onlineConsultationsUnavailable',
        captionKey: 'appointments.adminHelp.additionalGpServices',
      });
      EventBus.$emit(UPDATE_TITLE, 'appointments.adminHelp.onlineConsultationsUnavailable');

      this.available = false;
      return;
    }

    this.$store.dispatch('onlineConsultations/setJourneyInfo', {
      provider: this.provider,
      serviceDefinitionId: this.serviceDefinitionId,
    });

    this.available = true;
  },
  beforeRouteLeave(to, from, next) {
    let shouldContinue = true;

    if (to.path === LOGIN_PATH) {
      next(shouldContinue);
    }

    if (this.$store.getters['pageLeaveWarning/shouldShowLeavingModal']) {
      this.$store.dispatch('pageLeaveWarning/setAttemptedRedirectRoute', to.path);
      this.showModal();

      shouldContinue = false;
    }

    if (shouldContinue && typeof window === 'object') {
      window.onbeforeunload = null;
    }

    next(shouldContinue);
  },
  beforeDestroy() {
    this.$store.dispatch('pageLeaveWarning/reset');
    this.$store.dispatch('onlineConsultations/clear', true);
  },
  methods: {
    showModal() {
      this.$store.dispatch('pageLeaveWarning/showLeavingModal');
    },
  },
};
</script>
