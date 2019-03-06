<template>
  <div :class="[$style.form, !$store.state.device.isNativeApp && $style.desktopWeb]">
    <textarea v-tabbing="textAreaClasses"
              ref="textArea"
              :id="id"
              v-model="textValue"
              :class="getStyleClasses"
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
  },
  data() {
    return {
      model: undefined,
    };
  },
  computed: {
    textValue: {
      get() {
        return this.model;
      },
      set(value) {
        this.model = value;
        this.$emit('input', value);
      },
    },
  },
  created() {
    this.model = this.initialContents;
  },
  methods: {
    focus() {
      this.$refs.textArea.focus();
    },
  },
};

</script>
<style module lang="scss" scoped>
@import '../../style/forms';
div {
 &.desktopWeb {
  .form {
   max-width: 540px;
  }
 }
}
</style>
