<template>
  <nhs-uk-layout :has-footer="!chromeless">
    <div v-if="chromeless" slot="header"/>
    <router-view/>
    <slot/>
    <div v-if="chromeless" slot="footer"/>
  </nhs-uk-layout>
</template>

<script>
import get from 'lodash/fp/get';
import NhsUkLayout from '@/components/layout/NhsUkLayout';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';
import { REDIRECT_PARAMETER } from '@/router/names';
import { isBlankString } from '@/lib/utils';
import { UPDATE_HEADER, EventBus } from '@/services/event-bus';
import OnUpdateTitleMixin from '@/plugins/mixinDefinitions/OnUpdateTitleMixin';

export default {
  name: 'NhsUkAppLayout',
  components: {
    NhsUkLayout,
  },
  mixins: [OnUpdateTitleMixin],
  metaInfo() {
    let { source } = this.$store.state.device;
    const { nativeVersion, webVersion } = this.$store.state.appVersion;

    if (!isBlankString(nativeVersion)) {
      source = `${source} (${nativeVersion})`;
    }

    const head = {
      htmlAttrs: { lang: this.$t('language') },
      title: this.title,
      titleTemplate: `%s - ${this.$t('appTitle')}`,
      meta: [
        { name: 'web version', content: webVersion },
        { name: 'platform', content: source },
      ],
      // TODO: needed if 'nojs' is not a thing?
      __dangerouslyDisableSanitizers: ['noscript'],
    };

    // TODO: needed if 'nojs' is not a thing?
    const durationSeconds = get('durationSeconds', this.$store.$cookies.get('nhso.session'));

    if (durationSeconds) {
      head.noscript = [{
        innerHTML: `<meta http-equiv="refresh" content="${durationSeconds};URL='/account/signout'">`,
        body: false,
      }];
    }

    const { analyticsCookieAccepted } = this.$store.state.termsAndConditions;
    const analyticsScriptUrl = this.$store.$env.ANALYTICS_SCRIPT_URL;

    if (analyticsScriptUrl !== 'NOT_SET' && analyticsCookieAccepted && !this.$route.meta.isAnonymous) {
      head.script = [{
        src: analyticsScriptUrl,
      }];
    }

    return head;
  },
  data() {
    return {
      header: '',
      title: '',
    };
  },
  beforeRouteUpdate(to, _, next) {
    EventBus.$emit(UPDATE_HEADER, to.meta);
    this.onUpdateTitle(to.meta);
    next();
  },
  computed: {
    chromeless() {
      if (this.$route.path !== INTERSTITIAL_REDIRECTOR_PATH) {
        return false;
      }

      const redirectPath = get(REDIRECT_PARAMETER)(this.$route.query);

      if (!redirectPath) {
        return true;
      }

      const services = this.$store.state.knownServices.knownServices
        .filter(service => redirectPath.includes(service.url));

      return !(services.length > 0 && services[0].showThirdPartyWarning === true);
    },
  },
  mounted() {
    EventBus.$emit(UPDATE_HEADER, this.$route.meta);
  },
};
</script>

<style lang="scss">
  @import "~nhsuk-frontend/packages/nhsuk";
  @import "../style/links";
</style>
