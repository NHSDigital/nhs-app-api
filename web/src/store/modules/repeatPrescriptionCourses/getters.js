export default {
  selectedPrescriptions(state) {
    const selectedCourses = [];

    if (state.repeatPrescriptionCourses) {
      state.repeatPrescriptionCourses.forEach((course) => {
        if (course.selected) {
          selectedCourses.push(course);
        }
      });
    }

    return selectedCourses;
  },
  isValid(state, getters) {
    return getters.selectedPrescriptions.length > 0;
  },
  specialRequestValid(state) {
    if (state.specialRequestNecessity === 'Mandatory') {
      if (!state.specialRequest || state.specialRequest.trim() === '') {
        return false;
      }
    }
    return true;
  },
  selectedIds(state, getters) {
    return getters.selectedPrescriptions.map(item => item.id);
  },
};
