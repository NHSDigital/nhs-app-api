<template>
  <div id="mainDiv">
    <home-header/>
    <login-or-register/>
  </div>
</template>
<script>

import HomeHeader from '@/components/HomeHeader';
import LoginOrRegister from '@/components/LoginOrRegister';

export default {
  components: {
    HomeHeader,
    LoginOrRegister,
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';

  #mainDiv {
    padding: 0;
    @include default_text;
  }

</style>
