import AdditionalDetails from '@/pages/organ-donation/additional-details';
import BackButton from '@/components/BackButton';
import find from 'lodash/fp/find';
import i18n from '@/plugins/i18n';
import {
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
} from '@/router/paths';
import { DECISION_OPT_IN, initialState } from '@/store/modules/organDonation/mutation-types';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

describe('additional-details', () => {
  // In this case we want string numbers to be coerced into actual numbers and vice versa.
  // eslint-disable-next-line eqeqeq
  const findOptionById = id => find(x => x.value == id);
  let $router;
  let $store;
  let state;
  let wrapper;

  const mountWrapper = () => {
    const store = $store || createStore({ state });
    return mount(AdditionalDetails, { $router, $store: store, mountOpts: { i18n } });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    $router = [];
    state = {
      organDonation: initialState(),
      device: {
        isNativeApp: false,
      },
    };
  });

  describe('decision set', () => {
    beforeEach(() => {
      state.organDonation.registration.decision = DECISION_OPT_IN;
      state.organDonation.registration.decisionDetails.all = true;
      $store = createStore({ state });
      wrapper = mountWrapper({ });
    });

    it('will translate the additional details subheader', () => {
      expect(wrapper.text()).toContain('Additional details');
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
          expect(option.text).toEqual('Please select');
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
          expect(option.text).toEqual('Please select');
        });
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

        it('will be a button with nhsuk-button style', () => {
          const classes = continueButton.classes();
          expect(classes).toContain('nhsuk-button');
        });

        describe('when clicked', () => {
          beforeEach(() => {
            continueButton.trigger('click');
          });

          it('will dispatch an organDonation/setAdditionalDetails event', () => {
            expect($store.dispatch).toHaveBeenCalledWith('organDonation/setAdditionalDetails', {
              ethnicityId: '',
              religionId: '',
            });
          });

          it('will push the confirmation page on the router', () => {
            expect(redirectTo)
              .toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
          });
        });
      });
    });

    describe('mounted', () => {
      it('will not push the organ donation index to the router', () => {
        expect(redirectTo).not.toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
      });
    });
  });
});
