import BackButton from '@/components/BackButton';
import RadioGroup from '@/components/RadioGroup';
import YourChoice from '@/pages/organ-donation/your-choice';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_FAITH } from '@/lib/routes';
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
        expect(wrapper.vm.currentChoice).toBe('');
      });
    });
  });
});
