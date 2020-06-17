import EnsureDecisionMixin, { EnsureCanSubmit, EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import { INDEX_PATH, ORGAN_DONATION_PATH } from '@/router/paths';
import {
  initialState,
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
} from '@/store/modules/organDonation/mutation-types';

const createStore = ({ decision, isWithdrawing = false, isNativeApp } = {}) => ({
  state: {
    device: {
      isNativeApp,
    },
    organDonation: {
      ...initialState(),
      ...{
        isWithdrawing,
        registration: {
          decision,
        },
      },
    },
  },
});

describe('ensure decision mixin', () => {
  describe('fetch', () => {
    let redirect;

    const fetch = ({ decision, isNativeApp }) => {
      redirect = jest.fn();

      EnsureDecisionMixin.fetch({
        redirect,
        store: createStore({ decision, isNativeApp }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, isNativeApp: false });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX_PATH);
      });
    });

    describe('native', () => {
      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('the user has appointed a representative', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_APPOINTED_REP, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, isNativeApp: true });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, isNativeApp: true });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });
  });
});

describe('ensure opt in decision mixin', () => {
  describe('fetch', () => {
    let redirect;

    const fetch = ({ decision, isNativeApp }) => {
      redirect = jest.fn();

      EnsureOptInDecision.fetch({
        redirect,
        store: createStore({ decision, isNativeApp }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, isNativeApp: false });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX_PATH);
      });
    });

    describe('native', () => {
      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('the user has appointed a representative', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, isNativeApp: true });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });
  });
});

describe('ensure can submit mixin', () => {
  describe('fetch', () => {
    let redirect;

    const fetch = ({ decision, isWithdrawing = false, isNativeApp }) => {
      redirect = jest.fn();

      EnsureCanSubmit.fetch({
        redirect,
        store: createStore({ decision, isWithdrawing, isNativeApp }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, isNativeApp: false });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX_PATH);
      });
    });

    describe('native', () => {
      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });
      });

      describe('the user has appointed a representative', () => {
        const decision = DECISION_APPOINTED_REP;

        beforeEach(() => {
          fetch({ decision, isNativeApp: true });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION_PATH);
        });

        describe('is withdrawing', () => {
          beforeEach(() => {
            fetch({ decision, isWithdrawing: true, isNativeApp: true });
          });

          it('will not call redirect', () => {
            expect(redirect).not.toHaveBeenCalled();
          });
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, isNativeApp: true });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, isNativeApp: true });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });
  });
});
