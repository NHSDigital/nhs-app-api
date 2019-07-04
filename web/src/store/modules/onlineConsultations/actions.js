import {
  CLEAR,
  SET_SESSION_ID,
  SET_STATUS,
  SET_QUESTION,
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
} from './mutation-types';
import {
  getSessionId,
  getQuestionnaireItem,
  getCarePlansAndReferralRequests,
  getAllIssues,
} from '@/lib/online-consultations/mappers/guidance-response';
import getQuestion from '@/lib/online-consultations/mappers/item';
import getParameters from '@/lib/online-consultations/mappers/parameters';
import { DATA_REQUIRED, SUCCESS } from '@/lib/online-consultations/constants/status-types';

const showError = (store) => {
  store.dispatch('onlineConsultations/clearAndSetError');
};

export default {
  clear({ commit }, resetRequestId) {
    commit(CLEAR, resetRequestId);
  },
  evaluateServiceDefinition({ commit, state, rootState }) {
    const store = this;
    const parameters = getParameters(state, rootState);

    if (parameters === undefined) {
      showError(store);
      return undefined;
    }

    return store.app.$cdsApi.postFhirServiceDefinitionEvaluate({
      parameters,
      serviceDefinitionId: 'GEC_ADM',
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

        if (sessionId === undefined || question === undefined) {
          showError(store);
          return;
        }

        commit(SET_SESSION_ID, sessionId);
        commit(SET_QUESTION, question);
        if (issues !== undefined) {
          commit(SET_VALIDATION_ERROR_FROM_RESPONSE, issues);
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
};
