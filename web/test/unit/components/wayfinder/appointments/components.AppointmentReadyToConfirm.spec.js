import AppointmentReadyToConfirm from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountAppointmentReadyToConfirm = ({ propsData = {} }) => mount(
  AppointmentReadyToConfirm,
  { propsData },
);

describe('Appointment Ready To Confirm Card', () => {
  let wrapper;

  describe('template', () => {
    wrapper = mountAppointmentReadyToConfirm({
      propsData: {
        appointmentId: 1,
        locationDescription: 'A Clinic, A Town, A Country',
        deepLinkUrl,
      },
    });

    it('will display a h3 header element', () => {
      const header = wrapper.find('h3');
      expect(header.exists()).toBe(true);
      expect(header.text()).toBe('Ready to confirm appointment');
    });

    it('will display 2 paragraphs', () => {
      const paragraphs = wrapper.findAll('p');

      expect(paragraphs.length).toBe(2);
      expect(paragraphs.at(0).text()).toBe('A Clinic, A Town, A Country');
      expect(paragraphs.at(1).text()).toBe('An appointment has been booked for you. You need to contact the clinic to confirm you are able to attend it.');
    });

    it('will display a button', () => {
      const button = wrapper.find('#contactTheClinicToConfirm-1');

      expect(button.exists()).toBe(true);
      expect(button.text()).toBe('Contact the clinic to confirm');
    });

    it('will call goToUrlViaRedirector when the confirm button is clicked', () => {
      const button = wrapper.find('#contactTheClinicToConfirm-1');

      button.trigger('click');

      expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
    });
  });
});
