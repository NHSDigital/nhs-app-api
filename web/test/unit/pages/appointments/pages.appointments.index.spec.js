import Appointments from '@/pages/appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('appointments hub', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxy = false,
    apptsProviders = [],
    context = false,
    isNativeApp = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: {
          isNativeApp,
        },
        serviceJourneyRules: {
          isLoaded: false,
          rules: {
            silverIntegrations: {
              secondaryAppointments: apptsProviders,
            },
          },
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxy,
      } });
    return mount(Appointments, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  it('will include the gp appointments link', () => {
    expect(wrapper.find('#btn_choices').exists()).toBe(true);
  });

  describe('hospital link', () => {
    const getHospitalLink = wrapperObj => wrapperObj.find('#btn_hospital');

    describe('sjr secondary appointments disabled but proxy false', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: [], isProxy: false });
      });

      it('will not show link', () => {
        expect(getHospitalLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr secondary appointments enabled but proxy true', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: [], isProxy: true });
      });

      it('will not show link', () => {
        expect(getHospitalLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr secondary appointments enabled and proxy false', () => {
      let hospitalAppointmentsLink;
      it('will show link for pkb', () => {
        wrapper = mountAs({ apptsProviders: ['pkb'], context: true, isProxy: false, isNativeApp: true });
        hospitalAppointmentsLink = getHospitalLink(wrapper);
        expect(hospitalAppointmentsLink.exists()).toBe(true);
      });
      it('will show link for ers', () => {
        wrapper = mountAs({ apptsProviders: ['ers'], context: true, isProxy: false });
        hospitalAppointmentsLink = getHospitalLink(wrapper);
        expect(hospitalAppointmentsLink.exists()).toBe(true);
      });
    });
  });
});
