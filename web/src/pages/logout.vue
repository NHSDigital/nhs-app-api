<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="!isSesssionExpired">
        <div class="nhsuk-u-padding-top-4">
          <p>
            {{ $t('logout.loggedOut.logIntoNHSAccountAgain') }}
          </p>
        </div>
        <br>
      </div>
      <div v-else>
        <p id="forSecurityYouAreAutoLoggedOutText">
          {{ $t('logout.loggedOut.sessionTimeOut.forSecurityYouAreAutoLoggedOut') }}
        </p>
        <p id="ifYouWereEnteringInfoText">
          {{ $t('logout.loggedOut.sessionTimeOut.ifYouWereEnteringInfo') }}
        </p>
      </div>
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
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';

export default {
  mixins: [OnUpdateTitleMixin],
  metaInfo() {
    return {
      title: this.isSesssionExpired ? `${this.$t('navigation.pages.titles.youHaveBeenLoggedOut')}` : `${this.$t('navigation.pages.titles.logout')}`,
      noscript: [
        { innerHTML: '<meta http-equiv="refresh" content="0;URL=\'/\'">', body: false },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };
  },
  beforeRouteUpdate(to, _, next) {
    EventBus.$emit(UPDATE_HEADER, to.meta);
    this.onUpdateTitle(to.meta);
    next();
  },
  data() {
    return {
      authoriseUrl: BEGINLOGIN_PATH,
      isButtonDisabled: false,
      isSesssionExpired: this.$store.state.session.showExpiryMessage,
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
    this.updateHeader();
    sessionStorage.clear();
    this.$store.dispatch('auth/logout');
  },
  methods: {
    updateHeader() {
      if (this.isSesssionExpired) {
        const headerText = this.$t('navigation.pages.headers.youHaveBeenLoggedOut');
        EventBus.$emit(UPDATE_HEADER, headerText, true);
      }
    },
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
