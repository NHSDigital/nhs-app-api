import each from 'jest-each';

import actions from '@/store/modules/modal/actions';
import { SHOW_MODAL } from '@/store/modules/modal/mutation-types';

describe('test modal/actions.js', () => {
  let store;
  let commit;

  const show = async config => actions.show(store, config);

  describe('show', () => {
    beforeEach(() => {
      store = { commit: jest.fn() };
      commit = jest.spyOn(store, 'commit');
    });

    each([
      ['content', { content: 'SomeComponent' }],
      ['content.name', { content: { name: 'SomeComponent' } }],
    ]).it('should ensure that there is a config object using %s', async (_, config) => {
      await show(config);

      expect(commit).toHaveBeenCalledWith(SHOW_MODAL, { content: 'SomeComponent' });
    });

    each([
      ['missing', undefined],
      ['missing content', {}],
    ]).it('should raise error when config is %s', async (_, config) => {
      expect(show(config)).rejects.toThrow('Modal config is required for modal display.');
    });
  });
});
