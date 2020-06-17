import DateFilterMapper from '@/store/modules/availableAppointments/dateFilter/Mapper';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import DateProvider from '@/services/DateProvider';
import FilterMutation from '@/store/modules/availableAppointments/mutation/FilterMutation';
import LoadMutation from '@/store/modules/availableAppointments/mutation/LoadMutation';
import {
  ADD_ERROR,
  CLEAR,
  CLEAR_ERROR,
  DESELECT,
  FILTER,
  INIT,
  LOAD,
  SELECT,
  SET_BOOKING_REASON_NECESSITY,
  SET_SELECTED_OPTIONS,
  BOOKING_JOURNEY_COMPLETE,
  BOOKING_JOURNEY_START,
} from './mutation-types';

const clearState = (state) => {
  state.slots = [];
  state.filteredSlots = [];
  state.telephoneNumbers = [];
  state.hasLoaded = false;
  state.filtersOptions = {
    types: [],
    locations: [],
    clinicians: [],
    dates: [],
  };
  state.selectedOptions = {
    type: '',
    location: '',
    clinician: '',
    date: DateFilterValues.ALL,
  };
  state.error = null;
};

export default {
  [ADD_ERROR](state, errorDetails) {
    state.error = errorDetails;
    state.hasLoaded = true;
  },
  [CLEAR](state) {
    clearState(state);
  },
  [CLEAR_ERROR](state) {
    state.error = null;
  },
  [DESELECT](state) {
    state.selectedSlot = null;
  },
  [FILTER](state) {
    const mutation = new FilterMutation(DateProvider, new DateFilterMapper(DateProvider));
    state.filteredSlots = mutation.execute(state.slots, state.selectedOptions);
  },
  [INIT](state) {
    clearState(state);
    state.selectedSlot = null;
    state.booked = false;
  },
  [LOAD](state, d) {
    const data = d || {};
    const mutation = new LoadMutation(DateProvider);
    const result = mutation.execute(data, this.$env.SIXTEEN_WEEKS_SLOTS_ENABLED) || {};

    state.bookingGuidance = data.bookingGuidance;
    state.bookingReasonNecessity = data.bookingReasonNecessity;
    state.hasLoaded = true;
    state.slots = result.slots;
    state.filtersOptions = result.filtersOptions;
    state.selectedOptions.location = result.defaultLocationSelectedOption;
    state.telephoneNumber = data.telephoneNumber;
    state.patientTelephoneNumbers = data.telephoneNumbers;
  },
  [SELECT](state, slot) {
    state.selectedSlot = slot;
  },
  [SET_BOOKING_REASON_NECESSITY](state, value) {
    state.bookingReasonNecessity = value;
  },
  [SET_SELECTED_OPTIONS](state, selectedOptions) {
    state.selectedOptions = selectedOptions;
  },
  [BOOKING_JOURNEY_COMPLETE](state) {
    state.bookingInProgress = false;
  },
  [BOOKING_JOURNEY_START](state) {
    state.bookingInProgress = true;
  },
};
