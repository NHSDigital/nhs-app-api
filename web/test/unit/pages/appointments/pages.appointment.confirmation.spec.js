import { redirectTo } from '@/lib/utils';
import Channel from '@/lib/channel';
import Confirmation from '@/pages/appointments/gp-appointments/confirmation';
import Necessity from '@/lib/necessity';
import { GP_APPOINTMENTS_PATH } from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('appointments confirmation page', () => {
  let $style;
  let state;
  let wrapper;

  const createState = slot => ({
    availableAppointments: {
      bookingReasonNecessity: Necessity.Mandatory,
      selectedSlot: slot,
      patientTelephoneNumbers: [],
    },
    device: {
      isNativeApp: false,
    },
    myAppointments: {
      disableCancellation: false,
    },
    session: {
      csrfToken: '12345',
    },
  });

  const mountConfirmation = () => mount(Confirmation, {
    $store: createStore({ state }),
    $style,
    stubs: {
      'page-title': '<div></div>',
    },
  });

  beforeEach(() => {
    redirectTo.mockClear();
    EventBus.$emit.mockClear();
  });

  describe('mounted', () => {
    describe('when no slot is selected', () => {
      beforeEach(() => {
        state = createState(undefined);
        wrapper = mountConfirmation();
      });

      it('will redirect to appointments hub page', () => {
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, GP_APPOINTMENTS_PATH);
      });
    });

    describe('when slot is selected', () => {
      beforeEach(() => {
        state = createState({ channel: Channel.Telephone });
        wrapper = mountConfirmation();
      });

      it('will not call redirect', () => {
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });
  });

  describe('errors', () => {
    beforeEach(() => {
      window.scrollTo = jest.fn();
    });

    describe('reason required', () => {
      beforeEach(() => {
        $style = {
          error: 'error',
        };

        state = createState({});
        wrapper = mountConfirmation();
      });

      describe('errors', () => {
        let button;
        let errorContainer;
        let reasonError;
        let telephoneError;

        const findErrorContainer = () => wrapper.find('[data-purpose=error-container]');
        const findReasonError = () => wrapper.find('[data-purpose=reason-error]');
        const findTelephoneError = () => wrapper.find('[data-purpose=telephone-error]');

        describe('empty telephone number', () => {
          beforeEach(() => {
            state = createState({ channel: Channel.Telephone });
            wrapper = mountConfirmation();
            wrapper.vm.symptoms = 'reason';
            button = wrapper.find('#btn_book_appointment');
            button.trigger('click');
            telephoneError = findTelephoneError();
          });

          it('will display the error when trying to submit without a telephone number', () => {
            expect(telephoneError.exists()).toEqual(true);
          });

          it('will add the error class to the telephone input element', () => {
            const telephoneText = wrapper.find('#telephoneNumberText');
            expect(telephoneText.classes()).toContain('nhsuk-input--error');
          });

          it('will set focus on the error component', () => {
            expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
          });

          describe('telephone number subsequently inputted', () => {
            beforeEach(() => {
              wrapper.vm.telephoneNumber = '1234';
            });

            it('will hide the telephone error when updated', () => {
              reasonError = findTelephoneError();
              expect(reasonError.exists()).toBe(false);
            });

            it('will hide the error container when updated', () => {
              errorContainer = findErrorContainer();
              expect(errorContainer.exists()).toBe(false);
            });
          });
        });

        describe('empty reason', () => {
          beforeEach(() => {
            button = wrapper.find('#btn_book_appointment');
            button.trigger('click');
            errorContainer = findErrorContainer();
            reasonError = findReasonError();
          });

          it('will display a error when trying to submit without a reason', () => {
            expect(errorContainer.exists()).toBe(true);
            expect(reasonError.exists()).toBe(true);
            expect(wrapper.vm.showError).toBe(true);
          });

          it('will set focus on the error component', () => {
            expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
          });

          describe('reason subsequently inputted', () => {
            beforeEach(() => {
              wrapper.vm.symptoms = 'boo';
            });

            it('will hide the reason error when updated', () => {
              reasonError = findReasonError();
              expect(reasonError.exists()).toBe(false);
            });

            it('will hide the error container when updated', () => {
              errorContainer = findErrorContainer();
              expect(errorContainer.exists()).toBe(false);
            });
          });
        });
      });
    });
  });
});
