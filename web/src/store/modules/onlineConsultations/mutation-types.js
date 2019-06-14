export const CLEAR = 'CLEAR';
export const SET_SERVICE_DEFINITION_ID = 'SET_ADMIN_HELP_SERVICE_DEFINITION_ID';
export const SET_QUESTION_FROM_GUIDANCE_RESPONSE = 'SET_QUESTION_FROM_GUIDANCE_RESPONSE';
export const SET_PREVIOUS_ROUTE = 'SET_PREVIOUS_ROUTE';

export const initialState = () => ({
  serviceDefinitionId: undefined,
  question: undefined,
  error: false,
  previousRoute: undefined,
});
