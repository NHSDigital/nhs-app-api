export const CLEAR = 'CLEAR';
export const CLEAR_VALIDATION = 'CLEAR_VALIDATION';
export const CLEAR_CLIENT_ERRORS = 'CLEAR_CLIENT_ERRORS';
export const SET_SESSION_ID = 'SET_SESSION_ID';
export const SET_GP_ADVICE_SERVICE_DEFINITION_ID = 'SET_GP_ADVICE_SERVICE_DEFINITION_ID';
export const UPDATE_REQUEST_ID = 'UPDATE_REQUEST_ID';
export const SET_STATUS = 'SET_STATUS';
export const SET_DATA_REQUIREMENTS = 'SET_DATA_REQUIREMENTS';
export const SET_QUESTION = 'SET_QUESTION';
export const SET_PREVIOUS_QUESTION = 'SET_PREVIOUS_QUESTION';
export const SET_ANSWER = 'SET_ANSWER';
export const SET_ANSWER_IS_VALID = 'SET_ANSWER_IS_VALID';
export const SET_VALIDATION_ERROR = 'SET_VALIDATION_ERROR';
export const SET_CARE_PLANS = 'SET_CARE_PLANS';
export const SET_REFERRAL_REQUESTS = 'SET_REFERRAL_REQUESTS';
export const SET_ERROR = 'SET_ERROR';
export const SET_ANSWER_IS_EMPTY = 'SET_ANSWER_IS_EMPTY';
export const SET_VALIDATION_ERROR_FROM_RESPONSE = 'SET_VALIDATION_ERROR_FROM_RESPONSE';
export const FILE_LOADING = 'FILE_LOADING';
export const FILE_LOAD_COMPLETE = 'FILE_LOAD_COMPLETE';
export const PREVIOUS_SELECTED = 'PREVIOUS_SELECTED';
export const SET_SERVICE_DEFINITIONS = 'SET_SERVICE_DEFINITIONS';
export const SET_PREVIOUS_ROUTE = 'SET_PREVIOUS_ROUTE';

export const initialState = () => ({
  sessionId: undefined,
  status: undefined,
  dataRequirements: undefined,
  question: undefined,
  serviceDefinitions: undefined,
  previousQuestion: undefined,
  previousSelected: false,
  previousAnswers: undefined,
  answer: undefined,
  answerIsValid: false,
  answerIsEmpty: true,
  latestErrorMessage: undefined,
  validationError: false,
  validationErrorMessage: undefined,
  previousRoute: undefined,
  carePlans: undefined,
  referralRequests: undefined,
  error: false,
  requestId: 0,
  isLoadingFile: false,
  issues: undefined,
  validationErrorMessageFromResponse: undefined,
  additionalValue: undefined,
  latestAdditionalValue: undefined,
  gpAdviceServiceDefinitionId: undefined,
});
