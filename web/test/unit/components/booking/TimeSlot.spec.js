import { mount, createLocalVue } from '@vue/test-utils';
import TimeSlot from '@/components/appointments/booking/TimeSlot';
import Vuex from 'vuex';
import { APPOINTMENT_CONFIRMATIONS } from '@/lib/routes';

const $t = key => `translate_${key}`;

const app = {
  $env: {
    TERMS_CONDITIONS_URL: 'http://example.com',
    PRIVACY_POLICY_URL: 'http://example.com',
    COOKIES_POLICY_URL: 'http://example.com',
  },
};

const $store = {
  state: {
    availableAppointments: {
      bookingReasonNecessity: 'very_important_reason',
      select: jest.fn(),
      deselect: jest.fn(),
    },
    myAppointments: {
      disableCancellation: '',
    },
  },
  dispatch: jest.fn(),
  app,
};

const timeSlot = {
  startTime: 50,
  endTime: 500,
};

const createTimeSlotComponent = () => {
  const $http = jest.fn();
  const localVue = createLocalVue();
  localVue.use(Vuex);

  return mount(TimeSlot, {
    localVue,
    mocks: {
      $http,
      $store,
      $t,
      $style: {
        selected: 'mock',
        timeSlot: 'mock',
      },
      $emit: jest.fn(),
    },
    propsData: {
      timeSlot,
    },
    showTemplate: () => true,
  });
};

describe('TimeSlot', () => {
  const wrapper = createTimeSlotComponent();

  it('creates links successfully', () => {
    const link = wrapper.vm.createLink(timeSlot);
    expect(link).toBe(`${APPOINTMENT_CONFIRMATIONS.path}?nojs=%7B%22availableAppointments%22%3A%7B%22bookingReasonNecessity%22%3A%22very_important_reason%22%2C%22selectedSlot%22%3A%7B%22startTime%22%3A50%2C%22endTime%22%3A500%7D%7D%2C%22myAppointments%22%3A%7B%22disableCancellation%22%3A%22%22%7D%7D`);
  });

  it('selects an appointment successfully', () => {
    const e = { preventDefault: jest.fn() };
    wrapper.vm.select(e);

    expect(wrapper.vm.isSelected).toBe(true);
    expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/select', timeSlot);
  });

  it('deselects an appointment sucessfully', () => {
    wrapper.vm.deselect();

    expect(wrapper.vm.isSelected).toBe(false);
    expect($store.dispatch).toHaveBeenCalledWith('availableAppointments/deselect');
  });
});
