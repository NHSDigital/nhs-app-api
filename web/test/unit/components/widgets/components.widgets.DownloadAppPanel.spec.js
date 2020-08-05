/* eslint-disable import/no-extraneous-dependencies */
import DownloadAppPanel from '@/components/widgets/DownloadAppPanel';
import { createStore, mount } from '../../helpers';

let wrapper;

mount(DownloadAppPanel, {
  $store: createStore({
    state: {
      device: {
        source: 'web',
      },
    },
  }),
});

describe('download app panel', () => {
  beforeEach(() => {
    wrapper = mount(DownloadAppPanel);
  });

  it('will have the correct content', () => {
    expect(wrapper.find('[data-id="panel-title"]').exists()).toBe(true);
    expect(wrapper.find('[data-id="apple-store-icon"]').exists()).toBe(true);
    expect(wrapper.find('[data-id="play-store-icon"]').exists()).toBe(true);
  });

  it('the title will contain the correct text', () => {
    expect(wrapper.find('[data-id="panel-title"]').text()).toBe('translate_web.home.appStorePanel.title');
  });

  it('the images will contain alt tags', () => {
    expect(wrapper.find('[data-id="apple-store-icon"]').find('img').attributes('alt')).toEqual('translate_web.home.appStorePanel.appStoreLabel');
    expect(wrapper.find('[data-id="play-store-icon"]').find('img').attributes('alt')).toEqual('translate_web.home.appStorePanel.googlePlayLabel');
  });
});
