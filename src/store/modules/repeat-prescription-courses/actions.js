import Vue from 'vue';
import {
  REPEAT_PRESCRIPTION_COURSES_LOADED,
} from './mutation-types';

export const load = ({ commit }) =>
  Vue
    .$http
    .getV1PatientCourses()
    .then((data) => {
      commit(REPEAT_PRESCRIPTION_COURSES_LOADED, data);
    });

export default {
  load,
};
