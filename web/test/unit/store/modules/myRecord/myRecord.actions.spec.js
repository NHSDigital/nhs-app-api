import actions from '@/store/modules/myRecord/actions';
import {
  ACCEPT_TERMS,
  LOADED,
  LOADED_DETAILED_TEST_RESULT,
  RESET_TERMS,
  TOGGLE_PATIENT_DETAIL,
} from '@/store/modules/myRecord/mutation-types';

const createApp = ({ record, patientDetails, testResults }) => ({
  $http: {
    getV1PatientDemographics: jest.fn().mockResolvedValue({ response: patientDetails }),
    getV1PatientMyRecord: jest.fn().mockResolvedValue({ response: record }),
    getV1PatientTestResult: jest.fn().mockResolvedValue({ response: testResults }),
  },
});

describe('my record actions', () => {
  describe('load', () => {
    const record = 'record';
    const patientDetails = 'patient';

    let app;
    let context;

    beforeEach(() => {
      app = createApp({ record, patientDetails });
      actions.app = app;
      context = {
        commit: jest.fn(),
      };
    });

    describe('acceptTerms', () => {
      beforeEach(async () => {
        await actions.acceptTerms(context);
      });

      it('will commit the accept terms mutation', () => {
        expect(context.commit).toHaveBeenCalledWith(ACCEPT_TERMS);
      });
    });

    describe('load', () => {
      beforeEach(async () => {
        await actions.load(context);
      });

      it('will request patient details', () => {
        expect(app.$http.getV1PatientDemographics).toHaveBeenCalledWith({});
      });

      it('will request my record data details', () => {
        expect(app.$http.getV1PatientMyRecord).toHaveBeenCalledWith({});
      });

      it('will commit the loaded mutation with patient data and my record data', () => {
        expect(context.commit).toHaveBeenCalledWith(LOADED, {
          record,
          patientDetails,
        });
      });
    });

    describe('loadDetailedTestResult', () => {
      const testResultId = 3456;
      const testResults = {};

      beforeEach(async () => {
        app.$http.getV1PatientTestResult.mockResolvedValue({ response: testResults });
        await actions.loadDetailedTestResult(context, testResultId);
      });

      it('will request patient test results with the supplied ID', () => {
        expect(app.$http.getV1PatientTestResult).toHaveBeenCalledWith({ testResultId });
      });

      it('will commit LOADED_DETAILED_TEST_RESULT with the received results', () => {
        expect(context.commit).toHaveBeenCalledWith(LOADED_DETAILED_TEST_RESULT, testResults);
      });
    });

    describe('resetTerms', () => {
      beforeEach(async () => {
        await actions.resetTerms(context);
      });

      it('will commit the reset terms mutation', () => {
        expect(context.commit).toHaveBeenCalledWith(RESET_TERMS);
      });
    });

    describe('togglePatientDetail', () => {
      beforeEach(() => {
        actions.togglePatientDetail(context);
      });

      it('will commit the toggle', () => {
        expect(context.commit).toHaveBeenCalledWith(TOGGLE_PATIENT_DETAIL);
      });
    });
  });
});
