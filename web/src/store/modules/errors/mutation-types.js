import ErrorSettings from '@/components/errors/Settings';

export const ADD_API_ERROR = 'ADD_API_ERROR';
export const SET_ROUTE_PATH = 'SET_ROUTE_PATH';
export const DISABLE_API_ERROR = 'DISABLE_API_ERROR';
export const CLEAR_ALL_API_ERRORS = 'CLEAR_ALL_API_ERRORS';
export const SET_CONNECTION_PROBLEM = 'SET_CONNECTION_PROBLEM';
export const SET_ERROR_TITLE = 'SET_ERROR_TITLE';
export const initialState = () => ({
  showApiError: true,
  apiErrors: [],
  pageSettings: ErrorSettings.default,
  hasConnectionProblem: false,
  routePath: '',
  errorTitle: '',
});
export const handledErrors = [464, 465];
export const standardErrors = [400, 403, 409, 460, 461, 466];
