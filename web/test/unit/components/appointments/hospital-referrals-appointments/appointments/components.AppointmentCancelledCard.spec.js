import AppointmentCancelled from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentCancelledCard';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { mount } from '../../../../helpers';

jest.mock('@/components/appointments/hospital-referrals-appointments/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = appointment => mount(
  AppointmentCancelled, {
    propsData: { item: appointment },
  },
);

describe('Appointment Cancelled Card', () => {
  let wrapper;
  let header;
  let locationDescription;
  let cancelledInfo;
  let appointmentDateTime;
  let viewAppointmentLink;

  beforeEach(() => {
    wrapper = mountComponent({
      locationDescription: 'A Clinic, A Town, A Country',
      appointmentDateTime: '2022-04-18T10:00:00',
      deepLinkUrl,
    });

    header = wrapper.find('h3');
    locationDescription = wrapper.find('[data-purpose="location-description"]');
    cancelledInfo = wrapper.find('[data-purpose="appointment-cancelled-info"]');
    appointmentDateTime = wrapper.find('[data-purpose="appointment-date-time"]');
    viewAppointmentLink = wrapper.find('[data-purpose="view-appointment-link"]');
  });

  it('will include the RedirectorMixin', () => {
    expect(AppointmentCancelled.mixins).toContain(RedirectorMixin);
  });

  it('will display a h3 header', () => {
    expect(header.text()).toBe('Cancelled appointment');
  });

  it('will display the formatted date', () => {
    expect(appointmentDateTime.find('.nhsuk-body').text()).toBe('Monday 18 April 2022');
  });

  it('will display formatted time', () => {
    expect(appointmentDateTime.find('.nhsuk-body-l').text()).toContain('10.00am');
  });

  it('will display the location', () => {
    expect(locationDescription.text()).toBe('A Clinic, A Town, A Country');
  });

  it('will display appointment cancelled message', () => {
    expect(cancelledInfo.text()).toBe('This appointment has been permanently cancelled. You do not need to do anything.');
  });

  it('will display a deep link', () => {
    expect(viewAppointmentLink.text()).toBe('View this appointment');
  });

  it('will call goToUrlViaRedirector when the view appointment link is clicked', () => {
    viewAppointmentLink.trigger('click');

    expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
  });
});
