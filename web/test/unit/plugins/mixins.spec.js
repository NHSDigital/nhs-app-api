/* eslint-disable import/no-extraneous-dependencies */
import Vue from 'vue';
import '@/plugins/mixins';

describe('mixins are loaded', () => {
  it('will have correct number of globally registered mounted mixins', () => {
    const expectedGlobalMountedMixins = [
      'ResetPageFocus',
    ];
    // assert
    const mountedFunctions = Vue.options.mounted;
    expect(mountedFunctions.length).toBe(expectedGlobalMountedMixins.length);
  });
});
