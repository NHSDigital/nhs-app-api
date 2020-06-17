import Warning from '@/components/my-record/Warning';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { createStore, shallowMount } from '../../helpers';

const createState = () => ({
  myRecord: initialState(),
  device: {
    isNativeApp: false,
  },
});

const createComponent = ({ $store = createStore({ state: createState() }) } = {}) => {
  const $style = {
    info: 'info',
    h2: 'h2',
    button: [],
  };

  return shallowMount(Warning, {
    $store,
    $style,
  });
};

describe('Warning', () => {
  let $store;
  let component;

  beforeEach(() => {
    $store = createStore({ state: createState() });
    component = createComponent({ $store });
  });

  describe('onContinueButtonClicked', () => {
    beforeEach(() => {
      component.vm.onContinueButtonClicked();
    });

    it('will accept the terms', () => {
      expect($store.dispatch).toHaveBeenCalledWith('myRecord/acceptTerms');
    });

    it('will load my record', () => {
      expect($store.dispatch).toHaveBeenCalledWith('myRecord/load');
    });
  });
});

