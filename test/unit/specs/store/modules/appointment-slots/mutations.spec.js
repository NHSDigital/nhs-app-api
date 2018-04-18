import mutations from '@/store/modules/appointment-slots/mutations';
import { SLOTS_LOADED } from '@/store/modules/appointment-slots/mutation-types';

describe('SLOTS_LOADED', () => {
  it('will set the appointment sessions on the state from the received slot data', () => {
    const state = {};
    const slots = {
      appointmentSessions: 'appointmentSessions',
    };

    mutations[SLOTS_LOADED](state, slots);

    expect(state.appointmentSessions).toEqual(slots.appointmentSessions);
  });

  it('will set the clincians on the state from the received slot data', () => {
    const state = {};
    const slots = {
      clincians: 'clincians',
    };

    mutations[SLOTS_LOADED](state, slots);

    expect(state.clincians).toEqual(slots.clincians);
  });

  it('will set the locations on the state from the received slot data', () => {
    const state = {};
    const slots = {
      locations: 'locations',
    };

    mutations[SLOTS_LOADED](state, slots);

    expect(state.locations).toEqual(slots.locations);
  });

  it('will set the slots on the state from the received slot data', () => {
    const state = {};
    const slots = {
      slots: 'slots',
    };

    mutations[SLOTS_LOADED](state, slots);

    expect(state.slots).toEqual(slots.slots);
  });
});
