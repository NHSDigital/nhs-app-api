import i18n from '@/plugins/i18n';
import * as dependency from '@/lib/utils';
import { PRESCRIPTIONS_PATH } from '@/router/paths';
import PharmacyChangeSuccessDetails from '@/components/nominatedPharmacy/PharmacyChangeSuccessDetails';
import OnlineOnlyPharmacyDetail from '@/components/nominatedPharmacy/OnlineOnlyPharmacyDetail';
import NominatedPharmacyChangeSuccess from '@/pages/nominated-pharmacy/change-success';
import PharmacyType from '@/lib/pharmacy-detail/pharmacy-types';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

describe('confirm nominated pharmacy', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      selectedNominatedPharmacy: {
        pharmacyType: PharmacyType.P1,
      },
      pharmacy: {
        pharmacyName: 'Boots',
        url: 'www.testurl.com',
      },
    },
  }) => state;

  const createStateWithNoUrl = (state = {
    device: {
      source: 'web',
    },
    nominatedPharmacy: {
      selectedNominatedPharmacy: {
        pharmacyType: PharmacyType.P2,
      },
      pharmacy: {
        pharmacyName: 'Boots',
      },
    },
  }) => state;


  const mountPage = () => mount(
    NominatedPharmacyChangeSuccess,
    {
      $store,
      mountOpts: {
        i18n,
      },
    },
  );

  describe('nominated pharmacy change success details for high street pharmacy', () => {
    let pharmacyChangeSuccessDetails;

    it('will exist', async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isHighStreetSelected = true;
      pharmacyChangeSuccessDetails = wrapper.find(PharmacyChangeSuccessDetails);
      expect(pharmacyChangeSuccessDetails.exists()).toBe(true);
    });

    it('will translate the header for pharmacy', async () => {
      const state = createState();
      state.nominatedPharmacy.selectedNominatedPharmacy.pharmacyType = PharmacyType.P1;
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state,
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      pharmacyChangeSuccessDetails = wrapper.find(PharmacyChangeSuccessDetails);
      const header = wrapper.find('div.nhsuk-grid-column-full > p:first-child');
      expect(header.text()).toBe('Your nominated pharmacy is:');
    });
  });

  describe('nominated pharmacy change success details for online only pharmacy with no url', () => {
    let onlinePharmacyChangeSuccessDetails;
    let postalWarning;
    let whatHappensNextWarning;
    let registrationWarningWithUrl;
    let registrationWarningWithNoUrl;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isOnlineOnlySelected = true;
      postalWarning = wrapper.find('#postal-warning');
      whatHappensNextWarning = wrapper.find('#what-happens-next');
      registrationWarningWithUrl = wrapper.find('#registrationWarningWithUrl');
      registrationWarningWithNoUrl = wrapper.find('#registrationWarningWithNoUrl');
    });

    it('will exist', async () => {
      onlinePharmacyChangeSuccessDetails = wrapper.find(OnlineOnlyPharmacyDetail);
      expect(onlinePharmacyChangeSuccessDetails.exists()).toBe(true);
    });

    it('will have the correct content when the pharmacy has url', async () => {
      expect(whatHappensNextWarning.exists()).toBe(true);
      expect(registrationWarningWithUrl.exists()).toBe(true);
      expect(registrationWarningWithNoUrl.exists()).toBe(false);
      expect(postalWarning.exists()).toBe(true);
    });

    it('will display the correct text', () => {
      const pharmacyUrl = 'www.testurl.com';
      $store.state.nominatedPharmacy.pharmacy.url = pharmacyUrl;
      expect(whatHappensNextWarning.text()).toBe('What happens next');
      expect(wrapper.find('#pharmacy-url').text()).toEqual(pharmacyUrl);
      expect(registrationWarningWithUrl.text()).toMatch(/You may need to register with Boots separately at\s+www\.testurl\.com\./);
      expect(postalWarning.text()).toBe('Your prescriptions will be sent to Boots. Once they’ve checked and prepared your prescriptions, Boots will send them to you in the post.');
    });
  });

  describe('nominated pharmacy change success details for online only pharmacy with url', () => {
    let postalWarning;
    let whatHappensNextWarning;
    let registrationWarningWithUrl;
    let registrationWarningWithNoUrl;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()), state: createStateWithNoUrl(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      wrapper.vm.isOnlineOnlySelected = true;
      postalWarning = wrapper.find('#postal-warning');
      whatHappensNextWarning = wrapper.find('#what-happens-next');
      registrationWarningWithUrl = wrapper.find('#registrationWarningWithUrl');
      registrationWarningWithNoUrl = wrapper.find('#registrationWarningWithNoUrl');
    });

    it('will have the correct content when the pharmacy has url', async () => {
      expect(whatHappensNextWarning.exists()).toBe(true);
      expect(registrationWarningWithUrl.exists()).toBe(false);
      expect(registrationWarningWithNoUrl.exists()).toBe(true);
      expect(postalWarning.exists()).toBe(true);
    });

    it('will display the correct text', () => {
      expect(whatHappensNextWarning.text()).toBe('What happens next');
      expect(registrationWarningWithNoUrl.text()).toBe('You may need to register with Boots separately.');
      expect(postalWarning.text()).toBe('Your prescriptions will be sent to Boots. Once they’ve checked and prepared your prescriptions, Boots will send them to you in the post.');
    });
  });

  describe('go to prescriptions link for desktop', () => {
    let goToPrescriptionsLink;

    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      wrapper = mountPage();
      await wrapper.vm.$nextTick();
      goToPrescriptionsLink = wrapper.find('#to-prescriptions-link').find('a');
    });

    it('will exist', () => {
      expect(goToPrescriptionsLink.exists()).toBe(true);
    });

    it('will display go to perscriptions text', () => {
      expect(goToPrescriptionsLink.text()).toEqual('Go to your prescriptions');
    });

    it('will redirect to prescriptions page when clicked', async () => {
      dependency.redirectTo = jest.fn();
      await goToPrescriptionsLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_PATH);
    });
  });

  describe('mounted', () => {
    beforeEach(() => {
      EventBus.$emit.mockClear();
      $store = createStore({ state: createState() });
      mountPage();
    });

    it('will emit UPDATE_HEADER with localised change success header as event', () => {
      expect(EventBus.$emit)
        .toHaveBeenCalledWith(UPDATE_HEADER, 'You have nominated a pharmacy', true);
    });

    it('will emit UPDATE_TITLE with localised change success title as event', () => {
      expect(EventBus.$emit)
        .toHaveBeenCalledWith(UPDATE_TITLE, 'You have nominated a pharmacy', true);
    });
  });
});
