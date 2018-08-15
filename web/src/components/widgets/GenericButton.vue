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
      const classes = [this.$style.button];
      this.buttonClasses.forEach((element) => {
        classes.push(this.$style[element]);
      });
      return classes;
    },
  },
};

</script>
<style module lang="scss" scoped>
  @import '../../style/buttons';

</style>
