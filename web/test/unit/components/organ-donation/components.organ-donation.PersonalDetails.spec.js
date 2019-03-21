import PersonalDetails from '@/components/organ-donation/PersonalDetails';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, initFilters, mount } from '../../helpers';

describe('Personal details', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    initFilters();

    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });

    wrapper = mount(PersonalDetails, {
      $store,
      propsData:
      {
        name: 'name',
        dateOfBirth: '29 January 2019',
        gender: 'gender',
        nhsNumber: 'nhsnumber',
      },
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
      expect(subheaderText.text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.subheader');
    });
  });

  describe('detail headers', () => {
    let detailheaders;

    beforeEach(() => {
      detailheaders = wrapper.findAll('h4');
    });

    it('will exist', () => {
      expect(detailheaders.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(detailheaders.at(0).text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.nameheader');
      expect(detailheaders.at(1).text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.dateofbirthheader');
      expect(detailheaders.at(2).text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.genderheader');
      expect(detailheaders.at(3).text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.nhsnumberheader');
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
      expect(text.at(0).text()).toContain('name');
      expect(text.at(1).text()).toContain('29 January 2019');
      expect(text.at(2).text()).toContain('gender');
      expect(text.at(3).text()).toContain('nhsnumber');
      expect(text.at(4).text()).toContain('translate_organDonation.reviewYourDecision.personalDetails.description');
    });
  });
});
