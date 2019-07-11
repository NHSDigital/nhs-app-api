import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import AppointmentGuidance from '@/pages/appointments/booking-guidance';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { createStore, mount, createRouter } from '../../helpers';
import { APPOINTMENT_BOOKING, APPOINTMENTS } from '@/lib/routes';

describe('booking guidance', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = (native = false) => {
    $router = createRouter();
    $store = createStore({
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

    return mount(AppointmentGuidance, { $store, $router });
  };

  it('will include the Appointment guidance menu if online consultations env var is truthy', () => {
    wrapper = mountAs();
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);

    wrapper = mountAs();
    expect(wrapper.find(AppointmentGuidanceMenu).exists()).toBe(true);
  });

  it('will include the back button if online consultations env var is truthy and it is native', () => {
    wrapper = mountAs(true);
    expect(wrapper.find('#back_btn').exists()).toBe(true);
  });

  it('will include the destop back link if online consultations env var is truthy and it is not native', () => {
    wrapper = mountAs(false);
    expect(wrapper.find(DesktopGenericBackLink).exists()).toBe(true);
  });

  it('will go to the previous page the back button when clicked', () => {
    wrapper = mountAs(true);
    wrapper.find('#back_btn').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the previous page when the desktop back link clicked', () => {
    wrapper = mountAs(false);
    wrapper.find(DesktopGenericBackLink).trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENTS.path);
  });

  it('will go to the appointments booking page when book appointment button clicked', () => {
    wrapper = mountAs();
    wrapper.find('#btn_appointment').trigger('click');

    expect($router.push).toHaveBeenCalledWith(APPOINTMENT_BOOKING.path);
  });
});


