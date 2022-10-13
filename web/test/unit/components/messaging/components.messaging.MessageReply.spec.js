import MessageReply from '@/components/messaging/MessageReply';
import { createStore, mount } from '../../helpers';

jest.useFakeTimers().setSystemTime(new Date('2022-08-19T08:50:48.586'));

let wrapper;
let $store;
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
  responseSentDateTime = null,
  isNativeApp = false,
} = {}) => {
  $store = createStore({
    state: {
      device: { isNativeApp },
    },
  });

  return mount(MessageReply, {
    $style: {
      [optionsContainerClass]: optionsContainerClass,
    },
    $store,
    propsData: {
      messageReply: {
        options: replyOptions,
        response,
        responseSentDateTime,
      },
      senderName: 'Test Sender',
    },
  });
};

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

    describe('an option is selected', () => {
      describe('send button is clicked', () => {
        it('will emit a click event', () => {
          wrapper.setData({ selectedRadioValue: 'YES' });
          wrapper.vm.onRadioButtonChanged('YES');

          expect($store.dispatch).toBeCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);

          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked.length).toBe(1);
        });
      });
    });

    describe('an option is not selected', () => {
      describe('send button is clicked', () => {
        it('will show an error message', () => {
          wrapper.setData({ selectedRadioValue: undefined });
          expect($store.dispatch).not.toHaveBeenCalled();

          wrapper.vm.onSendClicked();
          expect(wrapper.find('#replyoptions-error').text()).toContain('Select an option if you want to reply to this message');
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

    describe('an option is selected', () => {
      describe('send button is clicked', () => {
        it('will emit a click event', () => {
          wrapper.setData({ selectedCheckboxValue: 'CANCEL' });
          wrapper.vm.onRadioButtonChanged('CANCEL');
          expect($store.dispatch).toBeCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);

          wrapper.vm.onSendClicked();
          expect(wrapper.emitted().send_clicked.length).toBe(1);
        });
      });
    });

    describe('an option is not selected', () => {
      describe('send button is clicked', () => {
        it('will show an error message', () => {
          wrapper.setData({ selectedCheckboxValue: undefined });
          wrapper.vm.onRadioButtonChanged(undefined);
          expect($store.dispatch).toBeCalledWith('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);

          wrapper.vm.onSendClicked();
          expect(wrapper.find('#reply-checkbox-error').text()).toContain('Select the option if you want to reply to this message');
        });
      });
    });
  });

  describe('has a response', () => {
    beforeEach(() => {
      wrapper = mountComponent({
        response: 'NEVER',
        responseSentDateTime: '2022-08-19T08:50:48.586Z',
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
          responseSentDateTime: dateTime,
        });

        expect(wrapper.find('.message-reply__formatted-time').text()).toContain(description);
      });
    });
  });
});
