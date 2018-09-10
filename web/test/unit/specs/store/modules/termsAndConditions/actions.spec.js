import actions from '@/store/modules/termsAndConditions/actions';
import { SET_ACCEPTANCE } from '@/store/modules/termsAndConditions/mutation-types';

describe('termsAndConditions/actions', () => {
  let app;
  let commit;
  let cookieValue;
  let state;

  beforeEach(() => {
    state = { areAccepted: false };
    commit = jest.fn();
    cookieValue = {};
    app = {
      $cookies: {
        get: jest.fn().mockImplementation('nhso.session').mockReturnValue(cookieValue),
        set: jest.fn(),
      },
      $http: {
        getV1PatientTermsAndConditionsConsent: jest.fn(),
        postV1PatientTermsAndConditionsConsent: jest.fn(() => Promise.resolve()),
      },
      router: [],
    };
    actions.app = app;
  });

  describe('acceptTerms', () => {
    let consentTerms;

    beforeEach(() => {
      consentTerms = { consentTerms: 'boo' };
    });

    it('will call postV1PatientTermsAndConditionsConsent with the received consent terms ', () => {
      actions.acceptTerms({ commit }, consentTerms);
      expect(app.$http.postV1PatientTermsAndConditionsConsent).toBeCalledWith(consentTerms);
    });

    describe('post is successful', () => {
      beforeEach(async () => {
        await app
          .$http
          .postV1PatientTermsAndConditionsConsent
          .mockResolvedValue(() => '');

        await actions.acceptTerms({ commit }, consentTerms);
      });

      it('will commit SET_ACCEPTANCE as true when the post request completes successfully', async () => {
        expect(commit).toBeCalledWith(SET_ACCEPTANCE, true);
      });

      it('will set `termsAccepted` in the `nhso.session` cookie', () => {
        expect(app.$cookies.set).toBeCalledWith('nhso.session', { termsAccepted: true });
      });

      it('will push `/` to the router when the post request completes successfully', async () => {
        expect(app.router).toContain('/');
      });
    });

    describe('post fails', () => {
      beforeEach(async () => {
        await app
          .$http
          .postV1PatientTermsAndConditionsConsent
          .mockImplementation(() => Promise.reject());

        await actions.acceptTerms({ commit }, consentTerms);
      });

      it('will commit SET_ACCEPTANCE as false when the post request fails', async () => {
        expect(commit).toBeCalledWith(SET_ACCEPTANCE, false);
      });

      it('will not push `/` to the router when the post request fails', async () => {
        expect(app.router.length).toEqual(0);
      });
    });
  });

  describe('checkAcceptance', () => {
    describe('already accepted', () => {
      beforeEach(() => {
        state = { areAccepted: true };
        actions.checkAcceptance({ commit, state });
      });

      it('will not call getV1PatientTermsAndConditionsConsent ', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).not.toBeCalledWith({});
      });
    });

    describe('not already accepted', () => {
      beforeEach(async () => {
        state = { areAccepted: false };
        app.$http.getV1PatientTermsAndConditionsConsent = jest.fn(() => Promise.resolve({
          response: { consentGiven: true },
        }));

        await actions.checkAcceptance({ commit, state });
      });

      it('will call getV1PatientTermsAndConditionsConsent ', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalledWith({});
      });

      it('will commit the result from getV1PatientTermsAndConditionsConsent', () => {
        expect(commit).toBeCalledWith(SET_ACCEPTANCE, true);
      });
    });
  });
});
