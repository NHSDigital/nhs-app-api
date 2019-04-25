<template>
  <div :class="$style['radio-button-icon']" aria-hidden="true">
    <div v-if="isSelected" :class="$style.selected"/>
  </div>
</template>

<script>
export default {
  name: 'RadioButtonIcon',
  props: {
    selected: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    isSelected() {
      return this.selected;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/colours";
@import '../../style/radiobutton';
</style>
