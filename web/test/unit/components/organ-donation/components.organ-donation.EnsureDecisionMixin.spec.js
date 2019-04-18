import EnsureDecisionMixin, { EnsureCanSubmit, EnsureOptInDecision } from '@/components/organ-donation/EnsureDecisionMixin';
import { INDEX, ORGAN_DONATION } from '@/lib/routes';
import {
  initialState,
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
} from '@/store/modules/organDonation/mutation-types';

const createStore = ({ decision, isWithdrawing = false } = {}) => ({
  state: {
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

    const fetch = ({ decision, source }) => {
      redirect = jest.fn();

      EnsureDecisionMixin.fetch({
        redirect,
        route: {
          query: {
            source,
          },
        },
        store: createStore({ decision }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, source: 'web' });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX.path);
      });
    });

    describe('native', () => {
      const source = 'ios';

      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('the user has appointed a representative', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_APPOINTED_REP, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, source });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, source });
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

    const fetch = ({ decision, source }) => {
      redirect = jest.fn();

      EnsureOptInDecision.fetch({
        redirect,
        route: {
          query: {
            source,
          },
        },
        store: createStore({ decision }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, source: 'web' });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX.path);
      });
    });

    describe('native', () => {
      const source = 'ios';

      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('the user has appointed a representative', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, source });
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

    const fetch = ({ decision, isWithdrawing = false, source }) => {
      redirect = jest.fn();

      EnsureCanSubmit.fetch({
        redirect,
        route: {
          query: {
            source,
          },
        },
        store: createStore({ decision, isWithdrawing }),
      });
    };

    describe('not native', () => {
      beforeEach(() => {
        fetch({ decision: DECISION_OPT_IN, source: 'web' });
      });

      it('will call redirect with the INDEX path', () => {
        expect(redirect).toHaveBeenCalledWith(INDEX.path);
      });
    });

    describe('native', () => {
      const source = 'ios';

      describe('decision not made', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_UNKNOWN, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('the user has appointed a representative', () => {
        const decision = DECISION_APPOINTED_REP;

        beforeEach(() => {
          fetch({ decision, source });
        });

        it('will call redirect with the ORGAN_DONATION path', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });

        describe('is withdrawing', () => {
          beforeEach(() => {
            fetch({ decision, source, isWithdrawing: true });
          });

          it('will not call redirect', () => {
            expect(redirect).not.toHaveBeenCalled();
          });
        });
      });

      describe('decision is opt-out', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_OUT, source });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });

      describe('decision is opt-in', () => {
        beforeEach(() => {
          fetch({ decision: DECISION_OPT_IN, source });
        });

        it('will not call redirect', () => {
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });
  });
});
