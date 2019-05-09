import SendingEmailPage from '@/pages/gp-finder/sending-email';
import { mount, createStore } from '../../helpers';

describe('Sending Email Page', () => {
  it('will reset the throttling store when created', () => {
    const $store = createStore({
      state: {
        header: {
          headerText: '',
        },
        device: {
          isNativeApp: true,
        },
        throttling: {},
      },
    });
    mount(SendingEmailPage, {
      $store,
    });

    expect($store.dispatch).toHaveBeenCalledWith('throttling/setWaitingListChoice', undefined);
  });
});
