import mutations from '@/store/modules/flashMessage/mutations';
import { CLEAR, ADD, HAS_BEEN_SHOWN, SHOW } from '@/store/modules/flashMessage/mutation-types';

it('will add a flash message to the state', () => {
  const state = {
    message: '',
    type: '',
  };

  const flashMessage = {
    type: 'Warning',
    message: 'This is a test flash message',
  };

  mutations[ADD](state, flashMessage);

  expect(state.message).toEqual(flashMessage.message);
  expect(state.type).toEqual(flashMessage.type);
});

it('will clear the message state if the message has been shown', () => {
  const state = {
    message: 'Clear this message',
    type: 'Warning',
    hasBeenShown: true,
  };

  mutations[CLEAR](state);

  expect(state.message).toEqual('');

  // type is set to 'success' when clear is called
  expect(state.type).toEqual('success');
});

it('will not clear the message state if the message has not been shown', () => {
  const state = {
    message: 'Clear this message',
    type: 'Warning',
    hasBeenShown: false,
  };

  mutations[CLEAR](state);

  expect(state.message).toEqual('Clear this message');
  expect(state.type).toEqual('Warning');
});

it('will set the message to shown', () => {
  const state = {
    hasBeenShown: false,
  };

  mutations[HAS_BEEN_SHOWN](state);
  expect(state.hasBeenShown).toEqual(true);
});

it('will show the message if message is populated', () => {
  let state = {
    message: '',
    show: true,
  };

  mutations[SHOW](state);
  expect(state.show).toEqual(false);

  state = {
    message: 'Show this message',
    show: false,
  };

  mutations[SHOW](state);
  expect(state.show).toEqual(true);
});

