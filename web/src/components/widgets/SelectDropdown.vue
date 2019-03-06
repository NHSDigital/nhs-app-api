<template>
  <span :class="getStyleClasses">
    <select v-tabbing="defaultClasses"
            :id="selectId"
            v-model="selectedValue"
            :name="selectName"
            :aria-labelledby="aLabelledBy"
            :class="[
              $style['custom-dropdown__select'],
              $style['custom-dropdown__select_element'],
              $style['custom-dropdown__select--white']
            ]"
            :required="required"
            tabindex="0">
      <slot/>
    </select>
  </span>
</template>

<script>

import TabFocusMixin from './TabFocusMixin';

export default {
  components: {
    TabFocusMixin,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    value: {
      type: String,
      default: '',
    },
    selectId: {
      type: String,
      default: undefined,
    },
    selectName: {
      type: String,
      default: undefined,
    },
    errorBorder: {
      type: Boolean,
      default: false,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: true,
    },
  },
  computed: {
    selectedValue: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit('input', val);
      },
    },
    defaultClasses() {
      const dropdownClass = [this.$style.form, this.$style['custom-dropdown']];
      if (this.errorBorder) {
        dropdownClass.push(this.$style['validation-select-border']);
      }

      if (this.marginBottom) {
        dropdownClass.push(this.$style['validation-select-border']);
      }

      return dropdownClass;
    },
  },
};
</script>

<style module scoped lang="scss">
@import "../../style/forms";
@import "../../style/select";
@import "../../style/errorvalidation";

 select::-ms-expand {
  display: none;
 }
</style>
