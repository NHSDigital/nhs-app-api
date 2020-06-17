import TimeSlot from '@/components/appointments/booking/TimeSlot';
import { createStore, mount } from '../../helpers';

const timeSlot = {
  startTime: 50,
  endTime: 500,
};

const createTimeSlotComponent = $store => mount(TimeSlot, {
  $store,
  $style: {
    selected: 'mock',
    timeSlot: 'mock',
  },
  propsData: {
    timeSlot,
  },
});

describe('TimeSlot', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
      state: {
        availableAppointments: {
          bookingReasonNecessity: 'very_important_reason',
          select: jest.fn(),
          deselect: jest.fn(),
        },
        myAppointments: {
          disableCancellation: '',
        },
        device: {
          source: 'web',
        },
      },
    });

    wrapper = createTimeSlotComponent($store);
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
