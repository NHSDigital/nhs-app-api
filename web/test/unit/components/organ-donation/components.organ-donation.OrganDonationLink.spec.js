/* eslint-disable object-curly-newline */
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { ORGAN_DONATION } from '@/lib/routes';
import { createEvent, createStore, mount } from '../../helpers';

const ID_TEST_LINK = 'test-link';
const URL_EXTERNAL = 'http://foo.bar/';
const URL_INTERNAL = ORGAN_DONATION.path;
const BACK_LINK_OVERRIDE = '/correct-url';

describe('organ donation link', () => {
  let $env;
  let $router;
  let $store;
  let event;
  let link;
  let propsData;
  let wrapper;

  const mountAs = (params = { native: true }) => {
    $env = $env || {};
    $store = createStore({
      $env,
      state: {
        device: {
          isNativeApp: params.native,
        },
        navigation: {
          backLinkOverride: undefined,
        },
      },
    });
    propsData = {
      id: ID_TEST_LINK,
      backLinkOverride: BACK_LINK_OVERRIDE,
    };

    return mount(OrganDonationLink, { $env, $store, $router, propsData });
  };

  beforeEach(() => {
    $router = [];
    $env = { ORGAN_DONATION_URL: URL_EXTERNAL };
  });

  describe('`useIntegratedOrganDonation` computed property', () => {
    it('will be false when the app is not native', () => {
      wrapper = mountAs({ native: false });

      expect(wrapper.vm.useIntegratedOrganDonation).toBe(false);
    });

    it('will be true when the app is native', () => {
      wrapper = mountAs({ native: true });

      expect(wrapper.vm.useIntegratedOrganDonation).toBe(true);
    });
  });

  describe('link', () => {
    beforeEach(() => {
      wrapper = mountAs();
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will have the external URL for the href', () => {
      expect(link.attributes().href).toEqual(URL_INTERNAL);
    });

    it('will have the target set to "_self"', () => {
      expect(link.attributes().target).toEqual('_self');
    });
  });

  describe('onClickOrganDonation handler', () => {
    beforeEach(() => {
      event = createEvent({ currentTarget: { pathname: URL_INTERNAL } });
      wrapper = mountAs();
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will push to the router when the link is clicked', () => {
      wrapper.vm.onClickOrganDonation(event);
      expect($router).toContain(URL_INTERNAL);
    });

    it('will prevent the default action on the event', () => {
      wrapper.vm.onClickOrganDonation(event);
      expect(event.preventDefault).toHaveBeenCalled();
    });

    describe('back link override', () => {
      beforeEach(() => {
        event = createEvent({ currentTarget: { pathname: URL_INTERNAL } });
      });

      it('the correct value is added to the store', () => {
        wrapper = mountAs();
        wrapper.vm.onClickOrganDonation(event);
        expect($store.dispatch).toHaveBeenCalledWith('navigation/setBackLinkOverride', BACK_LINK_OVERRIDE);
      });
    });
  });
});
