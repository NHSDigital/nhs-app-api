<template>
  <main class="login-or-register">
    <LoginButton />

    <hr class="hr-text" v-bind:data-content="$t('common.or')">

    <button data-id="create-account-button" class="create-account-button button">
       {{ $t('loginOrRegister.createAccount') }}
    </button>
  </main>
</template>
<script>
import LoginButton from '@/components/LoginButton';

export default {
  components: {
    LoginButton,
  },
};
</script>
<style lang="scss"  scoped>
  @import '../style/colours';
  @import '../style/fonts';
  @import '../style/buttons';
  main {
    height: 200px;
    padding: 24px;
    box-sizing: border-box;

    .hr-text {
      font-family: $default;
      line-height: 1em;
      position: relative;
      outline: 0;
      border: 0;
      color: $mid_grey;
      text-align: center;
      height: 1.5em;
      margin-bottom: 16px;

      &:before {
        content: '';
        background: $mid_grey;
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
