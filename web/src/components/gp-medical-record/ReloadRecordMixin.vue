<script>
import { GP_MEDICAL_RECORD_NAME } from '@/router/names';

export default {
  beforeRouteLeave(to, from, next) {
    if (to.name === GP_MEDICAL_RECORD_NAME) {
      this.$store.dispatch('myRecord/reload', false);
    }
    next();
  },
};
</script>
