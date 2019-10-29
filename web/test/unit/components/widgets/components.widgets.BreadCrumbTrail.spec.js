/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import { INDEX, APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';
import { createLocalVue } from '@vue/test-utils';
import Vuex from 'vuex';
import { mount, createRouter, createStore } from '../../helpers';
import BreadCrumbTrail from '../../../../src/components/widgets/BreadCrumbTrail';

describe('BreadCrumbTrail.vue', () => {
  const BACKLINK_OVERRIDE = '/some-link';
  const router = createRouter();
  const localVue = createLocalVue();
  const goToUrl = jest.fn();

  localVue.use(Vuex);
  localVue.mixin(Vue.mixin({
    methods: {
      goToUrl,
    },
  }));

  let routeName = 'some-route';

  const createBreadCrumbTrail = (
    {
      $store = createStore({
        state: {
          device: {
            isNativeApp: false,
          },
          session: {
            csrfToken: 'some token',
          },
        },
      }),
      $route = {
        name: routeName,
      },
      $router = router,
      propsData,
    } = {},
  ) =>
    mount(BreadCrumbTrail, {
      $store,
      localVue,
      $style: {
        native: 'native',
      },
      $route,
      $router,
      propsData,
      stubs: { 'nuxt-link': '<a></a>' },
    });

  beforeEach(() => {
    routeName = 'some-route';
  });


  it('will not render a breadcrumb as there are no items to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [],
      },
    });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will not render a breadcrumb as the user is not logged in.', () => {
    const wrapper = createBreadCrumbTrail({
      $store: {
        state: {
          session: {
            csrfToken: undefined,
          },
        },
      },
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will render a single breadcrumb item to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [INDEX],
      },
    });

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`p>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);
  });

  it('will return a multiple breadcrumbs item to display.', () => {
    const wrapper = createBreadCrumbTrail({
      propsData: {
        routes: [INDEX, APPOINTMENT_BOOKING_GUIDANCE],
      },
    });

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${APPOINTMENT_BOOKING_GUIDANCE.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`p>a[to='${APPOINTMENT_BOOKING_GUIDANCE.path}']`)
      .exists()).toEqual(true);
  });

  describe('native', () => {
    it('will navigate to the correct place from generic pages', () => {
      const wrapper = createBreadCrumbTrail();

      wrapper.vm.backLinkClicked();
      expect(router.go).toHaveBeenCalledWith(-1);
    });

    it('will navigate to the correct place from organ donation', () => {
      routeName = 'organ-donation';
      const wrapper = createBreadCrumbTrail({
        $store: {
          state: {
            device: {
              isNativeApp: true,
            },
            session: {
              csrfToken: 'some token',
            },
            navigation: {
              backLinkOverride: '/some-link',
            },
          },
        },
      });

      wrapper.vm.backLinkClicked();
      expect(wrapper.vm.$route.name).toBe('organ-donation');
      expect(goToUrl).toHaveBeenCalledWith(BACKLINK_OVERRIDE);
    });
  });
});

