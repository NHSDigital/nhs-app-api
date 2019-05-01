import find from 'lodash/fp/find';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import Withdraw from '@/pages/organ-donation/withdraw-reason';
import { INDEX, ORGAN_DONATION, ORGAN_DONATION_REVIEW_YOUR_DECISION } from '@/lib/routes';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { $t, createRouter, createStore, mount } from '../../helpers';

const createState = ({ isWithdrawing = false, referenceData = {}, withdrawReasonId = '' } = {}) => ({
  device: {
    isNativeApp: false,
  },
  organDonation: {
    ...initialState(),
    ...{
      isWithdrawing,
      referenceData,
      withdrawReasonId,
    },
  },
});

describe('organ donation withdraw reason page', () => {
  // In this case we want string numbers to be coerced into actual numbers and vice versa.
  // eslint-disable-next-line eqeqeq
  const findOptionById = id => find(x => x.value == id);
  let state;
  let $store;
  let wrapper;
  let $router;

  beforeEach(() => {
    $router = createRouter();
    state = createState({
      referenceData: {
        withdrawReasons: [
          { id: 1, displayName: 'Select reason' },
          { id: 2, displayName: 'Other' },
          { id: 3, displayName: 'Leaving the UK' },
          { id: 4, displayName: 'Religious grounds' },
          { id: 5, displayName: 'I registered in error' },
          { id: 6, displayName: 'I\'ve changed my mind' },
          { id: 7, displayName: 'My family do not agree' },
        ],
      },
    });
    $store = createStore({ state });
  });

  describe('fetch', () => {
    let redirect;

    const fetch = ({ isWithdrawing = false, source }) => {
      redirect = jest.fn();

      wrapper.vm.$options.fetch({
        redirect,
        route: {
          query: {
            source,
          },
        },
        store: createStore({
          state: createState({ isWithdrawing }),
        }),
      });
    };

    beforeEach(() => {
      wrapper = mount(Withdraw, { $store });
    });

    describe('not native', () => {
      beforeEach(() => {
        fetch({ isWithdrawing: false, source: 'web' });
      });

      it('will redirect back to the home page', () => {
        expect(redirect).toBeCalledWith(INDEX.path);
      });
    });

    describe('native', () => {
      const source = 'ios';

      describe('not withdrawing', () => {
        beforeEach(() => {
          fetch({ isWithdrawing: false, source });
        });

        it('will redirect back to the organ donation page', () => {
          expect(redirect).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });

      describe('withdrawing', () => {
        beforeEach(() => {
          fetch({ isWithdrawing: true, source });
        });

        it('will not redirect', () => {
          expect(redirect).not.toBeCalled();
        });
      });
    });
  });

  describe('withdrawing', () => {
    beforeEach(() => {
      state.organDonation.isWithdrawing = true;
      wrapper = mount(Withdraw, { $store });
    });

    describe('reason', () => {
      it('will show decision header text', () => {
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.subheader');
      });

      it('will translate the reason body text', () => {
        expect($t).toHaveBeenCalledWith('organDonation.withdrawReason.bodyItems');
      });

      describe('dropdown', () => {
        let dropdown;
        let options;
        let elements;

        beforeEach(() => {
          dropdown = wrapper.find('#reason');
          options = dropdown.findAll('option');
          elements = options.wrappers.map(x => x.element);
        });

        it('will exist', () => {
          expect(dropdown.exists()).toBe(true);
        });

        it('will have options for each withdraw reason', () => {
          state.organDonation.referenceData.withdrawReasons.forEach((withdrawReason) => {
            const option = findOptionById(withdrawReason.id)(elements);
            expect(option).not.toBeUndefined();
            expect(option.value).toBe(withdrawReason.id.toString());
            expect(option.text).toBe(withdrawReason.displayName);
          });
        });

        it('will have a placeholder option', () => {
          const option = findOptionById('')(elements);
          expect(option).not.toBeUndefined();
          expect(option.text).toBe('translate_organDonation.withdrawReason.reason.placeholder');
        });
      });
    });

    describe('continue', () => {
      let continueButton;

      beforeEach(() => {
        continueButton = wrapper.find('#continue-button');
      });

      it('will exist', () => {
        expect(continueButton.exists()).toBe(true);
      });

      describe('no reason', () => {
        describe('click', () => {
          let scrollTo;

          beforeEach(() => {
            scrollTo = jest.fn();
            global.scrollTo = scrollTo;
            continueButton.trigger('click');
          });

          it('will show error dialog', () => {
            expect(wrapper.find('#errors').exists()).toBe(true);
          });

          it('will show inline error', () => {
            expect(wrapper.find(ErrorMessage).exists()).toBe(true);
          });

          it('will scroll to top', () => {
            expect(scrollTo).toHaveBeenCalledWith(0, 0);
          });
        });
      });

      describe('with reason', () => {
        describe('click', () => {
          let reasonId;

          beforeEach(() => {
            reasonId = 'Other';
            state.organDonation.withdrawReasonId = reasonId;
            wrapper = mount(Withdraw, { $store, $router });
            continueButton = wrapper.find('#continue-button');
            continueButton.trigger('click');
          });

          it('will not show error dialog', () => {
            expect(wrapper.find('#errors').exists()).toBe(false);
          });

          it('will not show inline error', () => {
            expect(wrapper.find(ErrorMessage).exists()).toBe(false);
          });

          it('will dispatch "organDonation/setWithdrawReasonId"', () => {
            expect($store.dispatch).toHaveBeenCalledWith('organDonation/setWithdrawReasonId', reasonId);
          });

          it('will push ORGAN_DONATION_REVIEW_YOUR_DECISION to the router', () => {
            expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION_REVIEW_YOUR_DECISION.path);
          });
        });
      });
    });

    describe('back button', () => {
      let backButton;

      beforeEach(() => {
        backButton = wrapper.find('#back-button');
      });

      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });

      it('will translate the generic back button text', () => {
        expect($t).toHaveBeenCalledWith('generic.backButton.text');
      });

      describe('click', () => {
        beforeEach(() => {
          wrapper = mount(Withdraw, { $store, $router });
          backButton = wrapper.find('#back-button');
          backButton.trigger('click');
        });

        it('will dispatch the "withdrawCancel" event', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/withdrawCancel');
        });

        it('will push the organ donation page on the router', () => {
          expect($router.push).toHaveBeenCalledWith(ORGAN_DONATION.path);
        });
      });
    });
  });
});

