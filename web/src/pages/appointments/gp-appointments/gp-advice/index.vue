<template>
  <div v-if="showTemplate && available !== undefined" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <online-consultations-unavailable v-if="!available"/>
      <message-dialog v-else-if="isError" role="alert">
        <message-text data-purpose="error-heading"
                      :is-header="true">
          {{ $t('appointments.gp_advice.errors.header') }}
        </message-text>
        <message-text data-purpose="reason-error"
                      :aria-label="$t('appointments.gp_advice.errors.message.label')">
          {{ $t('appointments.gp_advice.errors.message.text') }}
        </message-text>
      </message-dialog>
      <template v-else>
        <demographics-question v-if="!demographicsQuestionAnswered"
                               :provider="provider"
                               :provider-name="providerName"
                               :service-definition-id="serviceDefinitionId">
          <p>{{ $t('appointments.gp_advice.demographicsQuestion.p1') }}</p>
          <p>{{ $t('appointments.gp_advice.demographicsQuestion.p2') }}</p>
        </demographics-question>

        <condition-list v-else-if="conditionsList" :service-definitions="conditionsList"/>

        <orchestrator v-else :provider="provider" :service-definition-id="serviceDefinitionId"/>
      </template>
    </div>
  </div>
</template>

<script>
import OnlineConsultationsUnavailable from '@/components/online-consultations/OnlineConsultationsUnavailable';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import ConditionList from '@/components/online-consultations/ConditionList';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { LOGIN_PATH } from '@/router/paths';

const getServiceDefinitionId = store => (
  store.state.onlineConsultations.gpAdviceServiceDefinitionId ||
  store.state.serviceJourneyRules.rules.cdssAdvice.conditionsServiceDefinition
);

export default {
  name: 'GpAppointmentsGpAdvicePage',
  components: {
    MessageDialog,
    MessageText,
    Orchestrator,
    OnlineConsultationsUnavailable,
    DemographicsQuestion,
    ConditionList,
  },
  data() {
    return {
      provider: this.$store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      addJavascriptDisabledHeader: false,
      available: undefined,
    };
  },
  computed: {
    serviceDefinitionId() {
      return getServiceDefinitionId(this.$store);
    },
    demographicsQuestionAnswered() {
      return this.$store.state.onlineConsultations.demographicsQuestionAnswered;
    },
    isError() {
      return this.$store.state.onlineConsultations.error;
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    conditionsList() {
      return this.$store.state.onlineConsultations.conditionsList;
    },
    providerName() {
      return this.$store.state.onlineConsultations.adviceProviderName;
    },
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
  async created() {
    await this.$store.dispatch('onlineConsultations/serviceDefinitionIsValid', this.provider);

    if (this.$store.state.onlineConsultations.available === undefined) {
      // unable to get available status due to API error
      return;
    }

    if (this.$store.state.onlineConsultations.available === false) {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);

      EventBus.$emit(UPDATE_HEADER, {
        headerKey: 'appointments.gp_advice.unavailable.header',
        captionKey: 'appointments.gp_advice.unavailable.headerCaption',
      });
      EventBus.$emit(UPDATE_TITLE, 'appointments.gp_advice.unavailable.header');

      this.available = false;
      return;
    }

    this.$store.dispatch('onlineConsultations/setJourneyInfo', {
      provider: this.provider,
      serviceDefinitionId: getServiceDefinitionId(this.$store),
      addJavascriptDisabledHeader: this.addJavascriptDisabledHeader,
    });

    this.available = true;
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
