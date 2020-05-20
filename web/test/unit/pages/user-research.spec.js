import UserResearch from '@/pages/user-research';
import { mount } from '../helpers';

const mountUserResearch = ({ methods }) => mount(UserResearch, {
  $style: {
    error: 'error',
  },
  methods,
  state: {
    device: {
      isNativeApp: false,
    },
  },
});

describe('user research', () => {
  let wrapper;
  let conditionalRedirect;

  beforeEach(() => {
    conditionalRedirect = jest.fn();
    wrapper = mountUserResearch({
      methods: {
        conditionalRedirect,
      },
    });
  });

  describe('continue button', () => {
    let button;

    beforeEach(() => {
      button = wrapper.find('button');
    });

    it('will exist', () => {
      expect(button.exists()).toBe(true);
    });

    describe('click', () => {
      const clickButton = () => button.trigger('click');

      describe('when selection not made', () => {
        beforeEach(() => {
          clickButton();
        });

        it('will show error dialog', () => {
          expect(wrapper.find('.error').exists()).toBe(true);
        });

        it('will not call conditional redirect', () => {
          expect(conditionalRedirect).not.toBeCalled();
        });

        it('will show inline errors', () => {
          expect(wrapper.find('.error-message').exists()).toBe(true);
        });
      });

      describe.each([
        ['Yes', true],
        ['No', false],
      ])('when `%s` radio button is selected', (_, value) => {
        let radioButton;

        beforeEach(() => {
          radioButton = wrapper.find(`#radioButton-${value}`);
          radioButton.trigger('click');
          clickButton();
        });

        it('will not show error dialog', () => {
          expect(wrapper.find('.error').exists()).toBe(false);
        });

        it('will not show inline errors', () => {
          expect(wrapper.find('.error-message').exists()).toBe(false);
        });

        it('will call conditional redirect', () => {
          expect(conditionalRedirect).toBeCalled();
        });
      });
    });
  });
});
