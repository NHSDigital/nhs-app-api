import upliftRedirect from '@/middleware/upliftRedirect';
import * as dependency from '@/lib/utils';
import * as names from '@/router/names';


describe('uplift next', () => {
  let next;
  let store;
  dependency.createRoutePathObject = jest.fn(x => ({ path: x.path }));
  names.isNhsAppRouteName = jest.fn(true);

  const callsUpliftRedirect = ({ proofLevel, upliftRoute }) => {
    const to = {
      name: 'foo',
      meta: {
        proofLevel,
        upliftRoute,
      },
    };
    upliftRedirect({ next, to, store });
  };

  beforeEach(() => {
    next = jest.fn();
    store = {
      getters: jest.fn(),
    };
  });

  describe('route detail does not exists', () => {
    beforeEach(() => {
      callsUpliftRedirect({});
    });

    it('will not call `session/shouldUplift`', () => {
      expect(store.getters).not.toBeCalled();
    });

    it('will not next', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe.each([
    [undefined, undefined, undefined],
    ['P9', undefined, undefined],
    [undefined, 'name', '/path'],
  ])('missing route detail proof level (%s) and uplift name (%s) path (%s)', (proofLevel, name, path) => {
    beforeEach(() => {
      let upliftRoute;
      if (name || path) {
        upliftRoute = { name, path };
      }
      callsUpliftRedirect({ proofLevel, upliftRoute });
    });

    it('will not call `session/shouldUplift`', () => {
      expect(store.getters).not.toBeCalled();
    });

    it('will not next', () => {
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('route details with proof level, name and uplift path', () => {
    const proofLevel = 'foo-proof-level';
    const path = '/path';
    const name = 'name';

    describe('when should uplift is true', () => {
      beforeEach(() => {
        store.getters['session/shouldUplift'] = jest.fn().mockImplementation(() => true);
        callsUpliftRedirect({ proofLevel, upliftRoute: { name, path } });
      });

      it('will call `session/shouldUplift` passing in the proofLevel', () => {
        expect(store.getters['session/shouldUplift']).toBeCalledWith(proofLevel);
      });

      it('will next to `upliftRoute.path`', () => {
        expect(next).toBeCalledWith({ path });
      });
    });

    describe('when should uplift is false', () => {
      beforeEach(() => {
        store.getters['session/shouldUplift'] = jest.fn().mockImplementation(() => false);
        callsUpliftRedirect({ proofLevel, upliftRoute: { name, path } });
      });

      it('will call `session/shouldUplift` passing in the proofLevel', () => {
        expect(store.getters['session/shouldUplift']).toBeCalledWith(proofLevel);
      });

      it('will not next', () => {
        expect(next).not.toBeCalledWith(expect.anything);
        expect(next).toBeCalled();
      });
    });
  });
});
