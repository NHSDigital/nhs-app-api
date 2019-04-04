import { mount } from '../../helpers';
import questionBoolean from '@/components/online-consultations/QuestionBoolean';

describe('question text', () => {
  let wrapper;

  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(questionBoolean, {
        propsData,
      });
    wrapper = mountConfirmation({
      propsData: {
        name: 'name',
        questionId: 'qid',
        text: 'This is a <strong>sample question</strong>?',
      },
    });
  });

  it('will have a span element for the question', () => {
    expect(wrapper.find("[id='qid']")
      .exists())
      .toEqual(true);
  });

  it('should render the question property value as html', () => {
    expect(wrapper.find("[id='qid']").element.innerHTML)
      .toEqual('This is a <strong>sample question</strong>?');
  });
});

describe('radio buttons', () => {
  let wrapper;

  beforeEach(() => {
    const mountConfirmation = ({ propsData } = {}) =>
      mount(questionBoolean, {
        propsData,
      });

    wrapper = mountConfirmation({
      propsData: {
        value: '',
        trueId: 'tid',
        falseId: 'fid',
        optionOneLabel: 'labelOneText',
        optionTwoLabel: 'labelTwoText',
        name: 'name',
      },
      data: {
        selectedValue: '',
      },
    });
  });
  it('will have radio buttons for the question', () => {
    expect(wrapper.find("[id='name-Yes']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[id='name-No']")
      .exists())
      .toEqual(true);
  });

  it('will have true radio button with correct label', () => {
    expect(wrapper.find("[for='name-Yes']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-Yes']").element.innerHTML)
      .toEqual('labelOneText');
  });

  it('will have false radio button with correct label', () => {
    expect(wrapper.find("[for='name-No']")
      .exists())
      .toEqual(true);

    expect(wrapper.find("[for='name-No']").element.innerHTML)
      .toEqual('labelTwoText');
  });

  it('will emit true value when true clicked', () => {
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm._props.value).toBe('');
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-Yes']");
    expect(input).toBeDefined();

    input.trigger('click');

    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm.__emitted.input[0][0]).toBe('Yes');
  });

  it('will emit false value when false clicked', () => {
    expect(wrapper.vm).toBeDefined();
    expect(wrapper.vm.selected).toBeDefined();
    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm._props.value).toBe('');
    expect(wrapper.emitted('select')).not.toBeDefined();

    const input = wrapper.find("[id='name-No']");
    expect(input).toBeDefined();

    input.trigger('click');

    /* eslint-disable no-underscore-dangle */
    expect(wrapper.vm.__emitted.input[0][0]).toBe('No');
  });
});
