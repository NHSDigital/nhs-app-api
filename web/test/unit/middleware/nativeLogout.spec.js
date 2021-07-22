import nativeLogout from '@/middleware/nativeLogout';
import NativeApp from '@/services/native-app';

jest.mock('@/services/native-app');

describe('middleware/nativeLogout', () => {
  let router;
  let store;

  const next = jest.fn();

  NativeApp.sessionExpired = jest.fn();
  NativeApp.logout = jest.fn();

  const callNativeLogout = async ({ sessionShowExpiry, queryShowExpiry }) => {
    store = {
      state: {
        session: {
          showExpiryMessage: sessionShowExpiry,
        },
      },
    };
    router = {
      currentRoute: {
        query: {
          showExpiryMessage: queryShowExpiry,
        },
      },
    };

    await nativeLogout({ router, store, next });
  };

  afterEach(() => {
    NativeApp.supportsSessionExpired.mockClear();
    NativeApp.sessionExpired.mockClear();
    NativeApp.supportsLogout.mockClear();
    NativeApp.logout.mockClear();
  });

  describe.each([
    ['showExpiryMessage is set in session', true, false],
    ['showExpiryMessage is set in query', false, true],
    ['showExpiryMessage is set in session and query', true, true],
  ])('NativeApp methods are supported and %s', (_, sessionShowExpiry, queryShowExpiry) => {
    beforeEach(async () => {
      NativeApp.supportsSessionExpired.mockReturnValue(true);
      NativeApp.supportsLogout.mockReturnValue(true);

      await callNativeLogout({
        sessionShowExpiry,
        queryShowExpiry,
      });
    });

    it('will invoke sessionExpired NativeApp', () => {
      expect(NativeApp.sessionExpired).toHaveBeenCalled();
    });

    it('will not invoke logout NativeApp', () => {
      expect(NativeApp.logout).not.toHaveBeenCalled();
    });

    it('will not call next', () => {
      expect(next).not.toHaveBeenCalled();
    });
  });

  describe('NativeApp methods are supported and showExpiryMessage is not set', () => {
    beforeEach(async () => {
      NativeApp.supportsSessionExpired.mockReturnValue(true);
      NativeApp.supportsLogout.mockReturnValue(true);

      await callNativeLogout({
        sessionShowExpiry: false,
        queryShowExpiry: false,
      });
    });

    it('will invoke logout NativeApp', () => {
      expect(NativeApp.logout).toHaveBeenCalled();
    });

    it('will not invoke sessionExpired NativeApp', () => {
      expect(NativeApp.sessionExpired).not.toHaveBeenCalled();
    });

    it('will not call next', () => {
      expect(next).not.toHaveBeenCalled();
    });
  });

  describe('NativeApp methods are not supported', () => {
    beforeEach(async () => {
      NativeApp.supportsSessionExpired.mockReturnValue(false);
      NativeApp.supportsLogout.mockReturnValue(false);

      await callNativeLogout({
        sessionShowExpiry: false,
        queryShowExpiry: false,
      });
    });

    it('will call next', () => {
      expect(next).toHaveBeenCalled();
    });

    it('will not invoke logout NativeApp', () => {
      expect(NativeApp.logout).not.toHaveBeenCalled();
    });

    it('will not invoke sessionExpired NativeApp', () => {
      expect(NativeApp.sessionExpired).not.toHaveBeenCalled();
    });
  });
});
