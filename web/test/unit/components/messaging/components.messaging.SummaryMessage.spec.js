import SummaryMessage from '@/components/messaging/SummaryMessage';
import { createRouter, createStore, mount } from '../../helpers';
import { formatInboxMessageTime } from '@/lib/utils';
import { formatDate } from '@/plugins/filters';

jest.mock('@/lib/utils');
jest.mock('@/plugins/filters');

describe('summary message', () => {
  const dateTimeClass = 'nhs-app-message__date';
  const metaClass = 'nhs-app-message__meta';
  const subjectLineClass = 'nhs-app-message__subject-line';
  const title = 'Test sender';
  const subTitle = 'Test subject';
  const dateTime = '2019-09-14T02:15:12.356Z';
  const listIndex = 1;
  const href = '/messaging/messages';
  const unreadCountClass = 'nhs-app-message__count';
  let $router;
  let $store;
  let wrapper;

  const mountSummaryMessage = ({
    unreadCount = 0,
    hasUnreadMessages = false,
    dateFormat = undefined,
  } = {}) => mount(
    SummaryMessage, {
      $router,
      $store,
      $style: {
        [dateTimeClass]: dateTimeClass,
        [metaClass]: metaClass,
        [subjectLineClass]: subjectLineClass,
        [unreadCountClass]: unreadCountClass,
      },
      propsData: {
        title,
        subTitle,
        dateTime,
        dateFormat,
        unreadCount,
        hasUnreadMessages,
        listIndex,
        href,
      },
    },
  );

  beforeEach(() => {
    $router = createRouter();
    $store = createStore();
  });

  it('will set the href on the root element', () => {
    wrapper = mountSummaryMessage();
    expect(wrapper.attributes().href).toContain(href);
  });

  describe('unread count', () => {
    describe('has value', () => {
      let unreadCount;

      beforeEach(() => {
        wrapper = mountSummaryMessage({ unreadCount: 3, hasUnreadMessages: true });
        unreadCount = wrapper.find(`.${unreadCountClass}`);
      });

      it('will show unread count', () => {
        expect(unreadCount.exists()).toBe(true);
      });

      it('will be the same as passed value', () => {
        expect(unreadCount.text()).toBe('3');
      });
    });

    describe('has no value', () => {
      let unreadCount;

      beforeEach(() => {
        wrapper = mountSummaryMessage({ hasUnreadMessages: true });
        unreadCount = wrapper.find(`.${unreadCountClass}`);
      });

      it('will show unread count', () => {
        expect(unreadCount.exists()).toBe(true);
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

  describe('dateTime', () => {
    it('will format dateTime using formattedInboxMessageTime util if dateFormat not given', () => {
      const expectedFormattedTime = 'Sent 14 September 2019 at 2:15am';
      formatInboxMessageTime.mockImplementation(date => (
        date === dateTime
          ? expectedFormattedTime
          : ''
      ));

      wrapper = mountSummaryMessage();
      const sentTime = wrapper.find(`.${dateTimeClass}`);

      expect(sentTime.text()).toBe(expectedFormattedTime);
    });

    it('will format dateTime using provided dateFormat if given', () => {
      const expectedFormattedTime = '14/09/2019';
      const dateFormat = 'DD/MM/YYYY';
      formatDate.mockImplementation(date => (
        date === dateTime
          ? expectedFormattedTime
          : ''
      ));

      wrapper = mountSummaryMessage({ dateFormat });
      const sentTime = wrapper.find(`.${dateTimeClass}`);

      expect(sentTime.text()).toBe(expectedFormattedTime);
    });
  });

  describe('click', () => {
    beforeEach(() => {
      wrapper = mountSummaryMessage();
      wrapper.trigger('click');
    });

    it('will emit a click event', () => {
      expect(wrapper.emitted().click.length).toBe(1);
    });
  });
});
