import {
  INIT_APPOINTMENT,
  BOOK_SUCCESS,
  RESET_BOOKED,
  SAVE_SELECTED,
  RESET_SELECTED,
} from './mutation-types';

export default{
  init({ commit }) {
    commit(INIT_APPOINTMENT);
  },
  save({ commit }) {
    const selected = this.state.appointmentSlots.slots.find(slot => slot.selected);
    commit(SAVE_SELECTED, selected);
  },
  reset({ commit }) {
    commit(RESET_SELECTED);
  },
  resetJustBooked({ commit }) {
    commit(RESET_BOOKED);
  },
  bookAppointment({ commit }, slot) {
    const param = {
      appointmentBookRequest: slot,
    };
    this.app.$http
      .postV1PatientAppointments(param)
      .then(() => {
        commit(BOOK_SUCCESS);
        this.app.router.push('/appointments');
      });
  },
};
