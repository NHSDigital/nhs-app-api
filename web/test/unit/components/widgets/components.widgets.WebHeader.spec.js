import WebHeader from '@/components/widgets/WebHeader';
import { createStore, mount } from '../../helpers';

describe('WebHeader.vue', () => {
  let $store;
  let wrapper;
  const $env = {
    HELP_AND_SUPPORT_URL: 'https://help',
  };
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

    return mount(WebHeader, { $store,
      $env,
      stubs: {
        'nuxt-link': '<a></a>',
      },
    });
  };

  beforeEach(() => {
    wrapper = mountAs(WebHeader);
  });
  it('will have the service header design for the logo', () => {
    const logo = wrapper.find('#nhs-header-logo');
    expect(logo.exists()).toBe(true);
    expect(logo.find('#nhs_logo').attributes('class')).toBe('nhsuk-header__link nhsuk-header__link--service');
    expect(logo.find('#logo-text').text()).toBe('translate_webHeader.logoText');
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
