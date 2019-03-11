<template>
  <div class="radio-item">
    <input :id="inputId"
           :value="value"
           :name="name"
           :checked="checked"
           type="radio"
           :aria-describedby="hintId"
           @change.stop="selected">
    <label :for="inputId">{{ label }}</label>
    <span  v-if="hint" :id="hintId">{{ hint }}</span>
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
    id: {
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
    checked: {
      type: Boolean,
      default: false,
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
    isSelected() {
      return this.model === this.value;
    },
  },
  methods: {
    selected() {
      this.$emit('select', this.value);
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.selected(e);
      }
    },
  },
};
</script>
<style lang="scss" scoped>
@import '../../style/radiobutton';
</style>
