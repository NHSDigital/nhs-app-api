/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import GPMedicalRecord from '@/pages/gp-medical-record/';
import Warning from '@/components/my-record/Warning';
import Glossary from '@/components/Glossary';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, mount } from '../../helpers';

const $style = {
  info: 'info',
  h2: 'h2',
  button: [],
};

const createState = () => ({
  myRecord: initialState(),
  noJs: {
    myRecord: {},
  },
  device: {
    source: 'web',
  },
});

const createHttp = () => ({
  getV1PatientDemographics: jest.fn().mockResolvedValue({}),
  getV1PatientMyRecord: jest.fn().mockResolvedValue({
    hasSummaryRecordAccess: false,
  }),
});

const mountPage = ({ $http = createHttp(), $state = createState(), $store }) =>
  mount(GPMedicalRecord, { $http, $store: ($store || createStore({ $http, $state })), $style });

describe('gp-medical-record', () => {
  let $store;
  let page;

  beforeEach(() => {
    Vue.filter('longDate', () => {});
    $store = createStore({ $http: createHttp(),
      state: createState(),
      $env: {
        CLINICAL_ABBREVIATIONS_URL: 'www.foo.com',
        MY_RECORD_DOCUMENTS_ENABLED: true,
      } });
  });


  describe('terms not accepted', () => {
    beforeEach(() => {
      $store.state.myRecord.hasAcceptedTerms = false;
      page = mountPage({ $store });
    });

    it('will display the warning', () => {
      expect(page.find(Warning).exists()).toBe(true);
    });
  });

  describe('terms accepted', () => {
    beforeEach(() => {
      $store.state.myRecord.hasAcceptedTerms = true;
      page = mountPage({ $store });
    });

    it('will not display the warning', () => {
      expect(page.find(Warning).exists()).toBe(false);
    });

    it('will display the clinical feedback updates', () => {
      expect(page.find(Glossary).exists()).toBe(false);
    });
  });

  describe('has access to SCR', () => {
    beforeEach(() => {
      $store.state.myRecord.hasAcceptedTerms = true;
      $store.state.myRecord.record.hasSummaryRecordAccess = true;
      page = mountPage({ $store });
    });

    it('will display the clinical feedback updates', () => {
      expect(page.find(Glossary).exists()).toBe(true);
    });
  });
});
