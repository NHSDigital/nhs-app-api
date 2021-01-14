<template>
  <no-return-flow-layout>
    <div v-if="isIncompatible">
      <p>{{ this.$t('compatibility.incompatible.itRequiresIOS11') }}</p>
      <p>{{ this.$t('compatibility.ifYouHaveAlreadyRegistered') }}
        <a style="display:inline" href="https://www.nhsapp.service.nhs.uk" target="_blank"
           rel="noopener noreferrer">{{ this.$t('compatibility.browserLink') }}
        </a>
        {{ this.$t('compatibility.onYourDesktop') }}</p>
      <p> {{ this.$t('compatibility.incompatible.ifYouHaveNotRegistered') }} </p>
    </div>
    <div v-else>
      <p>{{ this.$t('compatibility.compatible.youNeedToUpdateYourSoftware') }}</p>
      <p>{{ this.$t('compatibility.ifYouHaveAlreadyRegistered') }}
        <a style="display:inline" href="https://www.nhsapp.service.nhs.uk" target="_blank"
           rel="noopener noreferrer">{{ this.$t('compatibility.browserLink') }}
        </a>
        {{ this.$t('compatibility.onYourDesktop') }}</p>
      <p> {{ this.$t('compatibility.compatible.ifYouHaveNotRegistered') }} </p>
      <ol class="nhsuk-u-margin-3">
        <li>{{ this.$t('compatibility.compatible.updateYourSoftwareOption') }}</li>
        <li>{{ this.$t('compatibility.compatible.registerInTheAppOption') }}</li>
      </ol>
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
  data() {
    return {
      isIncompatible: this.$route.query.incompatible === 'true',
    };
  },
  created() {
    this.$store.dispatch('compatibility/updateCompatibility', this.isIncompatible);
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
  @import "@/style/custom/ios-compatibility";
</style>
