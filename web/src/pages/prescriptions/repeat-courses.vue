<template>

  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">

    <glossary-header />

    <message-dialog v-if="error" message-type="error">
      <message-text>
        {{ $t('rp12.reasonMissing.summarySubHeader') }}
      </message-text>
      <message-list>
        <li>{{ $t('rp03.noMedicinesSelected') }}</li>
      </message-list>
    </message-dialog>

    <div v-if="showRepeatCourses" :class="$style.info">
      <p>{{ $t('rp03.subHeader') }}</p>
    </div>

    <div v-if="showRepeatCourses">
      <div :class="$style.panel">
        <div :class="{
          [$style['validation-inline']]: error,
          [$style['validation-border-left']]: error}">
          <error-message v-if="error" id="error-type">
            {{ $t('rp03.noMedicinesSelected') }}
          </error-message>
          <repeat-prescription
            v-for="repeatPrescription in repeatPrescriptionCourses"
            :key="repeatPrescription.id"
            :selected="repeatPrescription.selected"
            :prescription-details="repeatPrescription" />
        </div>
      </div>
      <div :class="$style.form" role="form">
        <label for="specialRequest">
          {{ $t('rp03.specialRequestsLabel') }}
        </label>
        <generic-text-area id="specialRequest"
                           v-model="specialRequest"
                           :initial-contents="specialRequest"
                           text-area-ref="specialRequest"
                           maxlength="1000"/>
        <p id="maxSpecialRequest" class="char">{{ $t('rp03.maxSpecialRequest') }}</p>
        <p id="disclaimer">{{ $t('rp03.disclaimer') }}</p>
      </div>
      <div :class="$style['info']">
        <p>
          {{ $t('rp03.changePharmacyText') }}
        </p>
      </div>
      <generic-button id="btn_order_prescription" :class="[$style.button, $style.green]"
                      @click.prevent="validate">
        {{ $t('rp03.continueButton') }}
      </generic-button>
    </div>

    <div v-if="showNoRepeatCourses" :class="$style.info">
      <h3>{{ $t('rp06.empty.subHeader') }}</h3>
      <p>
        {{ $t('rp06.empty.body') }}
      </p>
    </div>
    <generic-button v-if="hasLoaded"
                    id="back-to-prescriptions"
                    :class="[$style.button, $style.grey]"
                    @click="$router.push('/prescriptions')">
      {{ $t('rp03.backButton') }}
    </generic-button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import Spinner from '@/components/widgets/Spinner';
import RepeatPrescription from '@/components/RepeatPrescription';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GlossaryHeader from '@/components/GlossaryHeader';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';

export default {
  components: {
    GenericButton,
    Spinner,
    RepeatPrescription,
    MessageDialog,
    MessageText,
    MessageList,
    ErrorMessage,
    GlossaryHeader,
    GenericTextArea,
  },
  data() {
    return {
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest,
    };
  },
  computed: {
    error() {
      const { isValid, validated } = this.$store.state.repeatPrescriptionCourses;
      if (validated && !isValid) {
        const errors = [];

        errors.push(this.$t('rp03.noMedicinesSelected'));

        this.$store.app.$analytics.validationError(errors);
      }
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
        this.$router.push('/prescriptions/confirm-prescription-details');
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

<style module lang="scss" scoped>
  @import "../../style/forms";
  @import "../../style/panels";
  @import "../../style/info";
  @import "../../style/errorvalidation";
  @import "../../style/buttons";
</style>
