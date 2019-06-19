import NativeCallbacks from '@/services/native-app';
import get from 'lodash/fp/get';

const isComponentATopLevelPage = component =>
  get('$vnode.data.key')(component) === component.$route.path;

export default {
  name: 'ResetPageFocusMixin',
  mounted() {
    if (process.client
      && this.$store.state.device.isNativeApp
      && isComponentATopLevelPage(this)) {
      NativeCallbacks.resetPageFocus();
    }
  },
};
