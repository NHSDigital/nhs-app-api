/* eslint-disable object-curly-newline */
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { ORGAN_DONATION_PATH } from '@/router/paths';
import { ORGAN_DONATION_URL } from '@/router/externalLinks';
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

  const mountAs = ({
    organDonationDesktopEnabled = true,
  } = {}) => {
    $store = createStore({
      state: {
        navigation: {
          backLinkOverride: undefined,
        },
      },
      $env: {
        ORGAN_DONATION_DESKTOP_ENABLED: organDonationDesktopEnabled,
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

  describe('link', () => {
    beforeEach(() => {
      wrapper = mountAs();
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will have the internal URL for the href', () => {
      expect(link.attributes().href).toEqual(URL_INTERNAL);
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

  describe('not internal link', () => {
    beforeEach(() => {
      wrapper = mountAs({ organDonationDesktopEnabled: false });
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will have the external URL for the href', () => {
      expect(link.attributes().href).toEqual(ORGAN_DONATION_URL);
    });
  });

  describe('onClickOrganDonation handler for external', () => {
    beforeEach(() => {
      wrapper = mountAs({ organDonationDesktopEnabled: false });
      link = wrapper.find(`#${ID_TEST_LINK}`);
    });

    it('will not redirect to the organ donation path ', () => {
      wrapper = mountAs({ organDonationDesktopEnabled: false });
      wrapper.vm.onClickOrganDonation();
      expect(redirectTo).not.toHaveBeenCalledWith(wrapper.vm, ORGAN_DONATION_PATH);
    });
  });
});
