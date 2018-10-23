<template>
  <button v-tabbing="buttonStylingClasses" :id="id"
          :class="getStyleClasses"
          @click="clicked($event)">
    <slot/>
  </button>
</template>

<script>

import TabFocusMixin from './TabFocusMixin';

export default {
  components: {
    TabFocusMixin,
  },
  mixins: [TabFocusMixin.tabMixin],
  props: {
    buttonClasses: {
      type: Array,
      default: () => [],
    },
    id: {
      type: String,
      default: '',
    },
  },
  computed: {
    buttonStylingClasses() {
      const classes = [this.$style.button];
      this.buttonClasses.forEach((element) => {
        classes.push(this.$style[element]);
      });
      return classes;
    },
  },
  methods: {
    clicked(event) {
      this.$emit('click', event);
    },
  },
};

</script>

<style module lang="scss" scoped>
@import '../../style/buttons';
</style>
