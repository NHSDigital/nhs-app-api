<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog v-if="isError" role="alert">
        <message-text data-purpose="error-heading"
                      :is-header="true">
          {{ $t('appointments.gp_advice.errors.header') }}
        </message-text>
        <message-text data-purpose="reason-error"
                      :aria-label="$t('appointments.gp_advice.errors.message.label')">
          {{ $t('appointments.gp_advice.errors.message.text') }}
        </message-text>
      </message-dialog>

      <demographics-question v-else-if="!demographicsQuestionAnswered"
                             :provider="provider"
                             :service-definition-id="serviceDefinitionId"
                             :checkbox-label="getDemographicsCheckboxLabel">
        <message-dialog message-type="warning" icon-text="Important">
          <message-text :class="$style.warningText">
            {{ $t('appointments.admin_help.warning.warningText',
                  { providerName: getProviderName }) }}
          </message-text>
        </message-dialog>

        <template>
          <p>{{ $t('appointments.gp_advice.demographicsQuestion.p1') }}</p>
          <p>{{ $t('appointments.gp_advice.demographicsQuestion.p2',
                   { providerName: getProviderName }) }}</p>
        </template>
      </demographics-question>

      <orchestrator v-else :provider="provider" :service-definition-id="serviceDefinitionId"/>
    </div>
  </div>
</template>

<script>
import { get } from 'lodash/fp';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import { noJsParameterName } from '@/lib/noJs';
import { isAnswerValid } from '@/lib/online-consultations/answer-validators';
import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import { APPOINTMENT_GP_ADVICE_CONDITIONS } from '@/lib/routes';
import {
  ANSWERING_DEMOGRAPHICS_NAME,
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
} from '@/lib/online-consultations/constants/demographics';

export default {
  layout: 'nhsuk-layout',
  components: {
    MessageDialog,
    MessageText,
    Orchestrator,
    DemographicsQuestion,
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
    getProviderName() {
      return this.$store.state.serviceJourneyRules.rules.cdssAdvice.name;
    },
    getDemographicsCheckboxLabel() {
      const providerName = this.$store.state.serviceJourneyRules.rules.cdssAdvice.name;
      return this.$t(
        'appointments.admin_help.demographicsQuestion.checkboxLabel',
        { providerName },
      );
    },
  },
  async asyncData({ store, req, redirect, route }) {
    const serviceDefinitionId = route.query.serviceDefinitionId ||
        store.state.onlineConsultations.gpAdviceServiceDefinitionId;
    if (!serviceDefinitionId) {
      return redirect(302, APPOINTMENT_GP_ADVICE_CONDITIONS.path, null);
    }

    const requestBody = get('body', req);
    const noJsDataPresent = get(noJsParameterName, requestBody) !== undefined;
    const { question } = store.state.onlineConsultations;
    const answeringConsultationQuestion = question !== undefined;
    const previousClicked = get('direction', requestBody) === 'back';
    const answeringDemographics = get(ANSWERING_DEMOGRAPHICS_NAME, requestBody) !== undefined;
    const consentGiven =
      get(DEMOGRAPHICS_QUESTION_NAME, requestBody) === DEMOGRAPHICS_QUESTION_OPTION;

    const journeyInfo = {
      provider: store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      serviceDefinitionId,
      addJavascriptDisabledHeader: false,
    };

    if (noJsDataPresent) {
      if (answeringDemographics) {
        await store.dispatch('onlineConsultations/setDemographicsConsentGiven', consentGiven);
        await store.dispatch('onlineConsultations/setDemographicsQuestionAnswered');
        await store.dispatch('onlineConsultations/setAnswer', undefined);
      } else if (answeringConsultationQuestion) {
        journeyInfo.addJavascriptDisabledHeader = process.server;
        const answer = getAnswerFromRequestBody(requestBody, question);

        await store.dispatch('onlineConsultations/setAnswer', answer);
        await store.dispatch('onlineConsultations/setAnswerIsValid', isAnswerValid(answer, question));
        await store.dispatch('onlineConsultations/setValidationError');
      }
    }

    const demographicsAnswered = store.state.onlineConsultations.demographicsQuestionAnswered;
    const { answerIsValid } = store.state.onlineConsultations;

    if (!answeringConsultationQuestion && demographicsAnswered) {
      await store.dispatch('onlineConsultations/getServiceDefinition', journeyInfo);
    } else if (answerIsValid) {
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', journeyInfo);
    }

    if (previousClicked) {
      await store.dispatch('onlineConsultations/setPrevious');
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', journeyInfo);
    }

    return journeyInfo;
  },
  beforeDestroy() {
    this.$store.dispatch('onlineConsultations/clear', true);
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/fonts';
  .warningText {
    font-family: $default_web;
    font-weight: normal;
  }
</style>
