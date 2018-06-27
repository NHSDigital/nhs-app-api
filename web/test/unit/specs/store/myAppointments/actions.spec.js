import { LOADED, CANCEL_SUCCESS } from '../../../../../src/store/modules/myAppointments/mutation-types';
import actions from '../../../../../src/store/modules/myAppointments/actions';

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
  it('will delete the patients appointment from the backend', () => {
    const that = {
      app: {
        $http: {
          deleteV1PatientAppointments: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };

    cancel.call(that, { commit: jest.fn() }, { API_HOST });
    expect(that.app.$http.deleteV1PatientAppointments).toBeCalled();
  });

  it('will call commit to mark cancellation success', () => {
    const expected = {};

    const that = {
      app: {
        $http: {
          deleteV1PatientAppointments: () => Promise.resolve(expected),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();
    const data = { foo: 'bar' };

    cancel.call(that, ({ commit }, data)).then(() => {
      expect(commit).toBeCalledWith(CANCEL_SUCCESS);
    });
  });
});
