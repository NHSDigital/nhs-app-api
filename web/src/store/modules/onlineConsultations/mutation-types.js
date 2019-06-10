export const CLEAR = 'CLEAR';
export const SET_SERVICE_DEFINITION_ID = 'SET_ADMIN_HELP_SERVICE_DEFINITION_ID';
export const SET_QUESTION_FROM_GUIDANCE_RESPONSE = 'SET_QUESTION_FROM_GUIDANCE_RESPONSE';

export const initialState = () => ({
  serviceDefinitionId: undefined,
  question: undefined,
  error: false,
});
