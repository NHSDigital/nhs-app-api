import Vue from 'vue';
import GPAppointments from '@/pages/appointments/gp-appointments';
import { GP_APPOINTMENTS_PATH } from '@/router/paths';
import { mount, createStore, createRouter } from '../../helpers';

Vue.mixin({
  methods: {
    hasConnectionProblem() {
      return false;
    },
  },
});

describe('index.vue', () => {
  let $store;
  let $route;
  let $router;

  const mountPage = async ({ status,
    userSessionCreateReferenceCode,
    isNativeApp = false,
    query = {},
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        errors: {
          hasConnectionProblem: false,
        },
        myAppointments: {
          error: {
            status,
          },
          hasLoaded: true,
        },
        session: {
          userSessionCreateReferenceCode,
        },
      },
      getters: {
        'session/isLoggedIn': () => true,
      },
    });

    $router = createRouter();
    $router.currentRoute = {
      path: GP_APPOINTMENTS_PATH,
    };

    $route = {
      query,
      path: GP_APPOINTMENTS_PATH,
    };

    const gpAppointments = mount(GPAppointments, {
      $store, $route, $router, methods: { reload: jest.fn() },
    });

    // As the mounted hook is async in the Gp Appointments page, we must await $nextTick
    // to ensure any expected changes to the instance/dom have been applied.
    await gpAppointments.vm.$nextTick();

    return gpAppointments;
  };

  describe('errors', () => {
    it('will dispatch the retry function if the hasRetried flag is set on the route', async () => {
      await mountPage({ query: { hr: true } });
      expect($store.dispatch).toBeCalledWith('session/setRetry', true);
    });
  });
});
