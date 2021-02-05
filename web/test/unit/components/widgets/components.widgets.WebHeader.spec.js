import WebHeader from '@/components/widgets/WebHeader';
import { createStore, mount } from '../../helpers';

describe('WebHeader.vue', () => {
  let $store;
  let wrapper;
  const mountAs = () => {
    $store = createStore({
      state: {
        header: {
          miniMenuExpanded: false,
        },
        device: {
          isNativeApp: false,
        },
        session: {
          csrfToken: 'test',
        },
      },
    });
    $store.getters['session/isLoggedIn'] = jest.fn();

    return mount(WebHeader, { $store,
      stubs: {
        'router-link': '<a></a>',
      },
      $route: { meta: { helpUrl: 'https://help.url' } },
    });
  };

  beforeEach(() => {
    wrapper = mountAs(WebHeader);
  });

  it('will call toggleMiniMenu on button interaction', () => {
    const button = wrapper.find('#toggle-menu');
    jest.spyOn($store, 'dispatch');
    expect(button.exists()).toBe(true);
    button.trigger('click');
    expect($store.dispatch)
      .toHaveBeenCalledWith('header/toggleMiniMenu');
  });
});
