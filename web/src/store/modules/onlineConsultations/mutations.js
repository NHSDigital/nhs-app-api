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
} from './mutation-types';

export default {
  [CLEAR](state, resetRequestId) {
    state.sessionId = undefined;
    state.status = undefined;
    state.dataRequirements = undefined;
    state.question = undefined;
    state.previousQuestion = undefined;
    state.previousSelected = false;
    state.previousAnswers = undefined;
    state.answer = undefined;
    state.answerIsValid = false;
    state.answerIsEmpty = true;
    state.min = undefined;
    state.max = undefined;
    state.latestErrorMessage = undefined;
    state.validationError = false;
    state.validationErrorMessage = undefined;
    state.error = false;
    state.previousRoute = undefined;
    state.carePlans = undefined;
    state.referralRequests = undefined;
    state.isLoadingFile = false;
    state.issues = undefined;
    state.validationErrorMessageFromResponse = undefined;
    state.additionalValue = undefined;
    state.latestAdditionalValue = undefined;
    if (resetRequestId) {
      state.requestId = 0;
    }
  },
  [SET_SESSION_ID](state, sessionId) {
    state.sessionId = sessionId;
  },
  [SET_STATUS](state, status) {
    state.status = status;
  },
  [SET_DATA_REQUIREMENTS](state, dataRequirements) {
    state.dataRequirements = dataRequirements;
  },
  [SET_QUESTION](state, question) {
    state.question = question;
  },
  [SET_PREVIOUS_QUESTION](state, previousQuestion) {
    state.previousQuestion = previousQuestion;
  },
  [PREVIOUS_SELECTED](state) {
    state.previousSelected = true;
  },
  [SET_ANSWER](state, answer) {
    state.answer = answer;
  },
  [SET_ANSWER_IS_VALID](state, { isValid, message, isEmpty, additionalValue } = {}) {
    state.answerIsValid = isValid;
    state.latestErrorMessage = message;
    state.answerIsEmpty = isEmpty;
    state.additionalValue = additionalValue;
  },
  [SET_VALIDATION_ERROR](state) {
    state.validationError = !state.answerIsValid;
    state.validationErrorMessage = state.latestErrorMessage;
    state.latestAdditionalValue = state.additionalValue;
  },
  [SET_VALIDATION_ERROR_FROM_RESPONSE](state, issues) {
    state.validationError = !state.answerIsValid;
    state.validationErrorMessageFromResponse = issues;
  },
  [SET_PREVIOUS_ROUTE](state, previousRoute) {
    state.previousRoute = previousRoute;
  },
  [SET_CARE_PLANS](state, carePlans) {
    state.carePlans = carePlans;
  },
  [SET_REFERRAL_REQUESTS](state, referralRequests) {
    state.referralRequests = referralRequests;
  },
  [SET_ERROR](state, error) {
    state.error = error;
  },
  [SET_ANSWER_IS_EMPTY](state, answerIsEmpty) {
    state.answerIsEmpty = answerIsEmpty;
  },
  [UPDATE_REQUEST_ID](state) {
    state.requestId += 1;
  },
  [FILE_LOADING](state) {
    state.isLoadingFile = true;
  },
  [FILE_LOAD_COMPLETE](state) {
    state.isLoadingFile = false;
  },
  [CLEAR_VALIDATION](state) {
    state.validationError = undefined;
    state.validationErrorMessage = undefined;
  },
  [CLEAR_CLIENT_ERRORS](state) {
    state.validationError = false;
    state.validationErrorMessageFromResponse = undefined;
  },
};
