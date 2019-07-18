import { createStore, mount } from '../../helpers';
import ContentHeader from '../../../../src/components/widgets/ContentHeader';

describe('ContentHeader.vue', () => {
  let $route;
  let $store;
  let wrapper;

  const mountAs = ({ native = true }) => {
    const getter = {};
    getter['appVersion/isNativeVersionAfter'] = jest.fn();
    $store = createStore({
      state: {
        device: {
          isNativeApp: native,
        },
        header: {
          headerText: 'Test',
        },
      },
      getters: getter,
    });
    $route = {
      name: 'Login',
    };
    return mount(ContentHeader, { $store, $route });
  };

  beforeEach(() => {
    wrapper = mountAs({ native: true });
  });

  it('currentBreadCrumbs will return nothing when in Login page', () => {
    expect(wrapper.vm.currentBreadCrumbs).toEqual([]);
  });
});


