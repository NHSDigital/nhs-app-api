import each from 'jest-each';
import * as dependency from '@/lib/utils';
import ConfirmationPage from '@/pages/appointments/confirmation';
import { APPOINTMENT_BOOKING_SUCCESS } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');

describe('confirmation.vue', () => {
  let $store;
  let wrapper;

  const createConfirmationPage = () => mount(ConfirmationPage, {
    $store,
    stubs: {
      'page-title': '<div></div>',
    },
  });

  beforeEach(() => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: true,
        },
        myAppointments: {
          disableCancellation: false,
        },
        availableAppointments: {
          patientTelephoneNumbers: '3734',
          symptoms: '',
          selectedSlot: {
            channel: '',
          },
          bookingReasonNecessity: '',
          error: null,
        },
        session: {
          csrfToken: 'bookingReason',
        },
        bookingReasonNecessity: 'Mandatory',
      },
      getters: {
        'session/isProxying': false,
      },
    });

    dependency.redirectTo = jest.fn();
    wrapper = createConfirmationPage();
  });

  describe('booking reason text box on appointment confirmation page', () => {
    let reasonText;
    beforeEach(() => {
      reasonText = wrapper.find('#reasonText');
    });

    it('Text area will exist', () => {
      expect(reasonText.exists()).toBe(true);
    });

    it('Text area has max length html attribute set to 150', () => {
      expect(reasonText.attributes().maxlength).toEqual('150');
    });
  });

  describe('book', () => {
    let bookButton;

    beforeEach(() => {
      bookButton = wrapper.find('#btn_book_appointment');
    });

    it('will exist', () => {
      expect(bookButton.exists()).toBe(true);
    });

    describe('on click', () => {
      beforeEach(async () => {
        bookButton.trigger('click');
      });

      it('will dispatch `availableAppointments/book` action', () => {
        expect($store.dispatch)
          .toHaveBeenNthCalledWith(1, 'availableAppointments/book', jasmine.anything());
      });

      it('will redirect to booking success page', async () => {
        expect(dependency.redirectTo)
          .toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_BOOKING_SUCCESS.path);
      });
    });
  });

  describe('errors', () => {
    each([
      400,
      403,
      409,
      460,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      $store.state.availableAppointments.error = { status };
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
