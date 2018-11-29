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
      store: {
        state: {
          device: {
            source: '',
          },
        },
      },
    };
    actions.app = app;
  });

  describe('acceptTerms', () => {
    let consentTerms;

    beforeEach(() => {
      consentTerms = {
        consentRequest: {
          ConsentGiven: true,
          AnalyticsCookieAccepted: true,
        },
      };
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
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          { areAccepted: true, analyticsCookieAccepted: true },
        );
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
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          { areAccepted: false, analyticsCookieAccepted: false },
        );
      });
    });
  });

  describe('checkAcceptance', () => {
    describe('already accepted', () => {
      beforeEach(() => {
        state = { areAccepted: true, analyticsCookieAccepted: true };
        actions.checkAcceptance({ commit, state });
      });

      it('will not call getV1PatientTermsAndConditionsConsent ', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).not.toBeCalledWith({});
      });
    });

    describe('not already accepted', () => {
      beforeEach(async () => {
        state = { areAccepted: false, analyticsCookieAccepted: '' };
        app.$http.getV1PatientTermsAndConditionsConsent = jest.fn(() => Promise.resolve({
          response: { consentGiven: true, analyticsCookieAccepted: true },
        }));

        await actions.checkAcceptance({ commit, state });
      });

      it('will call getV1PatientTermsAndConditionsConsent ', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalledWith({});
      });

      it('will commit the result from getV1PatientTermsAndConditionsConsent', () => {
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          { areAccepted: true, analyticsCookieAccepted: true },
        );
      });
    });
  });
});
