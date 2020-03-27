import each from 'jest-each';
import PublicHealthNotification from '@/components/widgets/PublicHealthNotification';
import { mount } from '../../helpers';

let wrapper;

const mountPublicHealthNotification = ({
  title = 'Test title',
  body = 'Test body',
  type = 'callout',
  urgency = 'warning',
} = {}) => (
  mount(PublicHealthNotification, {
    propsData: {
      title, body, type, urgency,
    },
  })
);

describe('public health notification', () => {
  it('will require type, urgency, title, and body properties', () => {
    wrapper = mountPublicHealthNotification();

    ['type', 'urgency', 'title', 'body'].forEach((property) => {
      expect(wrapper.vm.$options.props[property].required).toBe(true);
    });
  });

  each([
    { type: 'callout', valid: true },
    { type: 'unknownType', valid: false },
  ]).it('type property will have validator to only allow known types', ({ type, valid }) => {
    wrapper = mountPublicHealthNotification();

    expect(wrapper.vm.$options.props.type.validator(type)).toBe(valid);
  });

  each([
    { urgency: 'warning', valid: true },
    { urgency: 'unknownUrgency', valid: false },
  ]).it('type property will have validator to only allow known urgencies', ({ urgency, valid }) => {
    wrapper = mountPublicHealthNotification();

    expect(wrapper.vm.$options.props.urgency.validator(urgency)).toBe(valid);
  });
});
