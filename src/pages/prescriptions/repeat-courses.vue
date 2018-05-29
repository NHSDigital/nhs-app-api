<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <div class="info">


        <div v-if="repeatPrescriptionCourses">

          <div class="panel">
            <div class="panel-content">
              <h5>{{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.header') }}</h5>
              <hr >

              <repeat-prescription
                v-for="repeatPrescription in repeatPrescriptionCourses"
                :key="repeatPrescription.id"
                :prescription-details="repeatPrescription" />

            </div>
          </div>

          <p>
            {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.contactGp') }}
          </p>
        </div>

        <div v-if="!repeatPrescriptionCourses">
          <p>
            <b>{{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.title') }}</b>
          </p>
        </div>

      </div>
      <nuxt-link to="/prescriptions" tag="button" class="button grey">
        {{ $t('prescriptions.backToYourPrescriptionsButton') }}
      </nuxt-link>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Spinner from '@/components/Spinner';
import RepeatPrescription from '@/components/RepeatPrescription';

export default {
  components: {
    Spinner,
    RepeatPrescription,
  },
  middleware: ['auth', 'meta'],
  computed: {
    repeatPrescriptionCourses() {
      const { response } = this.$store.state.repeatPrescriptionCourses;

      if (typeof response === 'undefined' || !response || !response.courses || response.courses.length === 0) {
        return null;
      }

      const { courses } = this.$store.state
        .repeatPrescriptionCourses.response;

      return courses;
    },
  },
  async fetch({ store }) {
    await store.dispatch('repeatPrescriptionCourses/load');
  },
};
</script>


<style lang="scss">
  @import "../../style/html";
  @import "../../style/elements";
  @import "../../style/buttons";
</style>
