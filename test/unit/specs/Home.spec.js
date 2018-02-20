import Vue from 'vue';
import Home from '@/components/Home';

describe('Home.vue', () => {
  let vm;

  beforeEach(() => {
    const Constructor = Vue.extend(Home);
    vm = new Constructor().$mount();
  });

  it('should contain a home header', () => {
    expect(vm.$el.querySelector('.home-header')).not.toBeNull();
  });

  it('should contain a LoginOrRegister component', () => {
    expect(vm.$el.querySelector('.login-or-register')).not.toBeNull();
  });
});
