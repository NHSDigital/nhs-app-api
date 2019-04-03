/* eslint-disable import/no-extraneous-dependencies */
import ConfirmationPage from '@/pages/appointments/confirmation';
import { mount } from '../../helpers';

describe('confirmation.vue', () => {
  let state;
  let wrapper;

  const createConfirmationPage = () => mount(ConfirmationPage, { state });

  beforeEach(() => {
    state = {
      device: {
        isNativeApp: true,
      },
      myAppointments: {
        disableCancellation: false,
      },
      availableAppointments: {
        patientTelephoneNumbers: '3734',
        symptoms: '',
      },
      session: {
        csrfToken: 'bookingReason',
      },
      bookingReasonNecessity: 'Mandatory',
    };
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
});
