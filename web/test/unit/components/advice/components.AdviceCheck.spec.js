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
    isProofLevel9 = true,
    silverIntegrationEnabled = true,
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
        'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationEnabled),
        'session/isLoggedIn': isLoggedIn,
        'session/isProofLevel9': isProofLevel9,
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
      ['Coronavirus', 0, { h2: 'Get advice about coronavirus', p: 'Find out what to do if you think you have coronavirus' }],
      ['Conditions and treatments', 1, { h2: 'Search conditions and treatments', p: 'Find trusted NHS information on hundreds of conditions' }],
      ['NHS 111 online', 2, { h2: 'Use NHS 111 online', p: 'Check if you need urgent help and find out what to do next' }],
      ['CDSS GP advice', 3, { h2: 'Ask your GP for advice', p: 'Consult your GP through an online form. Your GP surgery will reply by phone or email' }],
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

  it('will hide the GP advice menu item if logged in but cdssAdvice is disabled in SJR', () => {
    mountComponent({ cdssAdviceEnabled: false });

    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
  });

  it('will show the GP advice menu item if logged in and cdssAdvice is enabled in SJR', () => {
    mountComponent();

    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(true);
  });

  it('will hide the GP advice button if not logged in', () => {
    mountComponent({ isLoggedIn: false });

    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
  });

  it('will hide the GP advice button if logged in and cdssAdvice is enabled in SJR but user is not P9', () => {
    mountComponent({ isProofLevel9: false });

    expect(wrapper.find('#btn_gpAdvice').exists()).toBe(false);
  });

  it('will show engage medical advice if logged in and silver integration is enabled and user is P9', () => {
    mountComponent();

    expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(true);
  });

  it('will hide engage medical advice if logged in and silver integration is enabled and user is not P9', () => {
    mountComponent({ isProofLevel9: false });

    expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(false);
  });

  it('will hide engage medical advice if logged in and silver integration is disabled and user is P9', () => {
    mountComponent({ silverIntegrationEnabled: false });

    expect(wrapper.find('#btn_engage_medical_advice').exists()).toBe(false);
  });
});
