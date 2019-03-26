/* eslint-disable import/no-extraneous-dependencies */
import DataSharing from '@/pages/data-sharing/';
import { createStore, mount } from '../../helpers';
import { createLocalVue } from '@vue/test-utils';
import { initialState } from '@/store/modules/navigation/mutation-types';

const createState = () => ({
  device: {
    source: 'web',
  },
});
const state = createState();
const localVue = createLocalVue();
state.navigation = initialState();
const $store = createStore({ state });

it('will retrieve the token on the last on the page', () => {
  const wrapper = mount(DataSharing, {
    data() {
      return {
        pageIds: ['p1', 'p2', 'p3', 'p4'],
      };
    },
    localVue,
    $store,
  });

  const getNdopToken = jest.fn();
  wrapper.vm.getNdopToken = getNdopToken;
  const button = wrapper.find('#next-button');

  for(let i = 0; i <= 3; i++) {
    button.trigger('click');
    wrapper.setData({ pageIndex: i })

    if(i === 3) {
      expect(getNdopToken).toHaveBeenCalled();
    } else {
      expect(getNdopToken).not.toHaveBeenCalled();
    }
  }


});
