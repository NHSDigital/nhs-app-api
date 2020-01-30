<script>
import get from 'lodash/fp/get';
import { findByName, INDEX, INTERSTITIAL_REDIRECTOR, REDIRECT_PARAMETER } from '@/lib/routes';

export default {
  methods: {
    conditionalRedirect() {
      const redirectName = get(REDIRECT_PARAMETER)(this.$route.query);
      if (redirectName) {
        const internalRedirect = findByName(redirectName);
        if (internalRedirect) {
          this.$router.push(internalRedirect.path);
          return;
        }

        this.$router.push({
          path: INTERSTITIAL_REDIRECTOR.path,
          query: this.$route.query,
        });
        return;
      }

      this.$router.push(INDEX.path);
    },
  },
};
</script>
