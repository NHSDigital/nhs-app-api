import Vue from 'vue';
import LoginOrRegister from '@/components/LoginOrRegister';

describe('LoginOrRegister.vue', () => {
  let vm;

  beforeEach(() => {
    const Constructor = Vue.extend(LoginOrRegister);
    vm = new Constructor().$mount();
  });

  it('should contain a login button', () => {
    expect(vm.$el.querySelector('button[data-id="login-button"]')).not.toBeNull();
    expect(vm.$el.querySelector('button[data-id="login-button"]').textContent.trim())
      .toEqual('Login with your NHS account');
  });

  it('should contain a create account button', () => {
    expect(vm.$el.querySelector('button[data-id="create-account-button"]')).not.toBeNull();
    expect(vm.$el.querySelector('button[data-id="create-account-button"]').textContent.trim())
      .toEqual('Create an NHS account');
  });
});
