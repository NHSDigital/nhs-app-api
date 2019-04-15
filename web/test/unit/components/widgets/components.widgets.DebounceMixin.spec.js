import debounceMixin from '@/components/widgets/DebounceMixin';
import { mount } from '../../helpers';

jest.useFakeTimers();

const verifyDebounceClicks = ({ clickFuncName = 'clicked', createComponent }) => {
  let $store;
  let func;
  let component;

  const performClick = ({ wrapper, clicks = 1 }) => {
    for (let i = 0; i < clicks; i += 1) {
      wrapper.vm[clickFuncName]();
    }
  };

  const mountWrapperAndClick = ({ clickDelay, clicks = 10 } = {}) => {
    const wrapper = mount(component, {
      propsData: clickDelay ? { clickDelay } : undefined,
      $store,
    });

    performClick({ wrapper, clicks });
    return wrapper;
  };

  beforeEach(() => {
    func = jest.fn();

    $store = {
      app: {
        $env: {
          DEBOUNCE_SHORT: 500,
          DEBOUNCE_MEDIUM: 1000,
          DEBOUNCE_LONG: 2000,
        },
      },
    };

    component = createComponent(func);
  });

  it(`will execute '${clickFuncName}' just once when click delay set`, () => {
    mountWrapperAndClick({ clickDelay: 'medium' });
    expect(func).toBeCalledTimes(1);
  });

  it(`will execute '${clickFuncName}' freely when click delay set to none`, () => {
    mountWrapperAndClick({ clickDelay: 'none' });
    expect(func).toBeCalledTimes(10);
  });

  it(`will execute '${clickFuncName}' just once when click delay not set`, () => {
    mountWrapperAndClick();
    expect(func).toBeCalledTimes(1);
  });

  it.each([['short', 500], ['medium', 1000], ['long', 2000]])(
    `will execute '${clickFuncName}' after the expected delay expires`, (clickDelay, duration) => {
      const wrapper = mountWrapperAndClick({ clickDelay, clicks: 1 });
      setTimeout(() => {
        performClick({ wrapper });
        expect(func).toBeCalledTimes(2);
      }, duration);
    },
  );

  it.each([['short', 500], ['medium', 1000], ['long', 2000]])(
    'will suppress debounced function within debounce duration', (clickDelay, duration) => {
      const wrapper = mountWrapperAndClick({ clickDelay, clicks: 1 });
      setTimeout(() => {
        performClick({ wrapper });
        setTimeout(() => {
          expect(func).toBeCalledTimes(1);
        }, 100);
      }, duration - 20);
    },
  );
};

describe('DebounceMixin.vue', () => {
  const createComponentWithClickedFunction = func => ({
    render() {},
    mixins: [debounceMixin],
    methods: {
      clicked() {
        func();
      },
    },
  });

  const createComponentWithoutClickedFunction = func => ({
    render() {},
    mixins: [debounceMixin],
    created() {
      this.useDebounce(this.act, 'act');
    },
    methods: {
      act() {
        func();
      },
    },
  });

  describe('component with clicked method', () => {
    verifyDebounceClicks({ createComponent: createComponentWithClickedFunction });
  });

  describe('component with custom method', () => {
    verifyDebounceClicks({ createComponent: createComponentWithoutClickedFunction, clickFuncName: 'act' });
  });
});
