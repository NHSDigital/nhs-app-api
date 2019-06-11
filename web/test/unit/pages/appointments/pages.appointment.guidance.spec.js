import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import AppointmentGuidance from '@/pages/appointments/booking-guidance';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { createStore, mount, createRouter } from '../../helpers';
import { APPOINTMENT_BOOKING, APPOINTMENTS, SYMPTOMS } from '@/lib/routes';

describe('booking guidance', () => {
  let wrapper;
  let $store;
  let $env;
  let $router;

  const mountAs = ({ enabled, native } = {}) => {
    $router = createRouter();
    $env = { ...$env, ONLINE_CONSULTATIONS_ENABLED: enabled };
    $store = createStore({
      $env,
      $router,
      state: {
        device: {
          isNativeApp: native,
        },
        myAppointments: {
          disableCancellation: false,
        },
      },
    });

    return mount(AppointmentGuidance, { $store, $env, $router });
  };

  it('will include the Appointment guidance menu if online consultations env var is truthy', () => {
    wrapper = mountAs({ enabled: true, native: false });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);

    wrapper = mountAs({ enabled: 'true', native: false });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);
  });

  it('will include the guidance text if online consultations env var is falsy', () => {
    wrapper = mountAs({ enabled: false, native: false });
    const header = wrapper.find('#guidance_sub_header');
    const content = wrapper.find('div');

    expect(header.exists()).toBe(true);
    expect(header.text()).toContain('appointments.guidance.header');
    expect(content.text()).toContain('appointments.guidance.text');

    expect(content.text()).toContain('appointments.guidance.li1.header');
    expect(content.text()).toContain('appointments.guidance.li1.text');

    expect(content.text()).toContain('appointments.guidance.li2.header');
    expect(content.text()).toContain('appointments.guidance.li2.text');

    expect(content.text()).toContain('appointments.guidance.li3.header');
    expect(content.text()).toContain('appointments.guidance.li3.text');
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);

    wrapper = mountAs({ enabled: 'false', native: false });
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(false);
  });

  it('will include the check symptoms button if online consultations env var is falsy', () => {
    wrapper = mountAs({ enabled: false, native: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(true);

    wrapper = mountAs({ enabled: 'false', native: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(true);
  });

  it('will not include the check symptoms button if online consultations env var is truthy', () => {
    wrapper = mountAs({ enabled: true, native: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(false);

    wrapper = mountAs({ enabled: 'true', native: false });
    expect(wrapper.find('#btn_check_symptoms').exists()).toBe(false);
  });

  it('will include the back button if online consultations env var is truthy and it is native', () => {
    wrapper = mountAs({ enabled: true, native: true });
    expect(wrapper.find('#back_btn').exists()).toBe(true);

    wrapper = mountAs({ enabled: 'true', native: true });
    expect(wrapper.find('#back_btn').exists()).toBe(true);
  });

  it('will include the destop back link if online consultations env var is truthy and it is not native', () => {
    wrapper = mountAs({ enabled: true, native: false });
    expect(wrapper.find(DesktopGenericBackLink).exists()).toBe(true);

    wrapper = mountAs({ enabled: 'true', native: false });
    expect(wrapper.find(DesktopGenericBackLink).exists()).toBe(true);
  });

  it('will go to the previous page the back button when clicked', () => {
    wrapper = mountAs({ enabled: true, native: true });
    wrapper.find('#back_btn').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the previous page when the desktop back link clicked', () => {
    wrapper = mountAs({ enabled: true, native: false });
    wrapper.find(DesktopGenericBackLink).trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the appointments booking page when book appointment button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_appointment').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENT_BOOKING.path);
  });

  it('will go to the check symptoms page when check symptoms button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_check_symptoms').trigger('click');

    expect($router.push).toHaveBeenCalledWith(SYMPTOMS.path);
  });
});


