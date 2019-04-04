<template>
  <div :class="$style['nhsuk-radios__item']">
    <input :id="inputId"
           v-model="selectedValue"
           :class="$style['nhsuk-radios__input']"
           :value="value"
           :name="name"
           type="radio"
           :aria-describedby="hintId"
           :required="required"
           @click.stop="selected">
    <label :class="[$style['nhsuk-label'], $style['nhsuk-radios__label']]"
           :for="inputId">{{ label }}</label>
    <span v-if="hint" :id="hintId">{{ hint }}</span>
  </div>
</template>

<script>
export default {
  name: 'GenericRadioButton',
  props: {
    hint: {
      type: String,
      default: '',
    },
    label: {
      type: String,
      default: '',
    },
    name: {
      type: String,
      default: 'radioButton',
    },
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
    // eslint-disable-next-line vue/require-prop-types
    selectedValue: {
      default: '',
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      inputId: `${this.name}-${this.value}`,
    };
  },
  computed: {
    hintId() {
      return this.hint ? `${this.inputId}-hint` : '';
    },
  },
  methods: {
    selected() {
      this.$emit('select', this.value);
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_ifff";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_functions";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_mixins";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/colours";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/globals";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/components/label/label";
@import "../../../node_modules/nhsuk-frontend/packages/components/radios/radios";
</style>
