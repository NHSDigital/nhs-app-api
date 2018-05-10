import Vue from 'vue';
import { load } from '@/store/modules/repeat-prescription-courses/actions';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from '@/store/modules/repeat-prescription-courses/mutation-types';

const API_HOST = 'http://unit.test';

let original$http;

beforeEach(() => {
  original$http = Vue.$http;
});

afterEach(() => {
  Vue.$http = original$http;
});

describe('load', () => {
  it('will request repeat prescription courses from the backend', () => {
    Vue.$http = {
      getV1PatientCourses: jest.fn().mockResolvedValue(),
    };

    return load({ commit: jest.fn() }, { API_HOST })
      .then(() => {
        expect(Vue.$http.getV1PatientCourses).toBeCalled();
      });
  });

  it('will call commit with the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    Vue.$http = {
      getV1PatientCourses: () => Promise.resolve(expected),
    };

    const commit = jest.fn();

    return load({ commit }, { API_HOST })
      .then(() => expect(commit).toBeCalledWith(REPEAT_PRESCRIPTION_COURSES_LOADED, expected));
  });
});
