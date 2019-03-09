import Warning from '@/components/my-record/Warning';
import { initialState } from '@/store/modules/myRecord/mutation-types';
import { INDEX, MYRECORD } from '@/lib/routes';
import { createEvent, createStore, shallowMount } from '../../helpers';

const createState = () => ({
  myRecord: initialState(),
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
  let event;
  let component;

  beforeEach(() => {
    $store = createStore({ state: createState() });
    event = createEvent();
    component = createComponent({ $store });
  });

  it('will have a form that performs a get request to the my record path', () => {
    const form = component.find(`form[action="${MYRECORD.path}"]`);

    expect(form.exists()).toBe(true);
    expect(form.attributes().method).toEqual('get');
  });

  it('will have a form that performs a get request to the index path', () => {
    const form = component.find(`form[action="${INDEX.path}"]`);

    expect(form.exists()).toBe(true);
    expect(form.attributes().method).toEqual('get');
  });

  describe('onBackButtonClicked', () => {
    beforeEach(() => {
      component.vm.onBackButtonClicked(event);
    });

    it('will prevent default on the event', () => {
      expect(event.preventDefault).toHaveBeenCalled();
    });

    it('will push the index page onto the route', () => {
      expect(component.vm.$router).toContain(INDEX.path);
    });
  });

  describe('onContinueButtonClicked', () => {
    beforeEach(() => {
      component.vm.onContinueButtonClicked(event);
    });

    it('will prevent default on the event', () => {
      expect(event.preventDefault).toHaveBeenCalled();
    });

    it('will accept the terms', () => {
      expect($store.dispatch).toHaveBeenCalledWith('myRecord/acceptTerms');
    });
  });
});

