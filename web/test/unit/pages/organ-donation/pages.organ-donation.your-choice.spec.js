import BackButton from '@/components/BackButton';
import RadioGroup from '@/components/RadioGroup';
import YourChoice from '@/pages/organ-donation/your-choice';
import { ORGAN_DONATION, ORGAN_DONATION_FAITH } from '@/lib/routes';
import {
  DECISION_APPOINTED_REP,
  DECISION_OPT_IN,
  DECISION_OPT_OUT,
  DECISION_UNKNOWN,
  initialState,
} from '@/store/modules/organDonation/mutation-types';
import { $t, createRouter, createStore, mount } from '../../helpers';

const createState = (choice = '') => {
  const state = {
    organDonation: initialState(),
  };

  state.organDonation.registration.decisionDetails.all = choice;
  return state;
};

describe('organ donation your choice page', () => {
  let $router;
  let $store;
  let $style;
  let state;
  let wrapper;

  const mountWrapper = () => mount(YourChoice, {
    $router,
    $store,
    $t,
    $style,
  });

  beforeEach(() => {
    $router = createRouter();
    state = createState();
    $store = createStore({ state });
    wrapper = mountWrapper();
  });

  it('will have radio buttons', () => {
    expect(wrapper.find(RadioGroup).exists()).toBe(true);
  });

  describe('no decision set', () => {
    beforeEach(() => {
      state.organDonation.registration.decision = DECISION_UNKNOWN;
    });

    describe('fetch (via mixin)', () => {
      it('will redirect back to the organ donation', () => {
        const redirect = jest.fn();
        wrapper.vm.$options.fetch({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
      });
    });
  });

  describe('has decision', () => {
    beforeEach(() => {
      wrapper = mountWrapper({ });
    });

    describe('opt in', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_OPT_IN;
      });

      describe('fetch (via mixin)', () => {
        it('will not redirect', () => {
          const redirect = jest.fn();
          wrapper.vm.$options.fetch({ redirect, store: $store });
          expect(redirect).not.toHaveBeenCalled();
        });
      });
    });

    describe('opt out', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_OPT_OUT;
      });

      describe('fetch (via mixin)', () => {
        it('will redirect back to the organ donation index', () => {
          const redirect = jest.fn();
          wrapper.vm.$options.fetch({ redirect, store: $store });
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });
    });

    describe('the user appointed a representative', () => {
      beforeEach(() => {
        state.organDonation.registration.decision = DECISION_APPOINTED_REP;
      });

      describe('fetch (via mixin)', () => {
        it('will redirect back to the organ donation index', () => {
          const redirect = jest.fn();
          wrapper.vm.$options.fetch({ redirect, store: $store });
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });
    });
  });

  describe('back', () => {
    describe('button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find(BackButton);
        $style = {
          button: 'button',
          grey: 'grey',
        };
      });

      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });
    });
  });

  describe('continue', () => {
    describe('button', () => {
      let continueButton;

      beforeEach(() => {
        $style = {
          button: 'button',
          green: 'green',
        };
        $store.state.organDonation.registration.decisionDetails.all = true;
        wrapper = mount(YourChoice, {
          $router,
          $store,
          $t,
          $style,
        });
        continueButton = wrapper.find('#continue-button');
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will display the continue button text for your choice', () => {
        const key = 'organDonation.yourChoice.continueButtonText';
        expect(continueButton.text()).toBe(`translate_${key}`);
      });

      it('will be set as a green button', () => {
        const classes = continueButton.classes();
        expect(classes).toContain($style.button);
        expect(classes).toContain($style.green);
      });

      describe('when clicked', () => {
        beforeEach(() => {
          continueButton.trigger('click');
        });

        it('will push the organ donation faith page on the router', () => {
          expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION_FAITH.path);
        });
      });
    });
  });

  describe('for new registrations', () => {
    describe('currentChoice', () => {
      it('will have no value', () => {
        expect(wrapper.vm.currentChoice).toEqual('');
      });
    });
  });
});
