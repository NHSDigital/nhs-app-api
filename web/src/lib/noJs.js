import isArray from 'lodash/fp/isArray';
import isString from 'lodash/fp/isString';
import { initialState as repeatCoursesInitialState } from '../store/modules/repeatPrescriptionCourses/mutation-types';
import { PRESCRIPTION_REPEAT_COURSES, INDEX } from './routes';

export const noJsParameterName = 'nojs';

export const createUri = ({ path, noJs }) => {
  const noJsJson = JSON.stringify(noJs);
  return `${path}?${noJsParameterName}=${encodeURIComponent(noJsJson)}`;
};

// noJsState handlers

const getDefaultResult = () => ({
  shouldRedirect: true,
  redirectPath: INDEX.path,
  redirectQuery: {},
  state: undefined,
});

const getSelectedPrescriptionInfoFromData = (prescriptionId, data) => ({
  id: prescriptionId,
  selected: true,
  ...JSON.parse(data[prescriptionId]),
});

export const parseSelectedRepeatCourses = ({ data }) => {
  const result = getDefaultResult();
  result.redirectPath = PRESCRIPTION_REPEAT_COURSES.path;

  if (data === undefined || Object.keys(data).length === 0) {
    return result;
  }

  const state = repeatCoursesInitialState();
  const repeatPrescriptionCourses = [];

  if (isArray(data.prescription)) {
    data.prescription.forEach((prescriptionId) => {
      repeatPrescriptionCourses.push(getSelectedPrescriptionInfoFromData(prescriptionId, data));
    });
  } else if (isString(data.prescription)) {
    repeatPrescriptionCourses.push(getSelectedPrescriptionInfoFromData(data.prescription, data));
  }

  if (data.specialRequestNecessity) {
    state.specialRequestNecessity = data.specialRequestNecessity;
  }

  if (data.specialRequest) {
    state.specialRequest = data.specialRequest.trim();
  }

  if (repeatPrescriptionCourses === [] || repeatPrescriptionCourses.length === 0) {
    result.redirectQuery.noneSelected = 1;
    return result;
  }

  if (state.specialRequestNecessity === 'Mandatory' && !state.specialRequest) {
    result.redirectQuery.missingSpecialRequest = 1;
    return result;
  }

  result.shouldRedirect = false;
  state.repeatPrescriptionCourses = repeatPrescriptionCourses;

  result.state = {
    repeatPrescriptionCourses: state,
  };

  return result;
};
