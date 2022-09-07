import ClearBackLinkOverrideMixin from '@/plugins/mixinDefinitions/ClearBackLinkOverrideMixin';
import { createStore, mount } from '../../helpers';
import {
  APPOINTMENTS,
  BOOKING,
  CONFIRMATION,
} from '@/router/routes/appointments';

describe('ClearBackLinkOverride', () => {
  let $store;
  let wrapper;

  const createPage = (route) => {
    const component = {
      template: '<div></div>',
      mixins: [ClearBackLinkOverrideMixin],
    };

    return mount(component, {
      $route: {
        ...route,
      },
      $store,
    });
  };

  describe('$route change', () => {
    beforeEach(() => {
      $store = createStore();
      $store.dispatch = jest.fn();
      jest.clearAllMocks();
      wrapper = createPage(BOOKING);
    });

    it('will not call clearBackLinkOverride', () => {
      wrapper.vm.$options.watch.$route.call(wrapper.vm, CONFIRMATION, BOOKING);

      expect($store.dispatch).not.toHaveBeenCalledWith('navigation/clearBackLinkOverride');
    });

    it('will call clearBackLinkOverride', () => {
      wrapper.vm.$options.watch.$route.call(wrapper.vm, APPOINTMENTS, BOOKING);

      expect($store.dispatch).toHaveBeenCalledWith('navigation/clearBackLinkOverride');
    });
  });
});
