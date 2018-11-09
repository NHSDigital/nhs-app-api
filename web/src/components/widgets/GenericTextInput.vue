<template>
  <div :class="$style.form">
    <input v-tabbing="textInputClasses"
           ref="textInput"
           :class="getStyleClasses"
           :id="id"
           :required="required"
           :aria-labelledby="aLabelledBy"
           :maxlength="maxlength"
           :name="inputName"
           v-model="textValue"
           :type="type"
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
    inputName: {
      type: String,
      required: true,
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
    initialContents: {
      type: String,
      default: undefined,
    },
    required: {
      type: Boolean,
      default: false,
    },
  },
  data: function data() { return { model: undefined }; },
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
  mounted() {
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
</style>
