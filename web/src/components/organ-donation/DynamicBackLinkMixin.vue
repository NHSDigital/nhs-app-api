<script>
import last from 'lodash/fp/last';
import { INDEX_PATH } from '@/router/paths';


export default {
  name: 'DynamicBackLinkMixin',
  computed: {
    backLink() {
      const lastPath = last(this.$router.history.router.previousPaths);
      return lastPath === undefined ? INDEX_PATH : lastPath;
    },
  },
  methods: {
    backClicked() {
      this.$router.goBack();
    },
  },
};
</script>
