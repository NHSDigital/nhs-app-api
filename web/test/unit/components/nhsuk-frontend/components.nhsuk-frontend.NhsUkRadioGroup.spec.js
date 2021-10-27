import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import LegendSize from '@/lib/legend-size';
import each from 'jest-each';
import { createStore, mount } from '../../helpers';

let wrapper;

const mountComponent = ({
  methods = undefined,
  currentValue = undefined,
  items = [],
  heading = 'test',
  noHeadingRequired = false,
  legendSize = undefined,
  error = false,
  errorText = '',
  name = '',
  renderAsHtml = false,
  enableErrorDialog = false,
} = {}) => {
  wrapper = mount(NhsUkRadioGroup, {
    methods,
    $store: createStore({
      state: {
        device: { isNativeApp: false },
      },
    }),
    propsData: {
      currentValue,
      items,
      heading,
      noHeadingRequired,
      legendSize,
      error,
      errorText,
      name,
      renderAsHtml,
      enableErrorDialog,
    },
  });
};

describe('nhsuk radio group', () => {
  describe('created', () => {
    it('will check and emit is value valid in created', () => {
      const checkAndEmitIsValueValid = jest.fn();

      mountComponent({ methods: { checkAndEmitIsValueValid }, currentValue: 'no' });

      expect(checkAndEmitIsValueValid).toHaveBeenCalledWith('no');
    });
  });

  describe('watch', () => {
    describe('choice', () => {
      it('will check and emit whether the new value is valid', () => {
        const checkAndEmitIsValueValid = jest.fn();
        mountComponent({ methods: { checkAndEmitIsValueValid }, currentValue: 'no' });
        checkAndEmitIsValueValid.mockClear();

        wrapper.setData({ choice: 'yes' });

        expect(checkAndEmitIsValueValid).toHaveBeenCalledWith('yes');
      });

      it('will emit the new value in an input event', () => {
        mountComponent({ currentValue: 'no' });

        wrapper.setData({ choice: 'yes' });

        expect(wrapper.emitted().input[0][0]).toEqual('yes');
      });
    });
  });

  describe('checkAndEmitIsValueValid', () => {
    it('will emit isValid true in a validate event when value is defined and exists in items', () => {
      mountComponent({
        items: [{ value: 'yes', label: 'Yes' }],
        currentValue: 'yes',
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
        currentValue: 'no',
      });

      expect(wrapper.emitted().validate[0][0].isValid).toBe(false);
    });
  });

  describe('legend', () => {
    it('will have a legend heading', () => {
      mountComponent({
        name: 'name',
        heading: 'Is this a legend?',
        legendSize: LegendSize.ExtraSmall,
      });

      const heading = wrapper.find('#name-legend');
      expect(heading.element.innerHTML).toEqual('Is this a legend?');
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
    describe('enableErrorDialog is true', () => {
      it('error summary is shown', () => {
        mountComponent({
          error: true,
          errorText: 'Select one or more options',
          name: 'urgency',
          enableErrorDialog: true,
        });

        const errorSummary = wrapper
          .find('[data-purpose="error-container"] [data-purpose="error"]');

        const validationError = errorSummary.find('ul li');

        expect(errorSummary.text()).toContain('There\'s a problem');
        expect(validationError.text()).toEqual('Select one or more options');
      });
    });

    it('will have an accessible inline error message', () => {
      mountComponent({
        error: true,
        errorText: 'Select one or more options',
        name: 'urgency',
      });

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

  describe('hints', () => {
    it('will display a hint if one is provided', () => {
      const items = [
        { value: 'yes', label: 'Yes', hint: { text: 'This is hint 1' } },
        { value: 'no', label: 'No', hint: { text: 'This is hint 2' } },
      ];
      mountComponent({
        items,
        name: 'urgency',
      });

      items.forEach((i) => {
        const hint = wrapper.find(`#urgency-${i.value}-hint`);

        expect(hint.text()).toEqual(i.hint.text);
      });
    });

    it('will not render the hint if no hint is provided', () => {
      const items = [{ value: 'yes', label: 'Yes' }, { value: 'no', label: 'No' }];
      mountComponent({
        items,
        name: 'urgency',
      });

      items.forEach((i) => {
        expect(wrapper.find(`#urgency-${i.value}-hint`).exists()).toBeFalsy();
      });
    });

    it('will allow the selective rendering of hints', () => {
      const items = [{ value: 'yes', label: 'Yes', hint: { text: 'This is hint 1' } }, { value: 'no', label: 'No' }];
      mountComponent({
        items,
        name: 'urgency',
      });

      items.forEach((i) => {
        if (i.hint) {
          const hint = wrapper.find(`#urgency-${i.value}-hint`);

          expect(hint.text()).toEqual(i.hint.text);
        } else {
          expect(wrapper.find(`#urgency-${i.value}-hint`).exists()).toBeFalsy();
        }
      });
    });
  });

  describe('noheadingRequired', () => {
    describe('true', () => {
      describe('heading is undefined', () => {
        mountComponent({
          noHeadingRequired: true,
          heading: undefined,
          name: 'name',
        });

        it('isnt rendered', () => {
          expect(wrapper.find('#name-legend').exists()).toBe(false);
        });
      });
    });
    describe('false', () => {
      const noHeadingRequired = false;

      it('validator will fail when no heading', () => {
        expect(NhsUkRadioGroup.props.heading.validator(undefined, noHeadingRequired))
          .toBe(false);
      });

      it('validator will fail when heading empty', () => {
        expect(NhsUkRadioGroup.props.heading.validator('', noHeadingRequired))
          .toBe(false);
      });
    });
  });

  describe('renderAsHtml', () => {
    const items = [
      {
        value: 'yes',
        label: '<label id="testLabel">Yes</label>',
      },
      { value: 'no',
        label: 'No',
      },
    ];

    describe('true', () => {
      it('will render html', () => {
        mountComponent({
          renderAsHtml: true,
          items,
          name: 'name',
        });

        const label = wrapper.find('#testLabel');

        expect(label.exists()).toBe(true);
      });
    });

    describe('false', () => {
      it('will render plain text', () => {
        mountComponent({
          renderAsHtml: false,
          items,
          name: 'name',
        });

        const label = wrapper.find("[for='name-yes']");

        expect(label.exists()).toBe(true);
        expect(label.element.innerHTML)
          .toEqual('&lt;label id="testLabel"&gt;Yes&lt;/label&gt;');
      });
    });
  });
});
