<template>
  <button :class="buttonStylingClasses" @click="$emit('on-click')">
    <slot/>
  </button>
</template>

<script>

export default {
  props: {
    buttonClasses: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    buttonStylingClasses() {
      const classes = ['button'].concat(this.buttonClasses);
      return classes;
    },
  },
};

</script>
<style lang="scss" scoped>
  @import '../style/buttons';

</style>
