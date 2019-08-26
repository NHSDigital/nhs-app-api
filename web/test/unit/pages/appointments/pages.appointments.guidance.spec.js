import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import BookingGuidancePage from '@/pages/appointments/booking-guidance';
import { createStore, mount, createRouter } from '../../helpers';
import { APPOINTMENT_BOOKING, APPOINTMENTS, SYMPTOMS } from '@/lib/routes';

describe('booking guidance', () => {
  let wrapper;
  let $store;
  let $router;
  let $style;

  const mountAs = ({ olcEnabled = false, isNativeApp = false } = {}) => {
    $router = createRouter();
    $store = createStore({
      $env: {
        ONLINE_CONSULTATIONS_ENABLED: olcEnabled,
      },
      $router,
      state: {
        device: {
          isNativeApp,
        },
        myAppointments: {
          disableCancellation: false,
        },
      },
    });
    $style = { info: 'info' };

    return mount(BookingGuidancePage, { $store, $router, $style });
  };

  it('will include the Appointment guidance menu if online consultations env var is truthy', () => {
    wrapper = mountAs({ olcEnabled: true });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);

    wrapper = mountAs({ olcEnabled: false });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);
  });

  it('will include the back button if online consultations env var is truthy and it is native', () => {
    wrapper = mountAs({ olcEnabled: true, isNativeApp: true });
    expect(wrapper.find('#back_btn').exists()).toBe(true);
  });

  it('will include the destop back link if online consultations env var is truthy and it is not native', () => {
    wrapper = mountAs({ olcEnabled: true, isNativeApp: false });
    expect(wrapper.find(DesktopGenericBackLink).exists()).toBe(true);
  });

  it('will go to the previous page the back button when clicked', () => {
    wrapper = mountAs({ olcEnabled: true, isNativeApp: true });
    wrapper.find('#back_btn').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the previous page when the desktop back link clicked', () => {
    wrapper = mountAs({ olcEnabled: true, isNativeApp: false });
    wrapper.find(DesktopGenericBackLink).trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the appointments booking page when book appointment button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_appointment').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENT_BOOKING.path);
  });

  it('will include the guidance text if online consultations env var is false', () => {
    wrapper = mountAs({ olcEnabled: false, isNativeApp: false });
    const content = wrapper.find('.info');

    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);

    expect(content.text()).toContain('translate_appointments.guidance.li1.header');
    expect(content.text()).toContain('translate_appointments.guidance.li1.text');
    expect(content.text()).toContain('translate_appointments.guidance.li2.header');
    expect(content.text()).toContain('translate_appointments.guidance.li2.text');
    expect(content.text()).toContain('translate_appointments.guidance.li3.header');
    expect(content.text()).toContain('translate_appointments.guidance.li3.text');
  });

  it('will include the check symptoms button if online consultations env var is false', () => {
    wrapper = mountAs({ olcEnabled: false, isNativeApp: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(true);
  });

  it('will not include the check symptoms button if online consultations env var is true', () => {
    wrapper = mountAs({ olcEnabled: true, isNativeApp: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(false);
  });

  it('will go to the check symptoms page when check symptoms button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_check_symptoms').trigger('click');

    expect($router.push).toHaveBeenCalledWith(SYMPTOMS.path);
  });
});


