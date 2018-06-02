import { mutations } from '../../../../../src/store/appointmentSlots';

const { SLOT_SELECTED, SLOTS_LOADED } = mutations;

describe('SLOT_SELECTED', () => {
  it('will set the selected slot to the slot with received ID', () => {
    const state = {
      slots: [],
    };
    SLOT_SELECTED(state, '1');
    expect(state.selectedSlotId).toEqual('1');
  });
});

describe('SLOTS_LOADED', () => {
  it('will set the appointment sessions on the state from the received slot data', () => {
    const state = {};
    const receivedData = {
      appointmentSessions: 'appointmentSessions',
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.appointmentSessions).toEqual(receivedData.appointmentSessions);
  });

  it('will set the clincians on the state from the received slot data', () => {
    const state = {};
    const receivedData = {
      clincians: 'clincians',
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.clincians).toEqual(receivedData.clincians);
  });

  it('will set the locations on the state from the received slot data', () => {
    const state = {};
    const receivedData = {
      locations: 'locations',
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.locations).toEqual(receivedData.locations);
  });

  it('will set the slots on the state from the received slot data', () => {
    const state = {};
    const receivedData = {
      slots: [{
        selected: false,
      }],
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.slots).toEqual(receivedData.slots);
  });

  it('will order the appointment slots by start time ascending', () => {
    const acending = [
      { startTime: '2018-04-21T17:11:59.084865+01:00', selected: false },
      { startTime: '2018-04-22T17:11:59.084865+01:00', selected: false },
      { startTime: '2018-04-22T17:13:59.084865+01:00', selected: false },
      { startTime: '2018-04-23T17:13:59.084865+01:00', selected: false },
    ];
    const state = {
      selected: false,
    };
    const receivedData = {
      slots: [acending[2], acending[1], acending[0], acending[3]],
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.slots).toEqual(acending);
  });

  it('will order the appointment slots by first clinician name when start times are the same', () => {
    const clinicians = [
      { id: '1', displayName: 'George' },
      { id: '2', displayName: 'Andrew' },
      { id: '3', displayName: 'Zoe' },
      { id: '4', displayName: 'Mandy' },
    ];

    const state = {};
    const receivedData = {
      slots: [
        {
          id: '3',
          startTime: '2018-04-22T17:11:59.084865+01:00',
          clinicianIds: ['4', '2'],
        },
        {
          id: '2',
          startTime: '2018-04-22T17:11:59.084865+01:00',
          clinicianIds: ['1', '4'],
        },
        {
          id: '1',
          startTime: '2018-04-21T17:11:59.084865+01:00',
          clinicianIds: ['2'],
        },
        {
          id: '4',
          startTime: '2018-04-22T17:11:59.084865+01:00',
          clinicianIds: ['3', '1'],
        },
      ],
      clinicians,
    };

    SLOTS_LOADED(state, receivedData);

    expect(state.slots.map(slot => slot.id)).toEqual(['1', '2', '3', '4']);
  });
});
