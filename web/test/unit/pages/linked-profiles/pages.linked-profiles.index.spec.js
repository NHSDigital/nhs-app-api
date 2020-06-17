import LinkedProfileIndex from '@/pages/linked-profiles/index';
import { LINKED_PROFILES_SUMMARY_PATH } from '@/router/paths';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';
import { create$T, createStore, mount } from '../../helpers';

const $t = create$T();

describe('linked profile is there', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
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

  const mountPage = () => mount(LinkedProfileIndex, { $store, $t });

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
      wrapper = mountPage();
      $store.getters['linkedAccounts/hasLinkedAccounts'] = true;
    });

    it('first linked user name and age is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-0');
      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.attributes('aria-label')).toBe('user 0. 20translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
      expect(linkedProfileMenuItem.text()).toContain('user 0');
      expect(wrapper.find('#linked-account-age-0').text()).toEqual('20translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
    });

    it('second linked user name and dob is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-1');

      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.attributes('aria-label')).toBe('user 1. 42translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
      expect(linkedProfileMenuItem.text()).toContain('user 1');
      expect(wrapper.find('#linked-account-age-1').text()).toEqual('42translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
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
  });
});
