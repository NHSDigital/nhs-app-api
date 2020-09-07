import GpAtHandContent from '@/components/GpAtHandContent';
import i18n from '@/plugins/i18n';
import { createStore, mount } from '../helpers';

describe('GpAtHandContent.vue', () => {
  let $store;
  let state;
  let wrapper;

  const createState = () => {
    state = {
      device: {
        isNativeApp: false,
      },
    };
    return state;
  };

  const $style = {
    desktopWeb: 'desktopWeb',
  };
  const propsData = {
    contentTag: 'gpAtHand.appointments.contentTag',
    headerTag: 'gpAtHand.appointments.headerTag',
  };

  const mountPage = () => mount(GpAtHandContent, {
    propsData,
    $store,
    $style,
    mountOpts: {
      i18n,
    },
  });


  beforeEach(() => {
    $store = createStore({ state: createState() });
    wrapper = mountPage();
  });

  describe('desktopWeb css class', () => {
    it('will exist when app is native', () => {
      state.device.isNativeApp = false;
      expect(wrapper.classes()).toContain($style.desktopWeb);
    });

    it('won\'t exist when app is native', () => {
      state.device.isNativeApp = true;
      expect(wrapper.classes()).not.toContain($style.desktopWeb);
    });
  });

  describe('header content', () => {
    it('will contain replacement for headerTag', () => {
      expect(wrapper.find('#guidance_sub_header').text()).toContain('Sorry, you cannot book GP appointments through the NHS App');
    });
  });

  describe('body content', () => {
    it('will contain replacement for contentTag', () => {
      expect(wrapper.text()).toContain('book appointments');
    });
  });
});
