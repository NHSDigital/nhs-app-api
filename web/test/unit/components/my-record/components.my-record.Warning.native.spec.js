import Warning from '@/components/my-record/Warning';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { INDEX } from '@/lib/routes';
import { createStore, shallowMount } from '../../helpers';

const createState = () => ({
  myRecord: initialState(),
  device: {
    isNativeApp: true,
  },
});

const createComponent = ({ $store = createStore({ state: createState() }), data = {} } = {}) => {
  const $style = {
    info: 'info',
    h2: 'h2',
    button: [],
  };

  return shallowMount(Warning, {
    $store,
    $style,
    data,
  });
};

describe('Warning', () => {
  let $store;
  let component;

  beforeEach(() => {
    $store = createStore({ state: createState() });
    component = createComponent({ $store });
  });

  it('will have a form that performs a get request to the index path when on native', () => {
    const form = component.find(`form[action="${INDEX.path}"]`);

    expect(form.exists()).toBe(true);
    expect(form.attributes().method).toEqual('get');
  });
});

