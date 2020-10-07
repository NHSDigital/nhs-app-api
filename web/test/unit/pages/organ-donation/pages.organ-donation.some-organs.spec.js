import BackButton from '@/components/BackButton';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import MessageDialog from '@/components/widgets/MessageDialog';
import SomeOrgans from '@/pages/organ-donation/some-organs';
import i18n from '@/plugins/i18n';
import { initialState, NO, NOT_STATED, YES } from '@/store/modules/organDonation/mutation-types';
import {
  ORGAN_DONATION_FAITH_PATH,
  ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

const allNoChoices = {
  heart: NO,
  lungs: NO,
  kidney: NO,
  liver: NO,
  corneas: NO,
  pancreas: NO,
  tissue: NO,
  smallBowel: NO,
};
const allNotStatedChoices = {
  heart: NOT_STATED,
  lungs: NOT_STATED,
  kidney: NOT_STATED,
  liver: NOT_STATED,
  corneas: NOT_STATED,
  pancreas: NOT_STATED,
  tissue: NOT_STATED,
  smallBowel: NOT_STATED,
};
const oneYesChoices = {
  heart: YES,
  lungs: NO,
  kidney: NO,
  liver: NO,
  corneas: NO,
  pancreas: NO,
  tissue: NO,
  smallBowel: NO,
};
const someNotSetChoices = {
  heart: NOT_STATED,
  lungs: YES,
  kidney: NOT_STATED,
  liver: NO,
  corneas: NO,
  pancreas: NO,
  tissue: NO,
  smallBowel: NO,
};

const createState = ({ choices = undefined } = {}) => {
  const state = {
    organDonation: initialState(),
    device: {
      isNativeApp: false,
    },
  };

  if (choices) {
    state.organDonation.registration.decisionDetails.choices = choices;
  }

  return state;
};

// Test for choices
describe('organ donation some organs page', () => {
  const $style = {
    error: 'error',
  };
  let $store;
  let wrapper;
  let $router;

  const mountSomeOrgans = ({ choices = undefined } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: createState({ choices }),
    });

    return mount(SomeOrgans, {
      $router,
      $store,
      $style,
      mountOpts: { i18n },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();

    wrapper = mountSomeOrgans();
    global.scrollTo = jest.fn();
  });

  it('will translate the some organs subheader', () => {
    expect(wrapper.text()).toContain('Your choice');
  });

  it('will translate the some organs description', () => {
    expect(wrapper.text()).toContain('Please select which organs and tissue you would like to donate:');
  });

  describe('more about organs', () => {
    let moreAboutOrgans;

    beforeEach(() => {
      moreAboutOrgans = wrapper.find('a');
    });

    it('will show the more about organs link', () => {
      expect(moreAboutOrgans.exists()).toBe(true);
    });

    it('will display find out more text', () => {
      expect(moreAboutOrgans.text()).toEqual('Find out more about organs and tissue');
    });

    describe('click', () => {
      beforeEach(() => {
        moreAboutOrgans.trigger('click');
      });

      it('will push the organ donation more about organs page on the router', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_MORE_ABOUT_ORGANS_PATH);
      });
    });
  });

  describe('organ choices', () => {
    let organChoices;

    beforeEach(() => {
      organChoices = wrapper.findAll(OrganChoice);
    });

    it('will have organ choices for all organs', () => {
      const numberOfChoices = Object
        .keys($store.state.organDonation.registration.decisionDetails.choices).length;
      expect(organChoices.length).toBe(numberOfChoices);
    });
  });

  describe('back', () => {
    describe('button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find(BackButton);
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
        continueButton = wrapper.find('#continue-button');
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will display the continue button text for some organs', () => {
        expect(continueButton.text()).toEqual('Continue');
      });

      it('will be a button with nhsuk-button style', () => {
        const classes = continueButton.classes();
        expect(classes).toContain('nhsuk-button');
      });

      describe('click', () => {
        const clickButton = () => wrapper.find('#continue-button').trigger('click');

        describe('when all choices not set', () => {
          beforeEach(() => {
            wrapper = mountSomeOrgans({ choices: allNotStatedChoices });
            clickButton();
          });

          it('will show error dialog', () => {
            expect(wrapper.find('.error').exists()).toBe(true);
          });

          it('will not push the organ donation additional details page on the router', () => {
            expect(redirectTo).not.toHaveBeenCalledWith(ORGAN_DONATION_FAITH_PATH);
          });

          it('will set focus on the error component', () => {
            expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
          });

          it('will show inline errors', () => {
            expect(wrapper.vm.showInlineErrors).toBe(true);
          });
        });

        describe('when all no choices are set', () => {
          beforeEach(() => {
            wrapper = mountSomeOrgans({ choices: allNoChoices });
            clickButton();
          });

          it('will show error dialog', () => {
            expect(wrapper.find('.error').exists()).toBe(true);
          });

          it('will not push the organ donation additional details page on the router', () => {
            expect(redirectTo).not.toHaveBeenCalledWith(ORGAN_DONATION_FAITH_PATH);
          });

          it('will not show inline errors', () => {
            expect(wrapper.vm.showInlineErrors).toBe(false);
          });
        });

        describe('when some choices are not set', () => {
          beforeEach(() => {
            wrapper = mountSomeOrgans({ choices: someNotSetChoices });
            clickButton();
          });

          it('will show error dialog', () => {
            expect(wrapper.find(MessageDialog).exists()).toBe(true);
          });

          it('will not push the organ donation additional details page on the router', () => {
            expect(redirectTo).not.toHaveBeenCalledWith(ORGAN_DONATION_FAITH_PATH);
          });

          it('will show inline errors', () => {
            expect(wrapper.vm.showInlineErrors).toBe(true);
          });
        });

        describe('when all are selected including minimun of one yes', () => {
          beforeEach(() => {
            wrapper = mountSomeOrgans({ choices: oneYesChoices });
            clickButton();
          });

          it('will not show error dialog', () => {
            expect(wrapper.find('.error').exists()).toBe(false);
          });

          it('will push the organ donation additional details page on the router', () => {
            expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_FAITH_PATH);
          });

          it('will not show inline errors', () => {
            expect(wrapper.vm.showInlineErrors).toBe(false);
          });
        });
      });
    });
  });

  describe('computed initial state', () => {
    it('will match the store state', () => {
      expect(wrapper.vm.currentChoices)
        .toEqual($store.state.organDonation.registration.decisionDetails.choices);
    });

    it('will fail all selected validation', () => {
      expect(wrapper.vm.areAllSelected).toEqual(false);
    });

    it('will fail at least one "Yes" selected validation', () => {
      expect(wrapper.vm.hasYesSelection).toEqual(false);
    });

    it('will fail choice made validation', () => {
      expect(wrapper.vm.hasMadeChoices).toEqual(false);
    });
  });

  describe('computed all no choices made state', () => {
    beforeEach(() => {
      $store.state.organDonation.registration.decisionDetails.choices = allNoChoices;
    });

    it('will match the store state', () => {
      expect(wrapper.vm.currentChoices)
        .toEqual($store.state.organDonation.registration.decisionDetails.choices);
    });

    it('will pass all selected validation', () => {
      expect(wrapper.vm.areAllSelected).toEqual(true);
    });

    it('will fail at least one "Yes" selected validation', () => {
      expect(wrapper.vm.hasYesSelection).toEqual(false);
    });

    it('will fail choice made validation', () => {
      expect(wrapper.vm.hasMadeChoices).toEqual(false);
    });
  });

  describe('computed some choices made state', () => {
    beforeEach(() => {
      $store.state.organDonation.registration.decisionDetails.choices = someNotSetChoices;
    });

    it('will match the store state', () => {
      expect(wrapper.vm.currentChoices)
        .toEqual($store.state.organDonation.registration.decisionDetails.choices);
    });

    it('will fail all selected validation', () => {
      expect(wrapper.vm.areAllSelected).toEqual(false);
    });

    it('will fail at least one "Yes" selected validation', () => {
      expect(wrapper.vm.hasYesSelection).toEqual(true);
    });

    it('will fail choice made validation', () => {
      expect(wrapper.vm.hasMadeChoices).toEqual(false);
    });
  });

  describe('computed all choices made minimum one yes state', () => {
    beforeEach(() => {
      $store.state.organDonation.registration.decisionDetails.choices = oneYesChoices;
    });

    it('will match the store state', () => {
      expect(wrapper.vm.currentChoices)
        .toEqual($store.state.organDonation.registration.decisionDetails.choices);
    });

    it('will pass all selected validation', () => {
      expect(wrapper.vm.areAllSelected).toEqual(true);
    });

    it('will pass at least one "Yes" selected validation', () => {
      expect(wrapper.vm.hasYesSelection).toEqual(true);
    });

    it('will pass choice made validation', () => {
      expect(wrapper.vm.hasMadeChoices).toEqual(true);
    });
  });
});
