/* eslint-disable object-curly-newline */
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { ORGAN_DONATION } from '@/lib/routes';
import { createEvent, createStore, mount } from '../../helpers';

const ID_TEST_LINK = 'test-link';
const URL_EXTERNAL = 'http://foo.bar/';
const URL_INTERNAL = ORGAN_DONATION.path;

describe('organ donation link', () => {
  let $env;
  let $router;
  let $store;
  let event;
  let link;
  let propsData;
  let wrapper;

  const mountAs = ({ enabled, native = true }) => {
    $env = { ORGAN_DONATION_INTEGRATION_ENABLED: enabled, ...$env };
    $store = createStore({
      $env,
      state: {
        device: {
          isNativeApp: native,
        },
      },
    });
    propsData = {
      id: ID_TEST_LINK,
    };

    return mount(OrganDonationLink, { $env, $store, $router, propsData });
  };

  beforeEach(() => {
    $router = [];
    $env = { ORGAN_DONATION_URL: URL_EXTERNAL };
  });

  describe('`useIntegratedOrganDonation` computed property', () => {
    describe('organ donation integration is disabled', () => {
      it('will be false when ORGAN_DONATION_INTEGRATION_ENABLED is false', () => {
        wrapper = mountAs({ enabled: false });
        expect(wrapper.vm.useIntegratedOrganDonation).toBe(false);
      });

      it('will be false when ORGAN_DONATION_INTEGRATION_ENABLED is "false"', () => {
        wrapper = mountAs({ enabled: 'false' });
        expect(wrapper.vm.useIntegratedOrganDonation).toBe(false);
      });
    });

    describe('organ donation integration is enabled', () => {
      it('will be false when the app is not native', () => {
        wrapper = mountAs({ enabled: true, native: false });

        expect(wrapper.vm.useIntegratedOrganDonation).toBe(false);
      });

      it('will be true when the app is native', () => {
        wrapper = mountAs({ enabled: true, native: true });

        expect(wrapper.vm.useIntegratedOrganDonation).toBe(true);
      });
    });
  });

  describe('link', () => {
    describe('organ donation integration is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ enabled: false });
        link = wrapper.find(`#${ID_TEST_LINK}`);
      });

      it('will have the external URL for the href', () => {
        expect(link.attributes().href).toEqual(URL_EXTERNAL);
      });

      it('will have the target set to "_blank"', () => {
        expect(link.attributes().target).toEqual('_blank');
      });
    });

    describe('organ donation integration is enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ enabled: true });
        link = wrapper.find(`#${ID_TEST_LINK}`);
      });

      it('will have the external URL for the href', () => {
        expect(link.attributes().href).toEqual(URL_INTERNAL);
      });

      it('will have the target set to "_self"', () => {
        expect(link.attributes().target).toEqual('_self');
      });
    });
  });

  describe('onClickOrganDonation handler', () => {
    describe('organ donation integration is disabled', () => {
      beforeEach(() => {
        event = createEvent({ currentTarget: { pathname: URL_INTERNAL } });
        wrapper = mountAs({ enabled: false });
        link = wrapper.find(`#${ID_TEST_LINK}`);
      });

      it('will not push to the router when the link is clicked', () => {
        wrapper.vm.onClickOrganDonation(event);
        expect($router).not.toContain(URL_INTERNAL);
      });

      it('will not prevent the default action on the event', () => {
        wrapper.vm.onClickOrganDonation(event);
        expect(event.preventDefault).not.toHaveBeenCalled();
      });
    });

    describe('organ donation integration is enabled', () => {
      beforeEach(() => {
        event = createEvent({ currentTarget: { pathname: URL_INTERNAL } });
        wrapper = mountAs({ enabled: true });
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
    });
  });
});
