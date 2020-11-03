import each from 'jest-each';
import HealthRecords from '@/pages/health-records';
import { createStore, mount, createRouter } from '../../helpers';

describe('healthRecords', () => {
  let linkElement;
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    integrationEnabled = true,
    isProxying = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp: false },
        knownServices: {
          knownServices: [{
            id: 'pkb',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => (integrationEnabled),
        'session/isProxying': isProxying,
      },
    });
    return mount(HealthRecords, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  describe('gp medical records link is always visible', () => {
    const linkElementGP = wrapperObj => wrapperObj.find('#btn_gp_medical_record');

    describe('silver integration rules enabled', () => {
      beforeEach(() => {
        wrapper = mountAs();
      });

      it('will show link', () => {
        expect(linkElementGP(wrapper).exists()).toBe(true);
      });
    });

    describe('silver integration rules disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ silverIntegrationSjrRules: [] });
      });

      it('will show link', () => {
        expect(linkElementGP(wrapper).exists()).toBe(true);
      });
    });

    describe('silver integration rules enabled, but proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ isProxying: true });
      });

      it('will show link', () => {
        expect(linkElementGP(wrapper).exists()).toBe(true);
      });
    });
  });

  describe('view third-party links', () => {
    each([
      ['pkb', 'Test Results', true, false, true],
      ['pkb', 'Test Results', true, true, false],
      ['pkb', 'Test Results', false, false, false],
      ['cie', 'Test Results', true, false, true],
      ['cie', 'Test Results', true, true, false],
      ['cie', 'Test Results', false, false, false],
      ['pkb', 'Care Plans', true, false, true],
      ['pkb', 'Care Plans', true, true, false],
      ['pkb', 'Care Plans', false, false, false],
      ['cie', 'Care Plans', true, false, true],
      ['cie', 'Care Plans', true, true, false],
      ['cie', 'Care Plans', false, false, false],
      ['pkb', 'Health Trackers', true, false, true],
      ['pkb', 'Health Trackers', true, true, false],
      ['pkb', 'Health Trackers', false, false, false],
      ['cie', 'Health Trackers', true, false, true],
      ['cie', 'Health Trackers', true, true, false],
      ['cie', 'Health Trackers', false, false, false],
      ['pkb', 'secondary shared links', true, false, true],
      ['pkb', 'secondary shared links', true, true, false],
      ['pkb', 'secondary shared links', false, false, false],
      ['cie', 'secondary shared links', true, false, true],
      ['cie', 'secondary shared links', true, true, false],
      ['cie', 'secondary shared links', false, false, false],
      ['gncr', 'correspondence', true, false, true],
      ['gncr', 'correspondence', true, true, false],
      ['gncr', 'correspondence', false, false, false],
    ]).describe('%s %s enabled is %s, proxy is %s', (
      provider, linkType, integrationEnabled, isProxying, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider + linkType.replace(/ /g, '')) {
          case 'pkbTestResults':
            linkElement = '#btn_pkb_test_results';
            break;
          case 'pkbCarePlans':
            linkElement = '#btn_pkb_care_plans';
            break;
          case 'cieCarePlans':
            linkElement = '#btn_pkb_cie_care_plans';
            break;
          case 'pkbHealthTrackers':
            linkElement = '#btn_pkb_health_trackers';
            break;
          case 'cieHealthTrackers':
            linkElement = '#btn_pkb_cie_health_trackers';
            break;
          case 'pkbSharedLinks':
            linkElement = '#btn_pkb_shared_links';
            break;
          case 'cieSharedLinks':
            linkElement = '#btn_pkb_cie_shared_links';
            break;
          case 'gncrMessages':
            linkElement = '#btn_gncr_messages_and_consultations';
            break;
          default:
            break;
        }

        wrapper = mountAs({ integrationEnabled, isProxying });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });
});
