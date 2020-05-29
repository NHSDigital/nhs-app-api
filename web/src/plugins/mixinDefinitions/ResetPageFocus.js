import NativeCallbacks from '@/services/native-app';
import get from 'lodash/fp/get';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

const isComponentATopLevelPage = component =>
  get('$vnode.data.key')(component) === component.$route.path;

export default {
  name: 'ResetPageFocusMixin',
  mounted() {
    if (isComponentATopLevelPage(this)) {
      if (this.$store.state.device.isNativeApp) {
        NativeCallbacks.resetPageFocus();
      }

      EventBus.$emit(FOCUS_NHSAPP_ROOT);
    }
  },
};
