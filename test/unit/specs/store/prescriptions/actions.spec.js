/* eslint-disable import/extensions */
import actions from '../../../../../src/store/modules/prescriptions/actions';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from '../../../../../src/store/modules/prescriptions/mutation-types';

const { load, clear } = actions;

describe('load', () => {
  it('will request prescriptions and courses from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientPrescriptions: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };
    return load.call(that, { commit: jest.fn() }).then(() => {
      expect(that.app.$http.getV1PatientPrescriptions).toBeCalled();
    });
  });

  it('will call commit with PRESCRIPTIONS_LOADED and the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    const that = {
      app: {
        $http: {
          getV1PatientPrescriptions: () => Promise.resolve(expected),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();

    return load
      .call(that, { commit })
      .then(() =>
        expect(commit).toBeCalledWith(PRESCRIPTIONS_LOADED, expected));
  });
});

describe('clear', () => {
  it('will call commit with PRESCRIPTIONS_CLEAR', () => {
    const commit = jest.fn();
    clear({ commit });
    expect(commit).toBeCalledWith(PRESCRIPTIONS_CLEAR);
  });
});
