<script>
import { GP_MEDICAL_RECORD } from '@/lib/routes';

export default {
  beforeRouteLeave(to, from, next) {
    if (from !== to && to.path === GP_MEDICAL_RECORD.path) {
      this.$store.dispatch('myRecord/reload', false);
    }
    next();
  },
};
</script>
