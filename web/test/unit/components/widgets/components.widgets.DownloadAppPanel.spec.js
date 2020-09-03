import DownloadAppPanel from '@/components/widgets/DownloadAppPanel';
import i18n from '@/plugins/i18n';
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
    wrapper = mount(DownloadAppPanel, { mountOpts: { i18n } });
  });

  it('will have the correct content', () => {
    expect(wrapper.find('[data-id="panel-title"]').exists()).toBe(true);
    expect(wrapper.find('[data-id="apple-store-icon"]').exists()).toBe(true);
    expect(wrapper.find('[data-id="play-store-icon"]').exists()).toBe(true);
  });

  it('the title will contain the correct text', () => {
    expect(wrapper.find('[data-id="panel-title"]').text()).toBe('Get the NHS App on your smartphone or tablet');
  });

  it('the images will contain alt tags', () => {
    expect(wrapper.find('[data-id="apple-store-icon"]').find('img').attributes('alt')).toEqual('Download on the App Store');
    expect(wrapper.find('[data-id="play-store-icon"]').find('img').attributes('alt')).toEqual('Get it on Google Play');
  });
});
