import Vue from 'vue';
import {
  SLOT_SELECTED,
  SLOTS_LOADED,
} from './mutation-types';

export const load = ({ commit }, { API_HOST }) =>
  Vue
    .$http
    .get(`${API_HOST}/patient/appointmentslots`)
    .then(({ data } = {}) => {
      commit(SLOTS_LOADED, data);
    });

export const select = ({ commit }, slotId) => {
  commit(SLOT_SELECTED, slotId);
};

export default {
  load,
  select,
};
