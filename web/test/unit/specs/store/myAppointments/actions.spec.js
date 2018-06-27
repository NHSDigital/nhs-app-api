import { LOADED } from '../../../../../src/store/modules/myAppointments/mutation-types';
import actions from '../../../../../src/store/modules/myAppointments/actions';

const API_HOST = 'http://unit.test';

const { load } = actions;

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
