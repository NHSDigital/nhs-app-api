import get from 'lodash/fp/get';
import { isTruthy } from '@/lib/utils';

export default {
  isLoading(state) {
    return isTruthy((get('loadingUrls')(state)).length);
  },
};
