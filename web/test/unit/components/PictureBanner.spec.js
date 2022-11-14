import PictureBanner from '@/components/PictureBanner';
import { createStore, mount } from '../helpers';

describe('PictureBanner.vue', () => {
  let wrapper;

  const mountPictureBanner = ({ displayName, nhsNumber, age, practice }) => mount(PictureBanner, {
    $store: createStore({
      state: {
        device: {
          isNativeApp: false,
        },
      },
    }),
    propsData: {
      displayName,
      nhsNumber,
      age,
      practice,
    },
  });

  describe('props data for non proxy', () => {
    describe('Name does have value', () => {
      beforeEach(() => {
        wrapper = mountPictureBanner({ displayName: 'USER DISPLAY NAME', nhsNumber: '123456789', age: undefined, practice: undefined });
      });

      it('will display user name section', () => {
        expect(wrapper.find('[data-sid="hero-user-name"]').exists()).toBe(true);
      });

      it('will display user name text matching passed in value', () => {
        expect(wrapper.find('[data-sid="hero-user-name"]').text()).toBe('USER DISPLAY NAME');
      });

      it('will display user nhs number section', () => {
        expect(wrapper.find('[data-sid="hero-user-nhs-number"]').exists()).toBe(true);
      });

      it('will display user age section', () => {
        expect(wrapper.find('[data-sid="hero-user-age"]').exists()).toBe(false);
      });

      it('will display user surgery section', () => {
        expect(wrapper.find('[data-sid="hero-user-practice"]').exists()).toBe(false);
      });
    });
  });

  describe('props data for proxy', () => {
    describe('Name does have value', () => {
      beforeEach(() => {
        wrapper = mountPictureBanner({ displayName: 'USER DISPLAY NAME', nhsNumber: undefined, age: '21', practice: 'surgery' });
      });

      it('will display user name section', () => {
        expect(wrapper.find('[data-sid="hero-user-name"]').exists()).toBe(true);
      });

      it('will display user name text matching passed in value', () => {
        expect(wrapper.find('[data-sid="hero-user-name"]').text()).toBe('USER DISPLAY NAME');
      });

      it('will display user nhs number section', () => {
        expect(wrapper.find('[data-sid="hero-user-nhs-number"]').exists()).toBe(false);
      });

      it('will display user age section', () => {
        expect(wrapper.find('[data-sid="hero-user-age"]').exists()).toBe(true);
      });

      it('will display user surgery section', () => {
        expect(wrapper.find('[data-sid="hero-user-practice"]').exists()).toBe(true);
      });
    });
  });
});
