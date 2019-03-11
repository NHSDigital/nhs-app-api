import Withdrawing from '@/components/organ-donation/Withdrawing';

import { mount } from '../../helpers';

describe('withdrawing', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(Withdrawing);
  });

  it('will display a subheader', () => {
    const subheader = wrapper.find('h3');
    expect(subheader.exists()).toBe(true);
    expect(subheader.text()).toEqual('translate_organDonation.reviewYourDecision.withdraw.subheader');
  });

  it('will display a paragraph', () => {
    const paragraph = wrapper.find('p');
    expect(paragraph.exists()).toBe(true);
    expect(paragraph.text()).toEqual('translate_organDonation.reviewYourDecision.withdraw.body');
  });
});
