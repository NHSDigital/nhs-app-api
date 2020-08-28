import i18n from '@/plugins/i18n';
import Withdrawing from '@/components/organ-donation/Withdrawing';

import { mount } from '../../helpers';

describe('withdrawing', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(Withdrawing, { mountOpts: { i18n } });
  });

  it('will display a subheader', () => {
    const subheader = wrapper.find('h3');
    expect(subheader.exists()).toBe(true);
    expect(subheader.text()).toEqual('What this means');
  });

  it('will display a paragraph', () => {
    const paragraph = wrapper.find('p');
    expect(paragraph.exists()).toBe(true);
    expect(paragraph.text()).toEqual('We will no longer know your decision about organ donation. Therefore, it will be considered that you have agreed to be an organ donor unless you are in an excluded group.');
  });
});
