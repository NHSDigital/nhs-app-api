import { INIT_APPOINTMENTS, SLOT_SELECTED, SLOTS_LOADED, SLOTS_CLEAR } from './mutation-types';

export default {
  getAppointmentsSlotsParameters: () => {
    const getToDate = () => {
      const rangeInDays = 14;

      const date = new Date();
      date.setDate(date.getDate() + rangeInDays);
      date.setUTCHours(0, 0, 0, 0);

      return date;
    };
    return {
      fromDate: new Date().toISOString(),
      toDate: getToDate().toISOString(),
    };
  },
  init({ commit }) {
    commit(INIT_APPOINTMENTS);
  },
  reset({ commit }) {
    commit(SLOTS_CLEAR);
  },
  load({ commit }) {
    const getFromDate = () => new Date();
    const getToDate = () => {
      const rangeInDays = 14;

      const date = getFromDate();
      date.setDate(date.getDate() + rangeInDays);
      date.setUTCHours(0, 0, 0, 0);

      return date;
    };
    const appointmentSlotParmas = {
      fromDate: getFromDate().toISOString(),
      toDate: getToDate().toISOString(),
    };
    return this.app.$http
      .getV1PatientAppointmentSlots(appointmentSlotParmas)
      .then((data) => {
        commit(SLOTS_LOADED, data);
      });
  },
  select({ commit }, slot) {
    commit(SLOT_SELECTED, slot);
  },
};
