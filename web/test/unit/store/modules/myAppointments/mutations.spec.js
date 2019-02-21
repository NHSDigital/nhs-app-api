/* eslint-disable import/extensions */
import mutations from '@/store/modules/myAppointments/mutations';

const { LOADED } = mutations;

describe('LOADED', () => {
  it('will set the past appointments on the state from the received past appointments data', () => {
    const state = {};
    const receivedData = {
      pastAppointments: ['appointment'],
    };

    LOADED(state, receivedData);

    expect(state.pastAppointments).toEqual(receivedData.pastAppointments);
  });

  it('will set the upcoming appointments on the state from the received upcoming appointments data', () => {
    const state = {};
    const receivedData = {
      upcomingAppointments: ['appointment'],
    };

    LOADED(state, receivedData);

    expect(state.upcomingAppointments).toEqual(receivedData.upcomingAppointments);
  });

  it('will order the upcoming appointments by start time ascending', () => {
    const ascending = [
      { startTime: '2018-04-21T17:11:59.084865+01:00' },
      { startTime: '2018-04-22T17:11:59.084865+01:00' },
      { startTime: '2018-04-22T17:13:59.084865+01:00' },
      { startTime: '2018-04-23T17:13:59.084865+01:00' },
    ];
    const state = {};
    const receivedData = {
      upcomingAppointments: [ascending[2], ascending[1], ascending[0], ascending[3]],
    };

    LOADED(state, receivedData);

    expect(state.upcomingAppointments).toEqual(ascending);
  });

  it('will order the past appointments by start time descending', () => {
    const descending = [
      { startTime: '2018-04-21T17:11:59.084865+01:00' },
      { startTime: '2018-04-20T17:11:59.084865+01:00' },
      { startTime: '2018-04-20T17:10:59.084865+01:00' },
      { startTime: '2018-04-20T17:10:59.084864+01:00' },
    ];
    const state = {};
    const receivedData = {
      pastAppointments: [descending[2], descending[1], descending[0], descending[3]],
    };

    LOADED(state, receivedData);

    expect(state.pastAppointments).toEqual(descending);
  });
});
