<template>
  <no-return-flow-layout>
    <div v-if="this.$route.query.incompatible">
      <p>{{ this.$t('login.compatibility.itRequiresIOS11') }}</p>
      <p>{{ this.$t('login.compatibility.ifYouHaveAlreadyRegistered') }}
        <a style="display:inline" href="https://www.nhsapp.service.nhs.uk" target="_blank"
           rel="noopener noreferrer">{{ this.$t('login.compatibility.browserLink') }}</a>
        {{ this.$t('login.compatibility.onYourDesktop') }}</p>
      <p> {{ this.$t('login.compatibility.ifYouHaveNotRegistered') }} </p>
    </div>
  </no-return-flow-layout>
</template>

<script>
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import { redirectTo } from '@/lib/utils';
import { LOGIN_PATH } from '@/router/paths';

export default {
  name: 'IOSCompatibility',
  components: {
    NoReturnFlowLayout,
  },
  mounted() {
    const { source } = this.$store.state.device;
    if (source === 'android' || source === 'web' || !this.$route.query.incompatible) {
      redirectTo(this, LOGIN_PATH);
    }
  },
};
</script>
<style module lang="scss">
  html {
    background-color: #f0f4f5 !important
  }
</style>
