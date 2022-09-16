import AppointmentPendingChange from '@/components/appointments/hospital-referrals-appointments/appointments/AppointmentPendingChangeCard';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { mount } from '../../../../helpers';

jest.mock('@/components/appointments/hospital-referrals-appointments/RedirectorMixin', () => ({
  methods: {
    goToUrlViaRedirector: jest.fn(),
  },
}));

const deepLinkUrl = 'https://appointments.stubs.local/1';

const mountComponent = appointment => mount(
  AppointmentPendingChange, {
    propsData: { item: appointment },
  },
);

describe('Appointment Pending Change Card', () => {
  let wrapper;
  let header;
  let locationDescription;
  let pendingChangeInfo;
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
    pendingChangeInfo = wrapper.find('[data-purpose="appointment-advice"]');
    appointmentDateTime = wrapper.find('[data-purpose="appointment-date-time"]');
    viewAppointmentLink = wrapper.find('[data-purpose="view-appointment-link"]');
  });

  it('will include the RedirectorMixin', () => {
    expect(AppointmentPendingChange.mixins).toContain(RedirectorMixin);
  });

  it('will display a h3 header', () => {
    expect(header.text()).toBe('Request to change or cancel appointment');
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

  it('will display appointment pending change message', () => {
    expect(pendingChangeInfo.text())
      .toBe(
        'Your request to change or permanently cancel this appointment is being reviewed. You do not need to do anything. ' +
        'If the request is not accepted, this appointment will still show as booked.');
  });

  it('will display a deep link', () => {
    expect(viewAppointmentLink.text()).toBe('View this appointment');
  });

  it('will call goToUrlViaRedirector when the view appointment link is clicked', () => {
    viewAppointmentLink.trigger('click');

    expect(RedirectorMixin.methods.goToUrlViaRedirector).toHaveBeenCalledWith(deepLinkUrl);
  });
});
