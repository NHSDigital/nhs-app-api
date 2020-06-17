/* eslint-disable object-curly-newline */
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { ORGAN_DONATION_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { createStore, mount } from '../../helpers';

jest.mock('@/lib/utils', () => ({
  ...jest.requireActual('@/lib/utils'),
  redirectTo: jest.fn(),
}));

const ID_TEST_LINK = 'test-link';
const URL_INTERNAL = ORGAN_DONATION_PATH;
const BACK_LINK_OVERRIDE = '/correct-url';

describe('organ donation link', () => {
  let $router;
  let $store;
  let link;
  let propsData;
  let wrapper;

  const mountAs = (params = { native: true }) => {
    $store = createStore({
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

    return mount(OrganDonationLink, { $store, $router, propsData });
  };

  beforeEach(() => {
    redirectTo.mockClear();
    $router = [];
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
      wrapper = mountAs();
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will redirect to the organ donation path ', () => {
      wrapper = mountAs();
      wrapper.vm.onClickOrganDonation();
      expect(redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
    });

    describe('back link override', () => {
      it('the correct value is added to the store', () => {
        wrapper = mountAs();
        wrapper.vm.onClickOrganDonation();
        expect($store.dispatch).toHaveBeenCalledWith('navigation/setBackLinkOverride', BACK_LINK_OVERRIDE);
      });
    });
  });
});
