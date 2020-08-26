import isObject from 'lodash/fp/isObject';
import isString from 'lodash/fp/isString';
import isFunction from 'lodash/fp/isFunction';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  name: 'OnUpdateHeaderMixin',
  beforeMount() {
    EventBus.$on(UPDATE_HEADER, this.onUpdateHeader);
  },
  beforeDestroy() {
    EventBus.$off(UPDATE_HEADER, this.onUpdateHeader);
  },
  methods: {
    onUpdateHeader(newHeader = {}, localised = false, overrideShowContentHeader = false) {
      if (isString(newHeader)) {
        this.setHeaderProp('header', newHeader, localised, overrideShowContentHeader);
        this.caption = undefined;
        this.captionSize = undefined;
        this.overrideShowContentHeader = overrideShowContentHeader;
        return;
      }

      if (!isObject(newHeader)) {
        return;
      }

      const { headerKey: header, captionKey: caption, captionSize } = newHeader;

      this.setHeaderProp('header', header, localised);
      this.setHeaderProp('caption', caption, localised);
      this.captionSize = captionSize;

      this.overrideShowContentHeader = overrideShowContentHeader;
    },
    setHeaderProp(headerProp, value, localised) {
      if (isFunction(value)) {
        this[headerProp] = value(this.$store, this.$i18n);
      } else {
        this[headerProp] = localised ? value : this.$t(value);
      }
    },
  },
};
