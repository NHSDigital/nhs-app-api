export const LOADED = 'LOADED';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const CLEAR_SELECTED_APPOINTMENT = 'CLEAR_SELECTED_APPOINTMENT';
export const SELECT = 'SELECT';
export const CLEAR_APPOINTMENTS = 'CLEAR_APPOINTMENTS';
export const CANCELLING_JOURNEY_START = 'CANCELLING_JOURNEY_START';
export const CANCELLING_JOURNEY_COMPLETE = 'CANCELLING_JOURNEY_COMPLETE';
export const initialState = () => ({
  pastAppointments: [],
  upcomingAppointments: [],
  cancellationReasons: [],
  selectedAppointment: null,
  hasLoaded: false,
  hasErrored: false,
  pastAppointmentsEnabled: false,
  cancellingInProgress: false,
});
