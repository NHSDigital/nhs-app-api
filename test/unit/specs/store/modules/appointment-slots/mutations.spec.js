import mutations from '@/store/modules/appointment-slots/mutations';
import { SLOTS_LOADED } from '@/store/modules/appointment-slots/mutation-types';

describe('SLOTS_LOADED', () => {
  it('will set the appointment slots on the state from the received slots', () => {
    const state = {};
    const slots = [];

    mutations[SLOTS_LOADED](state, slots);

    expect(state.appointmentSlots).toEqual(slots);
  });
});
