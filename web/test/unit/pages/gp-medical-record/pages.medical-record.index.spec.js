import each from 'jest-each';
import HealthRecords from '@/pages/health-records';
import { createStore, mount, createRouter } from '../../helpers';

describe('healthRecords', () => {
  let linkElement;
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    context = true,
    isProxying = false,
    isNativeApp = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp },
        knownServices: {
          knownServices: [{
            id: 'pkb',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxying,
      },
      $env: { YOUR_NHS_DATA_MATTERS_URL: 'testYourDataMattersUrl.com' },
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

  describe('view third-party care plans link', () => {
    each([
      ['pkb', 'Care Plans', true, false, true, true],
      ['pkb', 'Care Plans', true, true, true, false],
      ['pkb', 'Care Plans', false, false, true, false],
      ['cie', 'Care Plans', true, false, true, true],
      ['cie', 'Care Plans', true, true, true, false],
      ['cie', 'Care Plans', false, false, true, false],
      ['pkb', 'Health Trackers', true, false, true, true],
      ['pkb', 'Health Trackers', true, false, false, false],
      ['pkb', 'Health Trackers', true, true, true, false],
      ['pkb', 'Health Trackers', false, false, true, false],
      ['cie', 'Health Trackers', true, false, true, true],
      ['cie', 'Health Trackers', true, false, false, false],
      ['cie', 'Health Trackers', true, true, true, false],
      ['cie', 'Health Trackers', false, false, true, false],
    ]).describe('%s %s enabled is %s, proxy is %s, native is %s', (
      provider, linkType, context, isProxying, isNativeApp, expectedResult,
    ) => {
      switch (provider + linkType.replace(' ', '')) {
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
          linkElement = '#btn_pkb_health_trackers';
          break;
        default:
          break;
      }

      beforeEach(() => {
        wrapper = mountAs({ context, isProxying, isNativeApp });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });
});
