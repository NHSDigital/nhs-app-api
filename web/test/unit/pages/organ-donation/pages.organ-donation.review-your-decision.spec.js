import ReviewYourDecision from '@/pages/organ-donation/review-your-decision';
import { initialState, DECISION_OPT_OUT, DECISION_OPT_IN } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

describe('review your decision', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = () => {
    const state = {
      organDonation: initialState(),
    };

    return state;
  };

  const mountPage = () => mount(ReviewYourDecision, { $store, $style, $t });

  beforeEach(() => {
    $store = createStore({ state: createState() });
    $style = {
      button: 'button',
      green: 'green',
    };
    wrapper = mountPage();
  });

  describe('showErrors', () => {
    describe('has validationErrors', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = false;
        $store.state.organDonation.isPrivacyAccepted = false;
        wrapper = mountPage();
      });

      describe('submit button clicked', () => {
        beforeEach(() => {
          const submitButton = wrapper.find('#submit-button');
          submitButton.trigger('click');
        });

        it('will have a "showErrors" value of true', () => {
          expect(wrapper.vm.showErrors).toEqual(true);
        });

        it('will show the message dialog', () => {
          const errors = wrapper.find('#errors');
          expect(errors.exists()).toEqual(true);
        });

        it('will show a message for each validation error', () => {
          const errorTexts = wrapper.findAll('#errors li');
          expect(errorTexts.length).toEqual(wrapper.vm.validationErrors.length);
          expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.confirmation.errors.accuracy');
          expect($t).toHaveBeenCalledWith('organDonation.reviewYourDecision.confirmation.errors.privacy');
        });
      });

      describe('submit button not clicked', () => {
        it('will have a "showErrors" value of false', () => {
          expect(wrapper.vm.showErrors).toEqual(false);
        });

        it('will not show the message dialog', () => {
          const errors = wrapper.find('#errors');
          expect(errors.exists()).toEqual(false);
        });
      });
    });

    describe('has no validationErrors', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = true;
        $store.state.organDonation.isPrivacyAccepted = true;
        wrapper = mountPage();
      });

      it('will show the message dialog', () => {
        expect(wrapper.find('#errors').exists()).toEqual(false);
      });
    });
  });

  describe('submit my decision button', () => {
    let submitButton;

    beforeEach(() => {
      submitButton = wrapper.find('#submit-button');
    });

    it('will exist', () => {
      expect(submitButton.exists()).toBe(true);
    });

    it('will be a green button', () => {
      expect(submitButton.classes()).toContain($style.green);
      expect(submitButton.classes()).toContain($style.button);
    });

    describe('when opt out', () => {
      beforeEach(() => {
        $store.state.organDonation.registration.decision = DECISION_OPT_OUT;
      });

      it('will use "organDonation.reviewYourDecision.submitNoButton" for text', () => {
        expect(submitButton.text())
          .toEqual('translate_organDonation.reviewYourDecision.submitNoButton');
      });
    });

    describe('when opt in', () => {
      beforeEach(() => {
        $store.state.organDonation.registration.decision = DECISION_OPT_IN;
      });

      it('will use "organDonation.reviewYourDecision.submitButton" for text', () => {
        expect(submitButton.text())
          .toEqual('translate_organDonation.reviewYourDecision.submitButton');
      });
    });
  });

  describe('back button', () => {
    let backButton;

    beforeEach(() => {
      backButton = wrapper.find('#back-button');
    });

    it('will exist', () => {
      expect(backButton.exists()).toBe(true);
    });
  });

  describe('submit button', () => {
    let submitButton;

    beforeEach(() => {
      $store.state.organDonation.isAccuracyAccepted = false;
      $store.state.organDonation.isPrivacyAccepted = false;
      wrapper = mountPage();
      submitButton = wrapper.find('#submit-button');
    });

    it('will exist', () => {
      expect(submitButton.exists()).toBe(true);
    });

    describe('accuracy not accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isPrivacyAccepted = true;
      });

      it(
        'will add "organDonation.reviewYourDecision.confirmation.errors.accuracy" to the validation errors when clicked',
        () => {
          submitButton.trigger('click');
          expect(wrapper.vm.validationErrors)
            .toContain('organDonation.reviewYourDecision.confirmation.errors.accuracy');
        },
      );
    });

    describe('privacy not accepted', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = true;
      });

      it(
        'will add "organDonation.reviewYourDecision.confirmation.errors.privacy" to the validation errors when clicked',
        () => {
          submitButton.trigger('click');
          expect(wrapper.vm.validationErrors)
            .toContain('organDonation.reviewYourDecision.confirmation.errors.privacy');
        },
      );
    });

    describe('when clicked', () => {
      beforeEach(() => {
        $store.state.organDonation.isAccuracyAccepted = true;
        $store.state.organDonation.isPrivacyAccepted = true;
      });

      it('it will call organDonation/postRegistration', () => {
        submitButton.trigger('click');
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/postRegistration');
      });
    });
  });

  describe('faith details', () => {
    let faithDetails;

    beforeEach(() => {
      faithDetails = wrapper.find('#faithDetails');
    });

    describe('when opt out', () => {
      beforeEach(() => {
        $store.state.organDonation.registration.decision = DECISION_OPT_OUT;
      });

      it('will not show the faith details', () => {
        expect(faithDetails.exists()).toBe(false);
      });
    });

    describe('when opt in', () => {
      beforeEach(() => {
        $store.state.organDonation.registration.decision = DECISION_OPT_IN;
      });

      it('will show the faith details', () => {
        expect(faithDetails.exists()).toBe(false);
      });
    });
  });
});
