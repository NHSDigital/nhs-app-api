import Modal from '@/components/modal/Modal';
import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';
import { mount } from '../../helpers';

describe('Modal.vue', () => {
  const createModal = (
    {
      $store = {
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
      propsData,
    } = {},
  ) =>
    mount(Modal, {
      $store,
      propsData,
    });


  it('will not render modal content due as not visible in model.', () => {
    const wrapper = createModal({
      $store: {
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: false,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find("div[role='dialog'][aria-modal='true']")
      .exists()).toEqual(false);
  });

  it('will not render modal content as in native mode.', () => {
    const wrapper = createModal({
      $store: {
        state: {
          device: {
            isNativeApp: true,
          },
          modal: {
            config: {
              visible: true,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find("div[role='dialog'][aria-modal='true']")
      .exists()).toEqual(false);
  });

  it('will render modal content as visible in model.', () => {
    const wrapper = createModal({
      $store: {
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find("div[role='dialog'][aria-modal='true']")
      .exists()).toEqual(true);
  });

  it('will render modal content with default width.', () => {
    const wrapper = createModal({
      $store: {
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find("div[style='max-width: 400px;']")
      .exists()).toEqual(true);
  });

  it('will render modal content with overridden width.', () => {
    const wrapper = createModal({
      $store: {
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
              maxWidth: '500px',
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find("div[style='max-width: 500px;']")
      .exists()).toEqual(true);
  });

  it('will render modal internal content', () => {
    const wrapper = createModal({
      $store: {
        app: { $env: {} },
        $env: {
          SESSION_EXPIRING_WARNING_SECONDS: 60,
        },
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
              content: SessionExpiryModal.name,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find('p').text())
      .toEqual('translate_web.sessionExpiry.warningDurationInformation');
  });

  it('will not render modal internal content', () => {
    const wrapper = createModal({
      $store: {
        app: { $env: {} },
        $env: {
          SESSION_EXPIRING_WARNING_SECONDS: 60,
        },
        state: {
          device: {
            isNativeApp: false,
          },
          modal: {
            config: {
              visible: true,
              // No Internal Content
              content: undefined,
            },
          },
        },
        subscribe: jest.fn(() => Promise.resolve()),
      },
    });

    expect(wrapper.find('p')
      .exists()).toEqual(false);
  });
});
