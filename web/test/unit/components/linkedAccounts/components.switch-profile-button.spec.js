import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import { createStore, mount } from '../../helpers';
import { INDEX } from '@/lib/routes';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';

describe('switch profile button component', () => {
  let $store;
  let wrapper;

  const mainUserGuid = '1234-abc-dddd';

  const createState = (state = {
    device: {
      source: 'web',
    },
    linkedAccounts: {
      config: {
        patientId: mainUserGuid,
      },
    },
  }) => state;

  const mountComponent = () =>
    mount(SwitchProfileButton, { $store });

  describe('button behaviour', () => {
    beforeEach(() => {
      dependency.redirectTo = jest.fn();
      $store = createStore({
        dispatch: jest.fn(() => Promise.resolve()),
        state: createState(),
      });
      $store.getters['linkedAccounts/mainPatientId'] = mainUserGuid;
      wrapper = mountComponent();
    });

    it('should navigate when switch to my profile button is clicked', async () => {
      // act
      await wrapper.vm.switchProfileButtonClicked();

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile', { id: mainUserGuid });
      expect($store.dispatch).toHaveBeenCalledWith('serviceJourneyRules/init');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX.path);
    });
  });
});
