<template>
  <div :class="$style.betaBanner">
    <div :class="$style.betaLabel">BETA</div>
    <p :class="$style.betaLine">This is a new service.</p>
  </div>
</template>

<script>
export default {
  name: 'BetaBanner',
};
</script>

<style module lang="scss" scoped>
@import "../style/colours";
  .betaBanner {
    padding: 1em 1em 0.7em;
  }
  .betaLabel {
    display: block;
    font-weight: 700;
    font-size: 1em;
    line-height: 1.5em;
    color: $black;
    display: inline-block;
    margin-right: 0.6em;
    margin-left: -0.3em;
    margin-top: -0.3em;
    background: #ffb81c;
    padding: 0.3em 0.6em 0.3em 0.6em;
  }
  .betaLine {
    display: inline-block;
  }
</style>
