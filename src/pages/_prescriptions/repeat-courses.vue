<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <form @submit.prevent="validate">
        <div v-if="showRepeatCourses">
          <div :class="$style.panel">
            <h2 :class="$style.panelHeader">
              {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.header') }}
            </h2>
            <hr>
            <p v-if="error" style="color:#DA291C; font-weight: 700; margin-bottom: 16px;">
              <inline-error-icon />
              {{ $t('prescriptions.repeatCourses.noMedicinesSelected') }}
            </p>
            <repeat-prescription
              v-for="repeatPrescription in repeatPrescriptionCourses"
              :key="repeatPrescription.id"
              :selected="repeatPrescription.selected"
              :prescription-details="repeatPrescription" />
          </div>
          <p>
            {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.contactGp') }}
          </p>
          <br>
          <button id="btn_order_prescription" class="button green">
            {{ $t('prescriptions.repeatCourses.continue') }}
          </button>
        </div>

        <div v-if="showNoRepeatCourses">
          <p>
            <b>{{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.title') }}</b>
          </p>
          <br>
          <p>
            {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.contactGp') }}
          </p>
          <br>
        </div>

      </form>
      <nuxt-link
        v-if="hasLoaded"
        to="/prescriptions"
        tag="button"
        type="submit"
        class="button grey">
        {{ $t('prescriptions.repeatCourses.backToYourPrescriptionsButton') }}
      </nuxt-link>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Spinner from '@/components/Spinner';
import RepeatPrescription from '@/components/RepeatPrescription';
import InlineErrorIcon from '../../components/icons/InlineErrorIcon';

export default {
  components: {
    InlineErrorIcon,
    Spinner,
    RepeatPrescription,
  },
  middleware: ['auth', 'meta'],
  computed: {
    error() {
      const { isValid, validated } = this.$store.state.repeatPrescriptionCourses;
      return validated && !isValid;
    },
    repeatPrescriptionCourses() {
      const { repeatPrescriptionCourses } = this.$store.state.repeatPrescriptionCourses;
      if (typeof repeatPrescriptionCourses === 'undefined' || !repeatPrescriptionCourses || repeatPrescriptionCourses.length === 0) {
        return null;
      }
      return repeatPrescriptionCourses;
    },
    showNoRepeatCourses() {
      const { repeatPrescriptionCourses, hasLoaded } = this.$store.state.repeatPrescriptionCourses;
      return hasLoaded && repeatPrescriptionCourses.length === 0;
    },
    showRepeatCourses() {
      const { repeatPrescriptionCourses, hasLoaded } = this.$store.state.repeatPrescriptionCourses;
      return hasLoaded && repeatPrescriptionCourses.length > 0;
    },
    hasLoaded() {
      return this.$store.state.repeatPrescriptionCourses.hasLoaded;
    },
  },
  mounted() {
    if (!this.$store.state.repeatPrescriptionCourses.hasLoaded) {
      this.$store.dispatch('repeatPrescriptionCourses/load');
    }
  },
  methods: {
    validate() {
      const selectedCourses = [];
      this.$store.state.repeatPrescriptionCourses.repeatPrescriptionCourses.forEach((course) => {
        if (course.selected) {
          selectedCourses.push(course);
        }
      });
      if (selectedCourses.length !== 0) {
        this.$router.push('../prescriptions/confirm-prescription-details');
      } else {
        const validationObj = {
          isValid: selectedCourses.length !== 0,
          submitted: true,
        };
        this.$store.dispatch('repeatPrescriptionCourses/validate', validationObj);
      }
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";

  .panel {
    @include panel;
  }
  .panelHeader {
    @include panelHeader;
    font-family: $frutiger-bold!important;
  }

  .panelContent {
    display: table-cell;
    vertical-align: top;
  }

  .form {
    margin-bottom: 24px;
    padding: 0;
    display: block;
  }

  .formLabel {
    display: block;
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
    color: #4A4A4A;
    font-family: $frutiger-bold;
  }

  .textArea {
    @include text-area;
  }
</style>
