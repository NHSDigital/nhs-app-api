/* eslint-disable import/extensions */
import actions from '../../../../../src/store/modules/repeatPrescriptionCourses/actions';
import { REPEAT_PRESCRIPTION_COURSES_LOADED } from '../../../../../src/store/modules/repeatPrescriptionCourses/mutation-types';

const { load } = actions;

describe('load', () => {
  it('will request repeat prescription courses from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientCourses: jest.fn().mockResolvedValue(),
        },
      },
    };

    return load.call(that, { commit: jest.fn() }).then(() => {
      expect(that.app.$http.getV1PatientCourses).toBeCalled();
    });
  });

  it('will call commit with the data returned from the HTTP call', () => {
    const expected = {
      data: {
        response: {
          course: 'bar',
        },
      },
    };

    const that = {
      app: {
        $http: {
          getV1PatientCourses: () => Promise.resolve(expected),
        },
      },
    };

    const commit = jest.fn();

    return load
      .call(that, { commit })
      .then(() =>
        expect(commit).toBeCalledWith(
          REPEAT_PRESCRIPTION_COURSES_LOADED,
          expected,
        ));
  });
});
