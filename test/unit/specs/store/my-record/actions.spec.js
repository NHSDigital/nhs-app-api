/* eslint-disable */

import {
  actions,
  DEMOGRAPHICS_LOADED,
  ALLERGIES_LOADED,
} from '../../../../../src/store/myRecord';

const API_HOST = 'http://unit.test';

const { loadPatientDemographics, loadAllergiesAndAdverseReactions } = actions;

describe('loadPatientDemographics', () => {
  it('will request patients demographics information from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientMyRecordDemographics: jest.fn().mockResolvedValue(),
        },
      },
    };
    return loadPatientDemographics.call(that, { commit: jest.fn() }, { API_HOST }).then(() => {
      expect(that.app.$http.getV1PatientMyRecordDemographics).toBeCalled();
    });
  });

  it('will call commit with DEMOGRAPHICS_LOADED and the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
  };

    const that = {
      app: {
        $http: {
          getV1PatientMyRecordDemographics: () => Promise.resolve(expected),
        },
      },
    };

    const commit = jest.fn();

    return loadPatientDemographics.call(that, { commit })
      .then(() => expect(commit).toBeCalledWith(DEMOGRAPHICS_LOADED, expected));
  });
});

describe('loadAllergiesAndAdverseReactions', () => {
  it('will request patients allergies and adverse reactions information from the backend', () => {
    const that = {
      app: {
        $http: {
          getV1PatientMyRecordAllergies: jest.fn().mockResolvedValue(),
        },
      },
    };
    return loadAllergiesAndAdverseReactions.call(that, { commit: jest.fn() }, { API_HOST }).then(() => {
      expect(that.app.$http.getV1PatientMyRecordAllergies).toBeCalled();
    });
  });

  it('will call commit with ALLERGIES_LOADED and the data returned from the HTTP call', () => {
    const expected = {
      data: { foo: 'bar' },
    };

    const that = {
      app: {
        $http: {
          getV1PatientMyRecordAllergies: () => Promise.resolve(expected),
        },
      },
    };

    const commit = jest.fn();

    return loadAllergiesAndAdverseReactions.call(that, { commit })
      .then(() => expect(commit).toBeCalledWith(ALLERGIES_LOADED, expected));
  });
});
