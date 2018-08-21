import actions from './actions';
import mutations from './mutations';

export default {
  namespaced: true,
  state() {
    return {
      slots: new Map(),
      bookingGuidance: '',
      filteredSlots: [],
      hasLoaded: false,
      selectedSlot: null,
      booked: false,
      filtersOptions: {
        types: [],
        locations: [],
        clinicians: [],
        dates: [],
      },
      selectedOptions: {
        type: '',
        location: '',
        clinician: '',
        date: 'this_week',
      },
    };
  },
  actions,
  mutations,
};
