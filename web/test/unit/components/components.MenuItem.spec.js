import MenuItem from '@/components/MenuItem';
import { mount } from '../helpers';

const mountComponent = ({ count }) =>
  mount(MenuItem, {
    propsData: {
      id: 'testItem',
      count,
    },
  });

describe('count property', () => {
  let wrapper;

  it('shows the count when count is a positive number', () => {
    wrapper = mountComponent({ count: 12 });
    expect(wrapper.find('#count').exists()).toBe(true);
    expect(wrapper.find('#count').text()).toBe('12');
  });

  it('shows the count when count is zero', () => {
    wrapper = mountComponent({ count: 0 });
    expect(wrapper.find('#count').exists()).toBe(true);
    expect(wrapper.find('#count').text()).toBe('0');
  });

  it('does not show count span when there is no count', () => {
    wrapper = mountComponent({ count: undefined });
    expect(wrapper.find('#count').exists()).toBe(false);
  });
});
