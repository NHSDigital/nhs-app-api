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
  UPDATE_REQUEST_ID,
  FILE_LOADING,
  FILE_LOAD_COMPLETE,
  SET_VALIDATION_ERROR_FROM_RESPONSE,
  CLEAR_VALIDATION,
  PREVIOUS_SELECTED,
  CLEAR_CLIENT_ERRORS,
  SET_SERVICE_DEFINITIONS,
  SET_GP_ADVICE_SERVICE_DEFINITION_ID,
} from './mutation-types';
import {
  getDataRequirements,
  getSessionId,
  getQuestionnaireItem,
  getCarePlansAndReferralRequests,
  getPreviousQuestion,
  getQuestionnaireResponseAnswers,
  getAllIssues,
} from '@/lib/online-consultations/mappers/response';
import getQuestion from '@/lib/online-consultations/mappers/item';
import { getParameters, getAnswerFromItem } from '@/lib/online-consultations/mappers/parameters';
import { DATA_REQUIRED, SUCCESS } from '@/lib/online-consultations/constants/status-types';

const showError = (store) => {
  store.dispatch('onlineConsultations/clearAndSetError');
};

export default {
  clear({ commit }, resetRequestId) {
    commit(CLEAR, resetRequestId);
  },
  getServiceDefinitions({ commit }, params) {
    const store = this;
    const { provider } = params;

    return store.app.$cdsApi.getFhirServiceDefinitionByProvider({
      provider,
    }).then((response) => {
      commit(CLEAR);

      if (response === undefined) {
        showError(store);
        return;
      }

      commit(SET_SERVICE_DEFINITIONS, response);
    }).catch(() => {
      showError(store);
    });
  },
  getServiceDefinition({ commit }, params) {
    const store = this;
    const { serviceDefinitionId, provider } = params;

    return store.app.$cdsApi.getFhirServiceDefinitionByProviderByServicedefinitionid({
      serviceDefinitionId,
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
        return;
      }

      showError(store);
    }).catch(() => {
      showError(store);
    }).finally(() => {
      commit(UPDATE_REQUEST_ID);
    });
  },
  evaluateServiceDefinition({ commit, state, rootState }, params) {
    const store = this;
    const { serviceDefinitionId, provider, addJavascriptDisabledHeader } = params;
    const parameters = getParameters(state, rootState);

    if (parameters === undefined) {
      showError(store);
      return undefined;
    }


    return store.app.$cdsApi.postFhirServiceDefinitionByProviderByServicedefinitionidEvaluate({
      serviceDefinitionId,
      provider,
      addJavascriptDisabledHeader,
      parameters,
    }).then((response) => {
      commit(CLEAR);
      if (response === undefined) {
        showError(store);
        return;
      }

      const { status } = response;
      commit(SET_STATUS, status);

      if (status === DATA_REQUIRED) {
        const sessionId = getSessionId(response);
        const question = getQuestion(getQuestionnaireItem(response));
        const issues = getAllIssues(response);
        const previousQuestion = getQuestion(getPreviousQuestion(response));
        const previousAnswers = getQuestionnaireResponseAnswers(response);

        if (sessionId === undefined || question === undefined) {
          showError(store);
          return;
        }

        commit(SET_SESSION_ID, sessionId);
        commit(SET_QUESTION, question);
        commit(SET_PREVIOUS_QUESTION, previousQuestion);
        if (issues !== undefined) {
          commit(SET_VALIDATION_ERROR_FROM_RESPONSE, issues);
        }
        if (previousAnswers !== undefined) {
          const answersFormatted = getAnswerFromItem(question, previousAnswers);
          commit(SET_ANSWER, answersFormatted, question.type);
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
    }).catch(() => {
      showError(store);
    }).finally(() => {
      commit(UPDATE_REQUEST_ID);
    });
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
  clearClientErrors({ commit }) {
    commit(CLEAR_CLIENT_ERRORS);
  },
};
