import each from 'jest-each';
import i18n from '@/plugins/i18n';
import QuestionBoolean from '@/components/online-consultations/QuestionBoolean';
import { mount } from '../../helpers';

describe('radio buttons', () => {
  let wrapper;
  const mountQuestion = ({ propsData = {}, methods = {} } = {}) =>
    mount(QuestionBoolean, {
      propsData: {
        value: undefined,
        trueId: 'tid',
        falseId: 'fid',
        name: 'name',
        ...propsData,
      },
      methods,
      mountOpts: {
        i18n,
      },
    });

  it('will have radio buttons for the question', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[id='name-true']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[id='name-false']")
      .exists())
      .toEqual(true);
  });

  it('will have true radio button with correct label', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[for='name-true']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-true']").element.innerHTML).toEqual('Yes');
  });

  it('will have false radio button with correct label', () => {
    wrapper = mountQuestion();
    expect(wrapper.find("[for='name-false']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-false']").element.innerHTML).toEqual('No');
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
    expect(inputAttributes['aria-describedby']).toBe('nameerror optional-label');
  });

  it('will emit true value when true clicked', () => {
    wrapper = mountQuestion();
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    expect(wrapper.vm.$props.value).toBeUndefined();
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-true']");
    expect(input).toBeDefined();

    input.trigger('click');

    expect(wrapper.emitted('input')[0][0]).toBe('true');
  });

  it('will emit false value when false clicked', () => {
    wrapper = mountQuestion();
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    expect(wrapper.vm.$props.value).toBeUndefined();
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-false']");
    expect(input).toBeDefined();

    input.trigger('click');

    expect(wrapper.emitted('input')[0][0]).toBe('false');
  });

  each([{
    value: 'true',
    isValid: true,
  }, {
    value: 'false',
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
});
