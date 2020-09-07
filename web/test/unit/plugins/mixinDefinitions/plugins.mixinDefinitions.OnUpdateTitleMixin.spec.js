import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';
import { mount } from '../../helpers';

jest.mock('@/services/event-bus', () => ({
  ...jest.requireActual('@/services/event-bus'),
  EventBus: { $on: jest.fn(), $off: jest.fn(), $emit: jest.fn() },
}));

const mountOnUpdateTitleMixin = ({
  methods,
  $route = { meta: {} },
} = {}) => (mount({ template: '<div></div>', mixins: [OnUpdateTitleMixin] }, {
  methods,
  $route,
}));

let wrapper;

describe('OnUpdateTitleMixin', () => {
  beforeEach(() => {
    EventBus.$on.mockClear();
    EventBus.$off.mockClear();
  });

  describe('lifecycle hooks', () => {
    it('will register UPDATE_TITLE event in beforeMount', () => {
      OnUpdateTitleMixin.beforeMount();
      expect(EventBus.$on).toHaveBeenCalledWith(UPDATE_TITLE, OnUpdateTitleMixin.onUpdateTitle);
    });

    it('will deregister UPDATE_TITLE event in beforeDestroy', () => {
      OnUpdateTitleMixin.beforeDestroy();
      expect(EventBus.$off).toHaveBeenCalledWith(UPDATE_TITLE, OnUpdateTitleMixin.onUpdateTitle);
    });

    it('will call onUpdateTitle passing $route.meta in mounted', () => {
      const onUpdateTitle = jest.fn();
      const meta = { titleKey: 'aubergine' };

      mountOnUpdateTitleMixin({ methods: { onUpdateTitle }, $route: { meta } });

      expect(onUpdateTitle).toHaveBeenCalledWith(meta);
    });
  });

  describe('setTitle method', () => {
    let title;

    describe('title is a function', () => {
      beforeEach(() => {
        wrapper = mountOnUpdateTitleMixin();
        title = jest.fn(() => ('tomatoes'));
        wrapper.vm.setTitle(title);
      });

      it('will set the title data prop to result of title', () => {
        expect(wrapper.vm.title).toEqual('tomatoes');
      });

      it('will pass $store and $i18n as parameters', () => {
        expect(title).toHaveBeenCalledWith(wrapper.vm.$store, wrapper.vm.$i18n);
      });
    });

    describe('title is not a function', () => {
      describe('and is localised', () => {
        beforeEach(() => {
          wrapper = mountOnUpdateTitleMixin();
          title = 'i am not a function';
          wrapper.vm.setTitle(title, true);
        });

        it('will set the title data prop to title', () => {
          expect(wrapper.vm.title).toEqual('i am not a function');
        });
      });

      describe('and is not localised', () => {
        beforeEach(() => {
          wrapper = mountOnUpdateTitleMixin();
          title = 'appTitle';
          wrapper.vm.setTitle(title);
        });

        it('will set the title data prop to translated title', () => {
          expect(wrapper.vm.title).toEqual('NHS App');
        });
      });
    });
  });

  describe('onUpdateTitle method', () => {
    let setTitle;

    beforeEach(() => {
      setTitle = jest.fn();
      wrapper = mountOnUpdateTitleMixin({ methods: { setTitle } });
      setTitle.mockClear();
    });

    describe('newTitle is an empty object', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateTitle();
      });

      it('will call setTitle passing undefined and localised parameter', () => {
        expect(setTitle).toHaveBeenCalledWith(undefined, false);
      });
    });

    describe('newTitle is a string', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateTitle('carrots', false);
      });

      it('will call setTitle passing newTitle value and localised parameter', () => {
        expect(setTitle).toHaveBeenCalledWith('carrots', false);
      });
    });

    describe('newTitle is an object with a titleKey', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateTitle({ titleKey: 'onions' }, true);
      });

      it('will call setTitle passing newTitle.titleKey value and localised parameter', () => {
        expect(setTitle).toHaveBeenCalledWith('onions', true);
      });
    });

    describe('newTitle is neither a string nor object', () => {
      beforeEach(() => {
        wrapper.vm.onUpdateTitle(1, false);
      });

      it('will not call setTitle', () => {
        expect(setTitle).not.toHaveBeenCalled();
      });
    });
  });
});
