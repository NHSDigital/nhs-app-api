import moment from 'moment/moment';
import get from 'lodash/fp/get';
import { GP_SESSION_ERROR_STATUS } from '@/lib/utils';
import {
  PRESCRIPTIONS_CLEAR,
  PRESCRIPTIONS_LOADED,
  ADD_ERROR,
} from './mutation-types';

const createError = ({ response }) => ({
  status: response.status || '',
  serviceDeskReference: get('serviceDeskReference')(response.data) || '',
});

export default {
  async load({ commit, rootState }) {
    try {
      const data = await this.app.$http.getV1PatientPrescriptions({
        fromDate: moment().subtract(6, 'months').toISOString(),
        ignoreError: true,
      });

      commit(PRESCRIPTIONS_LOADED, data);

      if (rootState.device.isNativeApp) {
        sessionStorage.removeItem('hasRetried');
      }
      this.dispatch('session/setRetry', false);
    } catch (error) {
      if (error.response.status !== GP_SESSION_ERROR_STATUS) {
        this.dispatch('errors/addApiError', error);
      } else {
        commit(ADD_ERROR, createError(error));
      }
    } finally {
      this.dispatch('device/unlockNavBar');
    }
  },
  clear({ commit }) {
    commit(PRESCRIPTIONS_CLEAR);
  },
};
