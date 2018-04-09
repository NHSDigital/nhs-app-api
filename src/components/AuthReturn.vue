<script>

import { handleAuthReturn } from '@/services/authorization-service';

export default {
  name: '',
  mounted() {
    this.$nextTick(() => {
      handleAuthReturn();
      window.location.href = '/';
    });
  },
};
</script>
