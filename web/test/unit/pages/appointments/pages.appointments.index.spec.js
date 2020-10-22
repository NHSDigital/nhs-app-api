import each from 'jest-each';
import Appointments from '@/pages/appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('appointments hub', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxying = false,
    silverIntegrationAppointmentsEnabled = false,
    silverIntegrationEnabled = false,
    cdssAdminEnabled = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp: false },
        knownServices: {
          knownServices: [{
            id: 'engage',
            url: 'www.url.com',
          }],
        },
      },
      getters: {
        'serviceJourneyRules/cdssAdminEnabled': cdssAdminEnabled,
        'serviceJourneyRules/silverIntegrationAppointmentsEnabled': silverIntegrationAppointmentsEnabled,
        'serviceJourneyRules/silverIntegrationEnabled': () => (silverIntegrationEnabled),
        'session/isProxying': isProxying,
      },
    });
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

  describe('admin help link', () => {
    each([
      ['will not', 'disabled', 'disabled', false, false, false],
      ['will not', 'enabled', 'enabled', true, true, false],
      ['will not', 'disabled', 'enabled', false, true, false],
      ['will', 'enabled', 'disabled', true, false, true],
    ]).it('%s show the admin help link when cdssAdmin %s and proxy %s', (_, __, ___, admin, proxy, expected) => {
      wrapper = mountAs({ cdssAdminEnabled: admin, isProxying: proxy });

      expect(wrapper.find('#btn_adminHelp').exists()).toBe(expected);
    });
  });

  describe('view third-party links', () => {
    let linkElement;

    each([
      ['engage', 'admin', true, false, true],
      ['engage', 'admin', true, true, false],
      ['engage', 'admin', false, false, false],
    ]).describe('%s %s enabled is %s, proxy is %s', (
      provider, _, silverIntegrationEnabled, isProxying, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider) {
          case 'engage':
            linkElement = '#btn_engage_admin';
            break;
          default:
            break;
        }

        wrapper = mountAs({ silverIntegrationEnabled, isProxying });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });

  describe('hospital link', () => {
    const getHospitalLink = wrapperObj => wrapperObj.find('#btn_hospital');

    each([
      [true, false, true],
      [true, true, false],
      [false, false, false],
    ]).describe('secondary appointments enabled is %s, proxy is %s', (
      silverIntegrationAppointmentsEnabled, isProxying, expectedResult,
    ) => {
      let hospitalAppointmentsLink;

      it(`${expectedResult ? 'will' : 'will not'} show link`, () => {
        wrapper = mountAs({ silverIntegrationAppointmentsEnabled, isProxying });
        hospitalAppointmentsLink = getHospitalLink(wrapper);
        expect(hospitalAppointmentsLink.exists()).toBe(expectedResult);
      });
    });
  });
});
