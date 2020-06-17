import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import BookingGuidancePage from '@/pages/appointments/gp-appointments/booking-guidance';
import { GP_APPOINTMENTS_PATH, APPOINTMENT_BOOKING_PATH, SYMPTOMS_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount, createRouter } from '../../helpers';

jest.mock('@/lib/utils');

describe('booking guidance', () => {
  let wrapper;
  let $store;
  let $router;
  let $style;

  const mountAs = ({
    onlineConsultationsEnabled = false,
    isNativeApp = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      $router,
      state: {
        device: {
          isNativeApp,
        },
        myAppointments: {
          disableCancellation: false,
        },
      },
      getters: {
        'serviceJourneyRules/onlineConsultationsEnabled': onlineConsultationsEnabled,
      },
    });
    $style = { info: 'info' };

    return mount(BookingGuidancePage, {
      $store,
      $router,
      $style,
      stubs: {
        'page-title': '<div></div>',
      },
    });
  };

  beforeEach(() => {
    redirectTo.mockClear();
  });

  it('will include the Appointment guidance menu if cdss admin or cdss advice is enabled', () => {
    wrapper = mountAs({ onlineConsultationsEnabled: true });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);

    wrapper = mountAs();
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);
  });

  it('will include the destop back link if online consultations enabled and it is not native', () => {
    wrapper = mountAs({ onlineConsultationsEnabled: true });
    expect(wrapper.find(DesktopGenericBackLink).exists()).toBe(true);
  });

  it('will go to the previous page when the desktop back link clicked', () => {
    wrapper = mountAs({ onlineConsultationsEnabled: true });
    wrapper.find(DesktopGenericBackLink).find('a').trigger('click');

    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, GP_APPOINTMENTS_PATH);
  });

  it('will go to the appointments booking page when book appointment button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_appointment').trigger('click');

    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, APPOINTMENT_BOOKING_PATH);
  });

  it('will include the guidance text if online consultations env var is false', () => {
    wrapper = mountAs();
    const content = wrapper.find('#info');

    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);

    expect(content.text()).toContain('translate_appointments.guidance.li1.header');
    expect(content.text()).toContain('translate_appointments.guidance.li1.text');
    expect(content.text()).toContain('translate_appointments.guidance.li2.header');
    expect(content.text()).toContain('translate_appointments.guidance.li2.text');
    expect(content.text()).toContain('translate_appointments.guidance.li3.header');
    expect(content.text()).toContain('translate_appointments.guidance.li3.text');
  });

  it('will include the check symptoms button if online consultations env var is false', () => {
    wrapper = mountAs();
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(true);
  });

  it('will not include the check symptoms button if online consultations env var is true', () => {
    wrapper = mountAs({ onlineConsultationsEnabled: true });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(false);
  });

  it('will go to the check symptoms page when check symptoms button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_check_symptoms').trigger('click');

    expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, SYMPTOMS_PATH);
  });
});


