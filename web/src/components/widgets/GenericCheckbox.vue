<template>
  <div class="nhsuk-checkboxes__item" style="padding: 0 0 0 40px">
    <input :id="checkboxId"
           ref="checkbox"
           v-model="selectedCheckbox"
           class="nhsuk-checkboxes__input"
           type="checkbox"
           :name="name"
           :required="required"
           :value="value"
           @change="onChange"
           @keypress="onKeyDown">
    <label :id="`${checkboxId}-label`"
           class="nhsuk-label nhsuk-checkboxes__label"
           :for="checkboxId">
      <slot/>
    </label>
  </div>
</template>

<script>
import TabFocusMixin from './TabFocusMixin';

export default {
  name: 'GenericCheckbox',
  mixins: [TabFocusMixin.tabMixin],
  props: {
    name: {
      type: String,
      default: '',
    },
    checkboxId: {
      type: String,
      default: '',
    },
    label: {
      type: String,
      default: '',
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
    // eslint-disable-next-line vue/require-prop-types
    isSelected: {
      default: '',
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      labelId: `${this.checkboxId}-label`,
      selectedCheckbox: this.isSelected,
    };
  },
  methods: {
    onChange() {
      this.$emit('input', this.value);
    },
    onClick() {
      this.$refs.checkbox.click();
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.onClick();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  .nhsuk-checkboxes__input {
    height: 40px;
    width: 40px;
  }
</style>
