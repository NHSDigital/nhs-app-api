<script>
import GP_MEDICAL_RECORD_PATH from '@/router/paths';
import HEALTH_RECORDS from '@/router/routes/medical-record';

export default {
  beforeRouteLeave(to, from, next) {
    if (from !== to && to.path === (GP_MEDICAL_RECORD_PATH || HEALTH_RECORDS)) {
      this.$store.dispatch('myRecord/reload', false);
    }
    next();
  },
};
</script>
