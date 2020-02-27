<template>
  <component :is="tag" v-if="show">
    <slot />
  </component>
</template>

<script>
import sjrIf from '@/lib/sjrIf';

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
    // eslint-disable-next-line vue/require-prop-types
    context: {
      default: null,
    },
  },
  computed: {
    show() {
      return sjrIf({ $store: this.$store,
        journey: this.journey,
        disabled: this.disabled,
        context: this.context });
    },
  },
};
</script>
