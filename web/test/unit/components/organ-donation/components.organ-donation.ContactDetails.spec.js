import ContactDetails from '@/components/organ-donation/ContactDetails';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('contact details', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });

    wrapper = mount(ContactDetails, {
      $store,
      propsData: { address: 'address' },
    });
  });

  describe('subheader text', () => {
    let subheaderText;

    beforeEach(() => {
      subheaderText = wrapper.find('h3');
    });

    it('will exist', () => {
      expect(subheaderText.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(subheaderText.text()).toContain('translate_organDonation.reviewYourDecision.contactDetails.subheader');
    });
  });

  describe('address header', () => {
    let addressHeader;

    beforeEach(() => {
      addressHeader = wrapper.find('h4');
    });

    it('will exist', () => {
      expect(addressHeader.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(addressHeader.text()).toContain('translate_organDonation.reviewYourDecision.contactDetails.addressheader');
    });
  });

  describe('text', () => {
    let text;

    beforeEach(() => {
      text = wrapper.findAll('p');
    });

    it('will exist', () => {
      expect(text.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(text.at(0).text()).toContain('translate_organDonation.reviewYourDecision.contactDetails.text');
      expect(text.at(1).text()).toContain('address');
      expect(text.at(2).text()).toContain('translate_organDonation.reviewYourDecision.contactDetails.changeDetailsText');
    });
  });
});
