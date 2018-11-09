<template>
  <div :class="[$style.statusLabel, labelStyle]">{{ labelText }}</div>
</template>

<script>
export default {
  props: {
    labelText: {
      type: String,
      default: '',
      required: true,
    },
    status: {
      type: String,
      default: 'success',
      required: false,
    },
  },
  computed: {
    labelStyle() {
      switch (this.status) {
        case 'warning':
          return this.$style.warning;
        case 'error':
          return this.$style.error;
        default:
          return this.$style.success;
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/statuslabel';
</style>
