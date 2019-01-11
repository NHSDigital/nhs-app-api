/* eslint-disable object-curly-newline */
import RadioButton from '@/components/widgets/RadioButton';
import { createStore, mount } from '../../helpers';

describe('radio button', () => {
  let $store;
  let wrapper;

  beforeEach(() => {
    $store = createStore({ state: { radioState: true } });
  });

  describe('noJs computed properties', () => {
    beforeEach(() => {
      wrapper = mount(RadioButton, {
        $store,
        propsData: { action: 'foo', state: 'radio.state.foo', value: 'foo' },
      });
    });
    describe('noJsName', () => {
      it('will have a nojs name equal to state path', () => {
        expect(wrapper.vm.noJsName).toEqual('nojs.radio.state.foo');
      });
    });
  });

  describe('state same as value', () => {
    beforeEach(() => {
      wrapper = mount(RadioButton, {
        $store,
        propsData: { action: 'changeState', state: 'radioState', value: true },
      });
    });

    it('will dispatch `changeState` with the supplied value when clicked', () => {
      wrapper.find('label').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('changeState', true);
    });

    describe('computed properties', () => {
      describe('isSelected', () => {
        it('will be true', () => {
          expect(wrapper.vm.isSelected).toEqual(true);
        });
      });
    });
  });

  describe('state not the same as value', () => {
    beforeEach(() => {
      wrapper = mount(RadioButton, {
        $store,
        propsData: { action: 'changeState', state: 'radioState', value: false },
      });
    });

    it('will dispatch `changeState` with the supplied value when clicked', () => {
      wrapper.find('label').trigger('click');
      expect($store.dispatch).toHaveBeenCalledWith('changeState', false);
    });

    describe('computed properties', () => {
      describe('isSelected', () => {
        it('will be false', () => {
          expect(wrapper.vm.isSelected).toEqual(false);
        });
      });
    });
  });
});
