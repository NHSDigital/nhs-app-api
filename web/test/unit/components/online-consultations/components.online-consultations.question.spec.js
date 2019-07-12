import Question from '@/components/online-consultations/Question';
import each from 'jest-each';
import { mount } from '../../helpers';
import { SUCCESS } from '@/lib/online-consultations/constants/status-types';

const defaultPropsData = {
  text: 'test',
};
const optionalSuffix = '<br><p>(translate_appointments.admin_help.question.optional_label)</p>';

describe('Question', () => {
  describe('Question element', () => {
    each(['p', 'div', 'label'])
      .it('should have a question element based on the question-tag property', (questionTag) => {
        const wrapper = mount(Question, {
          propsData: {
            ...defaultPropsData,
            questionTag,
          },
        });

        expect(wrapper.find(`${questionTag}.nhsuk-label`).element).toBeDefined();
      });
    it('should render the text prop as html inside the question tag', () => {
      const wrapper = mount(Question, {
        propsData: {
          text: '<div class="slot-content"><p>Slot data</p></div>',
          questionTag: 'p',
        },
      });

      expect(wrapper.find('p>div.slot-content').element).toBeDefined();
    });
    it('should allow a for attribute to be set on the question (for labels)', () => {
      const wrapper = mount(Question, {
        propsData: {
          ...defaultPropsData,
          questionTag: 'label',
          labelFor: 'test-input-id',
        },
      });

      expect(wrapper.find('label[for=test-input-id]').element).toBeDefined();
    });
  });
  describe('Slots', () => {
    it('should have a default slot', () => {
      const options = {
        propsData: {
          ...defaultPropsData,
        },
        slots: {
          default: '<div class="slot-content"><p>Slot data</p></div>',
        },
      };
      const wrapper = mount(Question, options);

      expect(wrapper.find('div.slot-content').element).toBeDefined();
    });
  });
  describe('Computed properties', () => {
    describe('questionClass', () => {
      each([{
        isLegend: true,
        class: 'nhsuk-fieldset__legend',
      }, {
        isLegend: false,
        class: 'nhsuk-label',
      }]).it('should set an appropriate class on the question element if it is a \'legend\'', (data) => {
        const wrapper = mount(Question, {
          propsData: {
            ...defaultPropsData,
            isLegend: data.isLegend,
          },
        });

        expect(wrapper.vm.questionClass).toEqual(data.class);
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
        const wrapper = mount(Question, {
          propsData: {
            ...defaultPropsData,
            error: data.error,
          },
        });

        expect(wrapper.vm.formGroupClasses).toEqual(data.classes);
      });
    });
    describe('htmlText', () => {
      each([{
        required: true,
        status: 'test-status',
        expectedText: defaultPropsData.text,
      }, {
        required: false,
        status: SUCCESS,
        expectedText: defaultPropsData.text,
      }, {
        required: false,
        status: 'test-status',
        expectedText: `${defaultPropsData.text}${optionalSuffix}`,
      }]).it('should append optional unless status is success or question is required', ({ required, status, expectedText }) => {
        const wrapper = mount(Question, {
          propsData: {
            ...defaultPropsData,
            required,
          },
          state: {
            onlineConsultations: {
              status,
            },
          },
        });

        expect(wrapper.vm.htmlText).toEqual(expectedText);
      });
    });
  });
});
