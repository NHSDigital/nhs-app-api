<!-- eslint-disable vue/no-v-html -->
<template>
  <div class="nhsuk-form-group">
    <div class="nhsuk-checkboxes">
      <generic-checkbox v-for="checkbox in checkboxes"
                        :key="checkbox.code"
                        :value="checkbox.code"
                        :name="name"
                        :required="getRequired(checkbox)"
                        :checkbox-id="`checkbox-${checkbox.code}`"
                        :is-selected="selectedValues.includes(checkbox.code)"
                        @input="selectedValueChanged(checkbox)">
        <template v-if="renderAsHtml"><span v-html="checkbox.label"/></template>
        <template v-else>{{ checkbox.label }}</template>
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
      default: () => [],
    },
    renderAsHtml: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      selectedValues: this.value,
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
    getRequired(checkbox) {
      return (checkbox.required !== null && checkbox.required !== undefined)
        ? checkbox.required : this.required;
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/checkbox-group";
</style>
