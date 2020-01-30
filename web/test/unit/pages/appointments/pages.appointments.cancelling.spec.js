import each from 'jest-each';
import * as dependency from '@/lib/utils';
import AppointmentsCancellingPage from '@/pages/appointments/cancelling';
import { APPOINTMENT_CANCELLING_SUCCESS } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');

describe('cancelling.vue', () => {
  let $store;
  let wrapper;
  const selectedAppointmentId = 'appt-5';

  const createAppointmentCancellingPage = () => mount(AppointmentsCancellingPage, {
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
          cancellationReasons: [],
          selectedAppointment: {
            id: selectedAppointmentId,
          },
          error: null,
        },
        session: {
          csrfToken: 'bookingReason',
        },
      },
      getters: {
        'session/isProxying': false,
      },
    });

    dependency.redirectTo = jest.fn();
    wrapper = createAppointmentCancellingPage();
  });

  describe('cancel', () => {
    let cancelButton;

    beforeEach(() => {
      cancelButton = wrapper.find('#btn_cancel_appointment');
    });

    it('will exist', () => {
      expect(cancelButton.exists()).toBe(true);
    });

    describe('on click', () => {
      beforeEach(async () => {
        cancelButton.trigger('click');
      });

      it('will dispatch `myAppointments/cancel` action', () => {
        expect($store.dispatch)
          .toHaveBeenNthCalledWith(1, 'myAppointments/cancel', {
            appointmentId: selectedAppointmentId,
            cancellationReasonId: '',
          });
      });

      it('will redirect to appointments cancelling success page', async () => {
        expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm,
          APPOINTMENT_CANCELLING_SUCCESS.path);
      });
    });
  });

  describe('errors', () => {
    each([
      400,
      403,
      409,
      461,
      500,
      502,
      504,
    ]).it('will display an error dialog for status code: %s', (status) => {
      $store.state.myAppointments.error = { status };
      expect(wrapper.find(`#error-dialog-${status}`).exists()).toBe(true);
    });
  });
});
