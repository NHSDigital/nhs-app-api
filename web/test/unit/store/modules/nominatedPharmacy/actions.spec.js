import actions from '@/store/modules/nominatedPharmacy/actions';
import dependency from '@/lib/pharmacy-detail/mapper';
import { NOMINATED_PHARMACY_LOADED } from '@/store/modules/nominatedPharmacy/mutation-types';

jest.mock('@/lib/pharmacy-detail/mapper');

const { load } = actions;

describe('load', () => {
  beforeEach(() => {
    dependency.mockClear();
  });

  it('will call to format the opening times', () => {
    const pharmacyResponse = {
      pharmacyDetails: {
        name: 'pharmacy1',
        openingTimes: [],
        openingTimesFormatted: undefined,
      },
    };

    const that = {
      app: {
        $http: {
          getV1PatientNominatedPharmacy: () => Promise.resolve(pharmacyResponse),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();
    return load
      .call(that, { commit })
      .then(() =>
        expect(commit).toBeCalledWith(NOMINATED_PHARMACY_LOADED, pharmacyResponse));
  });

  it('will call to map pharmacy detail', () => {
    const pharmacyResponse = {
      pharmacyDetails: {
        name: 'pharmacy1',
        openingTimes: [],
      },
    };

    const that = {
      app: {
        $http: {
          getV1PatientNominatedPharmacy: () => Promise.resolve(pharmacyResponse),
        },
      },
      dispatch: jest.fn(),
    };

    const commit = jest.fn();
    return load
      .call(that, { commit })
      .then(() =>
        expect(dependency).toHaveBeenCalledTimes(1));
  });

  it('will ignore error', () => {
    const errorStatus = {
      ignoreError: true,
    };

    const that = {
      app: {
        $http: {
          getV1PatientNominatedPharmacy: jest.fn().mockRejectedValue(),
        },
      },
      dispatch: jest.fn(),
    };
    return load
      .call(that, { })
      .then(() => {
        expect(that.app.$http.getV1PatientNominatedPharmacy).toBeCalledWith(errorStatus);
      });
  });
});
