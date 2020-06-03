import ResetSpinnerMixin from '@/plugins/mixinDefinitions/ResetSpinner';
import { createStore, mount } from '../../helpers';

describe('ResetSpinner', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore();
    const component = {
      template: '<div></div>',
      mixins: [ResetSpinnerMixin],
    };
    wrapper = mount(component, {
      $store,
    });
  });

  it('will dispatch `spinner/prevent` with false', () => {
    expect($store.dispatch).toBeCalledWith('spinner/prevent', false);
  });

  describe('$route change', () => {
    beforeEach(() => {
      jest.clearAllMocks();
      wrapper.vm.$options.watch.$route.call(wrapper.vm, 'from', 'to');
    });

    it('will dispatch `spinner/prevent` with false', () => {
      expect($store.dispatch).toBeCalledWith('spinner/prevent', false);
    });
  });
});
