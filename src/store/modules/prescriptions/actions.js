import Vue from 'vue';
import moment from 'moment';
import {
  PRESCRIPTIONS_LOADED,
  PRESCRIPTIONS_CLEAR,
} from './mutation-types';

function getFromDate() {
  return moment().subtract(6, 'months').toISOString();
}

function getPrescriptionsParameters() {
  return {
    fromDate: getFromDate(),
  };
}

export const load = ({ commit }) =>
  Vue
    .$http
    .getV1PatientPrescriptions(getPrescriptionsParameters())
    .then((data) => {
      commit(PRESCRIPTIONS_LOADED, data);
    });

export const clear = ({ commit }) => commit(PRESCRIPTIONS_CLEAR);

export default {
  load,
  clear,
};
