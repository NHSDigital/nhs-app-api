import QuestionBoolean from '@/components/online-consultations/QuestionBoolean';
import each from 'jest-each';
import { mount } from '../../helpers';

describe('radio buttons', () => {
  let wrapper;
  const mountQuestion = ({ propsData = {}, methods = {} } = {}) =>
    mount(QuestionBoolean, {
      propsData: {
        value: undefined,
        trueId: 'tid',
        falseId: 'fid',
        optionOneLabel: 'labelOneText',
        optionTwoLabel: 'labelTwoText',
        name: 'name',
        ...propsData,
      },
      methods,
    });

  it('will have radio buttons for the question', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[id='name-Yes']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[id='name-No']")
      .exists())
      .toEqual(true);
  });

  it('will have true radio button with correct label', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[for='name-Yes']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-Yes']").element.innerHTML)
      .toEqual('labelOneText');
  });

  it('will have false radio button with correct label', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[for='name-No']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-No']").element.innerHTML)
      .toEqual('labelTwoText');
  });

  it('will emit true value when true clicked', () => {
    wrapper = mountQuestion();
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm._props.value).toBeUndefined();
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-Yes']");
    expect(input).toBeDefined();

    input.trigger('click');

    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm.__emitted.input[0][0]).toBe('Yes');
  });

  it('will emit false value when false clicked', () => {
    wrapper = mountQuestion();
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm._props.value).toBeUndefined();
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-No']");
    expect(input).toBeDefined();

    input.trigger('click');

    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm.__emitted.input[0][0]).toBe('No');
  });

  each([{
    value: 'Yes',
    isValid: true,
  }, {
    value: 'No',
    isValid: true,
  }, {
    value: undefined,
    isValid: true,
  }, {
    value: 'random',
    isValid: false,
  }, {
    value: '',
    isValid: false,
  }]).it('will validate the value prop to only allow', (data) => {
    wrapper = mountQuestion();
    const { validator } = wrapper.vm.$options.props.value;

    expect(validator && validator(data.value)).toBe(data.isValid);
  });
  it('will call checkAndEmitIsValueValid on create hook', () => {
    const checkAndEmitIsValueValid = jest.fn();
    wrapper = mountQuestion({
      methods: {
        checkAndEmitIsValueValid,
      },
    });
    expect(checkAndEmitIsValueValid).toHaveBeenCalledTimes(1);
  });

  it('will add an undefined option to validValues if not required', () => {
    wrapper = mountQuestion({
      propsData: {
        required: false,
      },
    });

    expect(wrapper.vm.validValues).toEqual(['Yes', 'No', undefined]);
  });

  it('will not add undefined option to validValues if required', () => {
    wrapper = mountQuestion();

    expect(wrapper.vm.validValues).toEqual(['Yes', 'No']);
  });
});
