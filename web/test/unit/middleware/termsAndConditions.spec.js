import termsAndConditions from '@/middleware/termsAndConditions';
import {
  LOGOUT_NAME,
  NOTIFICATIONS_NAME,
  TERMSANDCONDITIONS_NAME,
} from '@/router/names';
import { createConditionalRedirectRouteByName } from '@/lib/utils';
import { createStore } from '../helpers';

jest.mock('@/lib/utils');

describe('middleware/termsAndConditions', () => {
  const createConditionalRedirectRouteByNameResult = 'Return conditional route';
  let store;
  let to;
  let next;

  const callTermsAndConditions = () => termsAndConditions({ next, to, store });

  beforeAll(() => {
    createConditionalRedirectRouteByName
      .mockReturnValue(createConditionalRedirectRouteByNameResult);
  });

  beforeEach(() => {
    store = createStore({
      state: {
        termsAndConditions: {
          areAccepted: false,
          updatedConsentRequired: false,
        },
      },
    });

    to = {
      query: 'query',
      params: 'params',
    };

    next = jest.fn();
  });

  afterEach(() => {
    createConditionalRedirectRouteByName.mockClear();
  });

  describe('terms have been accepted and updated consent is not required', () => {
    beforeEach(async () => {
      store.state.termsAndConditions.areAccepted = true;
      store.state.termsAndConditions.updatedConsentRequired = false;
    });

    describe(`navigating to ${TERMSANDCONDITIONS_NAME}`, () => {
      beforeEach(async () => {
        to.name = TERMSANDCONDITIONS_NAME;
        await callTermsAndConditions();
      });

      it('will dispatch `termsAndConditions/checkAcceptance`', () => {
        expect(store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will create conditional notifications redirect route', () => {
        expect(createConditionalRedirectRouteByName).toBeCalledWith({
          name: NOTIFICATIONS_NAME,
          query: to.query,
          params: to.params,
          store,
        });
      });

      it('will call next with conditional notifications redirect route result', () => {
        expect(next).toBeCalledWith(createConditionalRedirectRouteByNameResult);
      });
    });

    describe('navigating to any other page', () => {
      beforeEach(async () => {
        to.name = 'foo';
        await callTermsAndConditions();
      });

      it('will dispatch `termsAndConditions/checkAcceptance`', () => {
        expect(store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will not create conditional redirect route', () => {
        expect(createConditionalRedirectRouteByName).not.toBeCalled();
      });

      it('will call next', () => {
        expect(next).toBeCalled();
      });
    });
  });

  describe.each([
    ['terms not accepted', false, true],
    ['updated consent required', true, true],
  ])('%s', (_, areAccepted, updatedConsentRequired) => {
    beforeEach(() => {
      store.state.termsAndConditions.areAccepted = areAccepted;
      store.state.termsAndConditions.updatedConsentRequired = updatedConsentRequired;
    });

    describe.each([
      LOGOUT_NAME,
      TERMSANDCONDITIONS_NAME,
    ])('navigating to %s', (name) => {
      beforeEach(async () => {
        to.name = name;
        await callTermsAndConditions();
      });

      it('will dispatch `termsAndConditions/checkAcceptance`', () => {
        expect(store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will not create conditional redirect route', () => {
        expect(createConditionalRedirectRouteByName).not.toBeCalled();
      });

      it('will call next', () => {
        expect(next).toBeCalled();
      });
    });

    describe('navigating to any other page', () => {
      beforeEach(async () => {
        to.name = 'foo';
        await callTermsAndConditions();
      });

      it('will dispatch `termsAndConditions/checkAcceptance`', () => {
        expect(store.dispatch).toBeCalledWith('termsAndConditions/checkAcceptance');
      });

      it('will not create conditional redirect route', () => {
        expect(createConditionalRedirectRouteByName).not.toBeCalled();
      });

      it('will call next with terms and conditions route', () => {
        expect(next).toBeCalledWith({
          name: TERMSANDCONDITIONS_NAME,
          query: to.query,
          params: to.params,
        });
      });
    });
  });
});
