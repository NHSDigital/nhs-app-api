import moment from 'moment/moment';
import {
  GP_SESSION_ERROR_STATUS,
  GP_SESSION_ON_DEMAND_ERROR_STATUS,
  createLocalError,
} from '@/lib/utils';
import {
  PRESCRIPTIONS_CLEAR,
  PRESCRIPTIONS_LOADED,
  ADD_ERROR,
} from './mutation-types';


export default {
  async load({ commit }) {
    try {
      const data = await this.app.$http.getV1PatientPrescriptions({
        fromDate: moment().subtract(6, 'months').toISOString(),
        ignoreError: true,
      });

      commit(PRESCRIPTIONS_LOADED, data);

      sessionStorage.removeItem('hasRetried');
    } catch (error) {
      if (error.response.status === GP_SESSION_ON_DEMAND_ERROR_STATUS) {
        return;
      }

      if (error.response.status === GP_SESSION_ERROR_STATUS) {
        commit(ADD_ERROR, createLocalError(error));
      } else {
        this.dispatch('errors/addApiError', error);
      }
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  clear({ commit }) {
    commit(PRESCRIPTIONS_CLEAR);
  },
};
