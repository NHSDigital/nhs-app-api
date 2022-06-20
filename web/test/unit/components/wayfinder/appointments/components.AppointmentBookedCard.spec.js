import AppointmentBooked from '@/components/wayfinder/appointments/AppointmentBookedCard';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { mount } from '../../../helpers';

jest.mock('@/components/wayfinder/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = appointment => mount(
  AppointmentBooked, {
    propsData: { item: appointment },
  },
);

describe('Appointment Booked Card', () => {
  let wrapper;
  let header;
  let locationDescription;
  let appointmentDateTime;
  let bookOrManageButton;

  beforeEach(() => {
    wrapper = mountComponent({
      appointmentDateTime: '2022-04-18T10:00:00',
      locationDescription: 'A Clinic, A Town, A Country',
      deepLinkUrl,
    });

    header = wrapper.find('h3');
    locationDescription = wrapper.find('[data-purpose="location-description"]');
    appointmentDateTime = wrapper.find('[data-purpose="appointment-date-time"]');
    bookOrManageButton = wrapper.find('[data-purpose="book-or-manage-appointment-button"]');
  });

  it('will display a h3 header', () => {
    expect(header.text()).toBe('Booked appointment');
  });

  it('will display the location', () => {
    expect(locationDescription.text()).toBe('A Clinic, A Town, A Country');
  });

  it('will display the formatted date', () => {
    expect(appointmentDateTime.find('strong').text()).toBe('Monday 18 April 2022');
  });

  it('will display formatted time', () => {
    expect(appointmentDateTime.find('span').text()).toContain('10.00am');
  });

  it('will display a button', () => {
    expect(bookOrManageButton.text()).toBe('View or manage this appointment');
  });

  it('will call goToUrlViaRedirector when the book button is clicked', () => {
    bookOrManageButton.trigger('click');

    expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
  });
});
