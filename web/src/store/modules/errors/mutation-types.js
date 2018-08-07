import ErrorSettings from '@/components/errors/Settings';

export const ADD_API_ERROR = 'ADD_API_ERROR';
export const SET_ROUTE_PATH = 'SET_ROUTE_PATH';
export const DISABLE_API_ERROR = 'DISABLE_API_ERROR';
export const CLEAR_ALL_API_ERRORS = 'CLEAR_ALL_API_ERRORS';
export const SET_CONNECTION_PROBLEM = 'SET_CONNECTION_PROBLEM';
export const ADD_ERROR = 'ADD_ERROR';
export const initialState = () => ({
  showApiError: true,
  apiErrors: [],
  pageSettings: ErrorSettings.default,
  hasConnectionProblem: false,
  routePath: '',
  errors: [],
});
