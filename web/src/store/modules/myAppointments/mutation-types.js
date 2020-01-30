export const ADD_ERROR = 'ADD_ERROR';
export const CANCELLING_JOURNEY_COMPLETE = 'CANCELLING_JOURNEY_COMPLETE';
export const CANCELLING_JOURNEY_START = 'CANCELLING_JOURNEY_START';
export const CLEAR = 'CLEAR';
export const CLEAR_APPOINTMENTS = 'CLEAR_APPOINTMENTS';
export const CLEAR_ERROR = 'CLEAR_ERROR';
export const CLEAR_SELECTED_APPOINTMENT = 'CLEAR_SELECTED_APPOINTMENT';
export const INIT = 'INIT';
export const LOADED = 'LOADED';
export const SELECT = 'SELECT';
export const initialState = () => ({
  cancellationReasons: [],
  cancellingInProgress: false,
  error: null,
  hasLoaded: false,
  pastAppointments: [],
  pastAppointmentsEnabled: false,
  upcomingAppointments: [],
});
