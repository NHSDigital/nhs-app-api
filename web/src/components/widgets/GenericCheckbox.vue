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
           @change="onChange">
    <label :id="`${checkboxId}-label`"
           class="nhsuk-label nhsuk-checkboxes__label"
           :for="checkboxId">
      <slot/>
    </label>
  </div>
</template>

<script>
export default {
  name: 'GenericCheckbox',
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
    isSelected: {
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
      labelId: `${this.checkboxId}-label`,
      selectedCheckbox: this.isSelected,
    };
  },
  methods: {
    onChange() {
      this.$emit('input', this.value);
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
