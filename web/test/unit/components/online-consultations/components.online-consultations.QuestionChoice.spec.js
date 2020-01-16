import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import RadioGroup from '@/components/RadioGroup';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import { mount } from '../../helpers';
import each from 'jest-each';

describe('questionChoice compoonent', () => {
  let wrapper;

  const mountQuestion = ({ renderAsHtml = false, legend = undefined } = {}) =>
    mount(QuestionChoice, {
      propsData: {
        name: 'name',
        options: [
          { code: 'first', description: 'first label' },
          { code: 'second', description: 'second label' },
          { code: 'third', description: 'third label' },
        ],
        renderAsHtml,
        legend,
      },
    });

  describe('renderAsHtml', () => {
    each([
      true,
      false,
    ]).it('will set renderAsHtml on radio group', (renderAsHtml) => {
      wrapper = mountQuestion({ renderAsHtml });

      const radioGroup = wrapper.find(RadioGroup);

      expect(radioGroup.vm.renderAsHtml).toEqual(renderAsHtml);
    });
  });

  describe('legend', () => {
    it('will include legend element if legend provided', () => {
      wrapper = mountQuestion({ legend: 'Question choice legend' });
      const legend = wrapper.find('legend');
      expect(legend.text()).toEqual('Question choice legend');
    });
  });

  describe('radio buttons', () => {
    beforeEach(() => {
      wrapper = mountQuestion();
    });

    it('will have radio button for the question with value of first', () => {
      const firstRadioButton = wrapper.findAll(GenericRadioButton).at(0);
      expect(firstRadioButton.vm.value).toEqual('first');
    });

    it('will have radio button for the question with value of second', () => {
      const secondRadioButton = wrapper.findAll(GenericRadioButton).at(1);
      expect(secondRadioButton.vm.value).toEqual('second');
    });

    it('will have radio button for the question with value of third', () => {
      const thirdRadioButton = wrapper.findAll(GenericRadioButton).at(2);
      expect(thirdRadioButton.vm.value).toEqual('third');
    });

    it('will have first radio button with correct label', () => {
      expect(wrapper.find("[for='name-first']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-first']").element.innerHTML)
        .toEqual('first label');
    });

    it('will have second radio button with correct label', () => {
      expect(wrapper.find("[for='name-second']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-second']").element.innerHTML)
        .toEqual('second label');
    });

    it('will have third radio button with correct label', () => {
      expect(wrapper.find("[for='name-third']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-third']").element.innerHTML)
        .toEqual('third label');
    });

    it('will emit first value when first clicked', () => {
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-first']");
      expect(input).toBeDefined();

      input.trigger('click');

      expect(wrapper.emitted('input')[0][0]).toBe('first');
    });

    it('will emit second value when second clicked', () => {
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-second']");
      expect(input).toBeDefined();

      input.trigger('click');

      expect(wrapper.emitted('input')[0][0]).toBe('second');
    });

    it('will emit third value when second clicked', () => {
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-third']");
      expect(input).toBeDefined();

      input.trigger('click');

      expect(wrapper.emitted('input')[0][0]).toBe('third');
    });
  });
});
