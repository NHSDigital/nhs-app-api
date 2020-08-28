import AreadyRegisteredLink from '@/components/organ-donation/AlreadyRegisteredLink';
import i18n from '@/plugins/i18n';
import { ORGAN_DONATION_ALREADY_REGISTERED_URL } from '@/router/externalLinks';
import { createStore, mount } from '../../helpers';

describe('already registered link', () => {
  let wrapper;
  let $store;

  const mountAlreadyRegisteredLink = () => mount(
    AreadyRegisteredLink,
    {
      $store,
      mountOpts: { i18n },
    },
  );

  beforeEach(() => {
    $store = createStore();
    wrapper = mountAlreadyRegisteredLink();
  });

  it('will display text from organDonation.links.alreadyRegisteredText', () => {
    expect(wrapper.text()).toEqual('Think you have registered already?');
  });

  describe('link', () => {
    let link;

    beforeEach(() => {
      $store = createStore();
      wrapper = mountAlreadyRegisteredLink();
      link = wrapper.find('a');
    });

    it('will exist', () => {
      expect(link.exists()).toBe(true);
    });

    it('will have the external already registered link, with target set', () => {
      expect(link.attributes().target).toEqual('_blank');
      expect(link.attributes().href).toEqual(ORGAN_DONATION_ALREADY_REGISTERED_URL);
    });
  });
});
