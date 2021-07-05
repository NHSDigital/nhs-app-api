import actions from '@/store/modules/termsAndConditions/actions';
import { SET_ACCEPTANCE, SET_UPDATED_CONSENT_REQUIRED } from '@/store/modules/termsAndConditions/mutation-types';

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
      state: {
        device: {
          source: '',
        },
      },
      $http: {
        getV1PatientTermsAndConditionsConsent: jest.fn(),
        postV1PatientTermsAndConditionsConsent: jest.fn(() => Promise.resolve()),
      },
      app: {
        router: [],
      },
    };
    actions.app = app;
    actions.$cookies = {
      get: jest.fn()
        .mockImplementation('nhso.session').mockReturnValue(cookieValue),
      set: jest.fn(),
    };
  });

  describe('acceptTerms', () => {
    let consentRequest;

    beforeEach(() => {
      consentRequest = {
        analyticsCookieAccepted: true,
        updatingConsent: undefined,
      };
    });

    it('will call postV1PatientTermsAndConditionsConsent with the received consent request', async () => {
      await actions.acceptTerms({ commit }, consentRequest);
      expect(app.$http.postV1PatientTermsAndConditionsConsent)
        .toBeCalledWith({ consentRequest: { consentGiven: true, ...consentRequest } });
    });

    describe('update terms and conditions post is successful', () => {
      beforeEach(async () => {
        app
          .$http
          .postV1PatientTermsAndConditionsConsent
          .mockResolvedValue(() => '');

        app.$http.getV1PatientTermsAndConditionsConsent = jest.fn(() => Promise.resolve({
          response: { consentGiven: true, analyticsCookieAccepted: true },
        }));

        await actions.acceptTerms({ commit }, { updatingConsent: true });
      });

      it('will call getV1PatientTermsAndConditionsConsent if update consent is set to true', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalled();
      });

      it('will commit SET_ACCEPTANCE', () => {
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          { areAccepted: true, analyticsCookieAccepted: true },
        );
      });

      it('will commit SET_UPDATED_CONSENT_REQUIRED', () => {
        expect(commit).toBeCalledWith(SET_UPDATED_CONSENT_REQUIRED, false);
      });
    });

    describe('post is successful', () => {
      beforeEach(async () => {
        app.$http.postV1PatientTermsAndConditionsConsent.mockResolvedValue(() => '');

        await actions.acceptTerms({ commit }, consentRequest);
      });

      it('will commit SET_ACCEPTANCE', () => {
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          { areAccepted: true, analyticsCookieAccepted: true },
        );
      });
    });

    describe('post fails', () => {
      let threwException = false;

      beforeEach(async () => {
        app.$http
          .postV1PatientTermsAndConditionsConsent
          .mockImplementation(() => Promise.reject());

        try {
          await actions.acceptTerms({ commit }, consentRequest);
        } catch {
          threwException = true;
        }
      });

      it('will throw exception', () => {
        expect(threwException).toBe(true);
      });

      it('will not commit', () => {
        expect(commit).not.toBeCalled();
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

    describe('already accepted via cookies', () => {
      beforeEach(async () => {
        actions.$cookies.get = jest.fn()
          .mockImplementation('nhso.session')
          .mockReturnValue(cookieValue)

          .mockImplementation('nhso.terms')
          .mockReturnValue({
            analyticsCookieAccepted: false,
            areAccepted: true,
            updatedConsentRequired: false });

        state = { areAccepted: false, analyticsCookieAccepted: false };
        await actions.checkAcceptance({ commit, state });
      });

      it('will not call getV1PatientTermsAndConditionsConsent ', () => {
        expect(app.$http.getV1PatientTermsAndConditionsConsent).not.toBeCalledWith({});
        expect(commit.mock.calls[0]).toEqual([
          SET_ACCEPTANCE, { areAccepted: true, analyticsCookieAccepted: false },
        ]);
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
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalledWith();
      });

      it('will commit the result from getV1PatientTermsAndConditionsConsent', () => {
        expect(commit).toBeCalledWith(
          SET_ACCEPTANCE,
          {
            analyticsCookieAccepted: true,
            areAccepted: true,
          },
        );
      });
    });

    describe('with no data returned', () => {
      it('will call getV1PatientTermsAndConditionsConsent ', async () => {
        state = { areAccepted: false, analyticsCookieAccepted: '' };
        app.$http.getV1PatientTermsAndConditionsConsent = jest.fn(() => Promise.resolve());

        try {
          await actions.checkAcceptance({ commit, state });
          fail();
        } catch (e) {
          expect(e.message).toEqual('No T&C response');
        }
      });
    });
  });
});
