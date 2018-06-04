<template>
  <div v-show="$store.state.session.showExpiryMessage" class="alertContainer">
    <h3>{{ $t('icons.sessionExpired.title') }}</h3>
  </div>
</template>

<style lang="scss">
 @import '../style/colours';
 .alertContainer {
   display: flex;
   align-items: center;
   justify-content: center;
   box-sizing: border-box;
   height: 32px;
   background-color: $warning;
   color: $dark_grey;
  }
</style>
