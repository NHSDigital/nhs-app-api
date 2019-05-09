<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <span v-if="error" :id="errorId" class="nhsuk-error-message">
      <span class="nhsuk-u-visually-hidden">{{ $t('generic.input.errors.messagePrefix') }}</span>
      {{ errorText }}
    </span>
    <textarea :id="id"
              ref="textArea"
              v-model="textValue"
              v-tabbing="textAreaClasses"
              tabindex="0"
              :class="inputClasses"
              :required="required"
              :aria-labelledby="aLabelledBy"
              :maxlength="maxlength"
              :name="name"
              autocomplete="off"
              autocorrect="off"
              autocapitalize="off"
              spellcheck="false"/>
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
      default: '255',
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
      return [
        this.$style['nhsuk-textarea'],
        this.error && this.$style['nhsuk-textarea--error'],
      ];
    },
    errorId() {
      return this.id ? `${this.id}-error-message` : 'error-message';
    },
  },
  methods: {
    focus() {
      this.$refs.textArea.focus();
    },
  },
};

</script>
<style module lang="scss" scoped>
  @import "../../style/colours";
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/components/textarea/textarea';

  div {
    &.desktopWeb {
      max-width: 540px;
    }

    .nhsuk-textarea {
      width: 100%;
    }

    .nhsuk-textarea:hover {
      box-shadow: 0 0 0 4px $hover_highlight;
      outline: 4px solid transparent;
      outline-offset: 4px;
    }
  }
</style>
