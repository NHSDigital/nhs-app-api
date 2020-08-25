import TermsAndConditions from '@/pages/terms-and-conditions';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import { mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

const mountTermsAndConditions = (updatedConsentRequired = false) => (mount(TermsAndConditions, {
  state: {
    device: { isNativeApp: true },
    termsAndConditions: {
      areAccepted: updatedConsentRequired,
      updatedConsentRequired,
    },
  },
  shallow: true,
}));

describe('Terms and Conditions', () => {
  beforeEach(() => {
    EventBus.$emit.mockClear();
  });

  describe('header', () => {
    describe('updated consent not required', () => {
      beforeEach(() => {
        mountTermsAndConditions();
      });

      it('will only emit UPDATE_HEADER with the default title', () => {
        expect(EventBus.$emit).toHaveBeenCalledTimes(1);
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'termsAndConditions.initial.title');
      });
    });

    describe('updated consent required', () => {
      beforeEach(() => {
        mountTermsAndConditions(true);
      });

      it('will only emit UPDATE_HEADER with the default title', () => {
        expect(EventBus.$emit).toHaveBeenCalledTimes(1);
        expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'termsAndConditions.updated.title');
      });
    });
  });
});
