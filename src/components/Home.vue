<template>
  <div id="mainDiv">
    <home-header/>
    <login-or-register/>
    <navigation-menu v-if="showMenu" />
  </div>
</template>
<script>
import HomeHeader from '@/components/HomeHeader';
import LoginOrRegister from '@/components/LoginOrRegister';
import NavigationMenu from '@/components/NavigationMenu';

export default {
  components: {
    HomeHeader,
    LoginOrRegister,
    NavigationMenu,
  },

  data() {
    return {
      showMenu: true,
    };
  },

  created() {
    if (this.$route.query.source === 'mobile') {
      this.showMenu = false;
    }
  },

  mounted() {
    if (this.showMenu) {
      this.$el.classList.add('navigationVisible');
    }
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

  .navigationVisible {
    margin-bottom: 70px;
  }

</style>
