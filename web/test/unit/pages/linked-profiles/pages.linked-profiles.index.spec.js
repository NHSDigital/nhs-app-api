import i18n from '@/plugins/i18n';
import LinkedProfileIndex from '@/pages/linked-profiles/index';
import { INDEX_PATH, LINKED_PROFILES_SUMMARY_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';
import NoLinkedProfiles from '@/components/linked-profiles/NoLinkedProfiles';
import { createStore, mount } from '../../helpers';

describe('linked profile index', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    serviceJourneyRules: {
      rules: {
        supportsLinkedProfiles: false,
      },
    },
    linkedAccounts: {
      items: [
        {
          id: 'user-id-0',
          fullName: 'user 0',
          ageMonths: '0',
          ageYears: '20',
        },
        {
          id: 'user-id-1',
          fullName: 'user 1',
          ageMonths: '10',
          ageYears: '42',
        },
      ],
    },
  }) => state;

  const mountPage = ({ hasRetried = false, timestamp = 123 } = {}) => mount(LinkedProfileIndex,
    { $store,
      mountOpts: { i18n },
      $route: {
        query: {
          hr: hasRetried,
          ts: timestamp,
        },
      },
    });

  describe('show linked profile links', () => {
    let linkedProfileMenuItem;

    beforeEach(() => {
      global.digitalData = {};
      dependency.redirectTo = jest.fn();
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      $store.app.$analytics = {
        trackButtonClick: jest.fn(),
      };
      $store.getters['linkedAccounts/hasLinkedAccounts'] = true;

      wrapper = mountPage();
    });

    it('first linked user name and age is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-0');
      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.attributes('aria-label')).toBe('user 0. 20 years old');
      expect(linkedProfileMenuItem.text()).toContain('user 0');
      expect(wrapper.find('#linked-account-age-0').text()).toEqual('20 years old');
      expect(wrapper.find(NoLinkedProfiles).exists()).toBe(false);
    });

    it('second linked user name and dob is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-1');

      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.attributes('aria-label')).toBe('user 1. 42 years old');
      expect(linkedProfileMenuItem.text()).toContain('user 1');
      expect(wrapper.find('#linked-account-age-1').text()).toEqual('42 years old');
      expect(wrapper.find(NoLinkedProfiles).exists()).toBe(false);
    });

    it('should navigate when specific linked profile is clicked', () => {
      // arrange
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-1');
      expect(linkedProfileMenuItem.exists()).toBe(true);

      // act
      linkedProfileMenuItem.trigger('click');

      // assert
      expect($store.app.$analytics.trackButtonClick)
        .toHaveBeenCalledWith(LINKED_PROFILES_SUMMARY_PATH, true);
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/select', $store.state.linkedAccounts.items[1]);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, LINKED_PROFILES_SUMMARY_PATH);
    });

    it('will dispatch the retry flag if hr query parameter is set', () => {
      wrapper = mountPage({ hasRetried: true });
      expect($store.dispatch).toHaveBeenCalledWith('session/setRetry', true);
    });

    it('will watch and reload if the timestamp value changes', () => {
      wrapper = mountPage({ timestamp: 12345 });
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/initialiseConfig');
    });
  });

  it('will redirect back to the home page if supportsLinkedProfiles is false', () => {
    dependency.redirectTo = jest.fn();
    $store = createStore({ state: createState() });
    wrapper = mountPage();

    expect(dependency.redirectTo)
      .toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
  });

  it('will not redirect back to the home page if supportsLinkedProfiles is true', () => {
    dependency.redirectTo = jest.fn();
    $store = createStore({ state: createState() });
    $store.state.serviceJourneyRules.rules.supportsLinkedProfiles = true;

    wrapper = mountPage();

    expect(dependency.redirectTo)
      .not
      .toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
  });

  it('will show the NoLinkedProfiles information when hasLinkedProfiles is false', () => {
    $store = createStore({ state: createState() });
    wrapper = mountPage();

    expect(wrapper.find(NoLinkedProfiles).exists())
      .toBe(true);
  });
});
