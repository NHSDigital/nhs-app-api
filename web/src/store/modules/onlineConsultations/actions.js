import get from 'lodash/fp/get';
import {
  getDataRequirements,
  getSessionId,
  getQuestionnaire,
  getQuestionnaireItem,
  getCarePlansAndReferralRequests,
  getPreviousQuestion,
  getQuestionnaireResponseAnswers,
  getAllIssues,
  getQuestionnaireId,
  getSelfOrChild,
} from '@/lib/online-consultations/mappers/response';
import { getQuestion, getConditionsList, getGeneralServiceDefinition } from '@/lib/online-consultations/mappers/item';
import { getParameters, getAnswerFromItem, getInputDataParameter } from '@/lib/online-consultations/mappers/parameters';
import { DATA_REQUIRED, SUCCESS } from '@/lib/online-consultations/constants/status-types';
import ServiceDefinitionTypes from '@/lib/online-consultations/constants/service-definition-types';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import {
  CLEAR,
  SET_SESSION_ID,
  SET_STATUS,
  SET_DATA_REQUIREMENTS,
  SET_QUESTION,
  SET_PREVIOUS_QUESTION,
  SET_ANSWER,
  SET_ANSWER_IS_VALID,
  SET_VALIDATION_ERROR,
  SET_PREVIOUS_ROUTE,
  SET_CARE_PLANS,
  SET_REFERRAL_REQUESTS,
  SET_ERROR,
  SET_ANSWER_IS_EMPTY,
  FILE_LOADING,
  FILE_LOAD_COMPLETE,
  SET_VALIDATION_ERROR_FROM_RESPONSE,
  CLEAR_VALIDATION,
  PREVIOUS_SELECTED,
  SET_GP_ADVICE_SERVICE_DEFINITION_ID,
  SET_DEMOGRAPHICS_CONSENT_GIVEN,
  SET_DEMOGRAPHICS_QUESTION_ANSWERED,
  SET_ADMIN_PROVIDER_NAME,
  SET_ADVICE_PROVIDER_NAME,
  SET_CONDITIONS_LIST,
  SET_JOURNEY_INFO,
  SET_IS_AVAILABLE,
  SET_SELF_OR_CHILD_REQUIRED,
  SET_SELF_OR_CHILD_INPUT_DATA,
  SET_DISCLAIMER_INPUT_DATA,
  SET_SERVICE_DEFINITION_ID,
  SET_CHILD_JOURNEY_SELECTED,
  SET_DEFAULT_CONDITION,
  SET_PRE_DOB,
} from './mutation-types';

const initialiseLeaveWarnings = (store) => {
  // Enable navigation prompt
  store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);

  if (typeof window !== 'object') {
    return;
  }

  const browserString
    = store.app.$options.i18n.t('navigation.leavePage.ifYouEnteredInformationItWillNotBeSaved');

  window.onbeforeunload = function handleBeforeUnload(event) {
    event.preventDefault();
    return browserString;
  };
};

const selfOrChildIds = ['PRE_POV_SELF', 'PRE_POV_CHILD'];
const dobAnswerIds = ['PRE_DOB_PARENT', 'PRE_DOB_SELF'];

const isTermsAndConditions = (parameters) => {
  if (typeof parameters !== 'object') {
    return false;
  }

  return parameters.find(p =>
    p.name === 'inputData'
      && Array.isArray(p.resource.item)
      && p.resource.item.find(i => i.linkId.startsWith('GLO_PRE_DISCLAIMERS')));
};

const showError = (store) => {
  store.dispatch('onlineConsultations/clearAndSetError');
};

const getServiceDefinitionType = (journeyRules, serviceDefinitionId) => {
  if (serviceDefinitionId === journeyRules.cdssAdmin.serviceDefinition ||
    journeyRules.cdssAdmin.knownGeneralServiceDefinitions.includes(serviceDefinitionId)) {
    return ServiceDefinitionTypes.AdminHelp;
  }
  if (serviceDefinitionId === journeyRules.cdssAdvice.serviceDefinition) {
    return ServiceDefinitionTypes.ConditionList;
  }
  if (journeyRules.cdssAdvice.knownGeneralServiceDefinitions.includes(serviceDefinitionId)) {
    return ServiceDefinitionTypes.GeneralAdvice;
  }
  return ServiceDefinitionTypes.ConditionAdvice;
};

export default {
  clear({ commit }, clearAll) {
    commit(CLEAR, clearAll);
  },
  async setProviderNames({ commit }, { adminProviderName, adviceProviderName }) {
    if (adminProviderName !== 'none') {
      await this.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails({
        provider: adminProviderName,
      }).then((providerName) => {
        commit(SET_ADMIN_PROVIDER_NAME, providerName);
      }).catch(() => {});
    }
    if (adviceProviderName !== 'none') {
      await this.app.$httpV2.getV2CdssServiceDefinitionByProviderDetails({
        provider: adviceProviderName,
      }).then((providerName) => {
        commit(SET_ADVICE_PROVIDER_NAME, providerName);
      }).catch(() => {});
    }
  },
  getServiceDefinition({ commit, rootState }, params = {}) {
    const store = this;
    const { serviceDefinitionId, provider } = params;

    const serviceDefinitionType = getServiceDefinitionType(
      rootState.serviceJourneyRules.rules, serviceDefinitionId,
    );

    return store.app.$httpV3.postV3CdssServiceDefinitionByProvider({
      serviceDefinitionMetaData: {
        id: serviceDefinitionId,
        type: serviceDefinitionType,
      },
      provider,
    }).then((response) => {
      commit(CLEAR);
      if (response === undefined) {
        showError(store);
        return;
      }

      const dataRequirements = getDataRequirements(response);

      if (dataRequirements === undefined) {
        showError(store);
        return;
      }

      commit(SET_DATA_REQUIREMENTS, dataRequirements);

      if (dataRequirements.questionnaireResponse) {
        const question = getQuestion(getQuestionnaireItem(response));

        if (question === undefined) {
          showError(store);
          return;
        }

        commit(SET_STATUS, DATA_REQUIRED);
        commit(SET_QUESTION, question);
        commit(SET_DEMOGRAPHICS_QUESTION_ANSWERED);
        return;
      }

      showError(store);
    }).catch(() => {
      showError(store);
    });
  },
  evaluateServiceDefinition({ commit, state, rootState }, params = {}) {
    const store = this;
    const {
      serviceDefinitionId,
      provider,
      addJavascriptDisabledHeader,
      answeringConditionsQuestion,
    } = params;

    const serviceDefinitionType = getServiceDefinitionType(
      rootState.serviceJourneyRules.rules, serviceDefinitionId,
    );
    const parameters = getParameters(
      state, rootState, answeringConditionsQuestion, serviceDefinitionId, serviceDefinitionType,
    );

    if (parameters === undefined) {
      showError(store);
      return undefined;
    }

    if (isTermsAndConditions(parameters.parameter)) {
      initialiseLeaveWarnings(store);
    }

    const requestParams = {
      provider,
      addJavascriptDisabledHeader,
      parameters,
    };

    if (answeringConditionsQuestion) {
      requestParams.parameters.parameter.push(state.disclaimerInputData);
      requestParams.demographicsConsentGiven = !!state.demographicsConsentGiven;
    } else if (
      state.dataRequirements &&
      state.dataRequirements.patient &&
      state.demographicsConsentGiven
    ) {
      requestParams.demographicsConsentGiven = !!state.demographicsConsentGiven;
    }

    if (state.question && state.question.id.startsWith('GLO_PRE_DISCLAIMERS')) {
      requestParams.demographicsConsentGiven = !!state.demographicsConsentGiven;
      const inputData = getInputDataParameter(state);
      commit(SET_DISCLAIMER_INPUT_DATA, { name: inputData.name, resource: inputData.resource });
    }

    if (state.answer && ((Array.isArray(state.answer) && state.answer[0] === 'PRE_POV_SELFONLY_SELF') ||
        selfOrChildIds.includes(state.answer))) {
      const inputData = getInputDataParameter(state);
      commit(SET_SELF_OR_CHILD_INPUT_DATA,
        { name: inputData.name, resource: inputData.resource });
    }

    if (state.answer === 'PRE_POV_CHILD') {
      commit(SET_CHILD_JOURNEY_SELECTED, true);
    }

    return store.app.$httpV3.postV3CdssServiceDefinitionByProviderEvaluate(
      requestParams,
    ).then((response) => {
      commit(CLEAR);
      if (response === undefined) {
        showError(store);
        return;
      }

      const { status } = response;
      commit(SET_STATUS, status);

      if (status === DATA_REQUIRED) {
        const dataRequirements = getDataRequirements(response);

        if (dataRequirements !== undefined) {
          commit(SET_DATA_REQUIREMENTS, dataRequirements);
        }

        const sessionId = getSessionId(response);
        const questionnaire = getQuestionnaire(response, getQuestionnaireId(response));
        const question = getQuestion(getQuestionnaireItem(response));
        const conditionsList = getConditionsList(questionnaire);

        // The default condition if the user cannot find a condition relevant
        // to them from the conditions list
        const defaultCondition = getQuestionnaire(response, 'DEFAULT_CONDITION');

        const isConditionsQuestion = questionnaire.id ===
          rootState.serviceJourneyRules.rules.cdssAdvice.serviceDefinition;
        const issues = getAllIssues(response);
        const previousQuestion = getQuestion(getPreviousQuestion(response));
        const previousAnswers = getQuestionnaireResponseAnswers(response);
        const selfOrChildAnswerRequired = getSelfOrChild(response);

        if ((sessionId === undefined ||
            question === undefined) &&
          question &&
          !dobAnswerIds.includes(question.id) &&
          conditionsList === undefined) {
          showError(store);
          return;
        }

        commit(SET_SESSION_ID, sessionId);
        commit(SET_PREVIOUS_QUESTION, previousQuestion);
        if (isConditionsQuestion) {
          // The default condition will not exist if user is too young
          let defaultConditionLinkId = '';
          if (defaultCondition) {
            defaultConditionLinkId = getGeneralServiceDefinition(defaultCondition);
          }

          commit(SET_DEFAULT_CONDITION, defaultConditionLinkId);
          commit(SET_CONDITIONS_LIST, conditionsList);
        } else {
          commit(SET_QUESTION, question);
        }
        if (issues !== undefined) {
          commit(SET_VALIDATION_ERROR_FROM_RESPONSE, issues);
        }

        if (previousAnswers) {
          if ((previousAnswers.linkId === 'PRE_DOB_SELF') ||
              previousAnswers.linkId === 'PRE_DOB_PARENT') {
            state.question = {
              id: previousAnswers.linkId,
              type: QuestionTypes.DATE_AS_STRING,
            };
            state.answer = previousAnswers.answer[0].valueDate;
            state.answerIsEmpty = false;

            commit(SET_PRE_DOB, getInputDataParameter(state));
          } else {
            const answersFormatted = getAnswerFromItem(question, previousAnswers);
            commit(SET_ANSWER, answersFormatted, question.type);
          }
        }

        if (selfOrChildAnswerRequired) {
          commit(SET_SELF_OR_CHILD_REQUIRED, selfOrChildAnswerRequired);
        }

        return;
      }

      if (status === SUCCESS) {
        const actions = getCarePlansAndReferralRequests(response);

        if (actions === undefined) {
          showError(store);
          return;
        }

        commit(SET_CARE_PLANS, actions.carePlans);
        commit(SET_REFERRAL_REQUESTS, actions.referralRequests);
        return;
      }

      showError(store);
    }).catch((error) => {
      if (!store.state.errors.pageSettings.ignoredErrors.includes(get('response.status', error, 0))) {
        showError(store);
      }
    });
  },
  /* eslint-disable prefer-destructuring */
  async serviceDefinitionIsValid({ commit }, provider) {
    let response;
    let isAvailable;

    try {
      response = await this.app.$httpV2.getV2CdssServiceDefinitionByProviderIsValid({
        provider,
        returnResponse: true,
      });
    } catch (error) {
      // client side throws error instead of returning response with non 200 status code
      response = error.response;
    }

    if (response) {
      if (response.status === 580) {
        isAvailable = false;
      } else if (response.status === 204) {
        isAvailable = true;
      }
    }

    commit(SET_IS_AVAILABLE, isAvailable);
  },
  setAnswer({ commit }, answer) {
    commit(SET_ANSWER, answer);
  },
  setGpAdviceServiceDefinitionId({ commit }, id) {
    commit(SET_GP_ADVICE_SERVICE_DEFINITION_ID, id);
  },
  setAnswerIsValid({ commit }, validation) {
    commit(SET_ANSWER_IS_VALID, validation);
  },
  setValidationError({ commit }) {
    commit(SET_VALIDATION_ERROR);
  },
  setPreviousRoute({ commit }, previousRoute) {
    commit(SET_PREVIOUS_ROUTE, previousRoute);
  },
  setAnswerIsEmpty({ commit }, answerIsEmpty) {
    commit(SET_ANSWER_IS_EMPTY, answerIsEmpty);
  },
  clearAndSetError({ commit }) {
    commit(CLEAR);
    commit(SET_ERROR, true);
  },
  fileLoading({ commit }) {
    commit(FILE_LOADING);
  },
  fileLoadComplete({ commit }) {
    commit(FILE_LOAD_COMPLETE);
  },
  clearValidation({ commit }) {
    commit(CLEAR_VALIDATION);
  },
  setPrevious({ commit }) {
    commit(PREVIOUS_SELECTED);
  },
  setDemographicsConsentGiven({ commit }, consent) {
    commit(SET_DEMOGRAPHICS_CONSENT_GIVEN, consent);
  },
  setDemographicsQuestionAnswered({ commit }) {
    commit(SET_DEMOGRAPHICS_QUESTION_ANSWERED);
  },
  setJourneyInfo({ commit }, journeyInfo) {
    commit(SET_JOURNEY_INFO, journeyInfo);
  },
  setServiceDefinitionId({ commit }, id) {
    commit(SET_SERVICE_DEFINITION_ID, id);
  },
};
