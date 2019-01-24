<template>
  <div :class="$style['radio-button-label']" tabindex="0" @click="selected"
       @keypress="onKeyDown">
    <radio-button-icon :selected="isSelected" :id="value"/>
    <input :id="name + '-' + value" :value="value" :name="name"
           :checked="model === value" tabindex="-1" type="radio" @change="selected">
    <label :for="name + '-' + value"
           :aria-labelledby="aLabelledBy"
           :class="$style['radio-label']">
      <span v-if="label">{{ label }}</span>
      <slot/>
    </label>
  </div>
</template>

<script>
import RadioButtonIcon from '@/components/icons/RadioButtonIcon';

export default {
  components: {
    RadioButtonIcon,
  },
  props: {
    id: {
      type: String,
      default: undefined,
    },
    name: {
      type: String,
      default: 'radioButton',
    },
    value: {
      type: String,
      default: '0',
    },
    label: {
      type: String,
      default: undefined,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    model: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    isSelected() {
      return this.model === this.value;
    },
  },
  methods: {
    selected(event) {
      this.$emit('select', this.value);
      event.stopPropagation();
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.selected(e);
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
@import '../../style/forms';
@import '../../style/radiobutton';

.radio-label {
  font-weight: normal;
}
</style>
