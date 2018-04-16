<template>
  <div id="mainDiv">
    <spinner></spinner>
    <main class="content">
      // TODO: Remove in next task: exists to prove current task.
      Data: {{ JSON.stringify(this.appointmentSlots) }}
    </main>
  </div>
</template>

<script>
import axios from 'axios';
import Spinner from '@/components/Spinner';

export default {
  components: {
    Spinner,
  },
  data() {
    axios
      .get(`${this.$config.API_HOST}/patient/appointmentslots`)
      .then((data) => {
        this.appointmentSlots = data;
        this.spinnerOff();
      });

    return {
      appointmentSlots: [],
    };
  },
  mounted() {
    this.spinnerOn();
  },
  methods: {
    spinnerOn() {
      this.$root.$emit('show-loading-spinner');
    },
    spinnerOff() {
      this.$root.$emit('hide-loading-spinner');
    },
  },
};
</script>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';
</style>
