import LinkedProfileSummary from '@/pages/linked-profiles/summary';
import { create$T, createStore, mount } from '../../helpers';
import { LINKED_PROFILES } from '@/lib/routes';
import '@/plugins/filters';

const $t = create$T();

describe('linked profile is there', () => {
  let $store;
  let wrapper;
  let $state;

  const createState = (state = {
    device: {
      source: 'web',
    },
    linkedAccounts: {
      selectedLinkedAccount: {
        id: 'user-id-0',
        name: 'mr user 0',
        dateOfBirth: '2019-07-04T00:00:00.000',
        gpPracticeName: 'practice x',
        nhsNumber: 999111222,
        canBookAppointment: true,
        canOrderRepeatPrescription: true,
        canViewMedicalRecord: false,
        displayPersonalizedContent: false,
      },
    },
  }) => state;

  const createStateWithPersonalizedContentTrue = (state = {
    device: {
      source: 'web',
    },
    linkedAccounts: {
      selectedLinkedAccount: {
        id: 'user-id-0',
        name: 'mr user 0',
        dateOfBirth: '2019-07-04T00:00:00.000',
        gpPracticeName: 'practice x',
        nhsNumber: 999111222,
        canBookAppointment: true,
        canOrderRepeatPrescription: true,
        canViewMedicalRecord: false,
        displayPersonalizedContent: true,
      },
    },
  }) => state;

  const mountPage = () => mount(LinkedProfileSummary, { $store, $t, $state });

  describe('show linked profile links', () => {
    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      wrapper = mountPage();
    });

    describe('asyncData', () => {
      it('loads the account summary when selectedLinkedAccount is populated', async () => {
        await wrapper.vm.$options.asyncData({ store: $store });

        expect($store.dispatch).toHaveBeenCalledWith(
          'linkedAccounts/loadAccountAccessSummary',
          'user-id-0',
        );
      });

      it('redirects to linked accounts when selectedLinkedAccount is not populated', async () => {
        // arrange
        $store.state.linkedAccounts.selectedLinkedAccount = null;
        const redirectMock = jest.fn();

        // act
        await wrapper.vm.$options.asyncData({ store: $store, redirect: redirectMock });

        // assert
        expect($store.dispatch).not.toHaveBeenCalledWith('linkedAccounts/loadAccountAccessSummary');
        expect(redirectMock).toHaveBeenCalledWith(302, LINKED_PROFILES.path, null);
      });
    });

    it('displays the correct text and icons for the selected profile', () => {
      const switchButton = wrapper.find('#btn-switch-profile');
      const canBookAppointment = wrapper.find('[id="book-an-appointment"]');
      const canOrderRepeatPrescription = wrapper.find('[id="order-repeat-prescription"]');
      const canViewMedicalRecord = wrapper.find('[id="view-medical-record"]');

      // assert
      expect(switchButton.exists()).toBe(true);
      expect(canBookAppointment.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(true);
      expect(canOrderRepeatPrescription.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(true);
      expect(canViewMedicalRecord.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(false);

      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'translate_pageHeaders.linkedProfilesSummary');
      expect($store.dispatch).toHaveBeenCalledWith('pageTitle/updatePageTitle', 'translate_pageTitles.linkedProfilesSummary');
    });
  });

  describe('icons are not focusable', () => {
    beforeEach(() => {
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
    });

    it('focus is disabled on icons of services you can access', () => {
      const appointmentIconAttributes = wrapper
        .find('[id="book-an-appointment"]')
        .find('svg[class*="nhsuk-icon__tick"]').attributes();

      const prescriptionIconAttributes = wrapper
        .find('[id="order-repeat-prescription"]')
        .find('svg[class*="nhsuk-icon__tick"]').attributes();

      expect(appointmentIconAttributes.focusable).toEqual('false');
      expect(prescriptionIconAttributes.focusable).toEqual('false');
    });

    it('focus is disabled on icons of services you cannot access', () => {
      const medicalRecordIconAttributes = wrapper
        .find('[id="view-medical-record"]')
        .find('svg[class*="nhsuk-icon__cross"]').attributes();

      expect(medicalRecordIconAttributes.focusable).toEqual('false');
    });
  });

  describe('displayPersonalisedButton dependent on displayPersonalizedContent', () => {
    it('will not display displayPersonalisedButton when displayPersonalizedContent is false', () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      wrapper = mountPage();
      const button = wrapper.find('#btn-switch-profile');
      expect(button.exists()).toEqual(true);
      expect(button.text()).toEqual('translate_linkedProfiles.switchProfileButtonWithoutName');
    });

    it('will display displayPersonalisedButton when displayPersonalizedContent is true', () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createStateWithPersonalizedContentTrue(),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      wrapper = mountPage();
      const button = wrapper.find('#btn-switch-profile');
      expect(button.exists()).toEqual(true);
      expect(button.text()).toEqual('translate_linkedProfiles.switchProfileButton');
    });
  });
});
