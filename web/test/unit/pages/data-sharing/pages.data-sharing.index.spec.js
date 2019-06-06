/* eslint-disable import/no-extraneous-dependencies */
import DataSharing from '@/pages/data-sharing/';
import { create$T, createStore, mount } from '../../helpers';
import { initialState } from '@/store/modules/navigation/mutation-types';

const $t = create$T();

describe('data sharing index', () => {
  let $store;
  let wrapper;

  const createState = (state = {
    device: {
      source: 'web',
    },
    navigation: initialState(),
  }) => state;

  const mountPage = () => mount(DataSharing, { $store, $t });
  let startNow;
  let insetText;

  const findInsetText = () => wrapper.find('.nhsuk-inset-text');

  beforeEach(() => {
    $store = createStore({ state: createState() });
    startNow = jest.fn();

    wrapper = mountPage();
    wrapper.vm.startNow = startNow;
  });

  it('will contain inset text', () => {
    for (let i = 0; i <= 3; i += 1) {
      wrapper.setData({ pageIndex: i });
      insetText = findInsetText();

      if (i === 0 || i === 3) {
        expect(insetText.exists()).toBe(true);
      }
    }
  });

  it('will not contain inset text', () => {
    for (let i = 0; i <= 3; i += 1) {
      wrapper.setData({ pageIndex: i });
      insetText = findInsetText();

      if (i === 1 || i === 2) {
        expect(insetText.exists()).toBe(false);
      }
    }
  });

  it('will change the page to a passed in index', () => {
    wrapper.vm.changePage(2);
    expect(wrapper.vm.pageIndex).toBe(2);
  });

  it('will have start now button on page 4', () => {
    const button = wrapper.find('#next-button');
    for (let i = 0; i <= 3; i += 1) {
      button.trigger('click');
      wrapper.setData({ pageIndex: i });

      if (i === 3) {
        expect(wrapper.find('#startNowButton').exists()).toBe(true);
      }
    }
  });
});
