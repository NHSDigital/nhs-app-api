import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';
import { APPOINTMENT_BOOKING_GUIDANCE, INDEX } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';

describe('BreadCrumbTrail.vue', () => {
  const BACKLINK_OVERRIDE = '/some-link';

  let $router;
  let goToUrl;
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
      propsData,
    } = {},
  ) =>
    mount(BreadCrumbTrail, {
      $store,
      $style: {
        native: 'native',
      },
      $route,
      $router,
      methods: {
        goToUrl,
      },
      propsData,
      stubs: { 'nuxt-link': '<a></a>' },
    });

  beforeEach(() => {
    routeName = 'some-route';
    goToUrl = jest.fn();
    $router = createRouter();
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
      expect($router.goBack).toHaveBeenCalled();
    });

    it('back link will exist and have the correct attributes', () => {
      const wrapper = createBreadCrumbTrail({
        $store: createStore({
          state: {
            device: {
              isNativeApp: true,
            },
            session: {
              csrfToken: 'some token',
            },
          },
        }),
        propsData: {
          routes: [INDEX],
        },
      });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      expect(backLink.exists()).toEqual(true);
      expect(backLink.attributes('tabindex')).toBe('0');
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
        propsData: {
          routes: [INDEX],
        },
      });
      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');
      expect(wrapper.vm.$route.name).toBe('organ-donation');
      expect(goToUrl).toHaveBeenCalledWith(BACKLINK_OVERRIDE);
    });

    it('will navigate to the correct place from the switch to my profile proxy page', () => {
      routeName = 'switch-profile';
      const wrapper = createBreadCrumbTrail({
        $store: {
          state: {
            device: {
              isNativeApp: true,
            },
            session: {
              csrfToken: 'some token',
            },
          },
        },
        propsData: {
          routes: [INDEX],
        },
      });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');
      expect(wrapper.vm.$route.name).toBe('switch-profile');
      expect(goToUrl).toHaveBeenCalledWith(INDEX.path);
    });
  });
});

