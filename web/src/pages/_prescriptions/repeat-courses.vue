<template>
  <div v-if="showTemplate" id="mainDiv">
    <spinner />
    <main :class="$style.content">
      <form @submit.prevent="validate">
        <div v-if="showRepeatCourses">
          <div :class="$style.panel">
            <h2 :class="$style.panelHeader">
              {{ $t('rp03.subHeader') }}
            </h2>
            <hr>
            <p v-if="error" style="color:#DA291C; font-weight: 700; margin-bottom: 16px;">
              <inline-error-icon />
              {{ $t('rp03.noMedicinesSelected') }}
            </p>
            <repeat-prescription
              v-for="repeatPrescription in repeatPrescriptionCourses"
              :key="repeatPrescription.id"
              :selected="repeatPrescription.selected"
              :prescription-details="repeatPrescription" />
          </div>
          <div role="form">
            <label :class="$style.formLabel" for="specialRequest">
              {{ $t('rp03.specialRequestsLabel') }}
            </label>
            <textarea
              id="specialRequest"
              ref="specialRequest"
              :class="$style.textArea"
              v-model="specialRequest"
              maxlength="1000"/>
            <p id="maxSpecialRequest">{{ $t('rp03.maxSpecialRequest') }}</p>
          </div>
          <br>
          <p :class="$style.prescription_not_shown">
            {{ $t('rp06.empty.contactGp') }}
          </p>
          <br>
          <button id="btn_order_prescription" :class="[$style.button, $style.green]">
            {{ $t('rp03.continueButton') }}
          </button>
        </div>

        <div v-if="showNoRepeatCourses" :class="$style.info">
          <h3>{{ $t('rp06.empty.subHeader') }}</h3>
          <p>
            {{ $t('rp06.empty.contactGp') }}
          </p>
        </div>

      </form>
      <nuxt-link
        v-if="hasLoaded"
        :class="[$style.button, $style.grey]"
        to="/prescriptions"
        tag="button"
        type="submit">
        {{ $t('rp03.backButton') }}
      </nuxt-link>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Spinner from '@/components/widgets/Spinner';
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
        let specialRequest = null;
        if (this.specialRequest) {
          specialRequest = this.specialRequest.trim();
        }
        const repeatPrescriptionCoursesAdditionalInfo = {
          specialRequest,
        };
        this.$store.dispatch('repeatPrescriptionCourses/updateAdditionalInfo', repeatPrescriptionCoursesAdditionalInfo);
        this.$router.push('../prescriptions/confirm-prescription-details');
      } else {
        const validationObj = {
          isValid: selectedCourses.length > 0,
          submitted: true,
        };
        this.$store.dispatch('repeatPrescriptionCourses/validate', validationObj);
        window.scrollTo(0, 0);
      }
    },
  },
};
</script>


<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";
  @import "../../style/spacings";
  @import "../../style/textstyles";
  @import "../../style/colours";

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
