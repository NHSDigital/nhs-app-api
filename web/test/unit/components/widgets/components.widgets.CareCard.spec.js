import each from 'jest-each';
import CareCard from '@/components/widgets/CareCard';

import { mount } from '../../helpers';

let wrapper;

const mountComponent = ({ slot = '', urgency = undefined } = {}) => {
  wrapper = mount(CareCard, {
    slots: { default: slot },
    propsData: {
      heading: 'Test Care Card Heading',
      urgency,
    },
  });
};

describe('Care card', () => {
  describe('slot', () => {
    it('will have a slot for rendering care card content', () => {
      mountComponent({ slot: '<p id="careCardContent">content</p>' });

      const content = wrapper.find('p#careCardContent');

      expect(content.exists()).toBe(true);
      expect(content.text()).toEqual('content');
    });
  });

  describe('urgency prop', () => {
    describe('validator', () => {
      let validator;

      beforeAll(() => {
        mountComponent();
        ({ validator } = wrapper.vm.$options.props.urgency);
      });

      each(['urgent', 'nonUrgent', 'immediate'])
        .it('will allow predefined styles', (urgency) => {
          const valid = validator(urgency);
          expect(valid).toBe(true);
        });

      it('will allow predefined styles', () => {
        const valid = validator('invalidUrgency');
        expect(valid).toBe(false);
      });
    });
  });

  describe('care card style', () => {
    each([
      { urgency: 'urgent', style: 'nhsuk-care-card--urgent' },
      { urgency: 'nonUrgent', style: 'nhsuk-care-card--non-urgent' },
      { urgency: 'immediate', style: 'nhsuk-care-card--immediate' },
    ]).it('will match urgency prop', ({ urgency, style }) => {
      mountComponent({ urgency });
      const careCard = wrapper.find(`.nhsuk-care-card.${style}`);
      expect(careCard.exists()).toBe(true);
    });

    it('will default if urgency not provided', () => {
      mountComponent();
      const careCard = wrapper.find('.nhsuk-care-card.nhsuk-care-card--non-urgent');
      expect(careCard.exists()).toBe(true);
    });
  });

  describe('heading', () => {
    it('will be styled like a heading level 3', () => {
      mountComponent();
      const heading = wrapper.find('.nhsuk-care-card__heading.nhsuk-heading-m');
      expect(heading.exists()).toBe(true);
    });
    each(['urgent', 'nonUrgent', 'immediate'])
      .it('will contain an appropriate prefix for accessibility', (urgency) => {
        mountComponent({ urgency });
        const prefixSpan = wrapper
          .find('.nhsuk-care-card__heading.nhsuk-heading-m')
          .element.firstChild.firstChild;
        expect(prefixSpan.innerHTML).toEqual(`translate_careCard.headingPrefix.${urgency}`);
      });
  });
});
