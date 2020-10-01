import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import LegendSize from '@/lib/legend-size';
import each from 'jest-each';
import { createStore, mount } from '../../helpers';

let wrapper;

const mountComponent = ({
  methods = undefined,
  value = undefined,
  items = [],
  heading = '',
  legendSize = undefined,
  error = false,
  errorText = '',
  name = '',
} = {}) => {
  wrapper = mount(NhsUkRadioGroup, {
    methods,
    $store: createStore({
      state: {
        device: { isNativeApp: false },
      },
    }),
    propsData: {
      value,
      items,
      heading,
      legendSize,
      error,
      errorText,
      name,
    },
  });
};

describe('nhsuk radio group', () => {
  describe('created', () => {
    it('will check and emit is value valid in created', () => {
      const checkAndEmitIsValueValid = jest.fn();

      mountComponent({ methods: { checkAndEmitIsValueValid }, value: 'no' });

      expect(checkAndEmitIsValueValid).toHaveBeenCalledWith('no');
    });
  });

  describe('watch', () => {
    describe('choice', () => {
      it('will check and emit whether the new value is valid', () => {
        const checkAndEmitIsValueValid = jest.fn();
        mountComponent({ methods: { checkAndEmitIsValueValid }, value: 'no' });
        checkAndEmitIsValueValid.mockClear();

        wrapper.setData({ choice: 'yes' });

        expect(checkAndEmitIsValueValid).toHaveBeenCalledWith('yes');
      });

      it('will emit the new value in an input event', () => {
        mountComponent({ value: 'no' });

        wrapper.setData({ choice: 'yes' });

        expect(wrapper.emitted().input[0][0]).toEqual('yes');
      });
    });
  });

  describe('checkAndEmitIsValueValid', () => {
    it('will emit isValid true in a validate event when value is defined and exists in items', () => {
      mountComponent({
        items: [{ value: 'yes', label: 'Yes' }],
        value: 'yes',
      });

      expect(wrapper.emitted().validate[0][0].isValid).toBe(true);
    });

    it('will emit isValid true in a validate event when value is undefined and required', () => {
      mountComponent();

      expect(wrapper.emitted().validate[0][0].isValid).toBe(false);
    });

    it('will emit isValid false in a validate event when value is defined and does not exist in items', () => {
      mountComponent({
        items: [{ value: 'yes', label: 'Yes' }],
        value: 'no',
      });

      expect(wrapper.emitted().validate[0][0].isValid).toBe(false);
    });
  });

  describe('legend', () => {
    it('will have a legend heading', () => {
      mountComponent({
        heading: 'Is this a legend?',
        legendSize: LegendSize.ExtraSmall,
      });

      const header = wrapper.find(`legend.${LegendSize.ExtraSmall} h1.nhsuk-fieldset__heading`);

      expect(header.element.innerHTML).toEqual('Is this a legend?');
    });

    each([
      [LegendSize.ExtraLarge, true],
      [LegendSize.Large, true],
      [LegendSize.Medium, true],
      [LegendSize.Small, true],
      [LegendSize.ExtraSmall, true],
      ['', false],
      [undefined, false],
    ]).it('will validate the legend size prop is one of nhs uk styles', (size, valid) => {
      expect(NhsUkRadioGroup.props.legendSize.validator(size)).toEqual(valid);
    });
  });

  describe('error messages', () => {
    beforeEach(() => {
      mountComponent({
        error: true,
        errorText: 'Select one or more options',
        name: 'urgency',
      });
    });

    it('will have a message dialog showing an error summary when there is an error', () => {
      const errorSummary = wrapper.find('[data-purpose="error-container"] [data-purpose="error"]');
      const validationError = errorSummary.find('ul li');

      expect(errorSummary.text()).toContain('There\'s a problem');
      expect(validationError.text()).toEqual('Select one or more options');
    });

    it('will have an accessible inline error message', () => {
      const inlineError = wrapper.find('.nhsuk-form-group--error .nhsuk-error-message');
      const accessiblePrefix = inlineError.find('.nhsuk-u-visually-hidden');

      expect(accessiblePrefix.text()).toEqual('Error:');
      expect(inlineError.text()).toContain('Select one or more options');
    });
  });

  describe('radios', () => {
    it('will render each item as a radio button with an associated label', () => {
      const items = [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }, { value: 'maybe' }];
      mountComponent({
        items,
        name: 'urgency',
      });

      items.forEach((i) => {
        const radio = wrapper.find(`#urgency-${i.value}[type=radio][name=urgency][value=${i.value}]`);
        const label = wrapper.find(`label[for=urgency-${i.value}]`);

        expect(radio.exists()).toBe(true);
        expect(label.text()).toEqual(i.label || i.value);
      });
    });
  });
});
