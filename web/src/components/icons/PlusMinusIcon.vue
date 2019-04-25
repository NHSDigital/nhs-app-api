<template>
  <div :class="$style['info-message-icon']" data-purpose="icon">
    {{ iconPlus ? '+' : '&minus;' }}
  </div>
</template>

<script>
export default {
  name: 'PlusMinusIcon',
  props: {
    iconPlus: {
      type: Boolean,
      default: true,
    },
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
