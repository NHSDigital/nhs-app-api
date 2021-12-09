import NotificationsGenericError from '@/pages/notifications/notifications-generic-error';
import { createStore, mount } from '../../helpers';

describe('notifications prompt page', () => {
  let $store;
  let wrapper;

  const mountPage = ({ state }) => {
    $store = createStore({ state });

    return mount(NotificationsGenericError, {
      $store,
      stubs: {
        'no-return-flow-layout': '<div><slot/></div>',
      },
    });
  };

  describe('content', () => {
    beforeEach(() => {
      wrapper = mountPage({
        state: {
          device: {
            isNativeApp: true,
          },
        },
      });
    });

    const normalizeString = str => str.replace(/\s+/g, ' ');

    it('will show the expected paragraph', () => {
      const listItems = wrapper.findAll('li');
      expect(normalizeString(listItems.at(0).text())).toEqual('Select Continue.');
      expect(normalizeString(listItems.at(1).text())).toEqual('Go to More.');
      expect(normalizeString(listItems.at(2).text())).toEqual('Select Account and settings.');
      expect(normalizeString(listItems.at(3).text())).toEqual('Then select Manage notifications.');
      expect(normalizeString(listItems.at(4).text())).toEqual('Turn on the notifications toggle.');
    });
  });
});
