import Glossary from '@/components/Glossary';
import { mount } from '../helpers';

jest.mock('@/lib/utils');

const mountGlossary = () => mount(Glossary, {
  $store: {
    state: { device: { isNativeApp: false } },
    $env: { BASE_NHS_APP_HELP_URL: 'http://stubs.local.bitraft.io/help-and-support/' },
  },
});

describe('Glossary', () => {
  let wrapper;

  describe('no explicit path or text', () => {
    wrapper = mountGlossary();

    const link = wrapper.find('#glossary-link');

    it('will have the correct url as the href', () => {
      expect(link.attributes('href'))
        .toEqual('http://stubs.local.bitraft.io/help-and-support/health-records-in-the-nhs-app/abbreviations-commonly-found-in-medical-records/');
    });
  });
});
