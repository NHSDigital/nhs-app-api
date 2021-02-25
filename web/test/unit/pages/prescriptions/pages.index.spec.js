import each from 'jest-each';
import i18n from '@/plugins/i18n';
import PrescriptionsPage from '@/pages/prescriptions/index';
import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS_VIEW_ORDERS_PATH, NOMINATED_PHARMACY_INTERRUPT_PATH, NOMINATED_PHARMACY_PATH } from '@/router/paths';
import { mount } from '../../helpers';

let linkElement;

const createStore = ({
  nominatedPharmacyEnabled = false,
  pharmacyName = undefined,
  sjrEnabled = true,
  isProxying = false,
  isNativeApp = false,
  context = true,
}) => ({
  dispatch: jest.fn(),
  $env: {},
  state: {
    device: { isNativeApp },
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName,
      },
    },
  },
  getters: {
    'knownServices/matchOneById': id => ({
      id,
      url: 'www.url.com',
    }),
    'nominatedPharmacy/nominatedPharmacyEnabled': nominatedPharmacyEnabled,
    'nominatedPharmacy/pharmacyName': pharmacyName,
    'serviceJourneyRules/nominatedPharmacyEnabled': sjrEnabled,
    'serviceJourneyRules/silverIntegrationEnabled': () => (context),
    'session/isProxying': isProxying,
  },
});

const mountPage = ($store) => {
  const page = mount(PrescriptionsPage, {
    $store,
    mountOpts: {
      i18n,
    },
  });
  return page;
};

describe('prescriptions hub index page', () => {
  let $store;
  let wrapper;
  let repeatPrescriptionsButton;
  let nominatedPharmacyPanel;
  let viewOrdersLink;

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
    expect(repeatPrescriptionsButton.text()).toEqual('Order a repeat prescription');
    expect(repeatPrescriptionsButton.exists()).toBe(true);
  });

  describe('when nominated pharmacy is enabled and nominated pharmacy set', () => {
    beforeEach(() => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true, pharmacyName: 'Boots' });
      $store.app = { $analytics: { trackButtonClick: jest.fn() } };
      wrapper = mountPage($store);
      nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      dependency.redirectTo = jest.fn();
    });

    it('will display the correct nominated pharmacy information and panel title', async () => {
      expect(nominatedPharmacyPanel.exists()).toBe(true);
      expect(nominatedPharmacyPanel.find('h2').text()).toEqual('Your nominated pharmacy');
      expect(nominatedPharmacyPanel.find('p').text()).toEqual('Boots');
    });

    it('will redirect to the correct page when panel is clicked', async () => {
      wrapper.vm.onNominatedPharmacyDetailClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_PATH);
    });
  });

  describe('when nominated pharmacy is enabled and no nominated pharmacy set', () => {
    beforeEach(() => {
      $store = createStore({ nominatedPharmacyEnabled: true, sjrEnabled: true });
      $store.app = { $analytics: { trackButtonClick: jest.fn() } };
      wrapper = mountPage($store);
      nominatedPharmacyPanel = wrapper.find('#nominated-pharmacy');
      dependency.redirectTo = jest.fn();
    });

    it('will display the nominated pharmacy help text and correct panel title', async () => {
      expect(nominatedPharmacyPanel.exists()).toBe(true);
      expect(nominatedPharmacyPanel.find('h2').text()).toEqual('Nominate a pharmacy');
      expect(nominatedPharmacyPanel.find('p').text()).toEqual('Choose a pharmacy for your prescriptions to be sent to');
    });

    it('will redirect to the correct page when panel is clicked', async () => {
      wrapper.vm.onNominatedPharmacyDetailClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, NOMINATED_PHARMACY_INTERRUPT_PATH);
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
      expect(viewOrdersLink.find('h2').text()).toEqual('View your orders');
      expect(viewOrdersLink.find('p').text()).toEqual('See repeat prescriptions you have ordered');
    });

    it('will go the view orders page when clicked', async () => {
      wrapper.vm.onViewOrdersClicked();
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_VIEW_ORDERS_PATH);
    });
  });

  describe('view third-party medicines link', () => {
    each([
      ['pkb', true, false, true],
      ['pkb', true, true, false],
      ['pkb', false, false, false],
      ['cie', true, false, true],
      ['cie', true, true, false],
      ['cie', false, false, false],
      ['pkbSecondaryCare', true, false, true],
      ['pkbSecondaryCare', true, true, false],
      ['pkbSecondaryCare', false, false, false],
      ['myCareView', true, false, true],
      ['myCareView', true, true, false],
      ['myCareView', false, false, false],
    ]).describe('%s medicines enabled is %s, proxy is %s', (
      provider, context, isProxying, expectedResult,
    ) => {
      beforeEach(() => {
        switch (provider) {
          case 'cie':
            linkElement = '#btn_pkb_cie_medicines';
            break;
          case 'pkb':
            linkElement = '#btn_pkb_medicines';
            break;
          case 'pkbSecondaryCare':
            linkElement = '#btn_pkb_secondary_care_medicines';
            break;
          case 'myCareView':
            linkElement = '#btn_pkb_my_care_view_medicines';
            break;
          default:
            break;
        }

        $store = createStore({ context, isProxying });
        wrapper = mountPage($store);
      });

      it(`${expectedResult ? 'will' : 'will not'} show the link`, () => {
        expect(wrapper.find(linkElement).exists()).toBe(expectedResult);
      });
    });
  });
});
