import i18n from '@/plugins/i18n';
import MoreAboutOrgans from '@/pages/organ-donation/more-about-organs';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, locale, mount } from '../../helpers';

const createState = () => ({
  organDonation: initialState(),
  device: {
    isNativeApp: true,
  },
});

describe('organ donation more about organs', () => {
  let $store;
  let wrapper;

  const mountMoreAboutOrgans = () => mount(MoreAboutOrgans, {
    $store,
    mountOpts: { i18n },
  });

  beforeEach(() => {
    $store = createStore({ state: createState() });
    wrapper = mountMoreAboutOrgans();
  });

  it('will show the header text', () => {
    expect(wrapper.find('h2').text()).toEqual('About organs and tissue');
  });

  describe('content items', () => {
    let subheaders;
    let bodies;

    beforeEach(() => {
      subheaders = wrapper.findAll('h3');
      bodies = wrapper.findAll('p');
    });

    it('will add each content item with a subheader', () => {
      const items = locale.organDonation.moreAboutOrgans.contentItems;
      items.forEach((item, index) => expect(subheaders.at(index).text()).toBe(item.subheader));
    });

    it('will add each content item with a body', () => {
      const items = locale.organDonation.moreAboutOrgans.contentItems;
      items.forEach((item, index) => expect(bodies.at(index).text()).toBe(item.body));
    });
  });
});
