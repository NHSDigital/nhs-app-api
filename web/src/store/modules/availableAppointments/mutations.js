/* eslint-disable import/extensions */
import DateProvider from '@/services/DateProvider';
import DateFilterValues from '@/store/modules/availableAppointments/dateFilter/Values';
import DateFilterMapper from '@/store/modules/availableAppointments/dateFilter/Mapper';
import LoadMutation from '@/store/modules/availableAppointments/mutation/LoadMutation';
import FilterMutation from '@/store/modules/availableAppointments/mutation/FilterMutation';
import {
  INIT,
  LOAD,
  SELECT,
  DESELECT,
  CLEAR,
  FILTER,
  SET_BOOKING_REASON_NECESSITY,
  SET_SELECTED_OPTIONS,
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
    date: DateFilterValues.THIS_WEEK,
  };
};

export default {
  [SELECT](state, slot) {
    state.selectedSlot = slot;
  },
  [DESELECT](state) {
    state.selectedSlot = null;
  },
  [CLEAR](state) {
    clearState(state);
  },
  [INIT](state) {
    clearState(state);
    state.selectedSlot = null;
    state.booked = false;
  },
  [SET_SELECTED_OPTIONS](state, selectedOptions) {
    state.selectedOptions = selectedOptions;
  },
  [SET_BOOKING_REASON_NECESSITY](state, value) {
    state.bookingReasonNecessity = value;
  },
  [LOAD](state, d) {
    const data = d || {};
    const mutation = new LoadMutation(DateProvider);
    const result = mutation.execute(data) || {};

    state.bookingGuidance = data.bookingGuidance;
    state.bookingReasonNecessity = data.bookingReasonNecessity;
    state.hasLoaded = true;
    state.slots = result.slots;
    state.filtersOptions = result.filtersOptions;
    state.selectedOptions.location = result.defaultLocationSelectedOption;
    state.telephoneNumber = data.telephoneNumber;
    state.patientTelephoneNumbers = data.telephoneNumbers;
  },
  [FILTER](state) {
    const mutation = new FilterMutation(DateProvider, new DateFilterMapper(DateProvider));
    state.filteredSlots = mutation.execute(state.slots, state.selectedOptions);
  },
};
