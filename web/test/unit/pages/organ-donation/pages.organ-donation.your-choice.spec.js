import i18n from '@/plugins/i18n';
import RadioGroup from '@/components/RadioGroup';
import YourChoice from '@/pages/organ-donation/your-choice';
import { ORGAN_DONATION_FAITH_PATH } from '@/router/paths';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { redirectTo } from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const createState = (choice = '') => {
  const state = {
    organDonation: initialState(),
    device: {
      isNativeApp: true,
    },
  };

  state.organDonation.registration.decisionDetails.all = choice;
  return state;
};

describe('organ donation your choice page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;

  const mountWrapper = () => mount(YourChoice, {
    $router,
    $store,
    mountOpts: { i18n },
  });

  beforeEach(() => {
    redirectTo.mockClear();

    $router = createRouter();
    state = createState();
    $store = createStore({ state });
    wrapper = mountWrapper();
  });

  it('will have radio buttons', () => {
    expect(wrapper.find(RadioGroup).exists()).toBe(true);
  });

  describe('continue', () => {
    describe('button', () => {
      let continueButton;

      beforeEach(() => {
        $store.state.organDonation.registration.decisionDetails.all = true;
        wrapper = mount(YourChoice, {
          $router,
          $store,
          mountOpts: { i18n },
        });
        continueButton = wrapper.find('#continue-button');
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will display the continue button text for your choice', () => {
        expect(continueButton.text()).toBe('Continue');
      });

      it('will be a button with nhsuk-button style', () => {
        const classes = continueButton.classes();
        expect(classes).toContain('nhsuk-button');
      });

      describe('when clicked', () => {
        beforeEach(() => {
          continueButton.trigger('click');
        });

        it('will push the organ donation faith page on the router', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_FAITH_PATH);
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
