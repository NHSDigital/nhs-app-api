import { REDIRECT_PARAMETER } from '@/router/names';
import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';

export default {
  name: 'RedirectorMixin',
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
  methods: {
    goToUrlViaRedirector(destination) {
      const encodedDestination = encodeURIComponent(destination);
      const path = `/${INTERSTITIAL_REDIRECTOR_PATH}?${REDIRECT_PARAMETER}=${encodedDestination}`;

      if (this.isNativeApp) {
        this.goToUrl(path);
        return;
      }

      window.open(path, '_blank', 'noopener,noreferrer');
    },
  },
};
