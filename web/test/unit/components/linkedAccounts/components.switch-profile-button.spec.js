import SwitchProfileButton from '@/components/switch-profile/SwitchProfileButton';
import { createStore, mount } from '../../helpers';
import { INDEX } from '@/lib/routes';
import * as dependency from '@/lib/utils';
import '@/plugins/filters';

describe('switch profile button component', () => {
  let $store;
  let wrapper;


  const createState = (state = {
    device: {
      source: 'web',
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
      wrapper = mountComponent();
    });

    it('should navigate when switch to my profile button is clicked', async () => {
      // act
      await wrapper.vm.switchProfileButtonClicked();

      // assert
      expect($store.dispatch).toHaveBeenCalledWith('linkedAccounts/switchToMainUserProfile');
      expect($store.dispatch).toHaveBeenCalledWith('myRecord/clear');
      expect($store.dispatch).toHaveBeenCalledWith('serviceJourneyRules/init');
      expect(dependency.redirectTo)
        .toHaveBeenCalledWith(wrapper.vm, INDEX.path);
    });
  });
});
