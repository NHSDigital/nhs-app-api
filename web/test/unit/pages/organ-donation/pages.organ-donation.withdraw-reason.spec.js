import find from 'lodash/fp/find';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import Withdraw from '@/pages/organ-donation/withdraw-reason';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import {
  INDEX_PATH,
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import locale from '@/locale/en/index';
import {
  ORGAN_DONATION_LAW_CHANGE_URL,
} from '@/router/externalLinks';
import { createRouter, createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const stateReferenceData = {
  withdrawReasons: [
    { id: 1, displayName: 'Select reason' },
    { id: 2, displayName: 'Other' },
    { id: 3, displayName: 'Leaving the UK' },
    { id: 4, displayName: 'Religious grounds' },
    { id: 5, displayName: 'I registered in error' },
    { id: 6, displayName: 'I\'ve changed my mind' },
    { id: 7, displayName: 'My family do not agree' },
  ],
};

const createState = ({ isWithdrawing = false, withdrawReasonId = '', isNativeApp } = {}) => ({
  device: {
    isNativeApp,
  },
  organDonation: {
    ...initialState(),
    ...{
      isWithdrawing,
      referenceData: stateReferenceData,
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

  const mountWrapper = (options) => {
    $router = createRouter();
    state = createState(options);
    $store = createStore({
      state,
    });

    return mount(Withdraw, {
      $router,
      $store,
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  describe('not native', () => {
    beforeEach(() => {
      wrapper = mountWrapper({ isWithdrawing: false, isNativeApp: false });
    });

    it('will redirect back to the home page', () => {
      expect(redirectTo).toBeCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('native', () => {
    describe('not withdrawing', () => {
      beforeEach(() => {
        wrapper = mountWrapper({ isWithdrawing: false, isNativeApp: true });
      });

      it('will redirect back to the organ donation page', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
      });
    });

    describe('withdrawing', () => {
      beforeEach(() => {
        mountWrapper({ isWithdrawing: true, isNativeApp: true });
      });

      it('will not redirect', () => {
        expect(redirectTo).not.toBeCalled();
      });
    });
  });

  describe('withdrawing', () => {
    beforeEach(() => {
      wrapper = mountWrapper({ isWithdrawing: true, isNativeApp: true });
    });

    describe('reason', () => {
      it('will show decision header text', () => {
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.subheader');
      });

      it('will translate the body text', () => {
        const { explanations, exclusions } = locale.organDonation.withdrawReason;
        explanations.forEach(item => expect(wrapper.text()).toContain(item));
        exclusions.forEach(item => expect(wrapper.text()).toContain(item));
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.moreAboutLawText');
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.amendBeforeLink');
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.amendLink');
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.amendAfterLink');
        expect(wrapper.text()).toContain('translate_organDonation.withdrawReason.familyText');
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

    describe('update redirect', () => {
      let updateLink;

      beforeEach(() => {
        updateLink = wrapper.find('#update');
      });

      it('will exist', () => {
        expect(updateLink.exists()).toBe(true);
      });

      it('will display text', () => {
        expect(updateLink.text()).toEqual('translate_organDonation.withdrawReason.amendLink');
      });

      it('will dispatch the "organDonation/amendDecision" action when clicked', () => {
        updateLink.trigger('click');
        expect($store.dispatch).toHaveBeenCalledWith('organDonation/amendStart');
      });
    });

    describe('law change', () => {
      let lawChangeLink;

      beforeEach(() => {
        lawChangeLink = wrapper.find('#law-change');
      });

      it('will exist', () => {
        expect(lawChangeLink.exists()).toBe(true);
      });

      it('will have target set to blank', () => {
        expect(lawChangeLink.attributes('target')).toEqual('_blank');
      });

      it('will have href set to ORGAN_DONATION_LAW_CHANGE_URL', () => {
        expect(lawChangeLink.attributes('href')).toEqual(ORGAN_DONATION_LAW_CHANGE_URL);
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
            wrapper = mountWrapper({
              isWithdrawing: true,
              isNativeApp: true,
              withdrawReasonId: reasonId,
            });
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
            expect($store.dispatch)
              .toHaveBeenCalledWith('organDonation/setWithdrawReasonId', reasonId);
          });

          it('will push ORGAN_DONATION_REVIEW_YOUR_DECISION to the router', () => {
            expect(redirectTo)
              .toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH);
          });
        });
      });
    });

    describe('back button', () => {
      let backButton;

      beforeEach(() => {
        wrapper = mountWrapper({ isNativeApp: false });
        backButton = wrapper.find('#back-button');
      });

      it('will exist', () => {
        expect(backButton.exists()).toBe(true);
      });

      it('will translate the generic back button text', () => {
        expect(backButton.text()).toEqual('translate_generic.backButton.text');
      });

      describe('click', () => {
        beforeEach(() => {
          backButton.trigger('click');
        });

        it('will dispatch the "withdrawCancel" event', () => {
          expect($store.dispatch).toHaveBeenCalledWith('organDonation/withdrawCancel');
        });

        it('will push the organ donation page on the router', () => {
          expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
        });
      });
    });
  });
});

