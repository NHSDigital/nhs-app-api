import WebFooter from '@/components/widgets/WebFooter';
import en from '@/locale/en/index';
import { mount } from '../../helpers';

describe('WebFooter.vue', () => {
  let wrapper;

  const $env = {
    TERMS_AND_CONDITIONS_URL: 'https://terms',
    PRIVACY_POLICY_URL: 'https://privacy',
    HELP_AND_SUPPORT_URL: 'https://help',
    ACCESSIBILITY_STATEMENT_URL: 'https://accessibility',
  };
  const urlAndTexts = {
    'myAccount.termsAndConditions': { url: $env.TERMS_AND_CONDITIONS_URL, text: 'Terms of use' },
    'myAccount.privacyPolicy': { url: $env.PRIVACY_POLICY_URL, text: 'Privacy policy' },
    'myAccount.helpAndSupport': { url: $env.HELP_AND_SUPPORT_URL, text: 'Help and support' },
    'myAccount.accessibilityStatement': { url: $env.ACCESSIBILITY_STATEMENT_URL, text: 'Accessibility statement' },
  };

  const $tKey = translated => translated.replace('translate_', '');
  const toEnglish = (key) => {
    const expandedKeys = key.split('.');
    let value = en;
    expandedKeys.forEach((k) => {
      value = value[k];
    });
    return value;
  };

  const retrieveUrlAndText = (w) => {
    const linkText = w.element.innerHTML.trim();
    expect(linkText.includes('translate_')).toBeTruthy();
    const translateKey = $tKey(linkText);
    expect(translateKey).toBeDefined();
    const urlAndText = urlAndTexts[translateKey];
    expect(urlAndText).toBeDefined();
    return urlAndText;
  };

  beforeEach(() => {
    wrapper = mount(WebFooter, { $env });
  });


  it('will verify that links in footer are correctly generated', () => {
    const linkElements = wrapper.findAll('ul li a');
    expect(linkElements.length).toBeGreaterThan(0);
    expect(linkElements.length).toEqual(Object.keys($env).length);
    linkElements.wrappers.forEach((w) => {
      const urlAndText = retrieveUrlAndText(w);

      const url = w.element.getAttribute('href');
      expect(url).toEqual(urlAndText.url);
    });
  });

  it('will verify that link texts are correctly translated and generated', () => {
    const linkElements = wrapper.findAll('ul li a');
    expect(linkElements.length).toBeGreaterThan(0);
    linkElements.wrappers.forEach((w) => {
      const linkText = w.element.innerHTML.trim();
      const translateKey = $tKey(linkText);
      const actualLinkText = toEnglish(translateKey);

      const urlAndText = retrieveUrlAndText(w);
      expect(actualLinkText).toEqual(urlAndText.text);
    });
  });

  it('will verify that links are open in a new window or tab', () => {
    const linkElements = wrapper.findAll('ul li a');
    expect(linkElements.length).toBeGreaterThan(0);
    linkElements.wrappers.forEach((w) => {
      const targetAttr = w.element.getAttribute('target');
      expect(targetAttr).toEqual('_blank');
    });
  });
});
