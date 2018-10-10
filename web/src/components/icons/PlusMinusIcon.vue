<template>
  <div :class="$style['info-message-icon']" data-purpose="icon">
    <span v-if="js">{{ iconPlus ? '+' : '&minus;' }}</span>
  </div>
</template>

<script>
export default {
  props: {
    iconPlus: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      js: false,
    };
  },
  computed: {
    icon() {
      if (!this.fillIcon) return '';
      return this.iconPlus ? '+' : '&minus;';
    },
  },
  mounted() {
    this.js = true;
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/colours";

.info-message-icon {
  background: $nhs_blue;
  color: white;
  font-size: 1.350em;
  width: 1.160em;
  height: 1.160em;
  border-radius: 0.938em;
  text-align: center;
  line-height: 1.160em;
  display: inline-block;
  flex-grow: 0;
  flex-shrink: 0;
  align-self: center;
}

</style>
