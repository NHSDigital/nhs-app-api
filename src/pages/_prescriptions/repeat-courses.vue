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
          <div role="form">
            <label :class="$style.formLabel" for="specialRequest">
              {{ $t('prescriptions.repeatCourses.specialRequestLabel') }}
            </label>
            <textarea
              id="specialRequest"
              ref="specialRequest"
              :class="$style.textArea"
              v-model="specialRequest"
              maxlength="1000"/>
            <p id="maxSpecialRequest">{{ $t('prescriptions.repeatCourses.maxSpecialRequest') }}</p>
          </div>
          <br/>
          <p :class="$style.prescription_not_shown">
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
  data() {
    return {
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest,
    };
  },
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
      if (selectedCourses.length > 0) {
        const repeatPrescriptionCoursesAdditionalInfo = {
          specialRequest: this.specialRequest,
        };
        this.$store.dispatch('repeatPrescriptionCourses/updateAdditionalInfo', repeatPrescriptionCoursesAdditionalInfo);
        this.$router.push('../prescriptions/confirm-prescription-details');
      } else {
        const validationObj = {
          isValid: selectedCourses.length > 0,
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
  @import "../../style/spacings";

  .formLabel {
    @include default_label;
    padding-top: 16px;
    padding-bottom: 8px;
  }

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

  .textArea {
    @include text-area;
  }

  .prescription_not_shown {
    margin-top: 10px;
  }
</style>
