import BackButton from '@/components/BackButton';
import Faith from '@/pages/organ-donation/faith';
import { initialState, YES, NO, NOT_STATED } from '@/store/modules/organDonation/mutation-types';
import { ORGAN_DONATION_ADDITIONAL_DETAILS } from '@/lib/routes';
import { $t, createRouter, createScrollTo, createStore, mount } from '../../helpers';

describe('organ donation faith page', () => {
  let $store;
  let $style;
  let wrapper;
  let $router;
  let state;

  const createState = () => {
    state = {
      organDonation: initialState(),
      device: {
        source: 'web',
      },
    };

    state.organDonation.registration.faithDeclaration = '';
    return state;
  };

  beforeEach(() => {
    $router = createRouter();
    $store = createStore({ state: createState() });
    wrapper = mount(Faith, {
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
        continueButton = wrapper.find('#continue-to-additional-details');
        $style = {
          button: 'button',
          green: 'green',
          error: 'error',
        };
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      it('will display the continue button text for the faith', () => {
        expect(continueButton.text()).toEqual('translate_organDonation.faith.continueButtonText');
      });

      it('will be set as a green button', () => {
        expect(continueButton.classes()).toContain($style.button);
        expect(continueButton.classes()).toContain($style.green);
      });

      describe('with no selected choice', () => {
        describe('when clicked', () => {
          let scrollTo;

          beforeEach(() => {
            scrollTo = createScrollTo();
            continueButton.trigger('click');
          });

          it('will show an error', () => {
            expect(wrapper.find('.error').exists()).toBe(true);
          });

          it('will not push the organ donation additional details page on the router', () => {
            expect($router.push).not.toHaveBeenCalledWith(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
          });

          it('will scroll to the top', () => {
            expect(scrollTo).toHaveBeenCalledWith(0, 0);
          });
        });
      });

      describe('after making a selection', () => {
        beforeEach(() => {
          state.organDonation.registration.faithDeclaration = YES;
        });

        describe('when clicked', () => {
          let scrollTo;

          beforeEach(() => {
            scrollTo = createScrollTo();
            continueButton.trigger('click');
          });

          it('will not show an error', () => {
            expect(wrapper.find('.error').exists()).toBe(false);
          });

          it('will not scroll to', () => {
            expect(scrollTo).not.toHaveBeenCalled();
          });

          it('will push the organ donation additional details page on the router', () => {
            expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION_ADDITIONAL_DETAILS.path);
          });
        });
      });
    });
  });

  describe('radio button', () => {
    let radioButton;

    describe('yes', () => {
      beforeEach(() => {
        radioButton = wrapper.find(`#radioButton-${YES}`);
      });

      it('will exist', () => {
        expect(radioButton.exists()).toBe(true);
      });

      describe('when clicked', () => {
        beforeEach(() => {
          radioButton.trigger('click');
        });

        it('will dispatch the "organDonation/setFaithDeclaration" action', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/setFaithDeclaration', YES);
        });
      });
    });

    describe('no', () => {
      beforeEach(() => {
        radioButton = wrapper.find(`#radioButton-${NO}`);
      });

      it('will exist', () => {
        expect(radioButton.exists()).toBe(true);
      });

      describe('when clicked', () => {
        beforeEach(() => {
          radioButton.trigger('click');
        });

        it('will dispatch the "organDonation/setFaithDeclaration" action', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/setFaithDeclaration', NO);
        });
      });
    });

    describe('prefer not to say', () => {
      beforeEach(() => {
        radioButton = wrapper.find(`#radioButton-${NOT_STATED}`);
      });

      it('will exist', () => {
        expect(radioButton.exists()).toBe(true);
      });

      describe('when clicked', () => {
        beforeEach(() => {
          radioButton.trigger('click');
        });

        it('will dispatch the "organDonation/setFaithDeclaration" action', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/setFaithDeclaration', NOT_STATED);
        });
      });
    });
  });
});
