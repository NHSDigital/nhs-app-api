import QuestionAttachment from '@/components/online-consultations/QuestionAttachment';
import GenericAttachment from '@/components/widgets/GenericAttachment';
import { questionAttachmentAnswerValid } from '@/lib/online-consultations/answer-validators';
import { mount } from '../../helpers';

jest.mock('@/lib/online-consultations/answer-validators');

let wrapper;
const getInputWrapper = (useComponent = true) => wrapper.find(useComponent ? GenericAttachment : 'input[type=file]');

const file = {
  name: 'test.png',
  type: 'image/png',
  size: 1048576,
};

const state = {
  device: {
    isNativeApp: false,
  },
};

const store = {
  state,
  dispatch: jest.fn(),
};

const mountQuestion = ({ propsData = {}, data = () => ({}), methods = {} } = {}) =>
  mount(QuestionAttachment, {
    $store: store,
    propsData,
    data,
    methods,
  });

describe('QuestionAttachment.vue', () => {
  afterEach(() => {
    store.dispatch.mockClear();
  });

  it('will have an input of type file', () => {
    // Arrange
    wrapper = mountQuestion();

    // Act
    const input = getInputWrapper();

    // Assert
    expect(input.exists()).toBe(true);
  });

  it('will have an aria described of optional-label if not required', () => {
    // Arrange
    wrapper = mountQuestion({
      propsData: {
        id: 'id',
        required: false,
        error: true,
        errorText: ['Error'],
      },
    });

    // Act
    const inputAttributes = wrapper.find('input').attributes();

    // Assert
    expect(inputAttributes['aria-describedby']).toBe('iderror optional-label id-error-message');
  });

  describe('Initial values', () => {
    beforeEach(() => {
      questionAttachmentAnswerValid.mockClear();
    });

    it('should emit if data is valid', () => {
      // Arrange
      questionAttachmentAnswerValid.mockReturnValue(true);
      wrapper = mountQuestion();

      // Act
      wrapper.vm.checkAndEmitIsValueValid(file);

      // Assert
      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toBe(1);
      expect(wrapper.emitted().validate[0][0]).toBe(true);
    });

    it('should emit if data is invalid', () => {
      questionAttachmentAnswerValid.mockReturnValue(false);

      wrapper = mountQuestion();
      wrapper.vm.checkAndEmitIsValueValid(file);

      expect(wrapper.emitted('validate')).toBeDefined();
      expect(wrapper.emitted().validate[0].length).toBe(1);
      expect(wrapper.emitted().validate[0][0]).toBe(false);
    });

    it('should create a file reader if running on client', () => {
      // Arrange
      const onFileLoad = jest.fn();
      const onFileError = jest.fn();
      const onFileAbort = jest.fn();
      const methods = {
        onFileLoad,
        onFileError,
        onFileAbort,
      };

      // Act
      wrapper = mountQuestion({ methods });

      // Assert
      expect(wrapper.vm.reader).toBeInstanceOf(FileReader);
    });
  });

  describe('attachment value change', () => {
    it('will check is valid, and emit attachment value', () => {
      // Arrange
      const checkAndEmitIsValueValid = jest.fn();
      wrapper = mountQuestion({
        methods: {
          checkAndEmitIsValueValid,
        },
      });
      const expectedValue = { attachmentValue: 'some stuff' };

      // Act
      wrapper.vm.attachmentValue = expectedValue;

      // Assert
      expect(checkAndEmitIsValueValid).toHaveBeenCalledWith(expectedValue);
      expect(wrapper.emitted('input')).toBeDefined();
      expect(wrapper.emitted().input[0].length).toBe(1);
      expect(wrapper.emitted().input[0][0]).toEqual(expectedValue);
    });
  });

  describe('component destroyed', () => {
    it('will call abort on file reader if defined', () => {
      // Arrange
      const onFileAbort = jest.fn();
      const reader = {
        abort() {
          onFileAbort();
        },
      };
      wrapper = mountQuestion();
      wrapper.vm.reader = reader;

      // Act
      wrapper.destroy();

      // Assert
      expect(onFileAbort).toHaveBeenCalled();
    });
    it('will not call abort on file reader if undefined', () => {
      // Arrange
      const onFileAbort = jest.fn();
      wrapper = mountQuestion({
        methods: {
          onFileAbort,
        },
      });

      // Act
      wrapper.destroy();

      // Assert
      expect(onFileAbort).not.toHaveBeenCalled();
    });
  });

  describe('methods', () => {
    describe('onSelectedFileChanged', () => {
      it('will get the first file from the event and call getFileAsBase64', () => {
        // Arrange
        const event = {
          target: {
            files: [{
              data: 'stuff',
            }],
          },
        };
        const getFileAsBase64 = jest.fn();
        wrapper = mountQuestion({
          methods: {
            getFileAsBase64,
          },
        });

        // Act
        wrapper.vm.onSelectedFileChanged(event);

        // Assert
        expect(wrapper.vm.file).toEqual(event.target.files[0]);
        expect(getFileAsBase64).toHaveBeenCalled();
      });
    });

    describe('getFileAsBase64', () => {
      describe('file is undefined', () => {
        it('will set attachmentValue to undefined', () => {
          // Arrange
          wrapper = mountQuestion({
            propsData: {
              value: {
                data: 'stuff',
              },
            },
          });

          // Act
          wrapper.vm.getFileAsBase64();

          // Assert
          expect(wrapper.vm.attachmentValue).toBeUndefined();
        });
      });

      describe('file is defined', () => {
        it('will set fileLoading and read data using file reader', () => {
          // Arrange
          const readAsDataURL = jest.fn();
          wrapper = mountQuestion();
          wrapper.vm.reader = {
            readAsDataURL,
          };
          wrapper.vm.file = file;

          // Act
          wrapper.vm.getFileAsBase64();

          // Assert
          expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/fileLoading');
          expect(store.dispatch).toHaveBeenCalledTimes(1);
          expect(readAsDataURL).toHaveBeenCalledWith(file);
          expect(readAsDataURL).toHaveBeenCalledTimes(1);
        });
      });
    });

    describe('onFileLoad', () => {
      it('will set fileLoading complete and update attachmentValue', () => {
        // Arrange
        const expectedAttachmentValue = {
          name: file.name,
          type: file.type,
          size: file.size,
          base64: 'thisissomeimagedata',
        };
        wrapper = mountQuestion();
        wrapper.vm.file = file;
        wrapper.vm.reader = {
          result: 'data:image/png;base64,thisissomeimagedata',
        };

        // Act
        wrapper.vm.onFileLoad();

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/fileLoadComplete');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(wrapper.vm.attachmentValue).toEqual(expectedAttachmentValue);
      });
    });

    describe('onFileError', () => {
      it('will set fileLoading complete and set attachmentValue to undefined', () => {
        // Arrange
        wrapper = mountQuestion({
          propsData: {
            value: {
              data: 'stuff',
            },
          },
        });

        // Act
        wrapper.vm.onFileError();

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/fileLoadComplete');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(wrapper.vm.attachmentValue).toBeUndefined();
      });
    });

    describe('onFileAbort', () => {
      it('will set fileLoading complete and set attachmentValue to undefined', () => {
        // Arrange
        wrapper = mountQuestion({
          propsData: {
            value: {
              data: 'stuff',
            },
          },
        });

        // Act
        wrapper.vm.onFileAbort();

        // Assert
        expect(store.dispatch).toHaveBeenCalledWith('onlineConsultations/fileLoadComplete');
        expect(store.dispatch).toHaveBeenCalledTimes(1);
        expect(wrapper.vm.attachmentValue).toBeUndefined();
      });
    });
  });
});
