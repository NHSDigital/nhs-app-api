import SwitchProfile from '@/pages/switch-profile/index';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { create$T, createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $emit: jest.fn() },
}));

const $t = create$T();

describe('switch profile page is there', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    linkedAccounts: {
      actingAsUser: {
        id: 'user-id-0',
        name: 'mr user 0',
        ageMonths: '10',
        ageYears: '25',
        gpPracticeName: 'practice x',
      },
      config: {
        patientId: '1234-abc-dddd',
      },
    },
  }) => state;

  const mountPage = () => mount(SwitchProfile, { $store, $t });

  describe('show switch profile switches', () => {
    let gpPracticeName;
    let switchToMyProfileButton;
    const mainUserGuid = '1234-abc-dddd';

    beforeEach(() => {
      global.digitalData = {};
      dependency.redirectTo = jest.fn();
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      $store.app.$analytics = {
        trackButtonClick: jest.fn(),
      };
      wrapper = mountPage();
      $store.getters['linkedAccounts/hasLinkedAccounts'] = true;
      $store.getters['linkedAccounts/mainPatientId'] = mainUserGuid;
    });

    it('updated header and title is correct', () => {
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_HEADER, 'translate_pageHeaders.switchProfile', true);
      expect(EventBus.$emit).toHaveBeenCalledWith(UPDATE_TITLE, 'translate_pageTitles.switchProfile', true);
    });

    it('proxy users age is visible', () => {
      expect(wrapper.find('[id="proxy-age"]').text()).toEqual('25translate_linkedProfiles.ageLabels.greaterThanOneYearOld');
    });

    it('gp practice name is not visible with null value', () => {
      $store.state.linkedAccounts.actingAsUser.gpPracticeName = null;
      gpPracticeName = wrapper.find('[id="proxy-gp-practice"]');
      expect(gpPracticeName.exists()).toBe(false);
    });

    it('gp practice name is visible with a value', () => {
      $store.state.gpPracticeName = 'A12345';
      gpPracticeName = wrapper.find('[id="proxy-gp-practice"]');
      expect(gpPracticeName.exists()).toBe(true);
    });

    it('should have a switch profile button', async () => {
      switchToMyProfileButton = wrapper.find('[id="switch-profile-button"]');
      expect(switchToMyProfileButton.exists()).toBe(true);
    });
  });
});
