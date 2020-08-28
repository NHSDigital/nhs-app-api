import FaithDetailsRegistered from '@/components/organ-donation/FaithDetailsRegistered';
import i18n from '@/plugins/i18n';
import { initialState } from '@/store/modules/organDonation/mutation-types';
import { createStore, mount } from '../../helpers';

describe('faith details registered', () => {
  let wrapper;
  let $store;

  beforeEach(() => {
    $store = createStore({
      state: {
        organDonation: initialState(),
      },
    });
  });

  describe('declaration Yes text', () => {
    let declarationText;

    beforeEach(() => {
      wrapper = mount(FaithDetailsRegistered, {
        $store,
        propsData: { declaration: 'Yes' },
        mountOpts: { i18n },
      });

      declarationText = wrapper.find('p');
    });

    it('will exist', () => {
      expect(declarationText.exists()).toBe(true);
    });

    it('will show the Yes text', () => {
      expect(declarationText.text()).toContain('When I die, I would like NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.');
    });
  });

  describe('declaration No text', () => {
    let declarationText;

    beforeEach(() => {
      wrapper = mount(FaithDetailsRegistered, {
        $store,
        propsData: { declaration: 'No' },
        mountOpts: { i18n },
      });

      declarationText = wrapper.find('p');
    });

    it('will exist', () => {
      expect(declarationText.exists()).toBe(true);
    });

    it('will show the No text', () => {
      expect(declarationText.text()).toContain('When I die, I do not want NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.');
    });
  });

  describe('declaration NotStated text', () => {
    let declarationText;

    beforeEach(() => {
      wrapper = mount(FaithDetailsRegistered, {
        $store,
        propsData: { declaration: 'NotStated' },
        mountOpts: { i18n },
      });

      declarationText = wrapper.find('p');
    });

    it('will exist', () => {
      expect(declarationText.exists()).toBe(true);
    });

    it('will show the NotStated text', () => {
      expect(declarationText.text()).toContain('I prefer not to say whether I want NHS staff to speak with my family (and anyone else appropriate) about how organ donation can go ahead in line with my faith and beliefs.');
    });
  });
});
