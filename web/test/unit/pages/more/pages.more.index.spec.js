import More from '@/pages/more';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createStore, mount } from '../../helpers';

describe('more', () => {
  let wrapper;
  let $store;

  const mountAs = (native = false) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
      },
    });

    return mount(More, { $store });
  };

  beforeEach(() => {
    wrapper = mountAs();
  });

  it('will include the organ donation link', () => {
    const link = wrapper.find(OrganDonationLink);
    expect(link.exists()).toBe(true);
  });
});
