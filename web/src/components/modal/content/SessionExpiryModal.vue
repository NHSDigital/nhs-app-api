<template>
  <div>
    <div :class="$style.modalBody">
      <p data-sid="warningDurationInformation" :class="$style.warningHeader">
        {{ $tc('generic.errors.forSecurityReasonsYouWillBeLoggedOutInMinutes',
               sessionExpiryInMinutes, {time: sessionExpiryInMinutes}) }}
      </p>
    </div>
    <generic-button id="modalExtendSession"
                    :button-classes="['nhsuk-button', $style['nhsuk-button-full-width']]"
                    @click.prevent="extendSession">
      {{ $t('generic.errors.stayLoggedIn') }}
    </generic-button>

    <div :class="$style.logoutPanel">
      <a id="modalExtendLogout"
         :class="[$style['nhsuk-action-link__link'], $style.logoutLink]"
         :href="logoutPath"
         data-purpose="logout"
         @click.prevent="logout">
        {{ $t('generic.logOut') }}
      </a>
    </div>

  </div>

</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import { LOGOUT_PATH } from '@/router/paths';

export default {
  name: 'SessionExpiryModal',
  components: {
    GenericButton,
  },
  computed: {
    sessionExpiryInMinutes() {
      const minutes = Math.floor(
        this.$store.$env.SESSION_EXPIRING_WARNING_SECONDS / 60,
      );
      return minutes < 1 ? 1 : minutes;
    },

    logoutPath() {
      return LOGOUT_PATH;
    },
  },
  methods: {
    extendSession() {
      this.$store.dispatch('modal/hide');
      this.$store.dispatch('session/extend');
    },

    logout() {
      this.$store.dispatch('modal/hide');
      this.$store.dispatch('auth/logout');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../../style/fonts";
  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/action-link/action-link';

  .nhsuk-button-full-width {
    width: 100%;
  }

  .logoutPanel {
    text-align: center;
  }

  .logoutLink {
    vertical-align: middle;
    padding-left: 0;
    text-align: center;
    color: $color_nhsuk-blue;

    &:visited {
      color: $color_nhsuk-blue;
    }
  }
</style>
