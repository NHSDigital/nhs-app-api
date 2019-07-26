import actions from '@/store/modules/flashMessage/actions';
import { ADD, HAS_BEEN_SHOWN, CLEAR, INIT, SHOW } from '@/store/modules/flashMessage/mutation-types';

const { addSuccess, addWarning, clear, init, hasBeenShown, show } = actions;
describe('addSuccess', () => {
  it('will call addSuccess with the flashMessage', () => {
    const flashMessage = 'This is a message';
    const commit = jest.fn();
    addSuccess({ commit }, flashMessage);
    expect(commit).toBeCalledWith(ADD, { message: flashMessage, type: 'success' });
  });
});

describe('addWarning', () => {
  it('will call addWarning with the sent value', () => {
    const flashMessage = 'This is a message';
    const commit = jest.fn();
    addWarning({ commit }, flashMessage);
    expect(commit).toBeCalledWith(ADD, { message: flashMessage, type: 'warning' });
  });
});

describe('clear', () => {
  it('will call clear', () => {
    const commit = jest.fn();
    clear({ commit });
    expect(commit).toBeCalledWith(CLEAR);
  });
});

describe('hasBeenShown', () => {
  it('will call hasBeenShown', () => {
    const commit = jest.fn();
    hasBeenShown({ commit });
    expect(commit).toBeCalledWith(HAS_BEEN_SHOWN);
  });
});

describe('init', () => {
  it('will call init', () => {
    const commit = jest.fn();
    init({ commit });
    expect(commit).toBeCalledWith(INIT);
  });
});

describe('show', () => {
  it('will call show', () => {
    const commit = jest.fn();
    show({ commit });
    expect(commit).toBeCalledWith(SHOW);
  });
});
