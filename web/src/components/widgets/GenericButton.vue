<template>
  <button :class="defaultClasses"
          @click="clicked($event)">
    <slot/>
  </button>
</template>

<script>
import DebounceMixin from '@/components/widgets/DebounceMixin';

export default {
  name: 'GenericButton',
  mixins: [DebounceMixin],
  props: {
    buttonClasses: {
      type: Array,
      default: () => [],
    },
  },
  computed: {
    defaultClasses() {
      return [...this.buttonClasses]
        .map(style => (((typeof (style) !== 'string') || style.match(/^nhsuk-/g))
          ? style : this.$style[style]));
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
