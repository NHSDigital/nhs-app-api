import MessageReply from '@/components/messaging/MessageReply';
import { mount } from '../../helpers';

jest.useFakeTimers().setSystemTime(new Date('2022-08-19T08:50:48.586'));

let wrapper;
const replyToThisMessageButtonId = 'showKeywordReplies';
const optionsContainerClass = 'messageReplyOptionsContainer';
const radionOptionsContainerId = 'radioOptions';
const checkboxOptionsContainerId = 'checkboxOptions';
const responseContainerId = 'messageReplyResponseContainer';

const replyMessageOptions = [
  { code: 'SMOKE', display: 'SMOKE' },
  { code: 'NO', display: 'NO' },
  { code: 'NEVER', display: 'NEVER' },
];

const mountComponent = ({
  replyOptions = replyMessageOptions,
  response = '',
  responseDateTime = null,
} = {}) => mount(MessageReply, {
  $style: {
    [optionsContainerClass]: optionsContainerClass,
  },
  propsData: {
    messageReply: {
      options: replyOptions,
      response,
      responseDateTime,
    },
    senderName: 'Test Sender',
  },
});

describe('message reply', () => {
  describe('showOptions property', () => {
    it('shows the "Reply to this Message" button when showOptions is false', () => {
      wrapper = mountComponent();
      wrapper.setData({ showOptions: false });
      expect(wrapper.find(`#${replyToThisMessageButtonId}`).exists()).toBe(true);
    });

    it('shows the keyword options container when showOptions is true', () => {
      wrapper = mountComponent();
      wrapper.setData({ showOptions: true });
      expect(wrapper.find(`#${replyToThisMessageButtonId}`).exists()).toBe(false);
      expect(wrapper.find(`.${optionsContainerClass}`).exists()).toBe(true);
    });
  });

  describe('has more than one reply option', () => {
    beforeEach(() => {
      wrapper = mountComponent();
      wrapper.setData({ showOptions: true });
    });

    it('shows the reply options using radio buttons', () => {
      expect(wrapper.find(`#${radionOptionsContainerId}`).exists()).toBe(true);
      expect(wrapper.find(`#${checkboxOptionsContainerId}`).exists()).toBe(false);
    });

    describe('no options is selected', () => {
      describe('send button is clicked', () => {
        it('will not emit a click event', () => {
          wrapper.setData({ selectedRadioValue: null });
          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked).toBe(undefined);
        });
      });
    });

    describe('an option is selected', () => {
      describe('send button is clicked', () => {
        it('will emit a click event', () => {
          wrapper.setData({ selectedRadioValue: 'YES' });
          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked.length).toBe(1);
        });
      });
    });
  });

  describe('has only one reply option', () => {
    beforeEach(() => {
      wrapper = mountComponent({ replyOptions: [{ code: 'CANCEL', disaply: 'CANCEL' }] });
      wrapper.setData({ showOptions: true });
    });

    it('shows the reply options using checkboxes', () => {
      expect(wrapper.find(`#${radionOptionsContainerId}`).exists()).toBe(false);
      expect(wrapper.find(`#${checkboxOptionsContainerId}`).exists()).toBe(true);
    });

    describe('no options is selected', () => {
      describe('send button is clicked', () => {
        it('will not emit a click event', () => {
          wrapper.setData({ selectedCheckboxValue: '' });
          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked).toBe(undefined);
        });
      });
    });

    describe('an option is selected', () => {
      describe('send button is clicked', () => {
        it('will emit a click event', () => {
          wrapper.setData({ selectedCheckboxValue: 'CANCEL' });
          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked.length).toBe(1);
        });
      });
    });
  });

  describe('has a response', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        response: 'NEVER',
        responseDateTime: '2022-08-19T08:50:48.586Z',
      });
    });

    it('will show the reply response container', () => {
      expect(wrapper.find(`#${responseContainerId}`).exists()).toBe(true);
    });

    describe.each([
      ['2022-08-19T09:50:48.586', 'today at 9:50am'],
      ['2022-08-18T21:00:48.586', 'yesterday at 9pm'],
      ['2022-04-25T12:00:48.586', 'on 25 April 2022 at midday'],
    ])('response time', (dateTime, description) => {
      it(`will describe ${dateTime} correctly`, () => {
        wrapper = mountComponent({
          response: 'NEVER',
          responseDateTime: dateTime,
        });

        expect(wrapper.find('.message-reply__formatted-time').text()).toContain(description);
      });
    });
  });
});
