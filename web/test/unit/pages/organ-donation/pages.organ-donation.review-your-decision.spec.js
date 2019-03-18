import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import BackButton from '@/components/BackButton';
import DecisionDetails from '@/components/organ-donation/DecisionDetails';
import ReviewYourDecision from '@/pages/organ-donation/review-your-decision';
import YourDecision from '@/components/organ-donation/YourDecision';
import {
  initialState,
  DECISION_OPT_OUT,
  DECISION_OPT_IN,
  STATE_OK,
} from '@/store/modules/organDonation/mutation-types';
import { $t, createScrollTo, createStore, initFilters, mount } from '../../helpers';

const createState = ({
  all = false,
  decision = DECISION_OPT_IN,
  isAccuracyAccepted = false,
  isPrivacyAccepted = false,
  isReaffirming = false,
  isWithdrawing = false,
  state = STATE_OK,
} = {}) => {
  const organDonation = {
    ...initialState(),
    ...{
      isAccuracyAccepted,
      isPrivacyAccepted,
      isReaffirming,
      isWithdrawing,
    },
  };

  organDonation.registration.decision = decision;
  organDonation.registration.decisionDetails.all = all;
  organDonation.registration.state = state;

  return {
    organDonation,
    device: {
      source: 'web',
    },
  };
};

describe('review your decision', () => {
  const $style = {
    button: 'button',
    green: 'green',
  };
  let $store;
  let wrapper;
  let scrollTo;

  const mountPage = ({ state, getters }) => {
    $store = createStore({ state, getters });
    return mount(ReviewYourDecision, { $store, $style, $t });
  };

  beforeEach(() => {
    initFilters();
    wrapper = mountPage({ state: createState() });
  });

  it('will show additional information', () => {
    expect(wrapper.find(AdditionalInformation).exists()).toBe(true);
  });

  it('will show your decision', () => {
    expect(wrapper.find(YourDecision).exists()).toBe(true);
  });

  it('will not show withdraw header text', () => {
    expect($t).not.toHaveBeenCalledWith('organDonation.reviewYourDecision.withdraw.subheader');
  });

  it('will not show withdraw body text', () => {
    expect($t).not.toHaveBeenCalledWith('organDonation.reviewYourDecision.withdraw.body');
  });

  it('will have a back button', () => {
    expect(wrapper.find(BackButton).exists()).toBe(true);
  });

  describe('is withdrawing', () => {
    beforeEach(() => {
      wrapper = mountPage({ state: createState({ isWithdrawing: true }) });
    });

    it('will not show additional information', () => {
      expect(wrapper.find(AdditionalInformation).exists()).toBe(false);
    });

    it('will not show decision details', () => {
      expect(wrapper.find(DecisionDetails).exists()).toBe(false);
    });

    it('will not show faith details', () => {
      expect(wrapper.find('#faithDetails').exists()).toBe(false);
    });

    it('will show withdraw header text', () => {
      expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.withdraw.subheader');
    });

    it('will show withdraw body text', () => {
      expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.withdraw.body');
    });
  });

  describe('submit my decision button', () => {
    let submitButton;
    let state;

    beforeEach(() => {
      state = createState();
      wrapper = mountPage({ state });
      submitButton = wrapper.find('#submit-button');
    });

    it('will exist', () => {
      expect(submitButton.exists()).toBe(true);
    });

    it('will be a green button', () => {
      const classes = submitButton.classes();
      expect(classes).toContain($style.green);
      expect(classes).toContain($style.button);
    });

    it('will use "organDonation.reviewYourDecision.submitButton" for text', () => {
      expect(submitButton.text()).toBe('translate_organDonation.reviewYourDecision.submitButton');
    });

    describe('when neither T&C have been accepted', () => {
      beforeEach(() => {
        state.organDonation.isAccuracyAccepted = false;
        state.organDonation.isPrivacyAccepted = false;
      });

      describe('click', () => {
        beforeEach(() => {
          scrollTo = createScrollTo();
          submitButton.trigger('click');
        });

        it('will have a "showErrors" value of true', () => {
          expect(wrapper.vm.showErrors).toBe(true);
        });

        it('will show the message dialog', () => {
          expect(wrapper.find('#errors').exists()).toBe(true);
        });

        it('will show a message for each validation error', () => {
          expect(wrapper.vm.validationErrors.length).toBe(2);
          expect(wrapper.findAll('#errors li').length).toBe(wrapper.vm.validationErrors.length);
          expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.confirmation.errors.accuracy');
          expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.confirmation.errors.privacy');
        });

        it('will scroll to the top', () => {
          expect(scrollTo).toHaveBeenCalledWith(0, 0);
        });
      });
    });

    describe('accuracy not accepted', () => {
      beforeEach(() => {
        state.organDonation.isPrivacyAccepted = true;
      });

      describe('click', () => {
        beforeEach(() => {
          scrollTo = createScrollTo();
          submitButton.trigger('click');
        });

        it('will have a "showErrors" value of true', () => {
          expect(wrapper.vm.showErrors).toBe(true);
        });

        it('will show the message dialog', () => {
          expect(wrapper.find('#errors').exists()).toBe(true);
        });

        it('will add "organDonation.reviewYourDecision.confirmation.errors.accuracy" to the validation errors', () => {
          expect(wrapper.vm.validationErrors).toContain('organDonation.reviewYourDecision.confirmation.errors.accuracy');
        });

        it('will scroll to the top', () => {
          expect(scrollTo).toHaveBeenCalledWith(0, 0);
        });
      });
    });

    describe('privacy not accepted', () => {
      beforeEach(() => {
        state.organDonation.isAccuracyAccepted = true;
      });

      describe('click', () => {
        beforeEach(() => {
          scrollTo = createScrollTo();
          submitButton.trigger('click');
        });

        it('will have a "showErrors" value of true', () => {
          expect(wrapper.vm.showErrors).toBe(true);
        });

        it('will show the message dialog', () => {
          expect(wrapper.find('#errors').exists()).toBe(true);
        });

        it('will add "organDonation.reviewYourDecision.confirmation.errors.privacy" to the validation errors', () => {
          expect(wrapper.vm.validationErrors).toContain('organDonation.reviewYourDecision.confirmation.errors.privacy');
        });

        it('will scroll to the top', () => {
          expect(scrollTo).toHaveBeenCalledWith(0, 0);
        });
      });
    });

    describe('click', () => {
      beforeEach(() => {
        state.organDonation.isAccuracyAccepted = true;
        state.organDonation.isPrivacyAccepted = true;
        submitButton.trigger('click');
      });

      it('will have a "showErrors" value of false', () => {
        expect(wrapper.vm.showErrors).toBe(false);
      });

      it('will not show the message dialog', () => {
        expect(wrapper.find('#errors').exists()).toBe(false);
      });

      it('it will call organDonation/submitDecision', async () => {
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/submitDecision');
      });
    });
  });

  describe('decision details', () => {
    describe('selected all organs', () => {
      beforeEach(() => {
        const state = createState({ decision: DECISION_OPT_IN, all: true });
        wrapper = mountPage({ state });
      });

      it('will show the opt-in decision text', () => {
        expect(wrapper.find(YourDecision).text())
          .toContain('translate_organDonation.reviewYourDecision.yourDecision.optinDecisionText');
      });

      it('will not show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toBe(false);
      });
    });

    describe('selected some organs', () => {
      beforeEach(() => {
        wrapper = mountPage({
          state: createState({ decision: DECISION_OPT_IN, all: false }),
          getters: {
            'organDonation/isSomeOrgans': true,
          },
        });
      });

      it('will show the opt-in some decision text', () => {
        expect(wrapper.find(YourDecision).text())
          .toContain('translate_organDonation.reviewYourDecision.yourDecision.optinSomeDecisionText');
      });

      it('will show the decision details', () => {
        expect(wrapper.find(DecisionDetails).exists()).toBe(true);
      });
    });
  });

  describe('faith details', () => {
    describe('when opt out', () => {
      beforeEach(() => {
        const state = createState({ decision: DECISION_OPT_OUT });
        wrapper = mountPage({ state });
      });

      it('will not show', () => {
        expect(wrapper.find('#faithDetails').exists()).toBe(false);
      });
    });

    describe('when opt in', () => {
      beforeEach(() => {
        const state = createState({ decision: DECISION_OPT_IN });
        wrapper = mountPage({ state });
      });

      it('will show', () => {
        expect(wrapper.find('#faithDetails').exists()).toBe(true);
      });
    });
  });

  describe('additional details', () => {
    describe('reaffirming', () => {
      const createLocalState = ({ all, decision }) =>
        createState({ all, decision, isReaffirming: true });

      describe('original registration', () => {
        describe('donating all organs', () => {
          beforeEach(() => {
            const state = createLocalState({
              all: true,
              decision: DECISION_OPT_IN,
            });
            state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
            state.organDonation.originalRegistration.decisionDetails.all = true;
            wrapper = mountPage({ state });
          });

          it('will not show additional details', () => {
            expect(wrapper.find(AdditionalInformation).exists()).toBe(false);
          });
        });

        describe('donating some organs', () => {
          let state;

          beforeEach(() => {
            state = createLocalState({
              all: false,
              decision: DECISION_OPT_IN,
            });
            state.organDonation.originalRegistration.decision = DECISION_OPT_IN;
            state.organDonation.originalRegistration.decisionDetails.all = false;
            wrapper = mountPage({ state });
          });

          it('will show additional details', () => {
            expect(wrapper.find(AdditionalInformation).exists()).toBe(true);
          });

          describe('change to all organs', () => {
            beforeEach(() => {
              state.organDonation.registration.decisionDetails.all = true;
              wrapper = mountPage({ state });
            });

            it('will show additional details', () => {
              expect(wrapper.find(AdditionalInformation).exists()).toBe(true);
            });
          });
        });

        describe('not donating organs', () => {
          beforeEach(() => {
            const state = createLocalState({
              all: false,
              decision: DECISION_OPT_OUT,
            });
            state.organDonation.originalRegistration.decision = DECISION_OPT_OUT;
            state.organDonation.originalRegistration.decisionDetails.all = false;
            wrapper = mountPage({ state });
          });

          it('will not show additional details', () => {
            expect(wrapper.find(AdditionalInformation).exists()).toBe(false);
          });
        });
      });
    });
  });
});
