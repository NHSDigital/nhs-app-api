import * as dependency from '@/lib/utils';
import Channel from '@/lib/channel';
import Confirmation from '@/pages/appointments/confirmation';
import Necessity from '@/lib/necessity';
import { APPOINTMENTS } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

describe('appointments confirmation page', () => {
  let $style;
  let state;
  let wrapper;

  const createState = slot => ({
    availableAppointments: {
      bookingReasonNecessity: Necessity.Mandatory,
      selectedSlot: slot,
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

  describe('mounted', () => {
    beforeEach(() => {
      dependency.redirectTo = jest.fn();
    });
    describe('when no slot is selected', () => {
      beforeEach(() => {
        state = createState(undefined);
        wrapper = mountConfirmation();
      });

      it('will redirect to appointments hub page', () => {
        expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS.path);
      });
    });

    describe('when slot is selected', () => {
      beforeEach(() => {
        state = createState({ channel: Channel.Telephone });
        wrapper = mountConfirmation();
      });

      it('will not call redirect', () => {
        expect(dependency.redirectTo).not.toHaveBeenCalled();
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
