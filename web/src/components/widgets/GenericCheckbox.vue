<template>
  <div class="nhsuk-checkboxes__item nhsuk-u-padding-bottom-1" style="padding: 0 0 0 40px">
    <input :id="checkboxId"
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
  mounted() {
    this.$emit('onCheckedChanged', this.selectedCheckbox);
  },
  methods: {
    onChange(event) {
      this.$emit('input', this.value);
      this.$emit('onCheckedChanged', event.target.checked);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/generic-checkbox";
</style>
