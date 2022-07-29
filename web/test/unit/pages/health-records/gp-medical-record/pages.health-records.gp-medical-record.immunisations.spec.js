/* eslint-disable import/no-extraneous-dependencies */
import ImmunisationsPage from '@/pages/health-records/gp-medical-record/immunisations';
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

  shallowMount(ImmunisationsPage, {
    $store,
    mountOpts: { i18n },
  });
};

describe('gp-medical-record immunisations audit log', () => {
  each([
    ['EMIS'],
    ['VISION'],
  ])
    .it('will post the audit log for %s', (supplierName) => {
      mountPage({ supplierName });
      const expectedOperation = 'PatientRecord_Section_View_Response';
      const expectedDetails = 'Patient record IMMUNISATIONS successfully retrieved.';
      expect($store.dispatch).toHaveBeenCalledWith('log/postOperationAudit', {
        operation: expectedOperation,
        details: expectedDetails,
      });
    });
});
