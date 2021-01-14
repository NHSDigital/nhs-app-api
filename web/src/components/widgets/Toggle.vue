<template>
  <div :class="{ [$style.toggleWrapper]: true, [$style.waiting]: isWaiting }">
    <toggle-spinner :id="`${checkboxId}_spinner`" v-visible="isWaiting" :class="$style.spinner" />
    <input :id="checkboxId"
           :class="$style.toggle"
           type="checkbox"
           :name="name"
           :checked="value"
           role="switch"
           @click.stop.prevent="onClick">
    <span :id="`span${checkboxId}`" v-visible="!isWaiting"
          @click.stop.prevent="onClick"/>
  </div>
</template>

<script>
import ToggleSpinner from '@/components/icons/ToggleSpinner';

export default {
  name: 'Toggle',
  components: {
    ToggleSpinner,
  },
  props: {
    name: {
      type: String,
      default: '',
    },
    checkboxId: {
      type: String,
      default: 'default-id',
    },
    isWaiting: {
      type: Boolean,
      default: false,
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
  },
  methods: {
    onClick() {
      if (!this.isWaiting) {
        this.$emit('input', !this.value);
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/toggle";
</style>
