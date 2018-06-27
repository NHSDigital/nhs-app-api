import mutations from '../../../../../src/store/modules/myAppointments/mutations';

const { LOADED } = mutations;

describe('LOADED', () => {
  it('will set the appointment sessions on the state from the received appointments data', () => {
    const state = {};
    const receivedData = {
      appointmentSessions: 'appointmentSessions',
    };

    LOADED(state, receivedData);

    expect(state.appointmentSessions).toEqual(receivedData.appointmentSessions);
  });

  it('will set the clincians on the state from the received appointments data', () => {
    const state = {};
    const receivedData = {
      clincians: 'clincians',
    };

    LOADED(state, receivedData);

    expect(state.clincians).toEqual(receivedData.clincians);
  });

  it('will set the locations on the state from the received appointments data', () => {
    const state = {};
    const receivedData = {
      locations: 'locations',
    };

    LOADED(state, receivedData);

    expect(state.locations).toEqual(receivedData.locations);
  });

  it('will order the appointments by start time ascending', () => {
    const acending = [
      { startTime: '2018-04-21T17:11:59.084865+01:00' },
      { startTime: '2018-04-22T17:11:59.084865+01:00' },
      { startTime: '2018-04-22T17:13:59.084865+01:00' },
      { startTime: '2018-04-23T17:13:59.084865+01:00' },
    ];
    const state = {};
    const receivedData = {
      appointments: [acending[2], acending[1], acending[0], acending[3]],
    };

    LOADED(state, receivedData);

    expect(state.appointments).toEqual(acending);
  });
});
