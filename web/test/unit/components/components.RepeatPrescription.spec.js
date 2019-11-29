import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import RepeatPrescription from '@/components/RepeatPrescription';
import { createStore, mount } from '../helpers';

const repeatPrescriptionCourses = [
  { id: 'repeat-course-id-1' },
  { id: 'repeat-course-id-2' },
  { id: 'repeat-course-id-3' },
];

const $store = (hasLoaded, submitted, selectedCoursesNoJs) =>
  createStore({
    state: {
      repeatPrescriptionCourses: {
        hasLoaded,
        submitted,
        repeatPrescriptionCourses,
        selectedCoursesNoJs,
      },
    },
  });

const mountComponent = ({ methods = {} } = {}) =>
  mount(RepeatPrescription, {
    $store: $store(),
    propsData: {
      value: repeatPrescriptionCourses,
    },
    methods,
  });

describe('Repeat Prescription', () => {
  let wrapper;

  beforeEach(() => {
    wrapper = mountComponent();
  });

  it('will display all checkboxes', () => {
    // Assert
    expect(wrapper.findAll(GenericCheckbox).length).toBe(3);
  });

  it('will verify an associated label for the repeat prescription checkbox', () => {
    // Assert
    expect(wrapper.find(`input[type='checkbox'][id='${repeatPrescriptionCourses[0].id}']`)
      .exists()).toEqual(true);
    expect(wrapper.find(`label[for='${repeatPrescriptionCourses[0].id}']`)
      .exists()).toEqual(true);
  });

  describe('checkbox selected', () => {
    it('should emit when `selectedValueChanged` is called', () => {
      // Act
      wrapper.find('input[type="checkbox"]').trigger('click');

      // Assert
      expect(wrapper.emitted('input')).toBeDefined();
    });

    it('should call on `selectedValueChanged` on select event', () => {
      // Arrange
      const selectedValueChanged = jest.fn();
      wrapper = mountComponent({ methods: { selectedValueChanged } });

      // Act
      wrapper.find('input[type="checkbox"]').trigger('click');

      // Assert
      expect(selectedValueChanged).toHaveBeenCalledTimes(1);
    });
  });
});
