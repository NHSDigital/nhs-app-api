import AppointmentReadyToConfirm from '@/components/wayfinder/appointments/AppointmentReadyToConfirmCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = appointment => mount(
  AppointmentReadyToConfirm, {
    propsData: { item: appointment },
  },
);

describe('Appointment Ready To Confirm Card', () => {
  let wrapper;
  let header;
  let locationDescription;
  let appointmentAdvice;
  let contactTheClinicButton;

  beforeEach(() => {
    wrapper = mountComponent({
      locationDescription: 'A Clinic, A Town, A Country',
      deepLinkUrl,
    });

    header = wrapper.find('h3');
    locationDescription = wrapper.find('[data-purpose="location-description"]');
    appointmentAdvice = wrapper.find('[data-purpose="appointment-advice"]');
    contactTheClinicButton = wrapper.find('[data-purpose="contact-the-clinic-button"]');
  });

  it('will display a h3 header element', () => {
    expect(header.text()).toBe('Ready to confirm appointment');
  });

  it('will display 2 paragraphs', () => {
    expect(locationDescription.text()).toBe('A Clinic, A Town, A Country');
    expect(appointmentAdvice.text()).toBe('An appointment has been booked for you. You need to contact the clinic to confirm you are able to attend it.');
  });

  it('will display a button', () => {
    expect(contactTheClinicButton.text()).toBe('Contact the clinic to confirm');
  });

  it('will call goToUrlViaRedirector when the confirm button is clicked', () => {
    contactTheClinicButton.trigger('click');

    expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
  });
});
