import LabelledToggle from '@/components/widgets/LabelledToggle';
import Toggle from '@/components/widgets/Toggle';
import { mount } from '../../helpers';

describe('LabelledToggle', () => {
  const label = 'Test label text';
  const mountLabelledToggle = ({ checkboxId, isWaiting = false } = {}) => mount(LabelledToggle, {
    propsData: {
      checkboxId,
      isWaiting,
      label,
    },
  });

  let wrapper;

  beforeEach(() => {
    wrapper = mountLabelledToggle();
  });

  it('will have Toggle component', () => {
    expect(wrapper.find(Toggle).exists()).toBe(true);
  });

  describe('props', () => {
    let emit;

    it('will have a default checkbox ID value', () => {
      expect(wrapper.find('input').attributes('id')).toBe('default-id');
    });

    it('will set label text', () => {
      expect(wrapper.find('label').text()).toBe(label);
    });

    describe('checkbox ID', () => {
      const checkboxId = 'test-id';

      beforeEach(() => {
        wrapper = mountLabelledToggle({ checkboxId });
      });

      it('will set the text label `for` attribute', () => {
        expect(wrapper.find('label').attributes('for')).toBe(checkboxId);
      });

      describe('Toggle component', () => {
        let toggle;

        beforeEach(() => {
          toggle = wrapper.find(Toggle);
        });

        it('will set the checkbox ID property', () => {
          expect(toggle.props('checkboxId')).toBe(checkboxId);
        });
      });
    });

    describe('is waiting', () => {
      beforeEach(() => {
        wrapper = mountLabelledToggle({ isWaiting: true });
        emit = jest.spyOn(wrapper.vm, '$emit');
      });

      it('will not emit `input` upon label click', () => {
        wrapper.find('label').trigger('click');
        expect(emit).not.toBeCalled();
      });
    });

    describe('is not waiting', () => {
      beforeEach(() => {
        wrapper = mountLabelledToggle({ isWaiting: false });
        emit = jest.spyOn(wrapper.vm, '$emit');
      });

      it('will emit `input` upon label click', () => {
        wrapper.find('label').trigger('click');
        expect(emit).toBeCalledWith('input', true);
      });
    });
  });
});
