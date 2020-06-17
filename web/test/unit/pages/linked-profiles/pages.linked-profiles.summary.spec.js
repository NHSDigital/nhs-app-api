import LinkedProfileSummary from '@/pages/linked-profiles/summary';
import { INDEX_PATH, LINKED_PROFILES_PATH } from '@/router/paths';
import '@/plugins/filters';
import * as dependency from '@/lib/utils';
import { create$T, createStore, mount } from '../../helpers';

dependency.redirectTo = jest.fn();

const $t = create$T();

describe('linked profile is there', () => {
  let $store;
  let wrapper;
  let $state;

  const createState = (customState = {}) =>
    Object.assign({
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
          showSummary: true,
        },
      },
    }, customState);

  const mountPage = () => mount(LinkedProfileSummary, { $store, $t, $state });

  const createPageWrapper = async () => {
    wrapper = mountPage();
    await wrapper.vm.$nextTick();
  };

  describe('show linked profile links with Access Summary', () => {
    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
        getters: {
          'store.getters.session/isLoggedIn': true,
        },
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
    });

    describe('mounted', () => {
      it('loads the account summary when selectedLinkedAccount is populated', async () => {
        await createPageWrapper();

        expect($store.dispatch).toHaveBeenCalledWith(
          'linkedAccounts/loadAccountAccessSummary',
          'user-id-0',
        );
      });

      it('redirects to linked accounts when selectedLinkedAccount is not populated', async () => {
        // arrange
        $store.state.linkedAccounts.selectedLinkedAccount = null;

        // act
        await createPageWrapper();

        // assert
        expect($store.dispatch).not.toHaveBeenCalledWith('linkedAccounts/loadAccountAccessSummary');
        expect(dependency.redirectTo).toHaveBeenCalledWith(wrapper.vm, LINKED_PROFILES_PATH);
      });
    });

    it('displays the correct text and icons for the selected profile', async () => {
      await createPageWrapper();
      const switchButton = wrapper.find('#btn-switch-profile');
      const canBookAppointment = wrapper.find('[id="book-an-appointment"]');
      const canOrderRepeatPrescription = wrapper.find('[id="order-repeat-prescription"]');
      const canViewMedicalRecord = wrapper.find('[id="view-medical-record"]');
      const blurb = wrapper.find('h2');

      // assert
      expect(switchButton.exists()).toBe(true);
      expect(canBookAppointment.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(true);
      expect(canOrderRepeatPrescription.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(true);
      expect(canViewMedicalRecord.find('svg[class*="nhsuk-icon__tick"]').exists()).toBe(false);
      expect(blurb.exists()).toBe(true);
    });

    it('will call switch and redirect to index when switch back button is clicked', async () => {
      await createPageWrapper();
      await wrapper.vm.switchProfileButtonClicked();

      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchProfile', $store.state.linkedAccounts.selectedLinkedAccount);
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX_PATH);
    });
  });

  describe('show linked profile links with No Access Summary', () => {
    beforeEach(async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState({
          linkedAccounts: {
            selectedLinkedAccount: {
              showSummary: false,
            },
          },
        }),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      await createPageWrapper();
    });

    it('displays the correct text and icons for the selected profile', () => {
      const switchButton = wrapper.find('#btn-switch-profile');
      const canBookAppointment = wrapper.find('[id="book-an-appointment"]');
      const canOrderRepeatPrescription = wrapper.find('[id="order-repeat-prescription"]');
      const canViewMedicalRecord = wrapper.find('[id="view-medical-record"]');
      const blurb = wrapper.find('p');

      // assert
      expect(switchButton.exists()).toBe(true);
      expect(canBookAppointment.exists()).toBe(false);
      expect(canOrderRepeatPrescription.exists()).toBe(false);
      expect(canViewMedicalRecord.exists()).toBe(false);
      expect(blurb.exists()).toBe(true);
    });
  });

  describe('displayPersonalisedButton dependent on displayPersonalizedContent', () => {
    beforeEach(() => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
        getters: {
          'store.getters.session/isLoggedIn': true,
        },
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
    });
    it('will not display displayPersonalisedButton when displayPersonalizedContent is false', async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState({
          linkedAccounts: {
            selectedLinkedAccount: {
              displayPersonalizedContent: false,
              showSummary: false,
            },
          },
        }),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      await createPageWrapper();
      const button = wrapper.find('#btn-switch-profile');
      expect(button.exists()).toEqual(true);
      expect(button.text()).toEqual('translate_linkedProfiles.switchProfileButtonWithoutName');
    });

    it('will display displayPersonalisedButton when displayPersonalizedContent is true', async () => {
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState({
          linkedAccounts: {
            selectedLinkedAccount: {
              displayPersonalizedContent: true,
              showSummary: true,
            },
          },
        }),
      });
      $store.getters['linkedAccounts/getSelectedLinkedAccount'] = $store.state.linkedAccounts.selectedLinkedAccount;
      wrapper = await mountPage();
      const button = wrapper.find('#btn-switch-profile');
      expect(button.exists()).toEqual(true);
      expect(button.text()).toEqual('translate_linkedProfiles.switchProfileButton');
    });
  });
});
