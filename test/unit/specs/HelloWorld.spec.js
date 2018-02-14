import Vue from 'vue';
import HelloWorld from '@/components/HelloWorld';
import NHSOnlineApi, { DEFAULT_VALUES } from '../mocks/NHSOnlineApi';

describe('HelloWorld.vue', () => {
  let vm;

  beforeEach(() => {
    const Constructor = Vue.extend(HelloWorld);
    vm = new Constructor({
      inject: {
        nhsOnlineApi: {
          default: new NHSOnlineApi(),
        },
      },
    }).$mount();
  });

  it('should render correct contents', () => {
    expect(vm.$el.querySelector('.bodyDiv h1').textContent).toEqual('NHS Online');
  });

  it('should present a button for getting values', () => {
    expect(vm.$el.querySelector('#get-values-button').textContent).toEqual('Get Values');
  });

  describe('getValues', () => {
    it('should set the values from the api when invoked', () =>
      vm.getValues().then(() => expect(vm.values).toEqual(DEFAULT_VALUES)));
  });
});
