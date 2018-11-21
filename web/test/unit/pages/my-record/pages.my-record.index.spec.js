/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import MyRecord from '@/pages/my-record/';
import Warning from '@/components/my-record/Warning';
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
});

const createHttp = () => ({
  getV1PatientDemographics: jest.fn().mockResolvedValue({}),
  getV1PatientMyRecord: jest.fn().mockResolvedValue({
    hasSummaryRecordAccess: false,
  }),
});

const mountPage = ({ $http = createHttp(), $state = createState(), $store }) =>
  mount(MyRecord, { $http, $store: ($store || createStore({ $http, $state })), $style });

describe('my-record', () => {
  let $store;
  let page;

  beforeEach(() => {
    Vue.filter('longDate', () => {});
    $store = createStore({ $http: createHttp(), state: createState() });
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

    describe('myRecordSectionClick', () => {
      it('will dispatch TOGGLE_PATIENT_DETAIL when the section is patientdetails', () => {
        page.vm.myRecordSectionClick('patientdetails');
        expect($store.dispatch).toHaveBeenCalledWith('myRecord/togglePatientDetail');
      });

      it('will not dispatch TOGGLE_PATIENT_DETAIL when the section is not patientdetails', () => {
        page.vm.myRecordSectionClick('bar');
        expect($store.dispatch).not.toHaveBeenCalledWith('myRecord/togglePatientDetail');
      });
    });
  });
});
