import actions from '@/store/modules/modal/actions';
import { SHOW_MODAL } from '@/store/modules/modal/mutation-types';

describe('test modal/actions.js', () => {
  beforeEach(() => {
  });

  describe('show', () => {
    it('should ensure that there is a config object using Component', () => {
      const store = { commit: jest.fn() };
      const commit = jest.spyOn(store, 'commit');

      actions.show(store, { content: { name: 'SomeComponent' } });

      expect(commit).toBeCalledWith(SHOW_MODAL, { content: 'SomeComponent' });
    });

    it('should ensure that there is a config object using Component name', () => {
      const store = { commit: jest.fn() };
      const commit = jest.spyOn(store, 'commit');

      actions.show(store, { content: 'SomeComponent' });

      expect(commit).toBeCalledWith(SHOW_MODAL, { content: 'SomeComponent' });
    });

    it('should raise error when no config object is not supplied', (done) => {
      const store = { commit: jest.fn() };

      try {
        actions.show(store);
        done.fail('should have raised an error.');
      } catch (err) {
        expect(err.message).toEqual('Modal config is required for modal display.');

        done();
      }
    });

    it('should raise error when no config object is not supplied', (done) => {
      const store = { commit: jest.fn() };

      try {
        actions.show(store, {});
        done.fail('should have raised an error.');
      } catch (err) {
        expect(err.message).toEqual('Modal config is required for modal display.');
        done();
      }
    });
  });
});
