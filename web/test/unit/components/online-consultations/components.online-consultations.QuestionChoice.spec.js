import QuestionChoice from '@/components/online-consultations/QuestionChoice';
// import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import { mount } from '../../helpers';

describe('questionChoice component', () => {
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

  describe('legend', () => {
    it('will include legend element if legend provided', () => {
      wrapper = mountQuestion({ legend: 'Question choice legend' });
      const legend = wrapper.find('#name-legend');
      expect(legend.text()).toEqual('Question choice legend');
    });
  });

  describe('radio buttons', () => {
    beforeEach(() => {
      wrapper = mountQuestion();
    });

    it('will have radio button for the question with value of first', () => {
      const firstRadioButton = wrapper.find('#name-first');
      expect(firstRadioButton.exists()).toBe(true);
    });

    it('will have radio button for the question with value of second', () => {
      const secondRadioButton = wrapper.find('#name-second');
      expect(secondRadioButton.exists()).toBe(true);
    });

    it('will have radio button for the question with value of third', () => {
      const thirdRadioButton = wrapper.find('#name-third');
      expect(thirdRadioButton.exists()).toBe(true);
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
