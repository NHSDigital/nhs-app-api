import SjrIf from '@/components/SjrIf';
import { createStore, mount } from '../helpers';

describe('SjrIf', () => {
  let wrapper;

  const mountSjrIf = ({ journey, tag, enabled, disabled } = {}) => mount(SjrIf, {
    $store: createStore({
      getters: {
        [`serviceJourneyRules/${journey}Enabled`]: enabled,
      },
    }),
    propsData: {
      disabled,
      journey,
      tag,
    },
    slots: {
      default: '<div id="slot-content"><p>Slot data</p></div>',
    },
  });

  const mountSjrIfWithContext = ({ journey, tag, disabled, context } = {}) => mount(SjrIf, {
    $store: createStore({
      getters: {
        [`serviceJourneyRules/${journey}Enabled`]: () => (context),
      },
    }),
    propsData: {
      context,
      disabled,
      journey,
      tag,
    },
    slots: {
      default: '<div id="slot-content"><p>Slot data</p></div>',
    },
  });

  describe('journey enabled', () => {
    beforeEach(() => {
      wrapper = mountSjrIf({ journey: 'test-journey', enabled: true });
    });

    it('will display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(true);
    });
  });

  describe('journey enabled with context', () => {
    beforeEach(() => {
      wrapper = mountSjrIfWithContext({ journey: 'test-journey', context: true });
    });

    it('will display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(true);
    });
  });

  describe('journey disabled', () => {
    beforeEach(() => {
      wrapper = mountSjrIf({ journey: 'test-journey', enabled: false });
    });

    it('will not display root element', () => {
      expect(wrapper.find('div').exists()).toBe(false);
    });

    it('will not display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(false);
    });
  });

  describe('journey disabled by prop', () => {
    beforeEach(() => {
      wrapper = mountSjrIf({ journey: 'test-journey', enabled: true, disabled: true });
    });

    it('will not display root element', () => {
      expect(wrapper.find('div').exists()).toBe(false);
    });

    it('will not display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(false);
    });
  });

  describe('journey disabled by context', () => {
    beforeEach(() => {
      wrapper = mountSjrIf({ journey: 'test-journey', context: false });
    });

    it('will not display root element', () => {
      expect(wrapper.find('div').exists()).toBe(false);
    });

    it('will not display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(false);
    });
  });

  describe('journey with context disabled by prop', () => {
    beforeEach(() => {
      wrapper = mountSjrIf({ journey: 'test-journey', disabled: true, context: true });
    });

    it('will not display root element', () => {
      expect(wrapper.find('div').exists()).toBe(false);
    });

    it('will not display slot content', () => {
      expect(wrapper.find('#slot-content').exists()).toBe(false);
    });
  });

  describe('root element', () => {
    describe('by default', () => {
      beforeEach(() => {
        wrapper = mountSjrIf({ journey: 'test-journey', enabled: true });
      });

      it('will be "DIV"', () => {
        expect(wrapper.find('div').exists()).toBe(true);
      });
    });

    describe('with "span" tag', () => {
      beforeEach(() => {
        wrapper = mountSjrIf({ journey: 'test-journey', tag: 'SPAN', enabled: true });
      });

      it('will be "span"', () => {
        expect(wrapper.find('span').exists()).toBe(true);
      });
    });
  });
});
