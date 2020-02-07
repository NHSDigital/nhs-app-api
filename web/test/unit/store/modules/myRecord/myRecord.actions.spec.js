import actions from '@/store/modules/myRecord/actions';
import {
  ACCEPT_TERMS,
  LOADED,
  LOADED_DETAILED_TEST_RESULT,
  SET_RELOAD,
  TOGGLE_PATIENT_DETAIL,
} from '@/store/modules/myRecord/mutation-types';

const createApp = ({ record, patientDetails, data }) => ({
  $http: {
    getV1PatientDemographics: jest.fn().mockResolvedValue({ response: patientDetails }),
    getV1PatientMyRecord: jest.fn().mockResolvedValue({ response: record }),
    getV1PatientTestResult: jest.fn().mockResolvedValue({ response: data }),
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
      actions.dispatch = jest.fn();

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
      let load;
      beforeEach(async () => {
        load = async () => actions.load(context);
      });

      it('will request patient details', async () => {
        await load();
        expect(app.$http.getV1PatientDemographics).toHaveBeenCalledWith({});
      });

      it('will request my record data details', async () => {
        await load();
        expect(app.$http.getV1PatientMyRecord).toHaveBeenCalledWith({});
      });

      it('will commit the loaded mutation with patient data and my record data', async () => {
        await load();
        expect(context.commit).toHaveBeenCalledWith(LOADED, {
          record,
          patientDetails,
        });
      });

      it('will catch an exeption in the demographics request', async () => {
        const error = new Error();
        app.$http.getV1PatientDemographics = jest.fn().mockImplementation(() => {
          throw error;
        });

        await expect(load).not.toThrow();
        expect(actions.dispatch).toHaveBeenCalledWith('errors/addApiError', error);
      });

      it('will catch an exeption in the patient record request', async () => {
        const error = new Error();
        app.$http.getV1PatientMyRecord = jest.fn().mockImplementation(() => {
          throw error;
        });

        await load();
        await expect(load).not.toThrow();
        expect(actions.dispatch).toHaveBeenCalledWith('errors/addApiError', error);
      });
    });

    describe('loadDetailedTestResult', () => {
      const testResultId = 3456;
      const data = {};

      beforeEach(async () => {
        app.$http.getV1PatientTestResult.mockResolvedValue({ response: data });
        await actions.loadDetailedTestResult(context, testResultId);
      });

      it('will request patient test results with the supplied ID', () => {
        expect(app.$http.getV1PatientTestResult).toHaveBeenCalledWith({ testResultId });
      });

      it('will commit LOADED_DETAILED_TEST_RESULT with the received results', () => {
        expect(context.commit).toHaveBeenCalledWith(LOADED_DETAILED_TEST_RESULT, { data });
      });
    });

    describe('reload', () => {
      const flag = false;
      beforeEach(() => {
        actions.reload(context, flag);
      });

      it('will commit SET_RELOAD with false', () => {
        expect(context.commit).toHaveBeenCalledWith(SET_RELOAD, flag);
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
