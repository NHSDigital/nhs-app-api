import find from 'lodash/fp/find';
import AdditionalDetails from '@/pages/organ-donation/additional-details';
import { ORGAN_DONATION, ORGAN_DONATION_CONFIRMATION } from '@/lib/routes';
import { DECISION_NOT_FOUND, DECISION_OPT_IN, initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createStore, mount } from '../../helpers';

describe('additional-details', () => {
  // In this case we want string numbers to be coerced into actual numbers and vice versa.
  // eslint-disable-next-line eqeqeq
  const findOptionById = id => find(x => x.value == id);
  let $router;
  let $store;
  let $style;
  let state;
  let wrapper;

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(AdditionalDetails, { $router, $store: store, $style });
  };

  beforeEach(() => {
    $router = [];
    $style = {
      button: 'button',
      green: 'green',
      grey: 'grey',
    };
    state = {
      organDonation: initialState(),
    };
  });

  describe('no decision set', () => {
    beforeEach(() => {
      state.organDonation.registration.decision = DECISION_NOT_FOUND;
      $store = createStore({ state });
      wrapper = mountWrapper({ });
    });

    describe('asyncData', () => {
      it('will redirect back to the organ donation index if the decision is not found', () => {
        const redirect = jest.fn();
        wrapper.vm.$options.asyncData({ redirect, store: $store });
        expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
      });
    });

    describe('mounted', () => {
      it('will push the organ donation index to the router', () => {
        expect($router).toContain(ORGAN_DONATION.path);
      });
    });
  });

  describe('decision set', () => {
    beforeEach(() => {
      state.organDonation.registration.decision = DECISION_OPT_IN;
      $store = createStore({ state });
      wrapper = mountWrapper({ });
    });

    it('will translate the additional details subheader', () => {
      expect($t).toHaveBeenCalledWith('organDonation.additionalDetails.subheader');
    });

    describe('asyncData', () => {
      it('will not redirect back to the organ donation index if the decision is not found', () => {
        const redirect = jest.fn();
        wrapper.vm.$options.asyncData({ redirect, store: $store });
        expect(redirect).not.toHaveBeenCalled();
      });

      it('will request reference data from the api', () => {
        wrapper.vm.$options.asyncData({ store: $store });
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/getReferenceData');
      });
    });

    describe('dropdowns', () => {
      beforeEach(() => {
        state.organDonation.referenceData = {
          ethnicities: [
            { id: 1, displayName: 'Asian or Asian British' },
            { id: 2, displayName: 'Black or Black British' },
            { id: 3, displayName: 'White - British' },
          ],
          religions: [
            { id: 4, displayName: 'No religion' },
            { id: 5, displayName: 'Christian' },
            { id: 6, displayName: 'Buddhist' },
          ],
        };

        $store = createStore({ state });
        wrapper = mountWrapper({ });
      });

      describe('ethnicity', () => {
        let dropdown;
        let options;
        let elements;

        beforeEach(() => {
          dropdown = wrapper.find('#ethnicity');
          options = dropdown.findAll('option');
          elements = options.wrappers.map(x => x.element);
        });

        it('will exist', () => {
          expect(dropdown.exists()).toBe(true);
        });

        it('will have options for each ethnicity', () => {
          state.organDonation.referenceData.ethnicities.forEach((ethnicity) => {
            const option = findOptionById(ethnicity.id)(elements);
            expect(option).not.toBeUndefined();
            expect(option.value).toEqual(ethnicity.id.toString());
            expect(option.text).toEqual(ethnicity.displayName);
          });
        });

        it('will have a placeholder option', () => {
          const option = findOptionById('')(elements);
          expect(option).not.toBeUndefined();
          expect(option.text).toEqual('translate_organDonation.additionalDetails.ethnicity.placeholder');
        });
      });

      describe('religion', () => {
        let dropdown;
        let options;
        let elements;

        beforeEach(() => {
          dropdown = wrapper.find('#religion');
          options = dropdown.findAll('option');
          elements = options.wrappers.map(x => x.element);
        });

        it('will exist', () => {
          expect(dropdown.exists()).toBe(true);
        });

        it('will have options for each religion', () => {
          state.organDonation.referenceData.religions.forEach((religion) => {
            const option = findOptionById(religion.id)(elements);
            expect(option).not.toBeUndefined();
            expect(option.value).toEqual(religion.id.toString());
            expect(option.text).toEqual(religion.displayName);
          });
        });

        it('will have a placeholder option', () => {
          const option = findOptionById('')(elements);
          expect(option).not.toBeUndefined();
          expect(option.text).toEqual('translate_organDonation.additionalDetails.religion.placeholder');
        });
      });
    });

    describe('back', () => {
      describe('button', () => {
        let backButton;

        beforeEach(() => {
          backButton = wrapper.find('#back-button');
        });

        it('will exist', () => {
          expect(backButton.exists()).toBe(true);
        });

        it('will display the back button text for the additional details', () => {
          const key = 'organDonation.additionalDetails.backButton';
          expect(backButton.text()).toEqual(`translate_${key}`);
        });

        it('will be set as a grey button', () => {
          expect(backButton.classes()).toContain($style.button);
          expect(backButton.classes()).toContain($style.grey);
        });

        describe('when clicked', () => {
          beforeEach(() => {
            backButton.trigger('click');
          });

          it('will push the organ donation page on the router', () => {
            expect($router).toContain(ORGAN_DONATION.path);
          });
        });
      });

      describe('form', () => {
        let backForm;

        beforeEach(() => {
          backForm = wrapper.find('#back-form');
        });

        it('will exist', () => {
          expect(backForm.exists()).toBe(true);
        });

        it('will have a method of "get"', () => {
          expect(backForm.attributes().method).toEqual('get');
        });

        it('will have an action of ORGAN_DONATION path', () => {
          expect(backForm.attributes().action).toEqual(ORGAN_DONATION.path);
        });
      });
    });

    describe('continue', () => {
      describe('button', () => {
        let continueButton;

        beforeEach(() => {
          continueButton = wrapper.find('#continue-form #continue-button');
        });

        it('will exist', () => {
          expect(continueButton.exists()).toBe(true);
        });

        it('will be set as a green button', () => {
          expect(continueButton.classes()).toContain($style.button);
          expect(continueButton.classes()).toContain($style.green);
        });

        describe('when clicked', () => {
          beforeEach(() => {
            continueButton.trigger('click');
          });

          it('will dispatch an organDonation/setAdditionalDetails event', () => {
            expect($store.dispatch).toHaveBeenCalledWith('organDonation/setAdditionalDetails', {
              ethnicityId: undefined,
              religionId: undefined,
            });
          });

          it('will push the confirmation page on the router', () => {
            expect($router).toContain(ORGAN_DONATION_CONFIRMATION.path);
          });
        });
      });

      describe('form', () => {
        let continueForm;
        beforeEach(() => {
          continueForm = wrapper.find('#continue-form');
        });

        it('will exist', () => {
          expect(continueForm.exists()).toBe(true);
        });

        it('will have a method of "post"', () => {
          expect(continueForm.attributes().method).toEqual('post');
        });

        it('will have an action of ORGAN_DONATION_CONFIRMATION route"', () => {
          expect(continueForm.attributes().action).toEqual(ORGAN_DONATION_CONFIRMATION.path);
        });
      });
    });

    describe('mounted', () => {
      it('will not push the organ donation index to the router', () => {
        expect($router).not.toContain(ORGAN_DONATION.path);
      });
    });
  });
});
