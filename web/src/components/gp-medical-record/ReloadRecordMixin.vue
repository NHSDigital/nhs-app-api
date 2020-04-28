<script>
import { GP_MEDICAL_RECORD, HEALTH_RECORDS } from '@/lib/routes';

export default {
  beforeRouteLeave(to, from, next) {
    if (from !== to && to.path === (GP_MEDICAL_RECORD.path || HEALTH_RECORDS)) {
      this.$store.dispatch('myRecord/reload', false);
    }
    next();
  },
};
</script>
