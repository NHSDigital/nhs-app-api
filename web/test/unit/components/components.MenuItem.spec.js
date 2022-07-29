import MenuItem from '@/components/MenuItem';
import { mount } from '../helpers';

const mountComponent = ({ count, showIndicator, isMessaging }) =>
  mount(MenuItem, {
    propsData: {
      id: 'testItem',
      count,
      showIndicator,
      isMessaging,
    },
  });

describe('MenuItem', () => {
  describe('showIndicator property', () => {
    let wrapper;

    it('does not show the yellow dot when showIndicator is false', () => {
      wrapper = mountComponent({ showIndicator: false });
      expect(wrapper.find('#testItem_discIndicator').exists()).toBe(false);
    });

    it('shows the yellow dot when showIndicator is true', () => {
      wrapper = mountComponent({ showIndicator: true });
      expect(wrapper.find('#testItem_discIndicator').exists()).toBe(true);
    });
  });

  describe('count property', () => {
    let wrapper;

    it('shows the count when count is a positive number', () => {
      wrapper = mountComponent({ count: 9 });
      expect(wrapper.find('#testItem_countIndicator').exists()).toBe(true);
      expect(wrapper.find('#testItem_countIndicator').text()).toBe('9');
    });

    it('shows the count when count is zero', () => {
      wrapper = mountComponent({ count: 0 });
      expect(wrapper.find('#testItem_countIndicator').exists()).toBe(true);
      expect(wrapper.find('#testItem_countIndicator').text()).toBe('0');
    });

    it('shows 9+ when the count is greater than 9', () => {
      wrapper = mountComponent({ count: 12, isMessaging: true });
      expect(wrapper.find('#testItem_countIndicator').exists()).toBe(true);
      expect(wrapper.find('#testItem_countIndicator').text()).toBe('9+');
    });

    it('does not show count span when there is no count', () => {
      wrapper = mountComponent({ count: undefined });
      expect(wrapper.find('#testItem_countIndicator').exists()).toBe(false);
    });
  });
});
