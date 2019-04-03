import { createStore, mount } from '../../helpers';
import HeaderMenu from '../../../../src/components/widgets/HeaderMenu';

describe('HeaderMenu.vue', () => {
  let $router;
  let $store;
  let wrapper;
  let menuItems;

  const mountAs = ({ native = true, expanded = true }) => {
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
        header: {
          miniMenuExpanded: expanded,
        },
      },
    });

    return mount(HeaderMenu, { $store, $router });
  };

  beforeEach(() => {
    wrapper = mountAs({ native: false, expanded: true });
  });

  it('will show desktop view shows correct number of menu items', () => {
    expect(wrapper.findAll('li').length).toEqual(6);
    menuItems = wrapper.findAll('li');
    expect(menuItems.at(4).find('a').text()).not.toEqual('translate_navigationMenu.moreLabel');
  });
});
