import {
  PRESCRIPTION_TYPE_PATH,
  PRESCRIPTIONS_CONTACT_SURGERY_PATH,
} from '@/router/paths';
import ContactSurgery from '@/pages/prescriptions/contact-surgery';
import * as dependency from '@/lib/utils';
import i18n from '@/plugins/i18n';
import { EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('Prescription contact surgery page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;
  let backLink;
  const emergencyPrescriptionText = '111.nhs.uk/emergency-prescription';
  const emergencyPrescriptionLink = 'https://111.nhs.uk/emergency-prescription';
  const emergencyPrescriptionsLinkId = '#emergencyPrescriptions-link';
  let getters;

  const createState = nativeApplication => ({
    nominatedPharmacy: {
      pharmacy: {
        pharmacyName: 'Boots',
      },
    },
    prescriptionType: {
      hasLoaded: true,
      error: null,
    },
    device: {
      isNativeApp: nativeApplication,
    },
    repeatPrescriptionCourses: {
      hasLoaded: true,
    },
  });

  beforeEach(() => {
    state = createState(false);
    getters = {};
    $store = createStore({ state, getters });
    $store.app = { $analytics: { trackButtonClick: jest.fn() } };

    wrapper = mount(
      ContactSurgery,
      {
        $router,
        $store,
        mountOpts: {
          i18n,
        },
      },
    );
  });

  it('emergency prescriptions link will exist', async () => {
    const link = wrapper.find(emergencyPrescriptionsLinkId);
    expect(link.exists()).toBe(true);
  });

  it('emergency prescriptions link will display the correct text', () => {
    const link = wrapper.find(emergencyPrescriptionsLinkId).find('a');
    expect(link.text()).toEqual(emergencyPrescriptionText);
  });

  it('emergency prescriptions link will go to the correct page when clicked', () => {
    const link = wrapper.find(emergencyPrescriptionsLinkId).find('a');
    expect(link.attributes().href).toEqual(emergencyPrescriptionLink);
  });

  it('will track prescriptions contact surgery path when emergency prescriptions link is clicked', () => {
    const link = wrapper.find(emergencyPrescriptionsLinkId).find('a');
    link.trigger('click');
    expect($store.app.$analytics.trackButtonClick)
      .toHaveBeenCalledWith(PRESCRIPTIONS_CONTACT_SURGERY_PATH, true);
  });

  describe('Native Back Button', () => {
    beforeEach(async () => {
      EventBus.$emit.mockClear();
      state = createState(true);
      getters = {};
      $store = createStore({ state, getters });
      wrapper = mount(
        ContactSurgery,
        {
          $router,
          $store,
          mountOpts: {
            i18n,
          },
        },
      );
      dependency.redirectTo = jest.fn();
    });

    it('will not exist', () => {
      expect(wrapper.find('#backToPrescriptions-link').exists()).toBe(false);
    });
  });

  describe('Web Back Button', () => {
    beforeEach(async () => {
      EventBus.$emit.mockClear();
      state = createState(false);
      getters = {};
      $store = createStore({ state, getters });
      wrapper = mount(
        ContactSurgery,
        {
          $router,
          $store,
          mountOpts: {
            i18n,
          },
        },
      );
      backLink = wrapper.find('#backToPrescriptions-link').find('a');
      dependency.redirectTo = jest.fn();
    });

    it('will exist', () => {
      expect(backLink.exists()).toBe(true);
    });

    it('will display the correct text', () => {
      expect(backLink.text()).toEqual('Back');
    });

    it('will go to the correct page when clicked', () => {
      backLink.trigger('click');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_TYPE_PATH);
    });
  });
});
