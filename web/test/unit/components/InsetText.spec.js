import InsetText from '@/components/InsetText';
import { createStore, mount } from '../helpers';

const paragraphs = ['para1', 'para2'];

const createInsetTextComponent = $store => mount(InsetText, {
  $store,
  propsData: {
    paragraphs,
  },
});

describe('InsetText', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
    });

    wrapper = createInsetTextComponent($store);
  });

  it('includes the hidden span', () => {
    expect(wrapper.find('.nhsuk-u-visually-hidden').exists()).toBe(true);
  });

  it('includes all paragraphs', () => {
    const paras = wrapper.findAll('p');

    expect(wrapper.find('p').exists()).toBe(true);
    paragraphs.forEach((paragraph, index) => expect(paras.at(index).text()).toBe(paragraph));
  });
});
