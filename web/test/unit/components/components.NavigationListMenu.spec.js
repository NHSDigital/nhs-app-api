import NavigationListMenu from '@/components/NavigationListMenu';
import { mount, createStore } from '../helpers';

const mountComponent = ({ isEnabled }) =>
  mount(NavigationListMenu, {
    $store: createStore({
      getters: {
        'serviceJourneyRules/hasLinkedAccountsEnabled': isEnabled,
      },
    }),
  });

describe('linked profiles visiblitiy', () => {
  let wrapper;

  it('shows Linked profiles link', () => {
    wrapper = mountComponent({ isEnabled: true });
    expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(true);
  });

  it('does not show Linked profiles link', () => {
    wrapper = mountComponent({ isEnabled: false });
    expect(wrapper.find('#menu-item-linkedProfiles').exists()).toBe(false);
  });
});
