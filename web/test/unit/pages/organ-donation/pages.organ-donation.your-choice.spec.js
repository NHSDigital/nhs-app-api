import BackButton from '@/components/BackButton';
import YourChoice from '@/pages/organ-donation/your-choice';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_FAITH } from '@/lib/routes';
import { $t, createStore, mount } from '../../helpers';

const createState = (choice = '') => {
  const state = {
    organDonation: initialState(),
  };

  state.organDonation.registration.decisionDetails.all = choice;

  return state;
};

describe('organ donation your choice page', () => {
  let $store;
  let wrapper;
  let $style;
  let $router;

  beforeEach(() => {
    $router = [];
    $store = createStore({ state: createState() });
    wrapper = mount(YourChoice, {
      $router,
      $store,
      $t,
      $style,
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
        expect(continueButton.text()).toEqual(`translate_${key}`);
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
          expect($router).toContain(ORGAN_DONATION_FAITH.path);
        });
      });
    });
  });

  describe('for new registrations', () => {
    let allOrgansButton;
    let someOrgansButton;

    beforeEach(() => {
      allOrgansButton = wrapper.findAll(GenericRadioButton).at(0);
      someOrgansButton = wrapper.findAll(GenericRadioButton).at(1);
    });

    describe('currentChoice', () => {
      it('will equal true', () => {
        expect(wrapper.vm.currentChoice).toEqual('');
      });
    });

    describe('all organs radio button', () => {
      it('will exist', () => {
        expect(allOrgansButton.exists()).toBe(true);
      });

      it('will not be selected', () => {
        expect(allOrgansButton.vm.isSelected).toEqual(false);
      });
    });

    describe('some organs radio button', () => {
      it('will exist', () => {
        expect(someOrgansButton.exists()).toBe(true);
      });

      it('will not be selected', () => {
        expect(someOrgansButton.vm.isSelected).toEqual(false);
      });
    });
  });
});
