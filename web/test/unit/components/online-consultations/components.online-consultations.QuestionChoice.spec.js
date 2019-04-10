import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import GenericRadioButton from '@/components/widgets/GenericRadioButton';
import { mount } from '../../helpers';

describe('questionChoice compoonent', () => {
  let wrapper;

  const mountQuestion = ({ propsData = {} } = {}) =>
    mount(QuestionChoice, {
      propsData: {
        name: 'name',
        options: [
          { code: 'first', description: 'first label' },
          { code: 'second', description: 'second label' },
          { code: 'third', description: 'third label' },
        ],
        ...propsData,
      },
    });

  describe('radio buttons', () => {
    it('will have radio button for the question with value of first', () => {
      wrapper = mountQuestion();
      const firstRadioButton = wrapper.findAll(GenericRadioButton).at(0);
      expect(firstRadioButton.vm.value).toEqual('first');
    });

    it('will have radio button for the question with value of second', () => {
      wrapper = mountQuestion();
      const secondRadioButton = wrapper.findAll(GenericRadioButton).at(1);
      expect(secondRadioButton.vm.value).toEqual('second');
    });

    it('will have radio button for the question with value of third', () => {
      wrapper = mountQuestion();
      const thirdRadioButton = wrapper.findAll(GenericRadioButton).at(2);
      expect(thirdRadioButton.vm.value).toEqual('third');
    });

    it('will have first radio button with correct label', () => {
      wrapper = mountQuestion();
      expect(wrapper.find("[for='name-first']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-first']").element.innerHTML)
        .toEqual('first label');
    });

    it('will have second radio button with correct label', () => {
      wrapper = mountQuestion();
      expect(wrapper.find("[for='name-second']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-second']").element.innerHTML)
        .toEqual('second label');
    });

    it('will have third radio button with correct label', () => {
      wrapper = mountQuestion();
      expect(wrapper.find("[for='name-third']")
        .exists())
        .toEqual(true);

      expect(wrapper.find("[for='name-third']").element.innerHTML)
        .toEqual('third label');
    });

    it('will emit first value when first clicked', () => {
      wrapper = mountQuestion();
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-first']");
      expect(input).toBeDefined();

      input.trigger('click');

      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm.__emitted.input[0][0]).toBe('first');
    });

    it('will emit second value when second clicked', () => {
      wrapper = mountQuestion();
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-second']");
      expect(input).toBeDefined();

      input.trigger('click');

      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm.__emitted.input[0][0]).toBe('second');
    });

    it('will emit third value when second clicked', () => {
      wrapper = mountQuestion();
      expect(wrapper.vm).toBeDefined();
      expect(wrapper.vm.selected).toBeDefined();

      expect(wrapper.vm.value).toBeUndefined();
      expect(wrapper.emitted('select')).not.toBeDefined();

      const input = wrapper.find("[id='name-third']");
      expect(input).toBeDefined();

      input.trigger('click');

      /* eslint-disable no-underscore-dangle */
      expect(wrapper.vm.__emitted.input[0][0]).toBe('third');
    });

    it('will add an undefined option to validValues if not required', () => {
      wrapper = mountQuestion({
        propsData: {
          required: false,
        },
      });

      expect(wrapper.vm.validValues).toEqual(['first', 'second', 'third', undefined]);
    });

    it('will not add undefined option to validValues if required', () => {
      wrapper = mountQuestion();

      expect(wrapper.vm.validValues).toEqual(['first', 'second', 'third']);
    });
  });
});
