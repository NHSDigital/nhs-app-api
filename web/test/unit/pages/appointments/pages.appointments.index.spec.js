import each from 'jest-each';
import Appointments from '@/pages/appointments';
import { createStore, createRouter, mount } from '../../helpers';

describe('appointments hub', () => {
  let wrapper;
  let $store;
  let $router;

  const mountAs = ({
    isProxy = false,
    silverIntegrationAppointmentsEnabled = false,
  } = {}) => {
    $router = createRouter();
    $store = createStore({
      state: {
        device: { isNativeApp: false },
      },
      getters: {
        'serviceJourneyRules/silverIntegrationAppointmentsEnabled': silverIntegrationAppointmentsEnabled,
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

    each([
      [true, false, true],
      [true, true, false],
      [false, false, false],
    ]).describe('secondary appointments enabled is %s, proxy is %s', (
      silverIntegrationAppointmentsEnabled, isProxy, expectedResult,
    ) => {
      let hospitalAppointmentsLink;

      it(`${expectedResult ? 'will' : 'will not'} show link`, () => {
        wrapper = mountAs({ silverIntegrationAppointmentsEnabled, isProxy });
        hospitalAppointmentsLink = getHospitalLink(wrapper);
        expect(hospitalAppointmentsLink.exists()).toBe(expectedResult);
      });
    });
  });
});
