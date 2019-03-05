<template>
  <button :id="id"
          :class="defaultClasses" :type="type"
          @click="clicked($event)">
    <slot/>
  </button>
</template>

<script>

export default {
  props: {
    buttonClasses: {
      type: Array,
      default: () => [],
    },
    id: {
      type: String,
      default: '',
    },
    type: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    defaultClasses() {
      return [...this.buttonClasses]
        .map(style => (typeof (style) === 'string' ? this.$style[style] : style));
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
