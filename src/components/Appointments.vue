<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <ul>
        <li :key="slot.id" v-for="slot in slots">
          <appointment-slot :appointmentSlot="slot"/>
        </li>
      </ul>
    </main>
  </div>
</template>

<script>
import AppointmentSlot from '@/components/AppointmentSlot';
import Spinner from '@/components/Spinner';
import { mapGetters } from 'vuex';

export default {
  components: {
    AppointmentSlot,
    Spinner,
  },
  computed: {
    ...mapGetters({
      slots: 'appointmentSlots/slots',
    }),
  },
  mounted() {
    this.$store.dispatch('appointmentSlots/load', this.$config);
  },
};
</script>

<style scoped="true">
  li {
    list-style: none;
    margin-bottom: 18px;
  }
</style>

<style lang="scss">
  @import '../style/html';
  @import '../style/textstyles';
  @import '../style/elements';
  @import '../style/fonts';
  @import '../style/colours';
  @import '../style/buttons';

  .content {
    margin-right: 18px;
  }
</style>
