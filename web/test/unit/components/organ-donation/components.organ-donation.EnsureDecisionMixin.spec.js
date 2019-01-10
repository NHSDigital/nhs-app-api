import EnsureDecisionMixin from '@/components/organ-donation/EnsureDecisionMixin';
import { ORGAN_DONATION } from '@/lib/routes';
import {
  initialState,
  DECISION_NOT_FOUND,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
} from '@/store/modules/organDonation/mutation-types';

describe('ensure decision mixin', () => {
  let redirect;
  let store;

  beforeEach(() => {
    redirect = jest.fn();
    store = {
      state: {
        organDonation: initialState(),
      },
    };
  });

  describe('decision not made', () => {
    beforeEach(() => {
      store.state.organDonation.registration.decision = DECISION_NOT_FOUND;
    });

    describe('fetch', () => {
      beforeEach(() => {
        EnsureDecisionMixin.fetch({ redirect, store });
      });

      it('will call redirect with the ORGAN_DONATION path', () => {
        expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
      });
    });
  });

  describe('decision is opt-out', () => {
    beforeEach(() => {
      store.state.organDonation.registration.decision = DECISION_OPT_OUT;
    });

    describe('fetch', () => {
      beforeEach(() => {
        EnsureDecisionMixin.fetch({ redirect, store });
      });

      it('will not call redirect', () => {
        expect(redirect).not.toHaveBeenCalled();
      });
    });
  });

  describe('decision is opt-in', () => {
    beforeEach(() => {
      store.state.organDonation.registration.decision = DECISION_OPT_IN;
    });

    describe('fetch', () => {
      beforeEach(() => {
        EnsureDecisionMixin.fetch({ redirect, store });
      });

      it('will not call redirect', () => {
        expect(redirect).not.toHaveBeenCalled();
      });
    });
  });
});
