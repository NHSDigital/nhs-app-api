<template>
  <div :class="$style['header-links']">
    <header-link :anchor-name="$t('navigation.header.helpAndSupport')"
                 :anchor-value="currentHelpUrl"
                 :anchor-id="'help-and-support-link'"/>
    <strong aria-hidden="true">
      |
    </strong>
    <header-link :anchor-name="$t('navigation.header.more')"
                 :anchor-value="morePath"
                 :anchor-id="'more-link'"
                 :anchor-internal="true"/>
    <strong aria-hidden="true">
      |
    </strong>
    <header-link :anchor-name="$t('navigation.header.logout')"
                 :anchor-value="logoutPath"
                 :anchor-id="'account-logout'"
                 :anchor-internal="true"
                 :anchor-action="actionLogout()"/>
  </div>
</template>

<script>

import HeaderLink from '@/components/widgets/HeaderLink';
import {
  LOGOUT_PATH,
  MORE_PATH,
} from '@/router/paths';

export default {
  name: 'HeaderLinks',
  components: {
    HeaderLink,
  },
  data() {
    return {
      morePath: `/patient/${MORE_PATH}`,
      logoutPath: `/patient/${LOGOUT_PATH}`,
    };
  },
  methods: {
    actionLogout() {
      this.$store.dispatch('session/setActionedLogout', true);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/header-links";
</style>
