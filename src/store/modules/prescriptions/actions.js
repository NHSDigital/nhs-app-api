import moment from 'moment/moment';
import { PRESCRIPTIONS_CLEAR, PRESCRIPTIONS_LOADED, INIT_PRESCRIPTIONS } from './mutation-types';

function getFromDate() {
  return moment()
    .subtract(6, 'months')
    .toISOString();
}

function getPrescriptionsParameters() {
  return {
    fromDate: getFromDate(),
  };
}
export default {
  load({ commit }) {
    return this.app.$http
      .getV1PatientPrescriptions(getPrescriptionsParameters())
      .then((data) => {
        commit(PRESCRIPTIONS_LOADED, data);
      });
  },
  init({ commit }) {
    commit(INIT_PRESCRIPTIONS);
  },
  clear({ commit }) {
    commit(PRESCRIPTIONS_CLEAR);
  },
};
