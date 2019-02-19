import AreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import { createStore, mount } from '../../helpers';

describe('alread registered link', () => {
  let wrapper;
  let $store;

  const mountAlreadyRegisteredLink = () => mount(AreadyRegisteredLink, { $store });

  beforeEach(() => {
    $store = createStore();
    wrapper = mountAlreadyRegisteredLink();
  });

  it('will display text from organDonation.links.alreadyRegisteredText', () => {
    expect(wrapper.text()).toEqual('translate_organDonation.links.alreadyRegisteredText');
  });

  describe('link', () => {
    const URL_EXTERNAL = 'www.foo.com';
    let link;

    beforeEach(() => {
      $store = createStore({ $env: { ORGAN_DONATION_ALREADY_REGISTERED_URL: URL_EXTERNAL } });
      wrapper = mountAlreadyRegisteredLink();
      link = wrapper.find('a');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have the external already registered link, with target set', () => {
      expect(link.attributes().target).toEqual('_blank');
      expect(link.attributes().href).toEqual(URL_EXTERNAL);
    });
  });
});
