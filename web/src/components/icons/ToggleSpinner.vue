<template>
  <div class="loading-spinner-background" tabindex="-1" focusable="false">
    <div class="loading-spinner" tabindex="-1" focusable="false" />
  </div>
</template>

<script>
export default {
  name: 'ToggleSpinner',
};
</script>

<style lang="scss" scoped>
  .loading-spinner-background {
    background-color: rgba(255, 255, 255, 0.8);
    border-radius: 50%;
    width: 30px;
    height: 30px;
  }

  .loading-spinner {
    border-radius: 50%;
    width: 20px;
    height: 20px;
    border-top: 0.2em solid rgba(0, 94, 184, 1);
    border-right: 0.2em solid rgba(0, 94, 184, 1);
    border-bottom: 0.2em solid rgba(0, 94, 184, 1);
    border-left: 0.2em solid rgba(0, 94, 184, 0);
    animation: spinner-animation 1.5s infinite linear;
    background-color: rgba(255, 255, 255, 0);
    display: inline-block;
    margin: 5px;
  }
  @keyframes spinner-animation {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(360deg);
    }
  }
</style>
