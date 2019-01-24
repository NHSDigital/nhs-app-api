import YourChoice from '@/pages/organ-donation/your-choice';
import RadioButton from '@/components/widgets/RadioButton';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_FAITH } from '@/lib/routes';
import { $t, createStore, mount } from '../../helpers';

const createState = (choice = true) => {
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
        backButton = wrapper.find('#back-to-organdonation');
        $style = {
          button: 'button',
          grey: 'grey',
        };
      });

      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });

      it('will display the back button text for the your choice', () => {
        const key = 'organDonation.yourChoice.backButtonText';
        expect(backButton.text()).toEqual(`translate_${key}`);
      });

      it('will be set as a grey button', () => {
        expect(backButton.classes()).toContain($style.button);
        expect(backButton.classes()).toContain($style.grey);
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

  describe('for new registrations or when donate all organs is "true"', () => {
    let allOrgansButton;

    beforeEach(() => {
      allOrgansButton = wrapper.findAll(RadioButton).at(0);
    });

    describe('currentChoice', () => {
      it('will equal true', () => {
        expect(wrapper.vm.currentChoice).toEqual(true);
      });
    });

    describe('all organs radio button', () => {
      it('will exist', () => {
        expect(allOrgansButton.exists()).toBe(true);
      });

      it('will be selected', () => {
        expect(allOrgansButton.vm.isSelected).toEqual(true);
      });
    });
  });
});
