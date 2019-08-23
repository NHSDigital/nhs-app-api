<template>
  <component :is="tag" v-if="show">
    <slot />
  </component>
</template>

<script>
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
    show() {
      return !this.disabled === this.$store.getters[`serviceJourneyRules/${this.journey}Enabled`];
    },
  },
};
</script>

