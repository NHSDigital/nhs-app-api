import i18n from '@/plugins/i18n';
import MessagesSenderError from '@/components/errors/additional-info/MessagesSenderError';
import { mount } from '../../../helpers';

const mountWrapper = () => mount(
  MessagesSenderError,
  {
    state: {
      device: {
        source: 'web',
      },
      messaging: {
        selectedSender: 'senderOne',
      },
    },
    mountOpts: { i18n },
  },
);

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
