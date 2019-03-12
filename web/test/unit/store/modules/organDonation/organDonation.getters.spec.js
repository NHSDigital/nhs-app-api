import getters from '@/store/modules/organDonation/getters';
import {
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
} from '@/store/modules/organDonation/mutation-types';

describe('getters', () => {
  describe('isSomeOrgans', () => {
    const { isSomeOrgans } = getters;

    describe('decision', () => {
      let currentState;

      beforeEach(() => {
        currentState = {
          registration: {
            decision: '',
            decisionDetails: {
              all: false,
            },
          },
        };
      });

      it('will be false if the decision opt-out', () => {
        currentState.registration.decision = DECISION_OPT_OUT;
        expect(isSomeOrgans(currentState)).toEqual(false);
      });

      it('will be true if decision is opt-in', () => {
        currentState.registration.decision = DECISION_OPT_IN;
        expect(isSomeOrgans(currentState)).toEqual(true);
      });
    });

    describe('decisionDetails.all', () => {
      let currentState;

      beforeEach(() => {
        currentState = {
          registration: {
            decision: DECISION_OPT_IN,
            decisionDetails: {
              all: '',
            },
          },
        };
      });

      it('will be false if decisionDetails.all is true', () => {
        currentState.registration.decisionDetails.all = true;
        expect(isSomeOrgans(currentState)).toEqual(false);
      });

      it('will be true if decisionDetails.all is false', () => {
        currentState.registration.decisionDetails.all = false;
        expect(isSomeOrgans(currentState)).toEqual(true);
      });
    });
  });
});
