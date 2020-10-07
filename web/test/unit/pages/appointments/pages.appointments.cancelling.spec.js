import { redirectTo } from '@/lib/utils';
import AppointmentsCancellingPage from '@/pages/appointments/gp-appointments/cancelling';
import { APPOINTMENT_CANCELLING_SUCCESS_PATH } from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils');
jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('cancelling.vue', () => {
  let $store;
  let wrapper;
  const selectedAppointmentId = 'appt-5';

  const createAppointmentCancellingPage = () => mount(AppointmentsCancellingPage, { $store });

  beforeEach(() => {
    redirectTo.mockClear();

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
        expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_CANCELLING_SUCCESS_PATH);
      });
    });

    describe('shows error on click', () => {
      beforeEach(async () => {
        redirectTo.mockClear();
        EventBus.$emit.mockClear();

        $store = createStore({
          state: {
            device: {
              isNativeApp: true,
            },
            myAppointments: {
              cancellationReasons: ['not needed', 'cannot attend'],
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
        wrapper = createAppointmentCancellingPage();
        cancelButton = wrapper.find('#btn_cancel_appointment');
        cancelButton.trigger('click');
      });

      it('will show an error ', async () => {
        expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
        expect($store.dispatch).not.toHaveBeenCalled();
        expect(redirectTo).not.toHaveBeenCalled();
      });
    });
  });
});
