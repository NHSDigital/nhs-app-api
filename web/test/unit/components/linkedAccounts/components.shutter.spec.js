import { get, has } from 'lodash/fp';
import * as dependency from '@/lib/utils';
import Shutter from '@/components/linked-profiles/Shutter';
import locale from '@/locale';
import { INDEX } from '@/lib/routes';
import { createStore, mount } from '../../helpers';

const engLocale = locale.en;
const $t = key => get(key, engLocale);
const $te = key => has(key)(engLocale);

const mainPatientGivenName = 'main_patient_given_name';

describe('shutter component', () => {
  let $store;
  let $style;
  let wrapper;

  const createState = (state = {
    linkedAccounts: {
      actingAsUser: {
        givenName: mainPatientGivenName,
      },
    },
  }) => state;

  const mountPage = props => mount(Shutter, { $store, $style, propsData: props, $t, $te });

  describe('component behaviour', () => {
    let shutterSummaryText;
    let shutterSwitchText;
    let switchBackButton;

    beforeEach(() => {
      $store = createStore({ dispatch: jest.fn(() => Promise.resolve()), state: createState() });
      $store.getters = {};
      wrapper = mountPage({ feature: 'appointments' });
    });

    beforeEach(() => {
      shutterSummaryText = wrapper.find('#shutter-summary-text');
      shutterSwitchText = wrapper.find('#shutter-switch-text');
      switchBackButton = wrapper.find('#btn-switch-profile');
    });

    it('will correctly display the localised message for a feature e.g. appointments', () => {
      expect(shutterSummaryText.exists()).toBe(true);
      expect(shutterSwitchText.exists()).toBe(true);
      expect(switchBackButton.exists()).toBe(true);

      expect(shutterSummaryText.text()).toBe(`Contact ${mainPatientGivenName}'s GP surgery for more information. For urgent medical advice, go to 111.nhs.uk or call 111.`);
      expect(shutterSwitchText.text()).toBe('Switch to your profile to book appointments for yourself.');
    });

    it('will call switch and redirect to index when switch back button is clicked', async () => {
      dependency.redirectTo = jest.fn();
      await wrapper.vm.switchProfileButtonClicked();

      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile');
      expect($store.dispatch).toHaveBeenCalledWith('myRecord/clear');
      expect($store.dispatch).toHaveBeenCalledWith('serviceJourneyRules/init');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX.path);
    });
  });
});
