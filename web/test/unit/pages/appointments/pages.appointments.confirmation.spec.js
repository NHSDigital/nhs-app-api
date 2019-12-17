/* eslint-disable import/no-extraneous-dependencies */
import ConfirmationPage from '@/pages/appointments/confirmation';
import { mount } from '../../helpers';
import { APPOINTMENTS, APPOINTMENT_BOOKING_SUCCESS } from '@/lib/routes';
import * as dependency from '@/lib/utils';

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
          disableCancellation: false,
        },
        availableAppointments: {
          patientTelephoneNumbers: '3734',
          symptoms: '',
          selectedSlot: {
            channel: '',
          },
          bookingReasonNecessity: '',
        },
        session: {
          csrfToken: 'bookingReason',
        },
        bookingReasonNecessity: 'Mandatory',
      },
      getters: {
        'session/isProxying': false,
      },
    };

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

  describe('confirming the appointment', () => {
    it('when confirmed as proxy should redirect to confirmation page', async () => {
      // arrange
      $store.getters['session/isProxying'] = true;
      const e = { preventDefault: jest.fn() };

      // act
      await wrapper.vm.onConfirmButtonClicked(e);

      // assert
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'availableAppointments/book', jasmine.anything());

      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_BOOKING_SUCCESS.path);
    });

    it('when confirmed as main user should redirect to appointments home', async () => {
      // arrange
      $store.getters['session/isProxying'] = false;
      const e = { preventDefault: jest.fn() };

      // act
      await wrapper.vm.onConfirmButtonClicked(e);

      // assert
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(1, 'availableAppointments/book', jasmine.anything());
      expect($store.dispatch)
        .toHaveBeenNthCalledWith(2, 'flashMessage/addSuccess', jasmine.anything());
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, APPOINTMENTS.path);
    });
  });
});
