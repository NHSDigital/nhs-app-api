<template>
  <label :class="$style['radio-button-label']" :for="name + '-' + value"
         :aria-labelledby="aLabelledBy" tabindex="0" @keypress="onKeyDown">
    <radio-button-icon :selected="isSelected" :id="value"/>
    <input :id="name + '-' + value" :value="value" :name="name"
           :checked="model === value" tabindex="-1" type="radio" @change="selected">
    <div>
      <span v-if="label">{{ label }}</span>
      <slot/>
    </div>
  </label>
</template>

<script>
import RadioButtonIcon from '@/components/icons/RadioButtonIcon';

export default {
  name: 'GenericRadioButton',
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
    // eslint-disable-next-line vue/require-prop-types
    value: {
      default: '',
    },
    label: {
      type: String,
      default: undefined,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    // eslint-disable-next-line vue/require-prop-types
    model: {
      default: '',
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

.radio-button-label {
  font-weight: normal;
}
</style>
