export const ADD_ERROR = 'ADD_ERROR';
export const CLEAR = 'CLEAR';
export const CLEAR_ERROR = 'CLEAR_ERROR';
export const DESELECT = 'DESELECT';
export const FILTER = 'FILTER';
export const LOAD = 'LOAD';
export const INIT = 'INIT';
export const SELECT = 'SELECT';
export const SET_BOOKING_REASON_NECESSITY = 'SET_BOOKING_REASON_NECESSITY';
export const SET_SELECTED_OPTIONS = 'SET_SELECTED_OPTIONS';
export const BOOKING_JOURNEY_COMPLETE = 'BOOKING_JOURNEY_COMPLETE';
export const BOOKING_JOURNEY_START = 'BOOKING_JOURNEY_START';
export const initialState = () => ({
  booked: false,
  bookingGuidance: '',
  bookingInProgress: false,
  bookingReasonNecessity: '',
  error: null,
  filteredSlots: [],
  filtersOptions: {
    clinicians: [],
    dates: [],
    locations: [],
    types: [],
  },
  hasLoaded: false,
  patientTelephoneNumbers: [],
  selectedOptions: {
    clinician: '',
    date: 'all',
    location: '',
    type: '',
  },
  selectedSlot: null,
  slots: new Map(),
});
