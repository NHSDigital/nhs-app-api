import WaitingListJoinedPage from '@/pages/gp-finder/waiting-list-joined';
import { mount, createStore } from '../../helpers';

describe('Waiting List Joined/Not Joined page', () => {
  it('will reset the throttling store when destroyed', () => {
    const $store = createStore({
      state: {
        header: {
          headerText: '',
        },
        device: {
          isNativeApp: true,
        },
      },
    });
    mount(WaitingListJoinedPage, {
      $store,
    }).destroy();

    expect($store.dispatch).toHaveBeenCalledWith('throttling/init');
  });
});
