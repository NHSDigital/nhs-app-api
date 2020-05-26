import HealthRecords from '@/pages/gp-medical-record';
import { createStore, mount, createRouter } from '../../helpers';

describe('healthRecords', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxying = false,
    isNativeApp = false,
    context = { serviceProvider: 'pkb',
      serviceType: 'carePlans' },
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
    const getGpMedicalRecordLink = wrapperObj =>
      wrapperObj.find('#btn_gp_medical_record');

    describe('pkb care plans enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true });
      });

      it('will show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb care plans is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: false });
      });

      it('will show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb care plans enabled, but proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isProxying: true });
      });

      it('will show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });
  });

  describe('pkb care plans link', () => {
    const getPkbCarePlansLink = wrapperObj =>
      wrapperObj.find('#btn_care_plans');

    describe('pkb care plans enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true });
      });

      it('will show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb care plans is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: false });
      });

      it('will not show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb care plans enabled but proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isProxying: true });
      });

      it('will not show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(false);
      });
    });
  });

  describe('pkb Health Trackers link', () => {
    const getPkbHealthTrackersLink = wrapperObj =>
      wrapperObj.find('#btn_health_trackers');

    describe('pkb health trackers enabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true });
      });

      it('will show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb health trackers is enabled, but is desktop', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: false });
      });

      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb health trackers is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ isNativeApp: true });
      });

      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb health trackers enabled, but proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true, isProxying: true });
      });

      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });
  });
});
