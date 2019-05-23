import InsetContent from '@/components/data-sharing/InsetContent';
import { createStore, mount } from '../../helpers';

const paragraphs = ['para1', 'para2'];

const createInsetContentComponent = $store => mount(InsetContent, {
  $store,
  propsData: {
    paragraphs,
  },
});

describe('InsetContent', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
    });

    wrapper = createInsetContentComponent($store);
  });

  it('includes the hidden span', () => {
    expect(wrapper.find('.nhsuk-u-visually-hidden').exists()).toBe(true);
    expect(wrapper.text()).toContain('generic.insetConent.heading');
  });

  it('includes all paragraphs', () => {
    const paras = wrapper.findAll('p');

    expect(wrapper.find('p').exists()).toBe(true);
    paragraphs.forEach((paragraph, index) => expect(paras.at(index).text()).toBe(paragraph));
  });
});
