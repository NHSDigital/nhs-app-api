export const SELECT = 'SELECT';
export const DESELECT = 'DESELECT';
export const CLEAR = 'CLEAR';
export const LOAD = 'LOAD';
export const INIT = 'INIT';
export const FILTER = 'FILTER';
export const SET_BOOKING_REASON_NECESSITY = 'SET_BOOKING_REASON_NECESSITY';
export const SET_SELECTED_OPTIONS = 'SET_SELECTED_OPTIONS';
export const BOOKING_JOURNEY_COMPLETE = 'BOOKING_JOURNEY_COMPLETE';
export const BOOKING_JOURNEY_START = 'BOOKING_JOURNEY_START';
export const initialState = () => ({
  slots: new Map(),
  bookingGuidance: '',
  bookingReasonNecessity: '',
  patientTelephoneNumbers: [],
  filteredSlots: [],
  hasLoaded: false,
  selectedSlot: null,
  booked: false,
  bookingInProgress: false,
  filtersOptions: {
    types: [],
    locations: [],
    clinicians: [],
    dates: [],
  },
  selectedOptions: {
    type: '',
    location: '',
    clinician: '',
    date: 'all',
  },
});
