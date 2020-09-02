import BackButton from '@/components/BackButton';
import i18n from '@/plugins/i18n';
import { redirectTo } from '@/lib/utils';
import { createRouter, mount } from '../helpers';

jest.mock('@/lib/utils');

const mountBackButton = ({ $router, propsData = {} }) => mount(
  BackButton,
  {
    $router,
    propsData,
    mountOpts: {
      i18n,
    },
  },
);

describe('Back Button', () => {
  let $router;
  let wrapper;

  beforeEach(() => {
    $router = createRouter();
    redirectTo.mockClear();
  });

  describe('no explicit path or text', () => {
    beforeEach(() => {
      wrapper = mountBackButton({ $router });
    });

    it('will display the default text', () => {
      expect(wrapper.text()).toEqual('Back');
    });

    it('will go to the previous page when clicked', () => {
      wrapper.trigger('click');
      expect($router.goBack).toHaveBeenCalled();
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
      expect(redirectTo).toHaveBeenCalledWith(wrapper.vm, gotoPath);
    });
  });
});
