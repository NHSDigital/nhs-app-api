import session from '@/middleware/session';

describe('session middleware', () => {
  let store;
  let dispatch;
  const next = jest.fn();

  const createStore = ({ isLoggedIn = true, hasLoaded = false } = {}) => {
    store = {
      state: {
        session: {
          hasLoaded,
        },
      },
      getters: {
        'session/isLoggedIn': () => (isLoggedIn),
      },
      dispatch,
    };
  };

  beforeEach(() => {
    dispatch = jest.fn().mockImplementation('session/getSession').mockResolvedValue(Promise.resolve());
  });

  describe('not logged in', () => {
    beforeEach(() => {
      createStore({ isLoggedIn: false });
    });

    it('will not dispatch get session', async () => {
      await session({ next, store });
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('has loaded session already', () => {
    beforeEach(() => {
      createStore({ hasLoaded: true });
    });

    it('will not dispatch get session', async () => {
      await session({ next, store });
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });

  describe('is logged in and has not loaded session', () => {
    beforeEach(() => {
      createStore();
    });

    it('will dispatch get session', async () => {
      await session({ next, store });
      expect(dispatch).toHaveBeenCalledWith('session/getSession');
      expect(next).not.toBeCalledWith(expect.anything);
      expect(next).toBeCalled();
    });
  });
});
