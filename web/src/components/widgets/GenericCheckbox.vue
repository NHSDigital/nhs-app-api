<template>
  <div :class="getStyleClasses">
    <div class="clickme" tabindex="-1" @click="check" @keypress="onKeyDown">
      <checked-icon :selected="selected" :id="checkboxId" />
    </div>
    <input v-tabbing="checkboxPanelStylingClasses"
           :class="checkboxStylingClasses"
           :value="checkboxId"
           :id="name + '-' + checkboxId"
           v-model="selected"
           type="checkbox"
           @click="check">
    <slot/>
  </div>
</template>

<script>

import CheckedIcon from '@/components/icons/CheckedIcon';
import TabFocusMixin from './TabFocusMixin';

export default {
  components: {
    TabFocusMixin,
    CheckedIcon,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    checkboxId: {
      type: String,
      default: 'checkbox',
    },
    name: {
      type: String,
      default: 'input',
    },
    selected: {
      type: Boolean,
      default: false,
    },
    checkboxClasses: {
      type: Array,
      default: () => [],
    },
  },
  data: function data() { return { checkedModel: false }; },
  computed: {
    checkboxPanelStylingClasses() {
      return [this.$style.form, this.$style['checkbox-panel']];
    },
    checkboxStylingClasses() {
      return [this.$style.form, this.$style.checkbox, this.$style['sr-only']];
    },
  },
  mounted() {
    this.checkedModel = this.selected;
  },
  methods: {
    check() {
      this.$emit('click');
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.check();
      }
    },
  },
};

</script>
<style module lang="scss" scoped>
@import '../../style/forms';
@import "../../style/accessibility";
</style>
