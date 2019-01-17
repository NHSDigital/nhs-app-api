import mutations from '@/store/modules/myRecord/mutations';
import {
  initialState,
  ACCEPT_TERMS,
  LOADED,
  LOADED_DETAILED_TEST_RESULT,
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
} from '@/store/modules/myRecord/mutation-types';

describe('my record mutations', () => {
  let state;
  let data;

  beforeEach(() => {
    data = {
      record: 'record',
      patientDetails: 'details',
    };

    state = initialState();
  });

  describe('ACCEPT_TERMS', () => {
    beforeEach(() => {
      mutations[ACCEPT_TERMS](state, data);
    });

    it('will set the my record term acceptance flag ', () => {
      expect(state.hasAcceptedTerms).toEqual(true);
    });
  });

  describe('LOADED', () => {
    beforeEach(() => {
      mutations[LOADED](state, data);
    });

    it('will set the patient details ', () => {
      expect(state.patientDetails).toEqual(data.patientDetails);
    });

    it('will set the my record', () => {
      expect(state.record).toEqual(data.record);
    });

    it('will set has loaded to true', () => {
      expect(state.hasLoaded).toEqual(true);
    });

    it('will set is patient details collapsed loaded to false', () => {
      expect(state.isPatientDetailsCollapsed).toEqual(false);
    });
  });

  describe('LOADED_DETAILED_TEST_RESULT', () => {
    const testResults = { data: 'foobar' };

    beforeEach(() => {
      mutations[LOADED_DETAILED_TEST_RESULT](state, testResults);
    });

    it('will set the test results data to the received test results', () => {
      expect(state.detailedTestResult.data).toEqual(testResults.data);
    });

    it('will set the has loaded property of the test results to true', () => {
      expect(state.detailedTestResult.hasLoaded).toEqual(true);
    });
  });

  describe('RESET_TERMS', () => {
    beforeEach(() => {
      state.hasAcceptedTerms = true;
      mutations[RESET_TERMS](state, data);
    });

    it('will set the my record term acceptance flag ', () => {
      expect(state.hasAcceptedTerms).toEqual(false);
    });
  });

  describe('TOGGLE_PATIENT_DETAIL', () => {
    it('will set is patient details collapsed to true if it is initially false', () => {
      state.isPatientDetailsCollapsed = false;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(true);
    });

    it('will set is patient details collapsed to false if it is initially true', () => {
      state.isPatientDetailsCollapsed = true;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(false);
    });

    it('will set is patient details collapsed to true if it is initially undefined', () => {
      state.isPatientDetailsCollapsed = undefined;
      mutations[TOGGLE_PATIENT_DETAIL](state);
      expect(state.isPatientDetailsCollapsed).toBe(true);
    });
  });
});
