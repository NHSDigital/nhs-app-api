import AppointmentBooked from '@/components/wayfinder/appointments/AppointmentBookedCard';
import { mount } from '../../../helpers';

const mountAppointmentBooked = ({ propsData = {} }) => mount(
  AppointmentBooked,
  {
    propsData,
  },
);

describe('Appointment Booked Card', () => {
  let wrapper;

  describe('template', () => {
    wrapper = mountAppointmentBooked({
      propsData: {
        appointmentDateTime: '2022-04-18T10:00:00',
        appointmentId: 1,
        deepLinkUrl: '',
        locationDescription: 'A Clinic, A Town, A Country',
      },
    });

    it('will display a h3 header', () => {
      const header = wrapper.find('h3');

      expect(header.exists()).toBe(true);
      expect(header.text()).toBe('Booked appointment');
    });

    it('will display the location', () => {
      const locationDescription = wrapper.find('#location-description-1');

      expect(locationDescription.exists()).toBe(true);
      expect(locationDescription.text()).toBe('A Clinic, A Town, A Country');
    });

    it('will display a button', () => {
      const button = wrapper.find('#bookOrManageAppointment-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('View or manage this appointment');
    });

    it('will display the formatted date', () => {
      expect(wrapper.find('#datetime-1 > strong').text()).toBe('Monday 18 April 2022');
    });

    it('will display formatted time', () => {
      expect(wrapper.find('#datetime-1 > span').text()).toContain('10.00am');
    });
  });
});
