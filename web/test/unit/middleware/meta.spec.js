import meta from '@/middleware/meta';
import { ACCOUNT } from '@/lib/routes';

describe('tests for meta.js', () => {
  let route;
  let store;
  let app;

  beforeEach(() => {
    store = {
      dispatch: jest.fn(),
      state: {
        device: {
          source: 'web',
        },
      },
    };

    route = { meta: {} };
    app = {
      i18n: {
        tc: jest.fn(),
      },
    };
  });

  it('will return the desktop header key for "My Account"', () => {
    route.name = ACCOUNT.name;
    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('pageHeaders.account');
  });

  it('will return the native header key for "Settings" : ios', () => {
    route.name = ACCOUNT.name;

    store.state.device.source = 'ios';

    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('pageHeaders.settings');
  });

  it('will return the native header key for "Settings" : android', () => {
    route.name = ACCOUNT.name;

    store.state.device.source = 'android';

    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('pageHeaders.settings');
  });
});
