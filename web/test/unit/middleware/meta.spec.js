import meta from '@/middleware/meta';
import { ACCOUNT, LINKED_PROFILES_SHUTTER_APPOINTMENTS } from '@/lib/routes';

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

  it('will set the title and pass in format arguments', () => {
    // using a route which has format arguments to make sure
    // the header and page title format them correctly
    route.name = LINKED_PROFILES_SHUTTER_APPOINTMENTS.name;
    const givenName = 'testName';
    store.state.linkedAccounts = {
      actingAsUser: {
        givenName,
      },
    };

    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('linkedProfiles.shutter.appointments.header');
    expect(route.meta.pageTitleKey).toBe('linkedProfiles.shutter.appointments.header');
    expect(route.meta.formatArguments).toEqual({ name: givenName });
    expect(app.i18n.tc)
      .toHaveBeenCalledWith(route.meta.headerKey, null, route.meta.formatArguments);
    expect(app.i18n.tc)
      .toHaveBeenCalledWith(route.meta.pageTitleKey, null, route.meta.formatArguments);
  });
});
