<template>
  <component :is="container" class="radio-group">
    <legend v-if="header">
      <h3>{{ header }}</h3>
    </legend>
    <generic-radio-button v-for="radio in radios" :key="radio.value"
                          :hint="radio.hint"
                          :label="radio.label"
                          :name="name"
                          :checked="currentValue === radio.value"
                          :value="radio.value"
                          @select="selected"/>
  </component>
</template>

<script>
import GenericRadioButton from '@/components/widgets/GenericRadioButton';

export default {
  name: 'RadioGroup',
  components: {
    GenericRadioButton,
  },
  props: {
    // eslint-disable-next-line vue/require-prop-types
    currentValue: {
      default: '',
    },
    header: {
      type: String,
      default: '',
    },
    name: {
      type: String,
      default: 'radioButton',
    },
    radios: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      container: this.header ? 'fieldset' : 'div',
    };
  },
  methods: {
    selected(value) {
      this.$emit('select', value);
    },
  },
};
</script>

<style lang="scss" scoped>
@import '../style/radiobutton';
</style>

