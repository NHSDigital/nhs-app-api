import Vue from 'vue';
import {
  SLOT_SELECTED,
  SLOTS_LOADED,
} from './mutation-types';

function getFromDate() {
  return new Date();
}

function getToDate() {
  const rangeInDays = 14;

  const date = getFromDate();
  date.setDate(date.getDate() + rangeInDays);
  date.setUTCHours(0, 0, 0, 0);

  return date;
}

function getAppointmentsSlotsParameters() {
  return {
    fromDate: getFromDate().toISOString(),
    toDate: getToDate().toISOString(),
  };
}

export const load = ({ commit }) =>
  Vue
    .$http
    .getV1PatientAppointmentSlots(getAppointmentsSlotsParameters())
    .then((data) => {
      commit(SLOTS_LOADED, data);
    });

export const select = ({ commit }, slotId) => {
  commit(SLOT_SELECTED, slotId);
};

export default {
  load,
  select,
};
