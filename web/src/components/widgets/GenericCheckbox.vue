<template>
  <div class="nhsuk-checkboxes__item" style="padding: 0 0 0 40px">
    <input :id="checkboxId"
           ref="checkbox"
           v-model="isSelected"
           :checked="selected"
           class="nhsuk-checkboxes__input"
           type="checkbox"
           :required="required"
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
    value: {
      type: Boolean,
      default: false,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      isSelected: this.value,
      labelId: `${this.checkboxId}-label`,
    };
  },
  computed: {
    selected: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit('input', val);
      },
    },
  },
  methods: {
    onChange($event) {
      this.selected = $event.target.checked;
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
