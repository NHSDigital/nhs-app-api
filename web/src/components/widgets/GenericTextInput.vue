<template>
  <div :class="[$style.form, !$store.state.device.isNativeApp && $style.desktopWeb]">
    <input :id="id"
           ref="textInput"
           v-model="textValue"
           v-tabbing="textInputClasses"
           :class="getStyleClasses"
           :aria-labelledby="aLabelledBy"
           :maxlength="maxlength"
           :required="required"
           :name="name"
           :type="type"
           :pattern="pattern"
           autocomplete="off"
           autocorrect="off"
           autocapitalize="off"
           spellcheck="false">
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
    name: {
      type: String,
      default: undefined,
    },
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
    textInputClasses: {
      type: Array,
      default() { return []; },
    },
    type: {
      type: String,
      default: 'text',
    },
    required: {
      type: Boolean,
      default: false,
    },
    initialContents: {
      type: String,
      default: undefined,
    },
    pattern: {
      type: String,
      default: undefined,
    },
  },
  data: function data() {
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
      this.$refs.textInput.focus();
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
