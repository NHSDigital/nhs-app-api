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
    let consentTerms;

    beforeEach(() => {
      consentTerms = {
        consentRequest: {
          ConsentGiven: true,
          AnalyticsCookieAccepted: true,
        },
      };
    });

    it('will call postV1PatientTermsAndConditionsConsent with the received consent terms ', async () => {
      await actions.acceptTerms({ commit }, consentTerms);
      expect(app.$http.postV1PatientTermsAndConditionsConsent).toBeCalledWith(consentTerms);
    });

    describe('update terms and conditions post is successful', () => {
      beforeEach(async () => {
        await app
          .$http
          .postV1PatientTermsAndConditionsConsent
          .mockResolvedValue(() => '');
        app.$http.getV1PatientTermsAndConditionsConsent = jest.fn(() => Promise.resolve({
          response: { consentGiven: true, analyticsCookieAccepted: true },
        }));
        consentTerms = {
          consentRequest: {
            ConsentGiven: true,
            AnalyticsCookieAccepted: true,
            UpdatingConsent: true,
          },
        };
      });

      it('will call postV1PatientTermsAndConditionsConsent if update consent is set to true', async () => {
        actions.acceptTerms({ commit }, consentTerms);
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalled();
      });
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
        expect(app.$http.getV1PatientTermsAndConditionsConsent).toBeCalledWith({});
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
