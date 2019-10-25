import LinkedProfileIndex from '@/pages/linked-profiles/index';
import { create$T, createStore, mount } from '../../helpers';
import { LINKED_PROFILES_SUMMARY } from '@/lib/routes';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';

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
          name: 'user 0',
          dateOfBirth: '2019-07-04T00:00:00.000',
        },
        {
          id: 'user-id-1',
          name: 'user 1',
          dateOfBirth: '2019-10-16T00:00:00.000',
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

    it('first linked user name and dob is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-0');
      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.text()).toContain('user 0');
      expect(wrapper.find('#linked-account-dob-0').text()).toEqual('4 July 2019');
    });

    it('second linked user name and dob is visible', () => {
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-1');

      expect(linkedProfileMenuItem.exists()).toBe(true);
      expect(linkedProfileMenuItem.text()).toContain('user 1');
      expect(wrapper.find('#linked-account-dob-1').text()).toEqual('16 October 2019');
    });

    it('should navigate when specific linked profile is clicked', () => {
      // arrange
      linkedProfileMenuItem = wrapper.find('#linked-account-menu-item-1');
      expect(linkedProfileMenuItem.exists()).toBe(true);

      // act
      linkedProfileMenuItem.trigger('click');

      // assert
      expect($store.app.$analytics.trackButtonClick)
        .toHaveBeenCalledWith(LINKED_PROFILES_SUMMARY.path, true);
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/select', $store.state.linkedAccounts.items[1]);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, LINKED_PROFILES_SUMMARY.path, null);
    });
  });
});
