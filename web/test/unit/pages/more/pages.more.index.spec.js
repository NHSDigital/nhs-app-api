import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { mount } from '../../helpers';

describe('more', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mount(More);
  });

  it('will include the organ donation link', () => {
    const link = wrapper.find(OrganDonationLink);
    expect(link.exists()).toBe(true);
  });
});
