/* eslint-disable guard-for-in, no-restricted-syntax, no-underscore-dangle */

import Vuex from 'vuex';
import { createLocalVue } from '@vue/test-utils';
import { actions } from '@/store/index';

const checkRecursively = (first, second) => {
  if (first instanceof String || first === undefined) return;
  expect(first).not.toBe(second);

  if (first instanceof Array && second instanceof Array) {
    for (const index in first) {
      checkRecursively(first[index], second[index]);
    }
  } else {
    for (const key in Object.keys(first)) {
      checkRecursively(first[key], second[key]);
    }
  }
};

describe('store', () => {
  beforeEach(() => {
    const localVue = createLocalVue();
    localVue.use(Vuex);
  });

  it('will have new state instances for each new store', () => {
    const storeOne = new Vuex.Store(actions);
    const storeTwo = new Vuex.Store(actions);

    for (const moduleKey of Object.keys(storeOne._modules.root._children)) {
      const moduleOne = storeOne._modules.root._children[moduleKey];
      const moduleTwo = storeTwo._modules.root._children[moduleKey];
      const firstState = moduleOne.state;
      const secondState = moduleTwo.state;

      checkRecursively(firstState, secondState);
    }
  });
});
