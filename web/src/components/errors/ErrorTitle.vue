<template>
  <div>
    <slot v-if="header"/>
  </div>
</template>

<script>
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

export default {
  name: 'ErrorTitle',
  props: {
    title: {
      type: String,
      required: true,
    },
    header: {
      type: String,
      default: '',
    },
  },
  beforeMount() {
    EventBus.$emit(UPDATE_HEADER, this.header || this.title);
    EventBus.$emit(UPDATE_TITLE, this.title);
  },
};
</script>
