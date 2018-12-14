<template>
  <div id="mainDiv" :class="[$style['no-padding'], 'pull-content']">
    <div :class="$style.info">
      <p>{{ $t('od1_1.subheader') }}</p>
    </div>
  </div>
</template>

<script>

</script>

<style module lang="scss" scoped>
@import "../../style/info";

.info {
  p {
    color: $dark_grey;
    font-weight: bold;
  }
}
</style>
