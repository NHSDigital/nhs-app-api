/* eslint-disable import/no-extraneous-dependencies */
/* eslint-disable no-plusplus */
import Vue from 'vue';
import { createLocalVue } from '@vue/test-utils';

import ParticipationPage from '@/pages/gp-finder/participation';
import AuthorisationService from '@/services/authorisation-service';

import { initialState as deviceInitialState } from '@/store/modules/device/mutation-types';
import { initialState as headerInitialState } from '@/store/modules/header/mutation-types';
import { initialState as throttlingInitialState } from '@/store/modules/throttling/mutation-types';

import { shallowMount, createStore } from '../../helpers';

jest.mock('@/services/authorisation-service');

describe(('GP Finder participation page'), () => {
  describe(('asyncData'), () => {
    let keyIndex;
    let $t;
    let localVue;
    let $store;
    let options;

    beforeEach(() => {
      AuthorisationService.mockClear();

      keyIndex = 0;
      $t = key => `translate_${key}_${keyIndex++}`;

      localVue = createLocalVue();
      localVue.mixin(Vue.mixin({
        methods: {
          goToUrl: jest.fn(),
        },
      }));

      $store = createStore({
        state: {
          throttling: throttlingInitialState(),
          device: deviceInitialState(),
          header: headerInitialState(),
        },
      });

      options = {
        localVue,
        $store,
        mocks: {
          $t,
        },
      };
    });

    it('will generate login values for login if practice is participating', () => {
      // arrange
      $store.state.throttling.selectedGpPractice = {
        PracticeParticipating: true,
      };
      const wrapper = shallowMount(ParticipationPage, options);

      // act
      wrapper.vm.$options.asyncData({ store: $store });

      // assert
      expect(AuthorisationService.mock.instances[0].generateLoginValues).toHaveBeenCalled();
    });

    it('will not generate login values for login if practice is participating', () => {
      // arrange
      $store.state.throttling.selectedGpPractice = {
        PracticeParticipating: false,
      };
      const wrapper = shallowMount(ParticipationPage, options);

      // act
      wrapper.vm.$options.asyncData({ store: $store });

      // assert
      expect(AuthorisationService.mock.instances).toEqual([]);
    });
  });
});

