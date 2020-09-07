import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';
import { INDEX_NAME } from '@/router/names';
import { INDEX_CRUMB } from '@/breadcrumbs/general';
import { APPOINTMENTS_CRUMB } from '@/breadcrumbs/appointments';
import { RouterLinkStub } from '@vue/test-utils';
import * as dependancy from '@/lib/utils';
import { createRouter, createStore, mount } from '../../helpers';

describe('BreadCrumbTrail.vue', () => {
  let $router;
  let goToUrl;
  let $store;

  const createBreadCrumbTrail = ({
    isNativeApp = false,
    routeName = 'some-route',
    crumbs = [INDEX_CRUMB],
    csrfToken = 'some token',
    backLinkOverride = undefined,
  } = {}) => {
    $store = createStore({
      state: {
        navigation: { backLinkOverride },
        device: { isNativeApp },
        session: { csrfToken },
        onlineConsultations: {},
      },
    });

    return mount(BreadCrumbTrail, {
      $store,
      methods: { goToUrl },
      propsData: { crumbs },
      $style: { native: 'native' },
      $route: { name: routeName },
      $router,
      stubs: { 'router-link': RouterLinkStub },
    });
  };

  beforeEach(() => {
    goToUrl = jest.fn();
    $router = createRouter();
    dependancy.createRouteByNameObject = jest.fn(x => `{ name: ${x.name} }`);
  });

  it('will not render a breadcrumb as the user is not logged in.', () => {
    const wrapper = createBreadCrumbTrail({ csrfToken: null });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will not render a breadcrumb as there are no items to display.', () => {
    const wrapper = createBreadCrumbTrail({ crumbs: [] });

    expect(wrapper.find("nav[to='Breadcrumb']")
      .exists()).toEqual(false);
  });

  it('will render a single breadcrumb item to display.', () => {
    const wrapper = createBreadCrumbTrail();

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    expect(dependancy.createRouteByNameObject)
      .toHaveBeenCalledWith({ name: INDEX_NAME, params: {}, store: $store });
    expect(wrapper.find(RouterLinkStub).props().to).toBe('{ name: index }');
  });

  it('will return a multiple breadcrumbs item to display.', () => {
    const wrapper = createBreadCrumbTrail({ crumbs: [INDEX_CRUMB, APPOINTMENTS_CRUMB] });

    expect(wrapper.find("nav[aria-label='Breadcrumb']")
      .exists()).toEqual(true);

    const links = wrapper.findAll(RouterLinkStub);

    expect(links.at(0).props().to).toBe('{ name: index }');
    expect(links.at(1).props().to).toBe('{ name: appointments }');
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

    it('will go to route override path if configured in route setup', () => {
      const wrapper = createBreadCrumbTrail({ isNativeApp: true, routeName: 'organ-donation' });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');
      backLink.trigger('click');

      expect(wrapper.vm.$route.name).toBe('organ-donation');
      expect(goToUrl).toHaveBeenCalledWith('more');
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
      expect(goToUrl).toHaveBeenCalledWith('/patient/:patientId?/');
    });

    it('back link will exist and have the correct attributes', () => {
      const wrapper = createBreadCrumbTrail({ isNativeApp: true });

      const backLink = wrapper.find('#native-back-breadcrumb').find('a');

      expect(backLink.exists()).toEqual(true);
      expect(backLink.attributes('tabindex')).toBe('0');
    });
  });
});

