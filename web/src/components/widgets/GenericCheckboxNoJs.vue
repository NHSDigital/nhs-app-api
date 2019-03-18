<template>
  <div :class="[$style.checkbox, customClasses]">
    <input :id="name + '-' + checkboxId"
           v-model="selected"
           :value="checkboxId"
           :name="name"
           :aria-labelledby="aLabelledBy"
           type="checkbox"
           @click="clicked">
    <label :id="labelId" :for="name + '-' + checkboxId" >
      <slot/>
    </label>
  </div>
</template>

<script>
export default {
  props: {
    name: {
      type: String,
      default: 'input',
    },
    checkboxId: {
      type: String,
      default: 'checkbox',
    },
    selected: {
      type: Boolean,
      default: false,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    labelId: {
      type: String,
      default: undefined,
    },
    customClasses: {
      type: Array,
      default: () => [],
    },
  },
  methods: {
    clicked() {
      this.$emit('click', this.checkboxId);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/forms';
</style>
