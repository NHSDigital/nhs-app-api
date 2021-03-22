<template>
  <div>
    <span v-if="error" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.errorPrefix') }}</span>
      {{ errorText }}
    </span>
    <textarea :id="id"
              ref="textArea"
              v-model="textValue"
              v-tabbing="textAreaClasses"
              tabindex="0"
              :rows="rows"
              :class="inputClasses"
              :required="required"
              :aria-labelledby="aLabelledBy"
              :maxlength="maxlength"
              :name="name"
              autocomplete="off"
              autocorrect="off"
              autocapitalize="off"
              spellcheck="false"
              @focus="onFocus"
              @keydown="onKeyDown"
              @keyup="onKeyUp"/>
  </div>
</template>

<script>

import TabFocusMixin from './TabFocusMixin';

export default {
  name: 'GenericTextArea',
  mixins: [TabFocusMixin.tabMixin],
  props: {
    maxlength: {
      type: String,
      default: undefined,
    },
    aLabelledBy: {
      type: String,
      default: undefined,
    },
    id: {
      type: String,
      default: undefined,
    },
    textAreaClasses: {
      type: Array,
      default() {
        return [];
      },
    },
    initialContents: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: false,
    },
    name: {
      type: String,
      default: undefined,
    },
    rows: {
      type: Number,
      default: undefined,
    },
    value: {
      type: String,
      default: '',
    },
    error: {
      type: Boolean,
      default: false,
    },
    errorText: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    textValue: {
      get() {
        return this.value;
      },
      set(value) {
        this.$emit('input', value);
      },
    },
    inputClasses() {
      return this.textAreaClasses.concat([
        this.$style['nhsuk-textarea'],
        this.error && this.$style['nhsuk-textarea--error'],
      ]);
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
  },
  methods: {
    focus() {
      this.$refs.textArea.focus();
    },
    onFocus(event) {
      this.$emit('focus', event);
    },
    keydown() {
      this.$refs.textArea.keydown();
    },
    onKeyDown(event) {
      this.$emit('keydown', event);
    },
    keyup() {
      this.$refs.textArea.keyup();
    },
    onKeyUp(event) {
      this.$emit('keyup', event);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/colours";
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/components/textarea/textarea';
</style>
