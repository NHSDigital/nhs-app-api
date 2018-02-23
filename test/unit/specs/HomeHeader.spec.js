import Vue from 'vue';
import HomeHeader from '@/components/HomeHeader';

describe('HomeHeader.vue', () => {
  let vm;

  beforeEach(() => {
    const Constructor = Vue.extend(HomeHeader);
    vm = new Constructor().$mount();
  });

  it('should contain an NHS Online logo', () => {
    expect(vm.$el.querySelector('.nhso_logo svg')).not.toBeNull();
  });

  it('should contain a welcome message', () => {
    const welcomeMessage = vm.$el.querySelector('h1');
    expect(welcomeMessage).not.toBeNull();
    expect(welcomeMessage.textContent.trim()).toEqual('Welcome!');
  });

  it('should contain a symptom checker', () => {
    const welcomeMessage = vm.$el.querySelector('.symptom_banner h2');
    expect(welcomeMessage).not.toBeNull();
    expect(welcomeMessage.textContent.trim()).toEqual('How are you feeling right now?');
  });

  it('should contain a symptom checker button', () => {
    const welcomeMessage = vm.$el.querySelector('#btn_home_symptoms');
    expect(welcomeMessage).not.toBeNull();
    expect(welcomeMessage.textContent.trim()).toEqual('Symptom checker');
  });
});
