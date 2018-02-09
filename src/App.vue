<template>
  <div id="app">
    <router-view/>
  </div>
</template>


<script>
import NHSOnlineApi from './services/nhsonlineapi';

export default {
  name: 'App',
  provide: {
    nhsOnlineApi: new NHSOnlineApi(process.env.API_HOST),
  },
};
</script>


<style>

</style>
