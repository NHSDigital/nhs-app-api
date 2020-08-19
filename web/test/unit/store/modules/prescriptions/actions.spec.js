import actions from '@/store/modules/prescriptions/actions';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from '@/store/modules/prescriptions/mutation-types';

const { load, clear } = actions;

describe('load', () => {
  const rootState = {
    device: {
      isNativeApp: false,
    },
  };
  it('will dispatch device/unlockNavBar', async () => {
    const dispatch = jest.fn();
    const that = {
      app: {
        $http: {
          getV1PatientPrescriptions: jest.fn().mockImplementation(() => Promise.resolve()),
        },
      },
      dispatch,
    };

    await load.call(that, { commit: jest.fn(), rootState });

    expect(dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  it('will request prescriptions and courses from the backend', async () => {
    const that = {
      app: {
        $http: {
          getV1PatientPrescriptions: jest.fn().mockImplementation(() => Promise.resolve()),
        },
      },
      dispatch: jest.fn(),
    };

    return load
      .call(that, { commit: jest.fn(), rootState })
      .then(() => expect(that.app.$http.getV1PatientPrescriptions).toBeCalled());
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
      .call(that, { commit, rootState })
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
