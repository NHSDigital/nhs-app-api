/* eslint-disable import/no-extraneous-dependencies */
import HealthConditionsPage from '@/pages/health-records/gp-medical-record/health-conditions';
import i18n from '@/plugins/i18n';
import { createStore, shallowMount } from '../../../helpers';
import each from 'jest-each';

let $store;

const mountPage = ({ supplierName = 'EMIS' } = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp: false },
      myRecord: {
        record: {
          supplier: supplierName,
        },
      },
    },
  });

  shallowMount(HealthConditionsPage, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record health conditions reactions audit log', () => {
  each([
    ['EMIS'],
    ['VISION'],
  ])
    .it('will post the audit log for %s', (supplierName) => {
      mountPage({ supplierName });
      const expectedOperation = 'PatientRecord_Section_View_Response';
      const expectedDetails = 'Patient record HEALTH CONDITIONS successfully retrieved.';
      expect($store.dispatch).toHaveBeenCalledWith('log/postOperationAudit', {
        operation: expectedOperation,
        details: expectedDetails,
      });
    });
});
