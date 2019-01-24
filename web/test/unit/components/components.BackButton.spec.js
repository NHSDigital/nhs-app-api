import BackButton from '@/components/BackButton';
import { createRouter, mount } from '../helpers';


const mountBackButton = ({ $router, propsData = {} }) => mount(BackButton, { $router, propsData });

describe('Back Button', () => {
  let $router;
  let wrapper;

  beforeEach(() => {
    $router = createRouter();
  });

  describe('no explicit path or text', () => {
    beforeEach(() => {
      wrapper = mountBackButton({ $router });
    });

    it('will display the default text', () => {
      expect(wrapper.text()).toEqual('translate_generic.backButton.text');
    });

    it('will go to the previous page when clicked', () => {
      wrapper.trigger('click');
      expect($router.go).toHaveBeenCalledWith(-1);
    });
  });

  describe('explict path and text', () => {
    const gotoPath = '/foo';

    beforeEach(() => {
      wrapper = mountBackButton({ $router, propsData: { gotoPath, text: 'foobar' } });
    });

    it('will display the specified text', () => {
      expect(wrapper.text()).toEqual('foobar');
    });

    it('will push the explict path to the router', () => {
      wrapper.trigger('click');
      expect($router.push).toHaveBeenCalledWith(gotoPath);
    });
  });
});
