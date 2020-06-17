<script>
import get from 'lodash/fp/get';
import { INDEX_PATH } from '@/router/paths';

export const isNativeApp = ({ store = {} }) =>
  get('state.device.isNativeApp')(store);

export default {
  fetch({ redirect, store }) {
    if (!isNativeApp({ store })) {
      redirect(INDEX_PATH);
    }
  },
};
</script>
