/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import GPMedicalRecord from '@/pages/gp-medical-record/';
import Warning from '@/components/my-record/Warning';
import Glossary from '@/components/Glossary';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, mount, createRouter } from '../../helpers';
import agreedToMedicalWarning from '@/lib/sessionStorage';

jest.mock('@/lib/sessionStorage');

const $style = {
  info: 'info',
  h2: 'h2',
  button: [],
};

const $router = createRouter();

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

const mountPage =
({ $http = createHttp(), $state = createState(), $store }) =>
  mount(GPMedicalRecord,
    { $http, $router, $store: ($store || createStore({ $http, $state })), $style });

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
    $router.previousPaths = ['/', '/gp-medical-record/medicines'];
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

  describe('should load medical record', () => {
    beforeEach(() => {
      process.client = true;
      process.server = false;
    });

    it('should not load the medical record if the terms are not accepted', () => {
      agreedToMedicalWarning.mockImplementation(() => false);

      page = mountPage({ $store });

      expect(page.vm.shouldLoadRecord()).toBe(false);
    });

    it('should load the medical record if the medical record is not loaded ' +
       'and the user comes from an external page', () => {
      agreedToMedicalWarning.mockImplementation(() => true);
      $store.state.myRecord.hasLoaded = false;
      $router.previousPaths = ['/', '/symptoms'];

      page = mountPage({ $store, $router });

      expect(page.vm.shouldLoadRecord()).toBe(true);
    });

    it('should load the medical record if the medical record is not loaded ' +
       'and the user comes from within the medical record', () => {
      agreedToMedicalWarning.mockImplementation(() => true);
      $store.state.myRecord.hasLoaded = false;
      $router.previousPaths = ['/', '/gp-medical-record/medicines'];

      page = mountPage({ $store, $router });

      expect(page.vm.shouldLoadRecord()).toBe(true);
    });

    it('should load the medical record if the medical record is already loaded' +
    'and the `reload` flag has been set to true ', () => {
      agreedToMedicalWarning.mockImplementation(() => true);
      $store.state.myRecord.hasLoaded = true;
      $store.state.myRecord.reload = true;

      page = mountPage({ $store, $router });

      expect(page.vm.shouldLoadRecord()).toBe(true);
    });

    it('should not load the medical record if the medical record is already loaded ' +
       'and the `reload` flag has been set to false ', () => {
      agreedToMedicalWarning.mockImplementation(() => true);
      $store.state.myRecord.hasLoaded = true;
      $store.state.myRecord.reload = false;

      page = mountPage({ $store, $router });

      expect(page.vm.shouldLoadRecord()).toBe(false);
    });
  });
});
