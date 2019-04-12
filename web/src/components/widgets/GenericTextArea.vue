<template>
  <div :class="[$style['nhsuk-form-group'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <textarea :id="id"
              ref="textArea"
              v-model="textValue"
              v-tabbing="textAreaClasses"
              :class="$style['nhsuk-textarea']"
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
  components: {
    TabFocusMixin,
  },
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
      default() { return []; },
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
  },
  methods: {
    focus() {
      this.$refs.textArea.focus();
    },
  },
};

</script>
<style module lang="scss" scoped>
@import "../../../node_modules/nhsuk-frontend/packages/core/settings/_spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_spacing";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_ifff";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/_functions";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/globals";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/colours";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/mixins";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/sass-mq";
 @import "../../../node_modules/nhsuk-frontend/packages/core/settings/typography";
 @import "../../../node_modules/nhsuk-frontend/packages/core/tools/typography";
@import "../../../node_modules/nhsuk-frontend/packages/components/textarea/textarea";


div {
 &.desktopWeb {
  .nhsuk-textarea {
   max-width: 540px;
  }
 }
}
</style>
