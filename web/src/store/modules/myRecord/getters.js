import get from 'lodash/fp/get';
import { isTruthy } from '@/lib/utils';

export default {
  patientDetailsExist(state) {
    return isTruthy(
      Object.keys(
        get('patientDetails')(state),
      ).length,
    );
  },
};
