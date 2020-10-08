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

    it('will show the expected paragraph', () => {
      const paragraphs = wrapper.findAll('p');
      expect(paragraphs.at(0).text()).toEqual('Select Continue, go to Settings and try again.');
    });
  });
});
