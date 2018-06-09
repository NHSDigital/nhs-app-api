export const SLOT_SELECTED = 'SLOT_SELECTED';
export const SLOTS_CLEAR = 'SLOTS_CLEAR';
export const SLOTS_LOADED = 'SLOTS_LOADED';
export const INIT_APPOINTMENTS = 'INIT_APPOINTMENTS';
export const initialState = {
  appointmentSessions: [],
  clinicians: [],
  locations: [],
  slots: [],
  hasLoaded: false,
  hasErrored: false,
  selectedSlotId: undefined,
};
