import Vue from 'vue';
import {
  SLOTS_LOADED,
} from './mutation-types';

export const load = ({ commit }, { API_HOST }) =>
  Vue
    .$http
    .get(`${API_HOST}/patient/appointmentslots`)
    .then(({ data } = {}) => {
      commit(SLOTS_LOADED, data);
    });

export default {
  load,
};
