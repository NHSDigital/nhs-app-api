import Question from '@/components/online-consultations/Question';
import each from 'jest-each';
import { mount } from '../../helpers';

const defaultPropsData = {
  text: 'test',
};

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

  describe('Optional label', () => {
    each([{
      required: true,
      status: 'dataRequired',
      visible: false,
    }, {
      required: false,
      status: 'success',
      visible: false,
    }, {
      required: false,
      status: 'dataRequired',
      visible: true,
    }]).it('should only render the optional label if required is false and olc status is not success', ({ required, status, visible }) => {
      const wrapper = mount(Question, {
        propsData: {
          ...defaultPropsData,
          required,
        },
        $store: {
          state: {
            onlineConsultations: {
              status,
            },
          },
        },
      });

      expect(wrapper.find('p.optionalLabel').exists()).toBe(visible);
    });
  });

  describe('Slots', () => {
    it('should have a question-slot', () => {
      const options = {
        propsData: {
          questionTag: 'p',
        },
        slots: {
          questionSlot: '<div class="question-slot-content"><p>Slot data</p></div>',
        },
      };
      const wrapper = mount(Question, options);

      expect(wrapper.find('div.question-slot-content').element).toBeDefined();
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
  });
});
