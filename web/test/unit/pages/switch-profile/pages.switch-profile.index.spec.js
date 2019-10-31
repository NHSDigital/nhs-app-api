import SwitchProfile from '@/pages/switch-profile/index';
import { create$T, createStore, mount } from '../../helpers';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';
import { INDEX } from '../../../../src/lib/routes';

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
        dateOfBirth: '1989-07-04T00:00:00.000',
        gpPracticeName: 'practice x',
        nhsNumber: '999111222',
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
      expect($store.dispatch).toHaveBeenCalledWith('header/updateHeaderText', 'translate_pageHeaders.switchProfile');
      expect($store.dispatch).toHaveBeenCalledWith('pageTitle/updatePageTitle', 'translate_pageTitles.switchProfile');
    });

    it('proxy users dob and nhs number is visible', () => {
      expect(wrapper.find('[id="proxy-date-of-birth"]').text()).toEqual('4 July 1989');
      expect(wrapper.find('[id=proxy-nhs-number]').text()).toEqual('999111222');
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

    it('should navigate when switch to my profile button is clicked', async () => {
      // arrange
      switchToMyProfileButton = wrapper.find('[id="switch-profile-button"]');
      expect(switchToMyProfileButton.exists()).toBe(true);

      // act
      await switchToMyProfileButton.trigger('click');

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile', { id: mainUserGuid });
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX.path);
    });
  });
});
