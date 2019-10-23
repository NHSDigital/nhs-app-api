import SummaryMessage from '@/components/messaging/SummaryMessage';
import { MESSAGING_MESSAGES } from '@/lib/routes';
import { createRouter, createStore, mount } from '../../helpers';

describe('summary message', () => {
  const dateTimeClass = 'nhs-app-message__date';
  const metaClass = 'nhs-app-message__meta';
  const subjectLineClass = 'nhs-app-message__subject-line';
  const sender = 'Test sender';
  const unreadCountClass = 'nhs-app-message__count';
  let $router;
  let $store;
  let wrapper;

  const mountSummaryMessage = ({ unreadCount = 0 } = {}) => mount(SummaryMessage, {
    $router,
    $store,
    $style: {
      [dateTimeClass]: dateTimeClass,
      [metaClass]: metaClass,
      [subjectLineClass]: subjectLineClass,
      [unreadCountClass]: unreadCountClass,
    },
    propsData: {
      message: {
        sentTime: '2019-09-14T02:15:12.356Z',
      },
      sender,
      unreadCount,
    },
  });

  beforeEach(() => {
    $router = createRouter();
    $store = createStore();
    wrapper = mountSummaryMessage();
  });

  it('will contain a link to `MESSAGING_MESSAGES`', () => {
    expect(wrapper.attributes().href).toContain(MESSAGING_MESSAGES.path);
  });

  describe('unread count', () => {
    describe('has value', () => {
      let unreadCount;

      beforeEach(() => {
        wrapper = mountSummaryMessage({ unreadCount: 3 });
        unreadCount = wrapper.find(`.${unreadCountClass}`);
      });

      it('will show unread count', () => {
        expect(unreadCount.exists()).toBe(true);
      });

      it('will be the same as passed value', () => {
        expect(unreadCount.text()).toBe('3');
      });
    });

    describe('none', () => {
      beforeEach(() => {
        wrapper = mountSummaryMessage({ unreadCount: 0 });
      });

      it('will not show unread count', () => {
        expect(wrapper.find(`.${unreadCountClass}`).exists()).toBe(false);
      });
    });
  });

  describe('sent time', () => {
    let sentTime;

    beforeEach(() => {
      sentTime = wrapper.find(`.${dateTimeClass}`);
    });

    it('will format sent time to `DD/MM/YYYY` london time', () => {
      expect(sentTime.text()).toBe('14/09/2019');
    });
  });

  describe('click', () => {
    beforeEach(() => {
      wrapper.trigger('click');
    });

    it('will dispatch `messaging/selectSender` with sender', () => {
      expect($store.dispatch).toBeCalledWith('messaging/selectSender', sender);
    });

    it('will push `MESSAGING_MESSAGES` to the router', () => {
      expect($router.push).toHaveBeenCalledWith(MESSAGING_MESSAGES.path);
    });
  });
});
