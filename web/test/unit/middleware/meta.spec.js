import meta from '@/middleware/meta';
import {
  ACCOUNT,
  INDEX,
  LINKED_PROFILES_SHUTTER_APPOINTMENTS,
} from '@/lib/routes';
import { createStore } from '../helpers';

describe('tests for meta.js', () => {
  let route;
  let store;
  let app;

  beforeEach(() => {
    store = createStore({
      state: {
        device: {
          source: 'web',
        },
      },
      getters: {
        'session/isProxying': false,
      },
    });

    route = { meta: {} };
    app = {
      i18n: {
        tc: jest.fn(),
      },
    };

    route.name = ACCOUNT.name;
    meta({ route, store, app });
  });

  it('will return the desktop header key for "My Account"', () => {
    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('pageHeaders.settings');
  });

  it('will return the native header key for "Settings" : ios', () => {
    store.state.device.source = 'ios';

    meta({ route, store, app });

    expect(route.meta.headerKey).toBe('pageHeaders.settings');
  });

  it('will return the native header key for "Settings" : android', () => {
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

  it('will call setRoutePath with the matched route when not proxying', () => {
    // arrange
    store.getters['session/isProxying'] = false;
    route.name = INDEX.name;

    const expectedRouteData = route;
    expectedRouteData.meta = {
      headerKey: 'pageHeaders.home',
      pageTitleKey: 'pageTitles.home',
    };

    // act
    meta({ route, store, app });

    // assert
    expect(store.dispatch).toHaveBeenCalledWith('errors/setRoutePath', expectedRouteData);
  });

  it('will dispatch `myAppointments/clearError`', () => {
    expect(store.dispatch).toBeCalledWith('myAppointments/clearError');
  });

  it('will dispatch `availableAppointments/clearError`', () => {
    expect(store.dispatch).toBeCalledWith('availableAppointments/clearError');
  });
});
