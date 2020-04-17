import ReceivedMessagePanel from '@/components/patient-practice-messaging/ReceivedMessagePanel';
import { createStore, createRouter, create$T, shallowMount } from '../../helpers';
import { formatInboxMessageTime, redirectTo } from '@/lib/utils';

jest.mock('@/lib/utils');

describe('Received Message Panel', () => {
  let wrapper;
  let store;
  let $router;

  beforeAll(() => {
    formatInboxMessageTime.mockImplementation(() => '2019-12-09T13:56:50.377Z');
  });

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
    wrapper = shallowMount(ReceivedMessagePanel, {
      propsData,
      $store: store,
      $router,
      $t: create$T(),
    });
  });

  it('will show the view and download links', () => {
    const downloadLink = wrapper.find('desktopgenericbacklink-stub[id="downloadLink"]');
    const viewLink = wrapper.find('desktopgenericbacklink-stub[id="viewLink"]');

    expect(downloadLink.vm.buttonText).toEqual('patient_practice_messaging.view_details.download');
    expect(viewLink.vm.buttonText).toEqual('patient_practice_messaging.view_details.view');
    expect(downloadLink.exists()).toBe(true);
    expect(viewLink.exists()).toBe(true);
  });

  it('will navigate to the view attachment when clicked', async () => {
    await wrapper.vm.viewClicked();
    expect(store.dispatch).toBeCalledWith('documents/loadDocument', '1');
    expect(redirectTo).toBeCalledWith(wrapper.vm, '/patient-practice-messaging/view-attachment');
  });

  it('will navigate to the download attachment when clicked', async () => {
    await wrapper.vm.downloadClicked();
    expect(store.dispatch).toBeCalledWith('documents/loadDocument', '1');
    expect(store.dispatch).toBeCalledWith('patientPracticeMessaging/setAttachmentId', '1');
    expect($router.push).toBeCalledWith({
      name: 'patient-practice-messaging-download-attachment',
      params: { date: '2019-12-09T13:56:50.377Z' },
    });
  });
});
