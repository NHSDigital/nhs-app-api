import SlotList from '@/components/appointments/booking/SlotList';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../../../helpers';

function makeProps(slots = []) {
  const props = {
    availableSlots: [
      [
        '2020-09-14', slots,
      ],
    ],
  };
  return props;
}

const createSlotListComponent = propsData => mount(SlotList, {
  propsData,
  $store: createStore({
    state: {
      device: {
        source: 'web',
      },
    },
  }),
  mountOpts: {
    i18n,
  },
});

describe('SlotList.vue', () => {
  it('will not show any slot information when no slots are available', () => {
    const component = createSlotListComponent(makeProps());
    const apptCount = component.find('#appointmentCount');

    expect(apptCount.exists()).toBe(false);
  });

  it('will use singlular slot information when only one slot is available', () => {
    const slots = [{ ref: 'slot_123' }];
    const component = createSlotListComponent(makeProps(slots));
    const apptCount = component.find('#appointmentCount');

    expect(apptCount.exists()).toBe(true);
    expect(apptCount.text()).toContain('1 available appointment');
  });

  it('will use plural slot information when more than one slot is available', () => {
    const slots = [{ ref: 'slot_123' }, { ref: 'slot_124' }];
    const component = createSlotListComponent(makeProps(slots));
    const apptCount = component.find('#appointmentCount');

    expect(apptCount.exists()).toBe(true);
    expect(apptCount.text()).toContain('2 available appointments');
  });
});
