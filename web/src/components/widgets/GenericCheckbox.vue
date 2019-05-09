<template>
  <div :class="getStyleClasses">
    <div :class="$style.clickme"
         aria-hidden="true"
         @click="onClick"
         @keypress="onKeyDown">
      <checked-icon :id="`${checkboxId}-icon`" :selected="selected"/>
    </div>
    <input :id="checkboxId"
           ref="checkbox"
           v-tabbing="[this.$style.form, this.$style['checkbox-panel']]"
           :checked="selected"
           :class="[this.$style.form, this.$style.checkbox, this.$style['sr-only']]"
           type="checkbox"
           @change="onChange">
    <label :id="`${checkboxId}-label`" :for="checkboxId">
      <slot/>
    </label>
  </div>
</template>

<script>

import CheckedIcon from '@/components/icons/CheckedIcon';
import TabFocusMixin from './TabFocusMixin';

export default {
  name: 'GenericCheckbox',
  components: {
    CheckedIcon,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    checkboxId: {
      type: String,
      default: 'checkbox',
    },
    value: Boolean,
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
@import '../../style/forms';
@import "../../style/accessibility";

.clickme {
  outline-style: none;
  cursor: pointer;
}
</style>
