import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';
import { APPOINTMENT_BOOKING_GUIDANCE, INDEX } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';

describe('BreadCrumbTrail.vue', () => {
  let $router;
  let goToUrl;

  const createBreadCrumbTrail = ({
    isNativeApp = false,
    routeName = 'some-route',
    routesProp = [INDEX],
    csrfToken = 'some token',
    backLinkOverride = undefined,
  } = {}) =>
    mount(BreadCrumbTrail, {
      $store: createStore({
        state: {
          navigation: { backLinkOverride },
          device: { isNativeApp },
          session: { csrfToken },
        },
      }),
      methods: { goToUrl },
      propsData: { routes: routesProp },
      $style: { native: 'native' },
      $route: { name: routeName },
      $router,
      stubs: { 'nuxt-link': '<a></a>' },
    });

  beforeEach(() => {
    goToUrl = jest.fn();
    $router = createRouter();
  });

  it('will not render a breadcrumb as the user is not logged in.', () => {
    const wrapper = createBreadCrumbTrail({ csrfToken: null });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will not render a breadcrumb as there are no items to display.', () => {
    const wrapper = createBreadCrumbTrail({ routesProp: [] });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will render a single breadcrumb item to display.', () => {
    const wrapper = createBreadCrumbTrail();

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(wrapper.find(`ol>li>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);

    expect(wrapper.find(`p>a[to='${INDEX.path}']`)
      .exists()).toEqual(true);
  });

  it('will return a multiple breadcrumbs item to display.', () => {
    const wrapper = createBreadCrumbTrail({ routesProp: [INDEX, APPOINTMENT_BOOKING_GUIDANCE] });

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
    it('will go back 1 page in the history if no override present', () => {
      const wrapper = createBreadCrumbTrail({
        isNativeApp: true,
        routeName: 'some-route-not-configured',
      });

      wrapper.vm.backLinkClicked();

      expect($router.goBack).toHaveBeenCalled();
    });

    it('will go to route override path if configured in routes.js', () => {
      const wrapper = createBreadCrumbTrail({ isNativeApp: true, routeName: 'organ-donation' });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');

      expect(wrapper.vm.$route.name).toBe('organ-donation');
      expect(goToUrl).toHaveBeenCalledWith('/more');
    });

    it('will go to the store override path if set ' +
       'and route override is configured with a default', () => {
      const wrapper = createBreadCrumbTrail({
        isNativeApp: true,
        routeName: 'organ-donation',
        backLinkOverride: '/store-organ-donation-override',
      });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');

      expect(wrapper.vm.$route.name).toBe('organ-donation');
      expect(goToUrl).toHaveBeenCalledWith('/store-organ-donation-override');
    });

    it('will go to the default path if override path set, route override ' +
       'is configured with a default, and ignoreStore is true', () => {
      const wrapper = createBreadCrumbTrail({
        isNativeApp: true,
        routeName: 'switch-profile',
        backLinkOverride: '/switch-profile-override',
      });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');

      expect(wrapper.vm.$route.name).toBe('switch-profile');
      expect(goToUrl).toHaveBeenCalledWith('/');
    });

    it('back link will exist and have the correct attributes', () => {
      const wrapper = createBreadCrumbTrail({ isNativeApp: true });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');

      expect(backLink.exists()).toEqual(true);
      expect(backLink.attributes('tabindex')).toBe('0');
    });
  });
});

