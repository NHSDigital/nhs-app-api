import Glossary from '@/components/Glossary';
import { mount } from '../helpers';

jest.mock('@/lib/utils');

const mountGlossary = () => mount(Glossary, {
  $store: {
    state: { device: { isNativeApp: false } },
    $env: {
      CLINICAL_ABBREVIATIONS_URL: 'http://stubs.local.bitraft.io:8080/external/nhsuk/abbreviations',
    },
  },
});

describe('Glossary', () => {
  let wrapper;

  describe('no explicit path or text', () => {
    wrapper = mountGlossary();

    const link = wrapper.find('#glossary-link');

    it('will have the correct url as the href', () => {
      expect(link.attributes('href'))
        .toEqual('http://stubs.local.bitraft.io:8080/external/nhsuk/abbreviations');
    });
  });
});
