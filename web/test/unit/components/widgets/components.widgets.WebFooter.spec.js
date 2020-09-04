import i18n from '@/plugins/i18n';
import WebFooter from '@/components/widgets/WebFooter';
import {
  TERMS_AND_CONDITIONS_URL,
  PRIVACY_POLICY_URL,
  HELP_AND_SUPPORT_URL,
  ACCESSIBILITY_STATEMENT_URL,
} from '@/router/externalLinks';
import { mount } from '../../helpers';

describe('WebFooter.vue', () => {
  let wrapper;

  const urlAndTexts = {
    'Terms of use': { url: TERMS_AND_CONDITIONS_URL, text: 'Terms of use' },
    'Privacy policy': { url: PRIVACY_POLICY_URL, text: 'Privacy policy' },
    'Help and support': { url: HELP_AND_SUPPORT_URL, text: 'Help and support' },
    'Accessibility statement': { url: ACCESSIBILITY_STATEMENT_URL, text: 'Accessibility statement' },
  };

  const retrieveUrlAndText = (w) => {
    const linkText = w.element.innerHTML.trim();
    const urlAndText = urlAndTexts[linkText];
    expect(urlAndText).toBeDefined();
    return urlAndText;
  };

  beforeEach(() => {
    wrapper = mount(WebFooter, { mountOpts: { i18n } });
  });

  it('will verify that links in footer are correctly generated', () => {
    const linkElements = wrapper.findAll('ul li a');
    expect(linkElements.length).toBeGreaterThan(0);
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
      const urlAndText = retrieveUrlAndText(w);
      expect(linkText).toEqual(urlAndText.text);
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
