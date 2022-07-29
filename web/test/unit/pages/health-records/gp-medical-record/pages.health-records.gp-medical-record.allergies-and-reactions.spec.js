/* eslint-disable import/no-extraneous-dependencies */
import AllergiesAndReactionsPage from '@/pages/health-records/gp-medical-record/allergies-and-reactions';
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

  shallowMount(AllergiesAndReactionsPage, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record allergies and adverse reactions audit log', () => {
  each([
    ['EMIS'],
    ['TPP'],
    ['VISION'],
  ])
    .it('will post the audit log for %s', (supplierName) => {
      mountPage({ supplierName });
      const expectedOperation = 'PatientRecord_Section_View_Response';
      const expectedDetails = 'Patient record ALLERGIES AND ADVERSE REACTIONS successfully retrieved.';
      expect($store.dispatch).toHaveBeenCalledWith('log/postOperationAudit', {
        operation: expectedOperation,
        details: expectedDetails,
      });
    });
});
