<template>
  <div class="pull-content">
    <advice-check/>
  </div>
</template>

<script>
import adviceCheck from '@/components/advice/AdviceCheck';

export default {
  components: {
    adviceCheck,
  },
  mounted() {
    this.$store.dispatch('device/unlockNavBar');
  },
};
</script>

