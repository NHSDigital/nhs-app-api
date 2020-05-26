import HospitalAppointments from '@/pages/appointments/hospital-appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('hospital appointments hub', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxy = false,
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
        device: { isNativeApp: false },
        knownServices: {
          knownServices: [{
            id: 'ers',
            url: 'www.url.com',
          },
          {
            id: 'pkb',
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
    const getMyrLink = wrapperObj => wrapperObj.find('#btn_manage_your_referral');

    describe('sjr ers secondary appointments disabled but proxy false', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: [], isProxy: false });
      });

      it('will not show link', () => {
        expect(getMyrLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr ers secondary appointments enabled but proxy true', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: [], isProxy: true });
      });

      it('will not show link', () => {
        expect(getMyrLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr ers secondary appointments enabled and proxy false', () => {
      let myrAppointmentsLink;
      it('will show link', () => {
        wrapper = mountAs({ apptsProviders: ['ers'], context: true, isProxy: false });
        myrAppointmentsLink = getMyrLink(wrapper);
        expect(myrAppointmentsLink.exists()).toBe(true);
      });
    });
  });
  describe('view pkb appointments', () => {
    const getPkbAppointmentsLink = wrapperObj => wrapperObj.find('#btn_pkb_appointments');

    describe('sjr pkb secondary appointments disabled but proxy false', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: [], isProxy: false });
      });

      it('will not show link', () => {
        expect(getPkbAppointmentsLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr pkb secondary appointments enabled but proxy true', () => {
      beforeEach(() => {
        wrapper = mountAs({ apptsProviders: ['pkb'], isProxy: true });
      });

      it('will not show link', () => {
        expect(getPkbAppointmentsLink(wrapper).exists()).toBe(false);
      });
    });

    describe('sjr pkb secondary appointments enabled and proxy false', () => {
      let pkbAppointmentsLink;
      it('will show link', () => {
        wrapper = mountAs({ apptsProviders: ['pkb'], context: true, isProxy: false });
        pkbAppointmentsLink = getPkbAppointmentsLink(wrapper);
        expect(pkbAppointmentsLink.exists()).toBe(true);
      });
    });
  });
});
