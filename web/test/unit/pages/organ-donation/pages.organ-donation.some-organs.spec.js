import BackButton from '@/components/BackButton';
import SomeOrgans from '@/pages/organ-donation/some-organs';
import OrganChoice from '@/components/organ-donation/OrganChoice';
import { ORGAN_DONATION_FAITH } from '@/lib/routes';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createRouter, createStore, mount } from '../../helpers';

const allNoChoices = {
  heart: 'No',
  lungs: 'No',
  kidney: 'No',
  liver: 'No',
  corneas: 'No',
  pancreas: 'No',
  tissue: 'No',
  smallBowel: 'No',
};
const oneYesChoices = {
  heart: 'Yes',
  lungs: 'No',
  kidney: 'No',
  liver: 'No',
  corneas: 'No',
  pancreas: 'No',
  tissue: 'No',
  smallBowel: 'No',
};
const someNotSetChoices = {
  heart: 'NotStated',
  lungs: 'Yes',
  kidney: 'NotStated',
  liver: 'No',
  corneas: 'No',
  pancreas: 'No',
  tissue: 'No',
  smallBowel: 'No',
};

const createState = (choice = false) => {
  const state = {
    organDonation: initialState(),
    device: {
      source: 'web',
    },
  };

  state.organDonation.registration.decisionDetails.all = choice;

  return state;
};

// Test for choices
describe('organ donation some organs page', () => {
  let $store;
  let wrapper;
  let $style;
  let $router;

  beforeEach(() => {
    $router = createRouter();
    $store = createStore({ state: createState() });
    wrapper = mount(SomeOrgans, {
      $router, $store, $t, $style,
    });
    global.scrollTo = jest.fn();
  });

  it('will translate the some organs subheader', () => {
    expect($t).toHaveBeenCalledWith('organDonation.someOrgans.subheader');
  });

  it('will translate the some organs description', () => {
    expect($t).toHaveBeenCalledWith('organDonation.someOrgans.description');
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
        $style = {
          button: 'button',
          green: 'green',
        };
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will display the continue button text for some organs', () => {
        const key = 'organDonation.someOrgans.continueButtonText';
        expect(continueButton.text()).toEqual(`translate_${key}`);
      });

      it('will be set as a green button', () => {
        expect(continueButton.classes()).toContain($style.button);
        expect(continueButton.classes()).toContain($style.green);
      });

      describe('validation is shown when all no choices are set', () => {
        beforeEach(() => {
          $style = {
            button: 'button',
            green: 'green',
            error: 'error',
          };
          $store.state.organDonation.registration.decisionDetails.choices = allNoChoices;
          wrapper = mount(SomeOrgans, {
            $router, $store, $t, $style,
          });
          continueButton = wrapper.find('#continue-button');
        });
        it('shown when all choices are "No"', () => {
          continueButton.trigger('click');
          expect(wrapper.find('.error').exists()).toBe(true);
        });
        it('will not push the organ donation additional details page on the router', () => {
          expect($router).not.toContain(ORGAN_DONATION_FAITH.path);
        });
      });

      describe('validation is shown when when some choices are not set', () => {
        beforeEach(() => {
          $style = {
            button: 'button',
            green: 'green',
            error: 'error',
          };
          $store.state.organDonation.registration.decisionDetails.choices = someNotSetChoices;
          wrapper = mount(SomeOrgans, {
            $router, $store, $t, $style,
          });
          continueButton = wrapper.find('#continue-button');
        });
        it('shown when some choices are not set', () => {
          continueButton.trigger('click');
          expect(wrapper.find('.error').exists()).toBe(true);
        });
        it('will not push the organ donation additional details page on the router', () => {
          expect($router).not.toContain(ORGAN_DONATION_FAITH.path);
        });
      });

      describe('validation is not shown when when all are selected including minumun of one yes', () => {
        beforeEach(() => {
          $style = {
            button: 'button',
            green: 'green',
            error: 'error',
          };
          $store.state.organDonation.registration.decisionDetails.choices = oneYesChoices;
          wrapper = mount(SomeOrgans, {
            $router, $store, $t, $style,
          });
          continueButton = wrapper.find('#continue-button');
        });
        it('shown when some choices are not set', () => {
          continueButton.trigger('click');
          expect(wrapper.find('.error').exists()).toBe(false);
        });
        it('will push the organ donation additional details page on the router', () => {
          continueButton.trigger('click');
          expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION_FAITH.path);
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

    it('will pass at least one "Yes" selected validation', () => {
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
