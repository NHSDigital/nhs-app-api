import each from 'jest-each';
import gpMedicalRecordAcceptance from '@/middleware/gpMedicalRecordAcceptance';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore } from '../helpers';
import {
  ACUTE_MEDICINES,
  ALLERGIESANDREACTIONS,
  CONSULTATIONS,
  CURRENT_MEDICINES,
  DIAGNOSIS_V2,
  DISCONTINUED_MEDICINES,
  DOCUMENT,
  DOCUMENTS,
  DOCUMENT_DETAIL,
  ENCOUNTERS,
  EVENTS,
  EXAMINATIONS_V2,
  GP_MEDICAL_RECORD,
  HEALTH_CONDITIONS,
  IMMUNISATIONS,
  MEDICAL_HISTORY,
  MEDICINES,
  PROCEDURES_V2,
  RECALLS,
  REFERRALS,
  TESTRESULTID,
  TESTRESULTS,
  TESTRESULTSDETAIL,
} from '@/lib/routes';

jest.mock('@/lib/sessionStorage');
const createState = () => ({
  GP_MEDICAL_RECORD: initialState(),
});

const createApp = ({ redirect, route, store }) => ({
  redirect,
  route,
  store,
});

describe('my-record acceptance middleware', () => {
  let app;
  let redirect;

  function checkForRedirect(name, path, hasAgreedToTerms, redirectExpected) {
    redirect = jest.fn();
    const store = createStore({ state: createState() });
    store.state.myRecord = { hasAcceptedTerms: hasAgreedToTerms };
    app = createApp({
      redirect,
      route: {
        name,
        path,
      },
      store,
    });
    gpMedicalRecordAcceptance(app);
    if (redirectExpected) {
      expect(redirect).toHaveBeenCalledWith(GP_MEDICAL_RECORD.path);
    } else {
      expect(redirect).not.toHaveBeenCalledWith(GP_MEDICAL_RECORD.path);
    }
  }

  describe('details pages', () => {
    const detailsPages = [
      ACUTE_MEDICINES,
      ALLERGIESANDREACTIONS,
      CONSULTATIONS,
      CURRENT_MEDICINES,
      DIAGNOSIS_V2,
      DISCONTINUED_MEDICINES,
      DOCUMENT,
      DOCUMENTS,
      DOCUMENT_DETAIL,
      ENCOUNTERS,
      EVENTS,
      EXAMINATIONS_V2,
      HEALTH_CONDITIONS,
      IMMUNISATIONS,
      MEDICAL_HISTORY,
      MEDICINES,
      PROCEDURES_V2,
      RECALLS,
      REFERRALS,
      TESTRESULTID,
      TESTRESULTS,
      TESTRESULTSDETAIL];

    describe('accepted warning and agreed to terms', () => {
      each(detailsPages)
        .it('will not redirect to main record page', (path) => {
          checkForRedirect(
            path.name,
            path.path,
            true,
            false,
          );
        });
    });

    describe('not agreed to terms', () => {
      each(detailsPages)
        .it('will redirect to main record page', (path) => {
          checkForRedirect(
            path.name,
            path.path,
            false,
            true,
          );
        });
    });
  });
});
