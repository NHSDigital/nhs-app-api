<template>
    <header>
        <router-link to="/">
            <home-icon/>
        </router-link>
        <account-icon/>
        <hr>
        <h1>Home</h1>
    </header>
</template>

<script>

import AccountIcon from '@/components/icons/AccountIcon';
import HomeIcon from '@/components/icons/HomeIcon';

export default {
  components: {
    AccountIcon,
    HomeIcon,
  },
};

</script>

<style lang="scss" scoped>
  @import "../style/menu";
</style>
