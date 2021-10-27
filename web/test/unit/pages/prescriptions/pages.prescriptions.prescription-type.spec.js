import {
  PRESCRIPTIONS_PATH,
  PRESCRIPTIONS_CONTACT_SURGERY_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import PrescriptionType from '@/pages/prescriptions/prescription-type';
import * as dependency from '@/lib/utils';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import i18n from '@/plugins/i18n';
import Vue from 'vue';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

describe('Prescription type page', () => {
  let $router;
  let $store;
  let state;
  let wrapper;
  let continueButton;
  let errorComponent;
  let backLink;
  let getters;

  const createState = nativeApplication => ({
    linkedAccounts: {
      actingAsUser: {
        canOrderRepeatPrescription: true,
      },
      config: {
        hasLoaded: true,
      },
    },
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

  beforeEach(async () => {
    EventBus.$emit.mockClear();
    state = createState(false, 'web');
    getters = { };
    $store = createStore({ state, getters });
    $store.app.$analytics = {
      trackButtonClick: jest.fn(),
    };
    wrapper = mount(
      PrescriptionType,
      {
        $router,
        $store,
        mountOpts: {
          i18n,
        },
      },
    );
    dependency.redirectTo = jest.fn();
    continueButton = wrapper.find('#continue-button');
  });

  describe('Radio buttons', () => {
    it('will exist', () => {
      expect(wrapper.find(NhsUkRadioGroup).exists()).toBe(true);
    });
  });

  describe('Continue button', () => {
    it('will exist', () => {
      expect(continueButton.exists()).toBe(true);
    });

    it('will display continue text', () => {
      expect(continueButton.text()).toEqual('Continue');
    });

    it('will redirect to the repeat prescription page when repeat radio selected', async () => {
      const radioButton = wrapper.find('#prescriptionType-PRESCRIPTION_TYPE_REPEAT');
      radioButton.trigger('click');
      continueButton.trigger('click');
      errorComponent = wrapper.find('#message-dialog');
      await Vue.nextTick();

      expect(errorComponent.exists()).toBe(false);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTION_REPEAT_COURSES_PATH);
    });

    it('will redirect to the contact surgery page when non repeat radio selected', async () => {
      const radioButton = wrapper.find('#prescriptionType-PRESCRIPTION_TYPE_NON_REPEAT');
      radioButton.trigger('click');
      continueButton.trigger('click');
      await Vue.nextTick();
      errorComponent = wrapper.find('#message-dialog');

      expect(errorComponent.exists()).toBe(false);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_CONTACT_SURGERY_PATH);
    });

    it('will not redirect to another page when no prescription type has been selected', async () => {
      continueButton.trigger('click');
      await Vue.nextTick();
      errorComponent = wrapper.find('#message-dialog');

      expect(errorComponent.exists()).toBe(true);
      expect(dependency.redirectTo).not.toHaveBeenCalled();
      expect(EventBus.$emit).toBeCalledWith(FOCUS_ERROR_ELEMENT);
    });

    it('will track prescriptions repeat courses path when repeat type is selected', async () => {
      const radioButton = wrapper.find('#prescriptionType-PRESCRIPTION_TYPE_REPEAT');
      radioButton.trigger('click');
      continueButton.trigger('click');
      await Vue.nextTick();

      expect($store.app.$analytics.trackButtonClick)
        .toHaveBeenCalledWith(PRESCRIPTION_REPEAT_COURSES_PATH, true);
    });

    it('will track prescriptions contact surgery path when non repeat type is selected', async () => {
      const radioButton = wrapper.find('#prescriptionType-PRESCRIPTION_TYPE_NON_REPEAT');
      radioButton.trigger('click');
      continueButton.trigger('click');
      await Vue.nextTick();

      expect($store.app.$analytics.trackButtonClick)
        .toHaveBeenCalledWith(PRESCRIPTIONS_CONTACT_SURGERY_PATH, true);
    });
  });

  describe('Native Back Button', () => {
    beforeEach(async () => {
      EventBus.$emit.mockClear();
      state = createState(true, 'web');
      getters = {};
      $store = createStore({ state, getters });
      wrapper = mount(
        PrescriptionType,
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
      expect(wrapper.find('#back-link').exists()).toBe(false);
    });
  });

  describe('Web Back Button', () => {
    beforeEach(async () => {
      EventBus.$emit.mockClear();
      state = createState(false, 'web');
      getters = {};
      $store = createStore({ state, getters });
      wrapper = mount(
        PrescriptionType,
        {
          $router,
          $store,
          mountOpts: {
            i18n,
          },
        },
      );
      backLink = wrapper.find('#back-link').find('a');
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
        .toHaveBeenCalledWith(wrapper.vm, PRESCRIPTIONS_PATH);
    });
  });
});
