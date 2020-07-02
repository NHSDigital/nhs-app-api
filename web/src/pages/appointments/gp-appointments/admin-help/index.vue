<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <online-consultations-unavailable v-if="!available"/>
      <message-dialog v-else-if="isError" role="alert">
        <message-text data-purpose="error-heading"
                      :is-header="true">
          {{ $t('appointments.admin_help.errors.header') }}
        </message-text>
        <message-text data-purpose="reason-error"
                      :aria-label="$t('appointments.admin_help.errors.message.label')">
          {{ $t('appointments.admin_help.errors.message.text') }}
        </message-text>
      </message-dialog>
      <template v-else>
        <demographics-question v-if="!demographicsQuestionAnswered"
                               :provider="provider"
                               :provider-name="getProviderName"
                               :service-definition-id="serviceDefinitionId">
          <p>{{ $t('appointments.admin_help.demographicsQuestion.p1') }}</p>
          <p>{{ $t('appointments.admin_help.demographicsQuestion.p2') }}</p>
          <p>{{ $t('appointments.admin_help.demographicsQuestion.p3') }}</p>
        </demographics-question>

        <orchestrator v-else :provider="provider" :service-definition-id="serviceDefinitionId"/>
      </template>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import OnlineConsultationsUnavailable from '@/components/online-consultations/OnlineConsultationsUnavailable';
import Orchestrator from '@/components/online-consultations/Orchestrator';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import DemographicsQuestion from '@/components/online-consultations/DemographicsQuestion';
import { noJsParameterName } from '@/lib/noJs';
import { isAnswerValid } from '@/lib/online-consultations/answer-validators';
import getAnswerFromRequestBody from '@/lib/online-consultations/noJs';
import { findByPath, LOGIN } from '@/lib/routes';
import {
  ANSWERING_DEMOGRAPHICS_NAME,
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
} from '@/lib/online-consultations/constants/nojsInputNames';

export default {
  layout: 'nhsuk-layout',
  components: {
    MessageDialog,
    MessageText,
    OnlineConsultationsUnavailable,
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
      return this.$store.state.onlineConsultations.adminProviderName;
    },
  },
  async asyncData({ store, req }) {
    const { provider } = store.state.serviceJourneyRules.rules.cdssAdmin;

    await store.dispatch('onlineConsultations/serviceDefinitionIsValid', provider);

    if (store.state.onlineConsultations.available === undefined) {
      // unable to get available status due to API error
      return {};
    }

    if (store.state.onlineConsultations.available === false) {
      store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);

      store.dispatch('header/updateHeaderText', store.app.i18n.tc('appointments.admin_help.unavailable.header'));
      store.dispatch('header/updateHeaderCaption', store.app.i18n.tc('appointments.admin_help.unavailable.headerCaption'));

      store.dispatch('pageTitle/updatePageTitle', store.app.i18n.tc('appointments.admin_help.unavailable.header'));

      return { available: false };
    }

    const requestBody = get('body', req);
    const handlingNoJS = get(noJsParameterName, requestBody) !== undefined;
    const { question } = store.state.onlineConsultations;
    const answeringConsultationQuestion = question !== undefined;
    const previousClicked = get('direction', requestBody) === 'back';
    const answeringDemographics = get(ANSWERING_DEMOGRAPHICS_NAME, requestBody) !== undefined;
    const consentGiven =
      get(DEMOGRAPHICS_QUESTION_NAME, requestBody) === DEMOGRAPHICS_QUESTION_OPTION;

    const journeyInfo = {
      provider,
      serviceDefinitionId: store.state.serviceJourneyRules.rules.cdssAdmin.serviceDefinition,
      addJavascriptDisabledHeader: false,
    };

    if (handlingNoJS) {
      if (answeringDemographics) {
        store.dispatch('onlineConsultations/setDemographicsConsentGiven', consentGiven);
        store.dispatch('onlineConsultations/setDemographicsQuestionAnswered');
        store.dispatch('onlineConsultations/setAnswer', undefined);
      } else if (answeringConsultationQuestion) {
        const answer = getAnswerFromRequestBody(requestBody, question);
        journeyInfo.addJavascriptDisabledHeader = process.server;
        store.dispatch('onlineConsultations/setAnswer', answer);
        store.dispatch('onlineConsultations/setAnswerIsValid', isAnswerValid(answer, question));
        store.dispatch('onlineConsultations/setValidationError');
      }
    }

    store.dispatch('onlineConsultations/setJourneyInfo', journeyInfo);

    const demographicsAnswered = store.state.onlineConsultations.demographicsQuestionAnswered;
    const { answerIsValid } = store.state.onlineConsultations;

    if (!answeringConsultationQuestion && demographicsAnswered) {
      await store.dispatch('onlineConsultations/getServiceDefinition', journeyInfo);
    } else if (answerIsValid || previousClicked) {
      if (previousClicked) {
        store.dispatch('onlineConsultations/setPrevious');
      }
      await store.dispatch('onlineConsultations/evaluateServiceDefinition', journeyInfo);
    }

    return {
      ...journeyInfo,
      available: true,
    };
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
