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
      selectedValues: {
        type: Array,
        default: [],
      },
    };
  },
  methods: {
    selectedValueChanged(checkbox) {
      this.checkboxes[this.checkboxes.indexOf(checkbox)].selected = !checkbox.selected;
      const selected = this.checkboxes.filter(c => c.selected).map(c => c.code);
      this.$emit('select', selected);
    },
  },
};
</script>

<style module lang="scss">
.nhsuk-fieldset{
 margin-bottom: 16px;
}
</style>
