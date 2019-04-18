<script>
import get from 'lodash/fp/get';
import Sources from '@/lib/sources';
import { INDEX } from '@/lib/routes';

export const isNativeApp = ({ route = {}, store = {} }) =>
  get('state.device.isNativeApp')(store) || Sources.isNative(get('query.source')(route));

export default {
  fetch({ redirect, route, store }) {
    if (!isNativeApp({ route, store })) {
      redirect(INDEX.path);
    }
  },
};
</script>
