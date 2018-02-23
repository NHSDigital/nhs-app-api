<template>
  <main class="login-or-register">
    <button data-id="login-button" class="button green">
      {{ $t('loginOrRegister.login') }}
    </button>

    <hr class="hr-text" v-bind:data-content="$t('common.or')">

    <button data-id="create-account-button" class="create-account-button button">
       {{ $t('loginOrRegister.createAccount') }}
    </button>
  </main>
</template>

<style lang="scss"  scoped>
  @import '../style/colours';
  @import '../style/textstyles';
  @import '../style/fonts';
  @import '../style/buttons';
  @import '../style/icons';
  main {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    height: 200px;
    padding: 24px;
    box-sizing: border-box;

    .hr-text {
      font-family: $default;
      line-height: 1em;
      position: relative;
      outline: 0;
      border: 0;
      color: $light_grey;
      text-align: center;
      height: 1.5em;
      margin-bottom: 16px;

      &:before {
        content: '';
        background: $light_grey;
        position: absolute;
        left: 0;
        top: 50%;
        width: 100%;
        height: 1px;
      }

      &:after {
        content: attr(data-content);
        position: relative;
        display: inline-block;
        padding: 0 .5em;
        line-height: 1.5em;
        color: #818078;
        background-color: $white;
      }
    }
  }
</style>
