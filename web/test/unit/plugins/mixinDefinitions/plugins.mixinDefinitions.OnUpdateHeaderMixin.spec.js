import OnUpdateHeaderMixin from '@/plugins/mixinDefinitions/OnUpdateHeaderMixin';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import { createStore, mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn() },
}));


const mountOnUpdateHeaderMixin = ({
  methods = {},
} = {}) => (
  mount({ template: '<div></div>', mixins: [OnUpdateHeaderMixin] }, {
    $route: {
      name: 'home',
      meta: { crumb: { defaultCrumb: [] } },
    },
    $store: createStore({
      state: {
        device: {},
        navigation: {},
      },
    }),
    methods,
  }));

describe('OnUpdateHeaderMixin', () => {
  let wrapper;

  beforeEach(() => {
    EventBus.$on.mockClear();
    EventBus.$off.mockClear();
  });

  describe('lifecycle hooks', () => {
    it('will register UPDATE_HEADER event in beforeMount', () => {
      OnUpdateHeaderMixin.beforeMount();
      expect(EventBus.$on).toHaveBeenCalledWith(UPDATE_HEADER, OnUpdateHeaderMixin.onUpdateHeader);
    });

    it('will deregister UPDATE_HEADER event in beforeDestroy', () => {
      OnUpdateHeaderMixin.beforeDestroy();
      expect(EventBus.$off).toHaveBeenCalledWith(UPDATE_HEADER, OnUpdateHeaderMixin.onUpdateHeader);
    });
  });

  describe('setHeaderProp method', () => {
    let value;

    describe('value is a function', () => {
      beforeEach(() => {
        wrapper = mountOnUpdateHeaderMixin();
        value = jest.fn(() => ('tomatoes'));
        wrapper.vm.setHeaderProp('testHeaderProp', value);
      });

      it('will set the headerProp data prop to result of value', () => {
        expect(wrapper.vm.testHeaderProp).toEqual('tomatoes');
      });

      it('will pass $store and $i18n as parameters', () => {
        expect(value).toHaveBeenCalledWith(wrapper.vm.$store, wrapper.vm.$i18n);
      });
    });

    describe('value is not a function', () => {
      describe('and is localised', () => {
        beforeEach(() => {
          wrapper = mountOnUpdateHeaderMixin();
          value = 'i am not a function';
          wrapper.vm.setHeaderProp('anotherHeaderProp', value, true);
        });

        it('will set the headerProp data prop to value', () => {
          expect(wrapper.vm.anotherHeaderProp).toEqual('i am not a function');
        });
      });

      describe('and is not localised', () => {
        beforeEach(() => {
          wrapper = mountOnUpdateHeaderMixin();
          value = 'appTitle';
          wrapper.vm.setHeaderProp('anotherHeaderProp', value);
        });

        it('will set the headerProp data prop to translated value', () => {
          expect(wrapper.vm.anotherHeaderProp).toEqual('NHS App');
        });
      });
    });
  });

  describe('onUpdateHeader method', () => {
    let setHeaderProp;

    beforeEach(() => {
      setHeaderProp = jest.fn();
      wrapper = mountOnUpdateHeaderMixin({ methods: { setHeaderProp } });
      setHeaderProp.mockClear();
    });

    describe('newHeader is an empty object', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateHeader();
      });

      it('will call setHeaderProp passing header, undefined, and localised parameter', () => {
        expect(setHeaderProp).toHaveBeenCalledWith('header', undefined, false);
      });

      it('will call setHeaderProp passing caption, undefined, and localised parameter', () => {
        expect(setHeaderProp).toHaveBeenCalledWith('caption', undefined, false);
      });
    });

    describe('newHeader is a string', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateHeader('carrots', false);
      });

      it('will call setHeaderProp passing header, newHeader value, and localised parameter', () => {
        expect(setHeaderProp).toHaveBeenCalledWith('header', 'carrots', false);
      });

      it('will set caption data prop to undefined', () => {
        expect(wrapper.vm.caption).toBeUndefined();
      });
    });

    describe('newHeader is an object with a header/caption keys', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateHeader({ headerKey: 'onions', captionKey: 'are great' }, true);
      });

      it('will call setHeaderProp passing header, newHeader.headerKey value, and localised parameter', () => {
        expect(setHeaderProp).toHaveBeenCalledWith('header', 'onions', true);
      });

      it('will call setHeaderProp passing caption, newHeader.captionKey value, and localised parameter', () => {
        expect(setHeaderProp).toHaveBeenCalledWith('caption', 'are great', true);
      });
    });

    describe('newHeader is neither a string nor object', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateHeader(1, false);
      });

      it('will not call setHeaderProp', () => {
        expect(setHeaderProp).not.toHaveBeenCalled();
      });
    });
  });
});
