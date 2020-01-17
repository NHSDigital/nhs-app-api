import Toggle from '@/components/widgets/Toggle';
import { mount } from '../../helpers';

describe('Toggle', () => {
  const $style = {
    waiting: 'waiting',
  };
  let wrapper;

  const mountToggle = ({ checkboxId, isWaiting } = {}) => mount(Toggle, {
    $style,
    propsData: {
      checkboxId,
      isWaiting,
    },
  });

  beforeEach(() => {
    wrapper = mountToggle();
  });

  describe('props', () => {
    let emit;

    it('will have a default checkbox ID value', () => {
      expect(wrapper.find('input').attributes('id')).toBe('default-id');
    });

    describe('checkbox ID', () => {
      const checkboxId = 'test-id';
      let checkbox;

      beforeEach(() => {
        wrapper = mountToggle({ checkboxId });
        checkbox = wrapper.find('input');
      });

      it('will set the checkbox ID', () => {
        expect(checkbox.attributes('id')).toBe(checkboxId);
      });

      it('will set the span `id` attribute', () => {
        expect(wrapper.find('span').attributes('id')).toBe(`span${checkboxId}`);
      });
    });

    describe('is waiting', () => {
      beforeEach(() => {
        wrapper = mountToggle({ isWaiting: true });
        emit = jest.spyOn(wrapper.vm, '$emit');
      });

      it('will have a `waiting` css class', () => {
        expect(wrapper.classes()).toContain($style.waiting);
      });

      it('will show spinner', () => {
        expect(wrapper.find('svg').isVisible()).toBe(true);
      });

      it('will hide toggle', () => {
        expect(wrapper.find('span').isVisible()).toBe(false);
      });

      describe('span', () => {
        let span;

        beforeEach(() => {
          span = wrapper.find('span');
        });

        it('will not emit `input` when clicked', () => {
          span.trigger('click');
          expect(emit).not.toBeCalled();
        });
      });
    });

    describe('is not waiting', () => {
      beforeEach(() => {
        wrapper = mountToggle({ isWaiting: false });
        emit = jest.spyOn(wrapper.vm, '$emit');
      });

      it('will not have a `waiting` css class', () => {
        expect(wrapper.classes()).not.toContain($style.waiting);
      });

      it('will hide spinner', () => {
        expect(wrapper.find('svg').isVisible()).toBe(false);
      });

      it('will show toggle', () => {
        expect(wrapper.find('span').isVisible()).toBe(true);
      });

      describe('span', () => {
        let span;

        beforeEach(() => {
          span = wrapper.find('span');
        });

        it('will emit `input` when clicked', () => {
          span.trigger('click');
          expect(emit).toBeCalledWith('input', true);
        });
      });
    });
  });
});
