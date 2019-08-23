<template>
  <component :is="tag" v-if="show">
    <slot />
  </component>
</template>

<script>
import srjIf from '@/lib/sjrIf';

export default {
  name: 'SjrIf',
  props: {
    journey: {
      type: String,
      default: undefined,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    tag: {
      default: 'DIV',
      type: String,
    },
  },
  computed: {
    isEnabled() {
      return srjIf({ $store: this.$store, journey: this.journey });
    },
    show() {
      return !this.disabled === this.$store.getters[`serviceJourneyRules/${this.journey}Enabled`];
    },
  },
};
</script>

