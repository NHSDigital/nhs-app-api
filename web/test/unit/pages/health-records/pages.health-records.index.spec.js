import each from 'jest-each';
import HealthRecords from '@/pages/health-records';
import * as utils from '@/lib/utils';
import { DATA_SHARING_OVERVIEW_PATH } from '@/router/paths';
import { YOUR_NHS_DATA_MATTERS_URL } from '@/router/externalLinks';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { createStore, mount, createRouter } from '../../helpers';

describe('healthRecords', () => {
  let linkElement;
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    integrationEnabled = true,
    isProxying = false,
    isNativeApp = false,
    ndopEnabled = true,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp },
      },
      $env: {
        YOUR_NHS_DATA_MATTERS_URL: 'https://www.nhs.uk/your-nhs-data-matters/',
      },
      getters: {
        'knownServices/matchOneById': id => ({
          id,
          url: 'www.url.com',
        }),
        'serviceJourneyRules/silverIntegrationEnabled': () => (integrationEnabled),
        'session/isProxying': isProxying,
        'serviceJourneyRules/ndopEnabled': ndopEnabled,
      },
    });
    return mount(HealthRecords, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
    utils.redirectTo = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  describe('visible links', () => {
    describe('when not proxying', () => {
      beforeEach(() => {
        wrapper = mountAs();
      });
      it('will include the organ donation link', () => {
        expect(wrapper.find(OrganDonationLink).exists()).toBe(true);
      });
      it('will include the ndop link', () => {
        expect(wrapper.find('#btn_data_sharing').exists()).toBe(true);
      });
    });

    describe('when proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ isProxying: true });
      });
      it('will not include the organ donation link', () => {
        expect(wrapper.find(OrganDonationLink).exists()).toBe(false);
      });
      it('will not include the ndop link', () => {
        expect(wrapper.find('#btn_data_sharing').exists()).toBe(false);
      });
    });
  });

  describe('navigateToDataSharing', () => {
    it('will redirect to DATA_SHARING_OVERVIEW_PATH if native', () => {
      wrapper = mountAs({ isNativeApp: true });
      wrapper.vm.navigateToDataSharing();
      expect(utils.redirectTo).toHaveBeenCalledWith(wrapper.vm, DATA_SHARING_OVERVIEW_PATH);
    });

    it('will navigate to ndop home page if not native', () => {
      wrapper = mountAs();
      wrapper.vm.navigateToDataSharing();
      expect(window.open).toHaveBeenCalledWith(YOUR_NHS_DATA_MATTERS_URL, '_blank');
    });
  });

  describe('ndopDisabled', () => {
    const linkElementNdop = wrapperObj => wrapperObj.find('#btn_data_sharing');
    it('will not show ndop link', () => {
      wrapper = mountAs({ ndopEnabled: false });
      expect(linkElementNdop(wrapper).exists()).toBe(false);
    });
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

    describe('ndop enabled', () => {
      beforeEach(() => {
        wrapper = mountAs();
      });

      it('will show link', () => {
        expect(linkElementGP(wrapper).exists()).toBe(true);
      });
    });

    describe('ndop disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ ndopEnabled: false });
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
      ['pkb', 'Care Plans', true, false, true],
      ['pkb', 'Care Plans', true, true, false],
      ['pkb', 'Care Plans', false, false, false],
      ['pkb', 'Health Trackers', true, false, true],
      ['pkb', 'Health Trackers', true, true, false],
      ['pkb', 'Health Trackers', false, false, false],
      ['pkb', 'secondary shared links', true, false, true],
      ['pkb', 'secondary shared links', true, true, false],
      ['pkb', 'secondary shared links', false, false, false],
      ['pkb', 'Record Sharing', true, false, true],
      ['pkb', 'Record Sharing', true, true, false],
      ['pkb', 'Record Sharing', false, false, false],
      ['gncr', 'correspondence', true, false, true],
      ['gncr', 'correspondence', true, true, false],
      ['gncr', 'correspondence', false, false, false],
      ['nhsd', 'Vaccine Record', true, false, true],
      ['nhsd', 'Vaccine Record', true, true, false],
      ['nhsd', 'Vaccine Record', false, false, false],
      ['netCompany', 'Vaccine Record', true, false, true],
      ['netCompany', 'Vaccine Record', true, true, false],
      ['netCompany', 'Vaccine Record', false, false, false],
      ['wellnessAndPrevention', 'Wellness and Prevention', true, false, true],
      ['wellnessAndPrevention', 'Wellness and Prevention', true, true, false],
      ['wellnessAndPrevention', 'Wellness and Prevention', false, false, false],
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
          case 'pkbHealthTrackers':
            linkElement = '#btn_pkb_health_trackers';
            break;
          case 'pkbSharedLinks':
            linkElement = '#btn_pkb_shared_links';
            break;
          case 'gncrMessages':
            linkElement = '#btn_gncr_messages_and_consultations';
            break;
          case 'nhsdVaccineRecord':
            linkElement = '#btn_nhsd_vaccine_record';
            break;
          case 'netCompanyVaccineRecord':
            linkElement = '#btn_netCompany_vaccine_record';
            break;
          case 'pkbRecordSharing':
            linkElement = '#btn_pkb_record_sharing';
            break;
          case 'wellnessAndPreventionWellnessandPrevention':
            linkElement = '#btn_wellness_and_prevention';
            break;
          default:
            break;
        }

        wrapper = mountAs({ integrationEnabled, isProxying });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(linkElement).toBeDefined();
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });
});
