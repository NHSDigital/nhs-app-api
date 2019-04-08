/* eslint-disable import/extensions */
import actions from '@/store/modules/myAppointments/actions';
import { LOADED } from '@/store/modules/myAppointments/mutation-types';

const API_HOST = 'http://unit.test';

const { load, cancel } = actions;

describe('load', () => {
  it('will request the patient appointments from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientAppointments: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };
    return load.call(that, { commit: jest.fn() }, { API_HOST }).then(() => {
      expect(that.app.$http.getV1PatientAppointments).toBeCalled();
    });
  });

  it('will call commit with the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    const that = {
      app: {
        $http: {
          getV1PatientAppointments: () => Promise.resolve(expected),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();

    return load
      .call(that, { commit }, { API_HOST })
      .then(() => expect(commit).toBeCalledWith(LOADED, expected));
  });
});

describe('cancel', () => {
  let that;
  let data;

  beforeEach(async () => {
    that = {
      app: {
        $http: {
          deleteV1PatientAppointments: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };
    data = { foo: 'bar' };
    process.client = true;
    await cancel.call(that, {}, data);
  });

  it('will delete the patients appointment from the backend', () => {
    expect(that.app.$http.deleteV1PatientAppointments)
      .toBeCalledWith({ appointmentCancelRequest: data });
  });

  it('will dispatch appointment_cancelled', () => {
    expect(that.dispatch).toBeCalledWith('analytics/satelliteTrack', 'appointment_cancelled');
  });
});
