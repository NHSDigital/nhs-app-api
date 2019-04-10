<template>
  <div class="nhsuk-radios__item">
    <input :id="inputId"
           v-model="selectedValue"
           class="nhsuk-radios__input"
           :value="value"
           :name="name"
           type="radio"
           :aria-describedby="hintId"
           :required="required"
           @click.stop="selected">
    <label class="nhsuk-label nhsuk-radios__label"
           :for="inputId">{{ label }}</label>
    <span v-if="hint" :id="hintId" class="nhsuk-hint nhsuk-radios__hint">{{ hint }}</span>
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
      return this.hint ? `${this.inputId}-hint` : undefined;
    },
  },
  methods: {
    selected() {
      this.$emit('select', this.value);
    },
  },
};
</script>
