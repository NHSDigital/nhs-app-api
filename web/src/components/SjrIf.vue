<template>
  <component :is="tag" v-if="isEnabled">
    <slot />
  </component>
</template>

<script>
export default {
  name: 'SjrIf',
  props: {
    journey: {
      type: String,
      required: true,
    },
    tag: {
      default: 'DIV',
      type: String,
    },
  },
  computed: {
    isEnabled() {
      return this.$store.getters[`serviceJourneyRules/${this.journey}Enabled`];
    },
  },
};
</script>

