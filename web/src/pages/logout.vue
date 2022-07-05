<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div class="nhsuk-u-padding-top-4">
        <p>
          {{ $t('logout.loggedOut.logIntoNHSAccountAgain') }}
        </p>
      </div>
      <br>
      <button id="loginButton"
              :button-classes="getButtonClasses"
              :class="$style['continueWithNhsLogin']"
              :aria-label="$t('logout.loggedOut.continueWithNhsLogin')"
              @click.stop.prevent="onContinueClicked">
        {{ $t('login.continueWithNhsLogin') }}
      </button>
    </div>
  </div>
</template>

<script>
import { BEGINLOGIN_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  metaInfo() {
    return {
      noscript: [
        { innerHTML: '<meta http-equiv="refresh" content="0;URL=\'/\'">', body: false },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN_PATH,
      isButtonDisabled: false,
    };
  },
  computed: {
    getButtonClasses() {
      return ['nhsuk-login', 'nhsuk-body', 'nhsuk-button'];
    },
  },
  created() {
    this.$store.dispatch('auth/logoutNoJs');
  },
  mounted() {
    sessionStorage.clear();
    this.$store.dispatch('auth/logout');
  },
  methods: {
    onContinueClicked() {
      if (!this.isButtonDisabled) {
        this.isButtonDisabled = true;
        this.$store.dispatch('analytics/satelliteTrack', 'login');
        redirectTo(this, BEGINLOGIN_PATH, this.$route.query);
      }
    },
  },
};
</script>
<style module lang="scss" scoped>
@import "@/style/custom/login";
</style>
