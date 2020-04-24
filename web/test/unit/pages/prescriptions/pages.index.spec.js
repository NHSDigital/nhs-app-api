import PrescriptionsPage from '@/pages/prescriptions/index';
import { mount } from '../../helpers';
import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS_VIEW_ORDERS, NOMINATED_PHARMACY_INTERRUPT, NOMINATED_PHARMACY } from '@/lib/routes';

const createStore = ({
  nominatedPharmacyEnabled = false,
  pharmacyName = undefined,
  sjrEnabled = true,
  isProxying = false,
  isNativeApp = false,
  context = { serviceProvider: 'pkb',
    serviceType: 'messages' },
}) => ({
  dispatch: jest.fn(),
  app: {
    $env: {
      DEBOUNCE_SHORT: 500,
      DEBOUNCE_MEDIUM: 1000,
      DEBOUNCE_LONG: 2000,
    },
  },
  state: {
    device: { isNativeApp },
    knownServices: {
      knownServices: [{
        id: 'pkb',
        url: 'www.url.com',
      }],
    },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName,
      },
    },
  },
  getters: {
    'nominatedPharmacy/nominatedPharmacyEnabled': nominatedPharmacyEnabled,
    'serviceJourneyRules/nominatedPharmacyEnabled': sjrEnabled,
    'session/isProxying': isProxying,
    'nominatedPharmacy/pharmacyName': pharmacyName,
    'serviceJourneyRules/silverIntegrationEnabled': () => (context),
  },
});

const mountPage = ($store) => {
  const page = mount(PrescriptionsPage, {
    $store,
  });
  return page;
};

describe('prescriptions hub index page', () => {
  let $store;
  let wrapper;
  let repeatPrescriptionsButton;
  let nominatedPharmacyPanel;
  let viewOrdersLink;

  beforeEach(() => {
    process.server = false;
    process.client = true;
  });

  it('will call repeatPrescriptionsCourses init, clearInterruptBackTo, and clear/load nominatedPharmacy when nominatedPharmacy is enabled by sjr and nominated pharamcy is not already loaded', async () => {
    $store = createStore({ pharmacyName: 'boots', sjrEnabled: true });
    $store.state.nominatedPharmacy.hasLoaded = false;
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch');
    expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/init');
    expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/clearInterruptBackTo');
    expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/clear');
    expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/load');
  });

  it('will call repeatPrescriptionsCourses init and clearInterruptBackTo, but not clear/load nominatedPharmacy when nominatedPharmacy is enabled by sjr and nominated pharamcy is loaded', async () => {
    $store = createStore({ pharmacyName: 'boots', sjrEnabled: true });
    $store.state.nominatedPharmacy.hasLoaded = true;
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch');
    expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/init');
    expect($store.dispatch).toHaveBeenCalledWith('nominatedPharmacy/clearInterruptBackTo');
    expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/clear');
    expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/load');
  });

  it('will call repeatPrescriptionsCourses init but not clearInterruptBackTo, clear/load nominatedPharmacy when nominatedPharmacy is not enabled by sjr', async () => {
    $store = createStore({ pharmacyName: 'boots', sjrEnabled: false });
    $store.state.nominatedPharmacy.hasLoaded = false;
    wrapper = mountPage($store);
    jest.spyOn($store, 'dispatch');
    expect($store.dispatch).toHaveBeenCalledWith('repeatPrescriptionCourses/init');
    expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/clearInterruptBackTo');
    expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/clear');
    expect($store.dispatch).not.toHaveBeenCalledWith('nominatedPharmacy/load');
  });

  it('will have the repeat prescription button displayed', async () => {
    $store = createStore({ pharmacyName: 'boots', sjrEnabled: true });
    wrapper = mountPage($store);
    repeatPrescriptionsButton = wrapper.find('#repeat-prescription-button');
    expect(repeatPrescriptionsButton.text()).toEqual('translate_gpPrescriptionsHub.menuOptions.orderRepeat');
    expect(repeatPrescriptionsButton.exists()).toBe(true);
  });

  describe('when nominated pharmacy is enabled and nominated pharmacy set', () => {
    beforeEach(() => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true, pharmacyName: 'Boots' });
      wrapper = mountPage($store);
      nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      dependency.redirectTo = jest.fn();
      $store.app.$analytics = {
        trackButtonClick: jest.fn(),
      };
    });

    it('will display the correct nominated pharmacy information and panel title', async () => {
      expect(nominatedPharmacyPanel.exists()).toBe(true);
      expect(nominatedPharmacyPanel.find('h2').text()).toEqual('translate_gpPrescriptionsHub.menuOptions.yourNominatedPharmacy');
      expect(nominatedPharmacyPanel.find('p').text()).toEqual('Boots');
    });

    it('will redirect to the correct page when panel is clicked', async () => {
      wrapper.vm.onNominatedPharmacyDetailClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY.path);
    });
  });

  describe('when nominated pharmacy is enabled and no nominated pharmacy set', () => {
    beforeEach(() => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true });
      wrapper = mountPage($store);
      nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      dependency.redirectTo = jest.fn();
      $store.app.$analytics = {
        trackButtonClick: jest.fn(),
      };
    });

    it('will display the nominated pharmacy help text and correct panel title', async () => {
      expect(nominatedPharmacyPanel.exists()).toBe(true);
      expect(nominatedPharmacyPanel.find('h2').text()).toEqual('translate_gpPrescriptionsHub.menuOptions.nominatePharmacy');
      expect(nominatedPharmacyPanel.find('p').text()).toEqual('translate_gpPrescriptionsHub.menuOptions.nominatePharmacyHelpText');
    });

    it('will redirect to the correct page when panel is clicked', async () => {
      wrapper.vm.onNominatedPharmacyDetailClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT.path);
    });
  });

  describe('when in proxy mode', () => {
    it('will not display the nomianted pharmacy panel', async () => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true, isProxying: true });
      wrapper = mountPage($store);
      nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      expect(nominatedPharmacyPanel.exists()).toBe(false);
    });
  });

  describe('view orders link', () => {
    beforeEach(() => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true });
      wrapper = mountPage($store);
      viewOrdersLink = wrapper.find('#view-orders');
      dependency.redirectTo = jest.fn();
    });

    it('will exist', async () => {
      expect(viewOrdersLink.exists()).toBe(true);
    });

    it('will have the correct title and description', async () => {
      expect(viewOrdersLink.find('h2').text()).toEqual('translate_gpPrescriptionsHub.menuOptions.viewOrders');
      expect(viewOrdersLink.find('p').text()).toEqual('translate_gpPrescriptionsHub.menuOptions.viewOrdersHelpText');
    });

    it('will go the view orders page when clicked', async () => {
      wrapper.vm.onViewOrdersClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_VIEW_ORDERS.path);
    });
  });

  describe('patient knows best medicines jump off point', () => {
    const getPkbMedicinesJumpOff = wrapperObj =>
      wrapperObj.find('#btn_pkb_medicines');

    describe('pkb medicines enabled, is native app, and not proxying, ', () => {
      beforeEach(() => {
        $store = createStore({ context: true, isNativeApp: true, isProxying: false });
        wrapper = mountPage($store);
      });

      it('will display the pkb medicines jump off point', async () => {
        expect(getPkbMedicinesJumpOff(wrapper).exists()).toBe(true);
      });

      it('the jump off point will have correct title and description', async () => {
        expect(getPkbMedicinesJumpOff(wrapper).find('h2').text()).toEqual('Hospital and other prescriptions');
        expect(getPkbMedicinesJumpOff(wrapper).find('p').text()).toEqual('See your current and past prescriptions');
      });
    });

    describe('pkb medicines enabled, is native app, and proxying, ', () => {
      it('will not display the pkb medicines jump off point', async () => {
        $store = createStore({ context: true, isNativeApp: true, isProxying: true });
        wrapper = mountPage($store);
        expect(getPkbMedicinesJumpOff(wrapper).exists()).toBe(false);
      });
    });

    describe('pkb medicines enabled, is desktop, and not proxying, ', () => {
      it('will display the pkb medicines jump off point', async () => {
        $store = createStore({ context: true, isNativeApp: false, isProxying: false });
        wrapper = mountPage($store);
        expect(getPkbMedicinesJumpOff(wrapper).exists()).toBe(true);
      });
    });

    describe('pkb medicines disabled ', () => {
      it('will not display the pkb medicines jump off point', async () => {
        $store = createStore({ context: false, isNativeApp: true, isProxying: false });
        wrapper = mountPage($store);
        expect(getPkbMedicinesJumpOff(wrapper).exists()).toBe(false);
      });
    });
  });
});
