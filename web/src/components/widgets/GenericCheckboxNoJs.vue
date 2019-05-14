<template>
  <div :class="[$style.checkbox, customClasses]">
    <input :id="name + '-' + checkboxId"
           v-model="selected"
           tabindex="0"
           :value="checkboxId"
           :name="name"
           :aria-labelledby="aLabelledBy"
           type="checkbox"
           @keyup.prevent="onKeyUp"
           @click="clicked">
    <label :id="labelId" :for="name + '-' + checkboxId" >
      <slot/>
    </label>
  </div>
</template>

<script>
export default {
  name: 'GenericCheckboxNoJs',
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
    onKeyUp(e) {
      if (e.keyCode === 32) {
        this.clicked();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/forms';
@import '../../style/desktopWeb/accessibility';
  input[type=checkbox]:focus + label::before {
    &:hover {
      @include outlineStyle;
    }
  }
</style>
