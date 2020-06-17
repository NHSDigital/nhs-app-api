import FindOutMoreLink from '@/components/organ-donation/FindOutMoreLink';
import { ORGAN_DONATION_FIND_OUT_MORE_URL } from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

describe('find out more link', () => {
  let wrapper;
  let $store;

  const mountFindOutMoreLink = () => mount(FindOutMoreLink, { $store });

  beforeEach(() => {
    $store = createStore();
    wrapper = mountFindOutMoreLink();
  });

  it('will display text from organDonation.links.findOutMoreText', () => {
    expect(wrapper.text()).toEqual('translate_organDonation.links.findOutMoreText');
  });

  describe('link', () => {
    let link;

    beforeEach(() => {
      $store = createStore();
      wrapper = mountFindOutMoreLink();
      link = wrapper.find('a');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have the external find out more link, with target set', () => {
      expect(link.attributes().target).toEqual('_blank');
      expect(link.attributes().href).toEqual(ORGAN_DONATION_FIND_OUT_MORE_URL);
    });
  });
});
