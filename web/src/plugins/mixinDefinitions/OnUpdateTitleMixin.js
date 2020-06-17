import isObject from 'lodash/fp/isObject';
import isString from 'lodash/fp/isString';
import isFunction from 'lodash/fp/isFunction';
import { UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  name: 'UpdateTitleMixin',
  beforeMount() {
    EventBus.$on(UPDATE_TITLE, this.onUpdateTitle);
  },
  beforeDestroy() {
    EventBus.$off(UPDATE_TITLE, this.onUpdateTitle);
  },
  mounted() {
    this.onUpdateTitle(this.$route.meta);
  },
  methods: {
    onUpdateTitle(newTitle = {}, localised = false) {
      if (isString(newTitle)) {
        this.setTitle(newTitle, localised);
        return;
      }

      if (!isObject(newTitle)) {
        return;
      }

      this.setTitle(newTitle.titleKey, localised);
    },
    setTitle(title, localised) {
      if (isFunction(title)) {
        this.title = title(this.$store, this.$i18n);
      } else {
        this.title = localised ? title : this.$t(title);
      }
    },
  },
};
