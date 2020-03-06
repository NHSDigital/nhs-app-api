import HospitalAppointments from '@/pages/appointments/hospital-appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('hospital appointments hub', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxy = false,
    isNativeApp = false,
    apptsProviders = [],
    context = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        serviceJourneyRules: {
          isLoaded: false,
          rules: {
            silverIntegrations: {
              secondaryAppointments: apptsProviders,
            },
          },
        },
        device: {
          isNativeApp,
        },
        knownServices: {
          knownServices: [{
            id: 'ers',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationEnabled': () => (context),
        'session/isProxying': isProxy,
      } });
    return mount(HospitalAppointments, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs();
    window.open = jest.fn();
  });

  it('will dispatch device/unlockNavBar when page mounted', () => {
    expect($store.dispatch).toHaveBeenCalledWith('device/unlockNavBar');
  });

  describe('manage your referrals link', () => {
    const getHospitalLink = wrapperObj => wrapperObj.find('#btn_manage_your_referral');

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
      it('will show link', () => {
        wrapper = mountAs({ apptsProviders: ['ers'], context: true, isProxy: false });
        hospitalAppointmentsLink = getHospitalLink(wrapper);
        expect(hospitalAppointmentsLink.exists()).toBe(true);
      });
    });
  });
});
