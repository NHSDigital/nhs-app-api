<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
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
                               :provider-name="getProviderName"
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
import get from 'lodash/fp/get';
import OnlineConsultationsUnavailable from '@/components/online-consultations/OnlineConsultationsUnavailable';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import ConditionList from '@/components/online-consultations/ConditionList';
import { noJsParameterName } from '@/lib/noJs';
import { isAnswerValid } from '@/lib/online-consultations/answer-validators';
import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import { findByPath, LOGIN } from '@/lib/routes';
import {
  ANSWERING_DEMOGRAPHICS_NAME,
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
  NHSAPP_SELECTED_CONDITION,
} from '@/lib/online-consultations/constants/nojsInputNames';

const getServiceDefinitionId = ({ requestBody, store }) => (
  get('serviceDefinitionId', requestBody) ||
  store.state.onlineConsultations.gpAdviceServiceDefinitionId ||
  store.state.serviceJourneyRules.rules.cdssAdvice.conditionsServiceDefinition
);

export default {
  layout: 'nhsuk-layout',
  components: {
    MessageDialog,
    MessageText,
    Orchestrator,
    OnlineConsultationsUnavailable,
    DemographicsQuestion,
    ConditionList,
  },
  computed: {
    serviceDefinitionId() {
      return getServiceDefinitionId({ store: this.$store });
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
    getProviderName() {
      return this.$store.state.onlineConsultations.adviceProviderName;
    },
  },
  beforeRouteLeave(to, from, next) {
    let shouldContinue = true;

    if (to.path === LOGIN.path) {
      next(shouldContinue);
    }

    if (this.$store.getters['pageLeaveWarning/shouldShowLeavingModal']) {
      const toPath = findByPath(to.path);

      this.$store.dispatch('pageLeaveWarning/setAttemptedRedirectRoute', toPath);
      this.showModal();

      shouldContinue = false;
    }

    if (shouldContinue && typeof window === 'object') {
      window.onbeforeunload = null;
    }

    next(shouldContinue);
  },
  async asyncData({ store, req }) {
    const { provider } = store.state.serviceJourneyRules.rules.cdssAdvice;

    await store.dispatch('onlineConsultations/serviceDefinitionIsValid', provider);

    if (!store.state.onlineConsultations.available) {
      store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);

      store.dispatch('header/updateHeaderText', store.app.i18n.tc('appointments.gp_advice.unavailable.header'));
      store.dispatch('header/updateHeaderCaption', store.app.i18n.tc('appointments.gp_advice.unavailable.headerCaption'));

      store.dispatch('pageTitle/updatePageTitle', store.app.i18n.tc('appointments.gp_advice.unavailable.header'));

      return { available: false };
    }

    const requestBody = get('body', req);
    let serviceDefinitionId = getServiceDefinitionId({ requestBody, store });
    let consentGiven =
      get(DEMOGRAPHICS_QUESTION_NAME, requestBody) === DEMOGRAPHICS_QUESTION_OPTION;
    const handlingNoJS = get(noJsParameterName, requestBody) !== undefined;
    const { question } = store.state.onlineConsultations;
    const answeringConsultationQuestion = question !== undefined;
    const previousClicked = get('direction', requestBody) === 'back';
    const answeringDemographics = get(ANSWERING_DEMOGRAPHICS_NAME, requestBody) !== undefined;
    const selectedCondition = get(NHSAPP_SELECTED_CONDITION, requestBody);

    const data = {
      available: true,
      provider,
      addJavascriptDisabledHeader: false,
    };

    if (selectedCondition) {
      data.addJavascriptDisabledHeader = process.server;
      serviceDefinitionId = selectedCondition;
      consentGiven = get(DEMOGRAPHICS_QUESTION_NAME, requestBody);
      store.dispatch('onlineConsultations/setGpAdviceServiceDefinitionId', serviceDefinitionId);
      store.dispatch('onlineConsultations/setDemographicsConsentGiven', consentGiven);
      store.dispatch('onlineConsultations/setDemographicsQuestionAnswered');
    } else if (handlingNoJS) {
      data.addJavascriptDisabledHeader = process.server;
      if (answeringDemographics) {
        store.dispatch('onlineConsultations/setDemographicsConsentGiven', consentGiven);
        store.dispatch('onlineConsultations/setDemographicsQuestionAnswered');
        store.dispatch('onlineConsultations/setAnswer', undefined);
      } else if (answeringConsultationQuestion) {
        const answer = getAnswerFromRequestBody(requestBody, question);
        store.dispatch('onlineConsultations/setAnswer', answer);
        store.dispatch('onlineConsultations/setAnswerIsValid', isAnswerValid(answer, question));
        store.dispatch('onlineConsultations/setValidationError');
      }
    }

    const demographicsAnswered = store.state.onlineConsultations.demographicsQuestionAnswered;
    const { answerIsValid } = store.state.onlineConsultations;
    const journeyInfo = {
      ...data,
      serviceDefinitionId,
      answeringConditionsQuestion: !!selectedCondition,
    };

    store.dispatch('onlineConsultations/setJourneyInfo', journeyInfo);

    if (selectedCondition) {
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', journeyInfo);
    } else if (!answeringConsultationQuestion && demographicsAnswered) {
      await store.dispatch('onlineConsultations/getServiceDefinition', journeyInfo);
    } else if (answerIsValid || previousClicked) {
      if (previousClicked) {
        store.dispatch('onlineConsultations/setPrevious');
      }
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', journeyInfo);
    }

    return data;
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
