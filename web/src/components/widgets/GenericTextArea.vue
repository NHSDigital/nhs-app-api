<template>
  <div :class="$style.form">
    <textarea v-tabbing="textAreaClasses"
              ref="textArea"
              :class="getStyleClasses"
              :id="id"
              :required="required"
              v-model="textValue"
              :aria-labelledby="aLabelledBy"
              :maxlength="maxlength"
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
      this.$refs.textArea.focus();
    },
  },
};

</script>
<style module lang="scss" scoped>
@import '../../style/forms';
</style>
