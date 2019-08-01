import GpAtHandContent from '@/components/GpAtHandContent';
import { create$T, createStore, mount } from '../helpers';

const $tMock = create$T();

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
    contentTag: 'gp_at_hand.appointments.contentTag',
    headerTag: 'gp_at_hand.appointments.headerTag',
  };

  const mountPage = () => mount(GpAtHandContent, {
    propsData,
    $store,
    $style,
    $t: (key) => {
      if (key === 'gp_at_hand.content.header') {
        return 'Sorry, you cannot {headerTag} through the NHS App';
      }
      if (key === 'gp_at_hand.appointments.headerTag') {
        return 'book GP appointments';
      }
      if (key === 'gp_at_hand.appointments.contentTag') {
        return 'book appointments';
      }
      return $tMock(key);
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
