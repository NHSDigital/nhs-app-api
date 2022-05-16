import AppointmentCancelled from '@/components/wayfinder/appointments/AppointmentCancelledCard';
import { mount } from '../../../helpers';

const mountAppointmentCancelled = ({ propsData = {} }) => mount(
  AppointmentCancelled,
  { propsData },
);

describe('Appointment Cancelled Card', () => {
  let wrapper;

  describe('template', () => {
    wrapper = mountAppointmentCancelled({
      propsData: {
        locationDescription: 'A Clinic, A Town, A Country',
        appointmentDateTime: '2022-04-18T10:00:00',
        appointmentId: '1',
      },
    });

    it('will display a h3 header', () => {
      const header = wrapper.find('h3');

      expect(header.exists()).toBe(true);
      expect(header.text()).toBe('Cancelled appointment');
    });

    it('will display the formatted date', () => {
      expect(wrapper.find('#datetime-1 > strong').text()).toBe('Monday 18 April 2022');
    });

    it('will display formatted time', () => {
      expect(wrapper.find('#datetime-1 > span').text()).toContain('10.00am');
    });

    it('will display the location', () => {
      const locationDescription = wrapper.find('#location-description-1');

      expect(locationDescription.exists()).toBe(true);
      expect(locationDescription.text()).toBe('A Clinic, A Town, A Country');
    });

    it('will display appointment cancelled message', () => {
      const cancelledDescription = wrapper.find('#cancelled-message-1');

      expect(cancelledDescription.exists()).toBe(true);
      expect(cancelledDescription.text()).toBe('This appointment has been cancelled. You do not need to do anything.');
    });

    it('will display a deep link', () => {
      const deepLink = wrapper.find('#view-this-appointment-1');

      expect(deepLink.exists()).toBe(true);
      expect(deepLink.text()).toBe('View this appointment');
    });
  });
});
