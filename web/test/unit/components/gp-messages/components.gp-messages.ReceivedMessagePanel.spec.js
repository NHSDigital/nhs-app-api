import i18n from '@/plugins/i18n';
import ReceivedMessagePanel from '@/components/gp-messages/ReceivedMessagePanel';
import { redirectTo } from '@/lib/utils';
import { createStore, createRouter, shallowMount } from '../../helpers';

jest.mock('@/lib/utils');

describe('Received Message Panel', () => {
  let wrapper;
  let store;
  let $router;

  beforeEach(() => {
    $router = createRouter();
    store = createStore({
      state: {
        device: { isNativeApp: true },
        dispatch: jest.fn(),
      },
    });
    const propsData = {
      message: {
        sender: 'Test',
        content: 'This is a test',
        sentDateTime: '2019-12-09T13:56:50.377Z',
        isUnread: false,
      },
      attachmentId: '1',
      index: 0,
      idPrefix: 'initial',
      messageContent: 'This is a test',
    };
    wrapper = shallowMount(
      ReceivedMessagePanel,
      {
        propsData,
        $store: store,
        $router,
        mountOpts: { i18n },
      },
    );
  });

  it('will show the view and download links', () => {
    const downloadLink = wrapper.find('#downloadLink');
    const viewLink = wrapper.find('#viewLink');

    expect(downloadLink.exists()).toBe(true);
    expect(downloadLink.vm.buttonText).toEqual('messages.download');
    expect(viewLink.exists()).toBe(true);
    expect(viewLink.vm.buttonText).toEqual('messages.view');
  });

  it('will navigate to the view attachment when clicked', async () => {
    await wrapper.vm.viewClicked();
    expect(store.dispatch).toBeCalledWith('documents/loadDocument', { documentIdentifier: '1', updateMetaData: true });
    expect(redirectTo).toBeCalledWith(wrapper.vm, 'messages/gp-messages/view-attachment');
  });

  it('will navigate to the download attachment when clicked', async () => {
    await wrapper.vm.downloadClicked();
    expect(store.dispatch).toBeCalledWith('documents/loadDocument', { documentIdentifier: '1', updateMetaData: true });
    expect(store.dispatch).toBeCalledWith('gpMessages/setAttachmentId', '1');
    expect($router.push).toBeCalledWith({
      name: 'messages-gp-messages-download-attachment',
      params: { date: '2019-12-09T13:56:50.377Z' },
    });
  });
});
