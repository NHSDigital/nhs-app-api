import Vue from 'vue';
import { load, clear } from '@/store/modules/prescriptions/actions';
import { PRESCRIPTIONS_LOADED, PRESCRIPTIONS_CLEAR } from '@/store/modules/prescriptions/mutation-types';

const API_HOST = 'http://unit.test';

let original$http;

beforeEach(() => {
  original$http = Vue.$http;
});

afterEach(() => {
  Vue.$http = original$http;
});

describe('load', () => {
  it('will request prescriptions and courses from the backend', () => {
    Vue.$http = {
      getV1PatientPrescriptions: jest.fn().mockResolvedValue(),
    };

    return load({ commit: jest.fn() }, { API_HOST })
      .then(() => {
        expect(Vue.$http.getV1PatientPrescriptions).toBeCalled();
      });
  });

  it('will call commit with PRESCRIPTIONS_LOADED and the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    Vue.$http = {
      getV1PatientPrescriptions: () => Promise.resolve(expected),
    };

    const commit = jest.fn();

    return load({ commit }, { API_HOST })
      .then(() => expect(commit).toBeCalledWith(PRESCRIPTIONS_LOADED, expected));
  });
});

describe('clear', () => {
  it('will call commit with PRESCRIPTIONS_CLEAR', () => {
    const commit = jest.fn();
    clear({ commit });
    expect(commit).toBeCalledWith(PRESCRIPTIONS_CLEAR);
  });
});
