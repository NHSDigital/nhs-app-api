<template>
  <div>
    <HomeHeader/>
    <LoginOrRegister />
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
  @import '../style/fonts';
  @import '../style/textstyles';
  @import '../style/colours';

  body {
    padding: 0;
    margin: 0;
    background: $background;
    @include default_text;
  }

  h1 {
    @include h1;
  }

  h2 {
    @include h2;
  }
</style>

<style lang="scss" scoped>
  * {
    padding: 0;
    margin: 0;
  }
</style>
