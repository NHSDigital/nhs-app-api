import {
  initialState,
  INIT_APPOINTMENT,
  BOOK_SUCCESS,
  RESET_BOOKED,
  SAVE_SELECTED,
  RESET_SELECTED,
} from './mutation-types';

/* eslint-disable no-shadow */
/* eslint-disable no-param-reassign */
/* eslint-disable no-unused-vars */
export default {
  [INIT_APPOINTMENT](state) {
    state = initialState;
  },
  [BOOK_SUCCESS](state) {
    state.justBookedAnAppointment = true;
  },
  [RESET_BOOKED](state) {
    state.justBookedAnAppointment = false;
  },
  [SAVE_SELECTED](state, selected) {
    state.tempSelectedSlot = selected;
  },
  [RESET_SELECTED](state) {
    state.tempSelectedSlot = undefined;
  },
};
