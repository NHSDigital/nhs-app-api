export const CLEAR = 'CLEAR';
export const CLEAR_VALIDATION = 'CLEAR_VALIDATION';
export const SET_SESSION_ID = 'SET_SESSION_ID';
export const SET_GP_ADVICE_SERVICE_DEFINITION_ID = 'SET_GP_ADVICE_SERVICE_DEFINITION_ID';
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
export const SET_PREVIOUS_ROUTE = 'SET_PREVIOUS_ROUTE';
export const SET_DEMOGRAPHICS_CONSENT_GIVEN = 'SET_DEMOGRAPHICS_CONSENT_GIVEN';
export const SET_DEMOGRAPHICS_QUESTION_ANSWERED = 'SET_DEMOGRAPHICS_QUESTION_ANSWERED';
export const SET_ADMIN_PROVIDER_NAME = 'SET_ADMIN_PROVIDER_NAME';
export const SET_ADVICE_PROVIDER_NAME = 'SET_ADVICE_PROVIDER_NAME';
export const SET_CONDITIONS_LIST = 'SET_CONDITIONS_LIST';

export const initialState = () => ({
  demographicsQuestionAnswered: false,
  demographicsConsentGiven: false,
  sessionId: undefined,
  status: undefined,
  dataRequirements: undefined,
  question: undefined,
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
  isLoadingFile: false,
  issues: undefined,
  validationErrorMessageFromResponse: undefined,
  additionalValue: undefined,
  latestAdditionalValue: undefined,
  gpAdviceServiceDefinitionId: undefined,
  adminProviderName: undefined,
  adviceProviderName: undefined,
  conditionsList: undefined,
});
