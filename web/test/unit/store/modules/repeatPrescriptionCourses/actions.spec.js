import actions from '@/store/modules/repeatPrescriptionCourses/actions';
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
  REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO,
} from '@/store/modules/repeatPrescriptionCourses/mutation-types';

const { load, orderRepeatPrescription, updateAdditionalInfo } = actions;

describe('load', () => {
  it('will request repeat prescription courses from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientCourses: jest.fn().mockResolvedValue(),
        },
      },
      dispatch: jest.fn(),
    };

    return load.call(that, { commit: jest.fn() }).then(() => {
      expect(that.app.$http.getV1PatientCourses).toBeCalled();
    });
  });

  it('will dispatch device/unlockNavBar', async () => {
    const dispatch = jest.fn();
    const that = {
      app: {
        $http: {
          getV1PatientCourses: jest.fn().mockImplementation(() => Promise.resolve()),
        },
      },
      dispatch,
    };

    await load.call(that, { commit: jest.fn() });

    expect(dispatch).toHaveBeenCalledWith('device/unlockNavBar');
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
      dispatch: jest.fn(),
    };

    const commit = jest.fn();

    return load
      .call(that, { commit })
      .then(() => {
        expect(commit).toBeCalledWith(REPEAT_PRESCRIPTION_COURSES_LOADED, expected);
      });
  });
});

describe('orderRepeatPrescription', () => {
  it('will request to order repeat prescriptions from the backend and redirect to prescriptions', () => {
    const that = {
      app: {
        $http: {
          postV1PatientPrescriptions: jest.fn().mockResolvedValue(),
        },
        router: {
          push: jest.fn().mockResolvedValue(),
        },
      },
    };

    const commit = jest.fn();

    const repeatPrescriptionOrder = {
      CourseIds: [123, 456],
      SpecialRequest: null,
    };

    const expectedRequest = {
      repeatPrescriptionRequest: { repeatPrescriptionOrder },
    };

    return orderRepeatPrescription
      .call(that, { commit }, { repeatPrescriptionOrder })
      .then(() => {
        expect(that.app.$http.postV1PatientPrescriptions).toBeCalledWith(expectedRequest);
      });
  });
});

describe('orderRepeatPrescription', () => {
  it('will request to order repeat prescriptions from the backend and redirect to prescriptions', () => {
    const that = {
      app: {
        $http: {
          postV1PatientPrescriptions: jest.fn().mockResolvedValue(),
        },
        router: {
          push: jest.fn().mockResolvedValue(),
        },
      },
    };

    const commit = jest.fn();

    const repeatPrescriptionOrder = {
      CourseIds: [123, 456],
      SpecialRequest: 'As soon as possible',
    };

    const expectedRequest = {
      repeatPrescriptionRequest: { repeatPrescriptionOrder },
    };

    return orderRepeatPrescription
      .call(that, { commit }, { repeatPrescriptionOrder })
      .then(() => {
        expect(that.app.$http.postV1PatientPrescriptions).toBeCalledWith(expectedRequest);
      });
  });
});

describe('updateAdditionalInfo', () => {
  it('will store additional info from the repeat courses page on the state', () => {
    const commit = jest.fn();

    const additionalInfo = {
      specialRequest: 'Please call me when prescription is ready',
    };

    updateAdditionalInfo({ commit }, additionalInfo);
    expect(commit).toBeCalledWith(REPEAT_PRESCRIPTION_UPDATE_ADDITIONAL_INFO, additionalInfo);
  });
});
