import GenericQuestionWrapper from '@/components/online-consultations/GenericQuestionWrapper';
import each from 'jest-each';
import { mount, shallowMount } from '../../helpers';

const defaultPropsData = {
  text: 'test',
};

describe('GenericQuestionWrapper', () => {
  describe('GenericQuestionWrapper', () => {
    it('should have a question component within a div', () => {
      const options = {
        propsData: {
          ...defaultPropsData,
        },
      };
      const wrapper = shallowMount(GenericQuestionWrapper, options);

      expect(wrapper.find('div question-stub').element).toBeDefined();
    });

    it('should have a default slot', () => {
      const options = {
        propsData: {
          ...defaultPropsData,
        },
        slots: {
          default: '<div class="slot-content"><p>Slot data</p></div>',
        },
      };
      const wrapper = mount(GenericQuestionWrapper, options);

      expect(wrapper.find('div.slot-content').element).toBeDefined();
    });
  });

  describe('Question component', () => {
    let options = {
      propsData: {
        id: 'testId',
        questionTag: 'a',
        text: 'test',
        error: false,
        labelFor: 'labelFor',
        isLegend: true,
        required: true,
      },
    };
    let wrapper = shallowMount(GenericQuestionWrapper, options);
    let questionVm = wrapper.find('question-stub').vm;

    it('should pass id correctly', () => {
      expect(questionVm.id).toEqual('testId');
    });

    it('should pass questionTag correctly', () => {
      expect(questionVm.questionTag).toEqual('a');
    });

    it('should pass labelFor correctly', () => {
      expect(questionVm.labelFor).toEqual('labelFor');
    });

    it('should pass text correctly', () => {
      expect(questionVm.text).toEqual('test');
    });

    it('should pass error correctly', () => {
      expect(questionVm.error).toEqual(false);
    });

    it('should pass isLegend correctly', () => {
      expect(questionVm.isLegend).toEqual(true);
    });

    it('should pass required correctly', () => {
      expect(questionVm.required).toEqual(true);
    });

    it('should be passed question slot', () => {
      options = {
        propsData: {
        },
        slots: {
          questionSlot: '<div class="question-slot-content"><p>Question slot data</p></div>',
        },
      };

      wrapper = shallowMount(GenericQuestionWrapper, options);
      questionVm = wrapper.find('question-stub').vm;
      const questionSlot = questionVm.$slots.questionSlot.pop();

      expect(questionSlot.tag).toEqual('div');
      expect(questionSlot.data.staticClass).toEqual('question-slot-content');
    });
  });

  describe('formGroupClasses', () => {
    each([{
      error: false,
      classes: 'nhsuk-form-group',
    }, {
      error: true,
      classes: ['nhsuk-form-group', 'nhsuk-form-group--error'],
    }]).it('should set appropriate classes on the root element if it there is an error', (data) => {
      const wrapper = mount(GenericQuestionWrapper, {
        propsData: {
          ...defaultPropsData,
          error: data.error,
        },
      });

      expect(wrapper.vm.formGroupClasses).toEqual(data.classes);
    });
  });
});
