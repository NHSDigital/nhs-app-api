import { getModalContent, registerModalContent } from '@/components/modal/modalRegister';

describe('test modalRegister.js', () => {
  beforeEach(() => {
  });

  describe('getModalContent', () => {
    it('should retrieve registered modal content from the register', () => {
      const modalContent = getModalContent('SessionExpiryModal');
      expect(modalContent.name).toEqual('SessionExpiryModal');
    });
  });

  describe('registerModalContent', () => {
    it('should retrieve registered modal content from the register', () => {
      registerModalContent({ name: 'TestModal' });
      const modalContent = getModalContent('TestModal');
      expect(modalContent.name).toEqual('TestModal');
    });

    it('should fail to register a duplicate modal content', (done) => {
      registerModalContent({ name: 'DuplicateTestModal' });
      try {
        registerModalContent({ name: 'DuplicateTestModal' });
        done.fail('Should fail');
      } catch (err) {
        expect(err.message).toEqual('Cannot register duplicate component [DuplicateTestModal].');

        done();
      }
    });

    it('should fail to register a anonymous modal content', (done) => {
      try {
        registerModalContent({});
        done.fail('Should fail');
      } catch (err) {
        expect(err.message).toEqual('Cannot register component without a name.');
        done();
      }
    });
  });
});
