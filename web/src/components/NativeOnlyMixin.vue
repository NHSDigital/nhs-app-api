<script>
import get from 'lodash/fp/get';
import { INDEX } from '@/lib/routes';

export const isNativeApp = ({ store = {} }) =>
  get('state.device.isNativeApp')(store);

export default {
  fetch({ redirect, store }) {
    if (!isNativeApp({ store })) {
      redirect(INDEX.path);
    }
  },
};
</script>
