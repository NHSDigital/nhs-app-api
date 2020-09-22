import each from 'jest-each';
import HospitalAppointments from '@/pages/appointments/hospital-appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('hospital appointments hub', () => {
  let linkElement;
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    context = true,
    isProxy = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp: false },
        knownServices: {
          knownServices: [{
            id: 'ers',
            url: 'www.url.com',
          }, {
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

  describe('view third-party jump-off links', () => {
    each([
      ['ers', 'Manage Your Referral', true, false, true],
      ['ers', 'Manage Your Referral', true, true, false],
      ['ers', 'Manage Your Referral', false, false, false],
      ['pkb', 'Appointments', true, false, true],
      ['pkb', 'Appointments', true, true, false],
      ['pkb', 'Appointments', false, false, false],
      ['cie', 'Appointments', true, false, true],
      ['cie', 'Appointments', true, true, false],
      ['cie', 'Appointments', false, false, false],
    ]).describe('%s %s enabled is %s, proxy is %s', (
      provider, linkType, context, isProxy, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider + linkType.replace(/ /g, '')) {
          case 'ersManageYourReferral':
            linkElement = '#btn_manage_your_referral';
            break;
          case 'cieAppointments':
            linkElement = '#btn_pkb_cie_appointments';
            break;
          case 'pkbAppointments':
            linkElement = '#btn_pkb_appointments';
            break;
          default:
            break;
        }

        wrapper = mountAs({ context, isProxy });
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });
});
