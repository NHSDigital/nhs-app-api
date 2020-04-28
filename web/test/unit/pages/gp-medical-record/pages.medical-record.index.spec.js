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

    describe('pkb care plans enabled and is native', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true });
      });

      it('will show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb enabled but is desktop', () => {
      it('will not show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb messaging is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ isNativeApp: true });
      });

      it('will not show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });

    describe('is pkb enabled, native and proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true, isProxying: true });
      });

      it('will not show link', () => {
        expect(getGpMedicalRecordLink(wrapper).exists()).toBe(true);
      });
    });
  });

  describe('pkb care plans link', () => {
    const getPkbCarePlansLink = wrapperObj =>
      wrapperObj.find('#btn_care_plans');

    describe('pkb care plans enabled and is native', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true });
      });

      it('will show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb enabled but is desktop', () => {
      it('will not show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb messaging is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ isNativeApp: true });
      });

      it('will not show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(false);
      });
    });

    describe('is pkb enabled, native and proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true, isProxying: true });
      });

      it('will not show link', () => {
        expect(getPkbCarePlansLink(wrapper).exists()).toBe(false);
      });
    });
  });

  describe('pkb Health Trackers link', () => {
    const getPkbHealthTrackersLink = wrapperObj =>
      wrapperObj.find('#btn_health_trackers');

    describe('pkb care plans enabled and is native', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true });
      });

      it('will show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb enabled but is desktop', () => {
      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb messaging is disabled', () => {
      beforeEach(() => {
        wrapper = mountAs({ isNativeApp: true });
      });

      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });

    describe('is pkb enabled, native and proxying', () => {
      beforeEach(() => {
        wrapper = mountAs({ context: true, isNativeApp: true, isProxying: true });
      });

      it('will not show link', () => {
        expect(getPkbHealthTrackersLink(wrapper).exists()).toBe(false);
      });
    });
  });
});
