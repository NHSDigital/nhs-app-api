import AdditionalInformation from '@/components/organ-donation/AdditionalInformation';
import i18n from '@/plugins/i18n';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('Additional information', () => {
  let wrapper;
  let $store;

  const mountAdditionalInformation = ({
    ethnicityId,
    religionId,
    religions,
    ethnicities }) => mount(AdditionalInformation, {
    $store,
    propsData: {
      ethnicityId,
      religionId,
      referenceData: {
        ethnicities,
        religions,
      },
    },
    mountOpts: { i18n },
  });

  const ethnicitiesRefData = [
    { id: 1, displayName: 'Asian or Asian British' },
    { id: 2, displayName: 'Black or Black British' },
    { id: 3, displayName: 'White - British' },
  ];

  const religionsRefData = [
    { id: 4, displayName: 'No religion' },
    { id: 5, displayName: 'Christian' },
    { id: 6, displayName: 'Buddhist' },
  ];

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });

    wrapper = mountAdditionalInformation({
      ethnicityId: '',
      religionId: '',
      ethnicities: ethnicitiesRefData,
      religions: religionsRefData,
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
      expect(subheaderText.text()).toContain('Additional information');
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
      expect(detailheaders.at(0).text()).toContain('Ethnicity');
      expect(detailheaders.at(1).text()).toContain('Religion');
    });
  });

  describe('text when no ethnicity or religion', () => {
    let text;

    beforeEach(() => {
      text = wrapper.findAll('p');
    });

    it('will exist', () => {
      expect(text.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(text.at(0).text()).toContain('You did not answer');
      expect(text.at(1).text()).toContain('You did not answer');
      expect(text.at(2).text()).toContain('This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.');
    });
  });

  describe('ethnicity id and religion id included', () => {
    let text;

    beforeEach(() => {
      wrapper = mountAdditionalInformation({
        ethnicityId: '1',
        religionId: '4',
        ethnicities: ethnicitiesRefData,
        religions: religionsRefData,
      });
      text = wrapper.findAll('p');
    });

    it('will exist', () => {
      expect(text.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(text.at(0).text()).toContain('Asian or Asian British');
      expect(text.at(1).text()).toContain('No religion');
      expect(text.at(2).text()).toContain('This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.');
    });
  });

  describe('ethnicity id included but no religion id', () => {
    let text;

    beforeEach(() => {
      wrapper = mountAdditionalInformation({
        ethnicityId: '1',
        religionId: '',
        ethnicities: ethnicitiesRefData,
        religions: religionsRefData,
      });
      text = wrapper.findAll('p');
    });

    it('will exist', () => {
      expect(text.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(text.at(0).text()).toContain('Asian or Asian British');
      expect(text.at(1).text()).toContain('You did not answer');
      expect(text.at(2).text()).toContain('This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.');
    });
  });

  describe('religion id included but no ethnicity id', () => {
    let text;

    beforeEach(() => {
      wrapper = mountAdditionalInformation({
        ethnicityId: '',
        religionId: '5',
        ethnicities: ethnicitiesRefData,
        religions: religionsRefData,
      });
      text = wrapper.findAll('p');
    });

    it('will exist', () => {
      expect(text.exists()).toBe(true);
    });

    it('will show the text', () => {
      expect(text.at(0).text()).toContain('You did not answer');
      expect(text.at(1).text()).toContain('Christian');
      expect(text.at(2).text()).toContain('This optional information is only used by the NHS to understand the make up of the NHS Organ Donor Register and is not stored against your registration.');
    });
  });
});
