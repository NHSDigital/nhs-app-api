import LinkedProfileIndex from '@/pages/linked-profiles/index';
import { create$T, createStore, mount } from '../../helpers';
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
          name: 'user 1',
          dateOfBirth: '2019-07-04T00:00:00.000',
        },
        {
          name: 'user 2',
          dateOfBirth: '2019-10-16T00:00:00.000',
        },
      ],
    },
  }) => state;

  const mountPage = () => mount(LinkedProfileIndex, { $store, $t });

  describe('show linked profile links', () => {
    let linkedProfileName;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      wrapper = mountPage();
      $store.getters['serviceJourneyRules/hasLinkedAccountsEnabled'] = true;
    });

    it('first linked user name and dob is visible', () => {
      linkedProfileName = wrapper.find('#linked-account-menu-item-0');
      expect(linkedProfileName.exists()).toBe(true);
      expect(linkedProfileName.text()).toContain('user 1');
      expect(wrapper.find('#linked-account-dob-0').text()).toEqual('4 July 2019');
    });

    it('second linked user name and dob is visible', () => {
      linkedProfileName = wrapper.find('#linked-account-menu-item-1');

      expect(linkedProfileName.exists()).toBe(true);
      expect(linkedProfileName.text()).toContain('user 2');
      expect(wrapper.find('#linked-account-dob-1').text()).toEqual('16 October 2019');
    });
  });
});
