<template>
  <div :class="['custom-dropdown', 'custom-dropdown--white', {selectError: errorBorder}]">
    <select v-model="selectedValue" :id="selectId" :name="selectName" required=""
            class="custom-dropdown__select custom-dropdown__select--white">
      <slot/>
    </select>
  </div>
</template>

<script>export default {
  props: {
    value: {
      type: String,
      default: '',
    },
    selectId: {
      type: String,
      default: undefined,
    },
    selectName: {
      type: String,
      default: undefined,
    },
    errorBorder: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    selectedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit('input', val);
      },
    },
  },
};
</script>

<style scoped lang="scss">
@import "../../style/select";
@import "../../style/colours";
.custom-dropdown {
  &.selectError {
    border: 1px $error solid;
  }
}
</style>
