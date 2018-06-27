export const LOADED = 'LOADED';
export const CLEAR = 'CLEAR';
export const INIT = 'INIT';
export const CLEAR_SELECTED_APPOINTMENT = 'CLEAR_SELECTED_APPOINTMENT';
export const SELECT = 'SELECT';
export const CLEAR_APPOINTMENTS = 'CLEAR_APPOINTMENTS';
export const CANCEL_SUCCESS = 'CANCEL_SUCCESS';
export const initialState = {
  appointmentSessions: [],
  clinicians: [],
  locations: [],
  appointments: [],
  cancellationReasons: [],
  selectedAppointment: null,
  justCancelledAnAppointment: false,
  hasLoaded: false,
  hasErrored: false,
};
