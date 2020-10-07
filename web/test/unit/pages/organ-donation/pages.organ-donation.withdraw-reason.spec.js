import ErrorMessage from '@/components/widgets/ErrorMessage';
import find from 'lodash/fp/find';
import i18n from '@/plugins/i18n';
import Withdraw from '@/pages/organ-donation/withdraw-reason';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import {
  INDEX_PATH,
  ORGAN_DONATION_PATH,
  ORGAN_DONATION_REVIEW_YOUR_DECISION_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import {
  ORGAN_DONATION_LAW_CHANGE_URL,
} from '@/router/externalLinks';
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
      mountOpts: { i18n },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    EventBus.$emit.mockClear();
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
        expect(wrapper.text()).toContain('Withdraw your previous organ donation decision');
      });

      it('will translate the body text', () => {
        expect(wrapper.text()).toContain('Withdrawing from the NHS Organ Donor Register is different from recording a decision not to donate (opting out). If you withdraw, we will not know your decision.');
        expect(wrapper.text()).toContain('In line with changes to the law around organ donation, you are considered to have agreed to be an organ donor, unless:');
        expect(wrapper.text()).toContain('you have recorded a decision not to donate');
        expect(wrapper.text()).toContain('you are in an excluded group');
        expect(wrapper.text()).toContain('Find out more about the ');
        expect(wrapper.text()).toContain('If you do not want to be an organ donor, the best way to tell us is to ');
        expect(wrapper.text()).toContain('update your decision');
        expect(wrapper.text()).toContain(' You can change your decision at any time.');
        expect(wrapper.text()).toContain('Whatever you decide, please make sure your family know your decision.');
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
          expect(option.text).toBe('Select reason');
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
        expect(updateLink.text()).toEqual('update your decision');
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
          beforeEach(() => {
            continueButton.trigger('click');
          });

          it('will show error dialog', () => {
            expect(wrapper.find('#errors').exists()).toBe(true);
          });

          it('will show inline error', () => {
            expect(wrapper.find(ErrorMessage).exists()).toBe(true);
          });

          it('will set focus on the error component', () => {
            expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
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

          it('will not set focus on the error component', () => {
            expect(EventBus.$emit).not.toHaveBeenCalledWith(FOCUS_ERROR_ELEMENT);
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
        expect(backButton.text()).toEqual('Back');
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

