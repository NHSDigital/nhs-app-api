import i18n from '@/plugins/i18n';
import MenuItem from '@/components/MenuItem';
import AdviceCheck from '@/components/advice/AdviceCheck';
import * as dependency from '@/lib/utils';
import each from 'jest-each';
import { createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

describe('Advice Check Menu', () => {
  let $store;
  let wrapper;

  const mountComponent = ({
    isLoggedIn = true,
    cdssAdviceEnabled = true,
    coronavirusInformationEnabled = true,
    isProofLevel9 = true,
    silverIntegrationEnabled = true,
    oneOneOneEnabled = true,
  } = {}) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
      getters: {
        'knownServices/matchOneById': id => ({
          id,
          url: 'www.url.com',
        }),
        'serviceJourneyRules/cdssAdviceEnabled': cdssAdviceEnabled,
        'serviceJourneyRules/coronavirusInformationEnabled': coronavirusInformationEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationEnabled),
        'session/isLoggedIn': () => isLoggedIn,
        'session/isProofLevel9': isProofLevel9,
        'serviceJourneyRules/oneOneOneEnabled': oneOneOneEnabled,
      },
    });

    wrapper = mount(AdviceCheck, { $store, mountOpts: { i18n } });
  };

  describe('Menu item content', () => {
    let menuItems;
    let menuItem;

    beforeEach(() => {
      mountComponent();
      menuItems = wrapper.findAll(MenuItem);
    });

    it('will contain the correct number of items ', () => {
      expect(menuItems.length).toBe(5);
    });

    each([
      ['Coronavirus', 0, { h2: 'Get advice about coronavirus (COVID-19)', p: 'Find information about COVID-19, including symptoms, testing, vaccination and self-isolation' }],
      ['Conditions and treatments', 1, { h2: 'Search conditions and treatments', p: 'Find trusted NHS information on hundreds of conditions' }],
      ['NHS 111 online', 2, { h2: 'Use NHS 111 online', p: 'Check if you need urgent help and find out what to do next' }],
      ['CDSS GP advice', 3, { h2: 'Ask your GP for advice', p: 'Answer questions online and get a response from your GP surgery. You may be able to get advice for your child if it\'s available at your surgery' }],
      ['Engage GP advice', 4, { h2: 'Ask your GP for advice', p: 'Answer questions online and get a response from your GP surgery' }],
    ]).describe('%s menu item', (_, position, text) => {
      beforeEach(() => {
        menuItem = menuItems.at(position);
      });

      it('will contain the correct header content', () => {
        expect(menuItem.find('h2').text()).toContain(text.h2);
      });

      it('will contain the correct paragraph content', () => {
        expect(menuItem.find('p').text()).toContain(text.p);
      });
    });
  });

  describe('If the user is not logged in', () => {
    beforeEach(() => {
      mountComponent({ isLoggedIn: false });
    });

    it('will hide the coronavirus advice button', () => {
      expect(wrapper.find('#btn_corona').exists()).toBe(false);
    });

    it('will hide the GP advice button', () => {
      expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
    });

    it('will hide the one one one menu item', () => {
      expect(wrapper.find('#btn_111').exists()).toBe(false);
    });
  });

  describe('If the user is logged in', () => {
    const isLoggedIn = true;

    describe('and coronavirus information is disabled in SJR', () => {
      mountComponent({ isLoggedIn, coronavirusInformationEnabled: false });

      it('will hide the coronavirus advice button', () => {
        expect(wrapper.find('#btn_corona').exists()).toBe(false);
      });
    });

    describe('and coronavirus information is enabled in SJR', () => {
      beforeEach(() => {
        mountComponent({ isLoggedIn, coronavirusInformationEnabled: true });
      });

      it('will show the coronavirus advice button', () => {
        expect(wrapper.find('#btn_corona').exists()).toBe(true);
      });
    });

    describe('and one one one is disabled in SJR', () => {
      beforeEach(() => {
        mountComponent({ isLoggedIn, oneOneOneEnabled: false });
      });

      it('will hide the one one one menu item', () => {
        expect(wrapper.find('#btn_111').exists()).toBe(false);
      });
    });

    describe('and one one one is enabled in SJR', () => {
      beforeEach(() => {
        mountComponent({ isLoggedIn, oneOneOneEnabled: true });
      });

      it('will show the one one one menu item', () => {
        expect(wrapper.find('#btn_111').exists()).toBe(true);
      });
    });

    describe('and cdss advice is disabled in SJR', () => {
      beforeEach(() => {
        mountComponent({ isLoggedIn, cdssAdviceEnabled: false });
      });

      it('will hide the GP advice menu item', () => {
        expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
      });
    });

    describe('and cdss advice is enabled in SJR', () => {
      const cdssAdviceEnabled = true;

      describe('and user is P9', () => {
        beforeEach(() => {
          mountComponent({ isLoggedIn, cdssAdviceEnabled, isProofLevel9: true });
        });

        it('will show the GP advice menu item', () => {
          expect(wrapper.find('#btn_gpAdvice').exists()).toBe(true);
        });
      });

      describe('and user is not P9', () => {
        beforeEach(() => {
          mountComponent({ isLoggedIn, cdssAdviceEnabled, isProofLevel9: false });
        });

        it('will hide the GP advice menu item', () => {
          expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
        });
      });
    });

    describe('and silver integration is disabled in SJR', () => {
      beforeEach(() => {
        mountComponent({ isLoggedIn, silverIntegrationEnabled: false });
      });

      it('will hide engage medical advice', () => {
        expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(false);
      });
    });

    describe('and silver integration is enabled in SJR', () => {
      const silverIntegrationEnabled = true;

      describe('and user is P9', () => {
        beforeEach(() => {
          mountComponent({ isLoggedIn, silverIntegrationEnabled, isProofLevel9: true });
        });

        it('will show engage medical advice', () => {
          expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(true);
        });
      });

      describe('and user is not P9', () => {
        beforeEach(() => {
          mountComponent({ isLoggedIn, silverIntegrationEnabled, isProofLevel9: false });
        });

        it('will hide engage medical advice', () => {
          expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(false);
        });
      });
    });
  });
});
