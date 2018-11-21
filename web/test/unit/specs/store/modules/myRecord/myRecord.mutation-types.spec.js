import isEmpty from 'lodash/fp/isEmpty';
import { initialState } from '@/store/modules/myRecord/mutation-types';

describe('my record mutation types', () => {
  describe('initial state', () => {
    let state;

    beforeEach(() => {
      state = initialState();
    });

    it('will have an accepted terms value of false', () => {
      expect(state.hasAcceptedTerms).toBe(false);
    });

    it('will have a has loaded value of false', () => {
      expect(state.hasLoaded).toBe(false);
    });

    it('will have a is patient details collapsed of true', () => {
      expect(state.isPatientDetailsCollapsed).toBe(true);
    });

    it('will have an empty object for patient details', () => {
      expect(state.patientDetails).not.toBeUndefined();
      expect(isEmpty(state.patientDetails)).toEqual(true);
    });

    it('will have an empty object for record', () => {
      expect(state.record).not.toBeUndefined();
      expect(isEmpty(state.record)).toEqual(true);
    });
  });
});
