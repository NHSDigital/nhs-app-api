/* eslint-disable import/no-extraneous-dependencies */
import AppointmentsCancellingPage from '@/pages/appointments/cancelling';
import { mount } from '../../helpers';
import { APPOINTMENT_CANCELLING_SUCCESS } from '@/lib/routes';
import * as dependency from '@/lib/utils';

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
    $store = {
      dispatch: jest.fn(),
      app: {
        $env: {},
      },
      state: {
        device: {
          isNativeApp: true,
        },
        myAppointments: {
          cancellationReasons: [],
          selectedAppointment: {
            id: selectedAppointmentId,
          },
        },
        session: {
          csrfToken: 'bookingReason',
        },
      },
      getters: {
        'session/isProxying': false,
      },
    };

    dependency.redirectTo = jest.fn();
    wrapper = createAppointmentCancellingPage();
  });

  describe('cancelling the appointment', () => {
    it('when cancelling as proxy should redirect to confirmation page', async () => {
      // arrange
      $store.getters['session/isProxying'] = true;

      // act
      await wrapper.vm.onCancelButtonClicked();

      // assert
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'myAppointments/cancel', {
          appointmentId: selectedAppointmentId,
          cancellationReasonId: '',
        });

      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_CANCELLING_SUCCESS.path);
    });

    it('when cancelled as main user should redirect to appointments home', async () => {
      // arrange
      $store.getters['session/isProxying'] = false;

      // act
      await wrapper.vm.onCancelButtonClicked();

      // assert
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'myAppointments/cancel', {
          appointmentId: selectedAppointmentId,
          cancellationReasonId: '',
        });
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_CANCELLING_SUCCESS.path);
    });
  });
});
