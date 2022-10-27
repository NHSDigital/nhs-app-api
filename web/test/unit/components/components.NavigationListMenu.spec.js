import NavigationListMenu from '@/components/NavigationListMenu';
import each from 'jest-each';
import { INDEX_PATH } from '@/router/paths';
import { mount, createStore, createRouter } from '../helpers';

let wrapper;
let $store;
let $router;
let goToUrl;

const mountAs = ({
  isNativeApp = false,
  isProofLevel9 = true,
  supportsLinkedProfiles = true,
  integrationEnabled = true,
  isProxying = false,
  totalUnreadMessageCount = 0,
} = {}) => {
  $router = createRouter();
  $store = createStore({
    state: {
      device: {
        isNativeApp,
      },
      serviceJourneyRules: {
        rules: {
          supportsLinkedProfiles,
        },
      },
      messaging: {
        totalUnreadMessageCount,
      },
    },
    getters: {
      'session/isProofLevel9': isProofLevel9,
      'serviceJourneyRules/silverIntegrationEnabled': () => integrationEnabled,
      'knownServices/matchOneById': jest.fn().mockImplementation(() => 'somewhere'),
      'session/isProxying': isProxying,
    },
  });
  return mount(NavigationListMenu, { $store, $router, methods: { goToUrl } });
};

beforeEach(() => {
  goToUrl = jest.fn();
  window.open = jest.fn();
});

describe('Navigation Links ', () => {
  describe('Netcompany Vaccine Record link', () => {
    each([
      ['P9', 'not proxying', 'defined', 'shown', true, false, true, true],
      ['P9', 'proxying', 'defined', 'hidden', true, true, true, false],
      ['P9', 'not proxying', 'not defined', 'hidden', true, false, false, false],
      ['P9', 'proxying', 'not defined', 'hidden', true, true, false, false],
      ['P5', 'not proxying', 'defined', 'hidden', false, false, true, false],
      ['P5', 'proxying', 'defined', 'hidden', false, true, true, false],
      ['P5', 'not proxying', 'not defined', 'hidden', false, false, false, false],
      ['P5', 'proxying', 'not defined', 'hidden', false, true, false, false],
    ])
      .it('A %s user that is %s, and has NetCompany vaccine record provider %s, will have NetCompany vaccine record link %s', (_, __, ___, ____, isProofLevel9, isProxying, integrationEnabled, isVisible) => {
        wrapper = mountAs({ isProofLevel9, isProxying, integrationEnabled });
        expect(wrapper.find('#btn_netCompany_vaccine_record').exists()).toBe(isVisible);
      });
  });

  describe('Netcompany P5 Vaccine Record link', () => {
    each([
      ['P9', 'not proxying', 'defined', 'hidden', true, false, true, false],
      ['P9', 'proxying', 'defined', 'hidden', true, true, true, false],
      ['P9', 'not proxying', 'not defined', 'hidden', true, false, false, false],
      ['P9', 'proxying', 'not defined', 'hidden', true, true, false, false],
      ['P5', 'not proxying', 'defined', 'shown', false, false, true, true],
      ['P5', 'proxying', 'defined', 'hidden', false, true, true, false],
      ['P5', 'not proxying', 'not defined', 'hidden', false, false, false, false],
      ['P5', 'proxying', 'not defined', 'hidden', false, true, false, false],
    ])
      .it('A %s user that is %s, and has NetCompany vaccine record provider %s, will have NetCompany vaccine record link %s', (_, __, ___, ____, isProofLevel9, isProxying, integrationEnabled, isVisible) => {
        wrapper = mountAs({ isProofLevel9, isProxying, integrationEnabled });
        expect(wrapper.find('#btn_netCompanyP5_vaccine_record').exists()).toBe(isVisible);
      });
  });

  describe('NBS Vaccine Record link', () => {
    each([
      ['P9', 'not proxying', 'defined', 'shown', true, false, true, true],
      ['P9', 'proxying', 'defined', 'hidden', true, true, true, false],
      ['P9', 'not proxying', 'not defined', 'hidden', true, false, false, false],
      ['P9', 'proxying', 'not defined', 'hidden', true, true, false, false],
      ['P5', 'not proxying', 'defined', 'hidden', false, false, true, false],
      ['P5', 'proxying', 'defined', 'hidden', false, true, true, false],
      ['P5', 'not proxying', 'not defined', 'hidden', false, false, false, false],
      ['P5', 'proxying', 'not defined', 'hidden', false, true, false, false],
    ])
      .it('A %s user that is %s, and has NBS vaccine record provider %s, will have NBS vaccine record link %s', (_, __, ___, ____, isProofLevel9, isProxying, integrationEnabled, isVisible) => {
        wrapper = mountAs({ isProofLevel9, isProxying, integrationEnabled });
        expect(wrapper.find('#btn_nbs_booking').exists()).toBe(isVisible);
      });
  });

  describe('Linked Accounts link', () => {
    each([
      ['shown', 'supports linked profiles', true, true],
      ['hidden', 'does not support linked profiles', false, false],
    ])
      .it('will be %s when the gp %s', (_, __, supportsLinkedProfiles, isVisible) => {
        wrapper = mountAs({ supportsLinkedProfiles });
        expect(wrapper.find('#linked-profiles-link').exists()).toBe(isVisible);
      });
    it('will dispatch to set the breadcrumb to the default and to setBackLinkOverride', () => {
      wrapper = mountAs({ supportsLinkedProfiles: true });
      wrapper.vm.navigateToLinkedProfiles();

      expect($store.dispatch).toBeCalledWith('navigation/setRouteCrumb', 'defaultCrumb');
      expect($store.dispatch).toBeCalledWith('navigation/setBackLinkOverride', INDEX_PATH);
    });
  });

  describe('P9 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: true });
    });
    it('will show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(true);
    });
    it('will show GP Health record link', () => {
      expect(wrapper.find('#menu-item-myRecord').exists()).toBe(true);
    });
    it('will show linked profiles link', () => {
      expect(wrapper.find('#linked-profiles-link').exists()).toBe(true);
    });
    it('will show nbs link', () => {
      expect(wrapper.find('#btn_nbs_booking').exists()).toBe(true);
    });
  });

  describe('P5 User', () => {
    beforeEach(() => {
      wrapper = mountAs({ isProofLevel9: false });
    });
    it('will not show prescriptions link', () => {
      expect(wrapper.find('#menu-item-prescriptions').exists()).toBe(false);
    });
    it('will not show GP Health record link', () => {
      expect(wrapper.find('#menu-item-myRecord').exists()).toBe(false);
    });
    it('will not show linked profiles link', () => {
      expect(wrapper.find('#linked-profiles-link').exists()).toBe(false);
    });
    it('will not show nbs link', () => {
      expect(wrapper.find('#btn_nbs_booking').exists()).toBe(false);
    });
  });

  describe('unread message indicators', () => {
    each([
      ['show the indicator when there is unread messages', 1, true],
      ['not show the indicator when there is no unread messages', 0, false],
    ]).it('will %s', async (_, count, indicatorShown) => {
      wrapper = mountAs({ totalUnreadMessageCount: count });
      expect(wrapper.find('#btn_messages_countIndicator').exists()).toBe(indicatorShown);
    });
  });
});
