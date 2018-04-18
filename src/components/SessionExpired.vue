<template>
  <div class="errorContainer">
    <h3>{{ $t('sessionExpired.title')}}.</h3>
  </div>
</template>

<style lang="scss">
  @import '../style/colours';
  .errorContainer {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 45px;
    background-color: $warning;
  }
</style>
