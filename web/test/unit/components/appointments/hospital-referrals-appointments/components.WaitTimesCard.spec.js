import WaitTimes from '@/components/appointments/hospital-referrals-appointments/appointments/WaitTimesCard';
import { mount } from '../../../helpers';

const mountComponent = waitTime => mount(
  WaitTimes, {
    propsData: { item: waitTime },
  },
);

describe('Wait Times Card', () => {
  let wrapper;
  let titleElement;
  let estimatedWaitToTreatmentDisplayDateElement;
  let referredDateElement;
  let providerNameElement;
  let specialityElement;

  beforeEach(() => {
    wrapper = mountComponent({
      estimatedWaitToTreatmentDisplayDate: 'February 2023',
      plannedWaitTime: '40',
      referredDate: '2022-04-18T10:00:00',
      providerName: 'Oak GP Surgery',
      speciality: 'Neurology',
    });

    titleElement = wrapper.find('h3');
    specialityElement = wrapper.find('[data-purpose="specialty"]');
    providerNameElement = wrapper.find('[data-purpose="provider-name"]');
    referredDateElement = wrapper.find('[data-purpose="referred-date"]');
    estimatedWaitToTreatmentDisplayDateElement = wrapper.find('[data-purpose="estimated-wait-date"]');
  });

  it('will display a h3 header', () => {
    expect(titleElement.text()).toBe('Waiting list');
  });

  it('will display the specialty', () => {
    expect(specialityElement.find('strong').text()).toBe('Neurology');
  });

  it('will display the provider name', () => {
    expect(providerNameElement.text()).toContain('Oak GP Surgery');
  });

  it('will display referred date', () => {
    expect(referredDateElement.text()).toBe('18 April 2022');
  });

  it('will display estimated wait to treatment display date', () => {
    expect(estimatedWaitToTreatmentDisplayDateElement.text()).toBe('February 2023');
  });
});
