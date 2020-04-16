import upliftRedirect from '@/middleware/upliftRedirect';
import { findByName } from '@/lib/routes';

jest.mock('@/lib/routes');

describe('uplift redirect', () => {
  let redirect;
  let store;

  const callsUpliftRedirect = () => upliftRedirect({ redirect, route: { name: 'foo' }, store });

  beforeEach(() => {
    redirect = jest.fn();
    store = {
      getters: jest.fn(),
    };
  });

  describe('route detail does not exists', () => {
    beforeEach(() => {
      findByName.mockReturnValue(undefined);
      callsUpliftRedirect();
    });

    it('will not call `session/shouldUplift`', () => {
      expect(store.getters).not.toBeCalled();
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });

  describe.each([
    [undefined, undefined],
    ['P9', undefined],
    [undefined, '/path'],
  ])('missing route detail proof level (%s) and uplift path (%s)', (proofLevel, upliftPath) => {
    beforeEach(() => {
      findByName.mockReturnValue({ proofLevel, upliftPath });
      callsUpliftRedirect();
    });

    it('will not call `session/shouldUplift`', () => {
      expect(store.getters).not.toBeCalled();
    });

    it('will not redirect', () => {
      expect(redirect).not.toBeCalled();
    });
  });

  describe('route details with proof level and uplift path', () => {
    const proofLevel = 'foo-proof-level';
    const upliftPath = '/path';

    beforeEach(() => {
      findByName.mockReturnValue({ proofLevel, upliftPath });
    });

    describe('when should uplift is true', () => {
      beforeEach(() => {
        store.getters['session/shouldUplift'] = jest.fn().mockImplementation(() => true);
        callsUpliftRedirect();
      });

      it('will call `session/shouldUplift` passing in the proofLevel', () => {
        expect(store.getters['session/shouldUplift']).toBeCalledWith(proofLevel);
      });

      it('will redirect to `upliftPath`', () => {
        expect(redirect).toBeCalledWith('302', upliftPath);
      });
    });

    describe('when should uplift is false', () => {
      beforeEach(() => {
        store.getters['session/shouldUplift'] = jest.fn().mockImplementation(() => false);
        callsUpliftRedirect();
      });

      it('will call `session/shouldUplift` passing in the proofLevel', () => {
        expect(store.getters['session/shouldUplift']).toBeCalledWith(proofLevel);
      });

      it('will not redirect', () => {
        expect(redirect).not.toBeCalled();
      });
    });
  });
});
