<template>
  <div class="nhsuk-form-group">
    <div class="nhsuk-checkboxes">
      <generic-checkbox v-for="checkbox in checkboxes"
                        :key="checkbox.code"
                        :value="checkbox.code"
                        :name="name"
                        :required="required"
                        :checkbox-id="`checkbox-${checkbox.code}`"
                        @input="selectedValueChanged(checkbox)">
        <span>{{ checkbox.label }}</span>
      </generic-checkbox>
    </div>
  </div>
</template>

<script>
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

export default {
  name: 'CheckboxGroup',
  components: {
    GenericCheckbox,
  },
  props: {
    name: {
      type: String,
      default: 'checkbox',
    },
    checkboxes: {
      type: Array,
      required: true,
    },
    required: {
      type: Boolean,
      default: true,
    },
    value: {
      type: Array,
      default: undefined,
    },
  },
  data() {
    return {
      selectedValues: [],
    };
  },
  methods: {
    selectedValueChanged(checkbox) {
      const { code } = checkbox;
      const selectedValueIndex = this.selectedValues.indexOf(code);

      if (selectedValueIndex >= 0) {
        this.selectedValues.splice(selectedValueIndex, 1);
      } else {
        this.selectedValues.push(code);
      }

      // need to slice to duplicate array to prevent
      // store mutations on subsequent selectedValueChanged event
      this.$emit('select', this.selectedValues.slice());
    },
  },
};
</script>

<style lang="scss">
.nhsuk-checkboxes {
  margin-bottom: 16px;
}
</style>
