import MessagesSenderError from '@/components/errors/additional-info/MessagesSenderError';
import { create$T, mount } from '../../../helpers';

const $tMock = create$T();

const mountWrapper = () => mount(MessagesSenderError, {
  state: {
    device: {
      source: 'web',
    },
    messaging: {
      selectedSender: 'senderOne',
    },
  },
  $t: (key) => {
    if (key === 'messaging.messages.errorText') {
      return 'If the problem continues and you need this information now, contact {senderName} directly.';
    }
    return $tMock(key);
  },
});

describe('MessagesSenderError', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountWrapper();
  });

  describe('text translations', () => {
    it('will contain replacement for senderName', () => {
      expect(wrapper.text()).toContain('If the problem continues and you need this information now, contact senderOne directly');
    });
  });
});
