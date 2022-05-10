import { INTERSTITIAL_REDIRECTOR_PATH } from '@/router/paths';

export default {
  name: 'RedirectorMixin',
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
  },
  props: {
    deepLinkUrl: {
      type: String,
      required: true,
    },
  },
  methods: {
    onClick() {
      const encodedDeepLink = encodeURIComponent(this.deepLinkUrl);
      const path = `/${INTERSTITIAL_REDIRECTOR_PATH}?redirect_to=${encodedDeepLink}`;

      if (this.isNativeApp) {
        this.goToUrl(path);
        return;
      }

      window.open(path, '_blank', 'noopener,noreferrer');
    },
  },
};
