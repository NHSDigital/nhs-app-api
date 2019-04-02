<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <glossary-header v-if="!showNoRepeatCourses" />
    <message-dialog v-if="error" message-type="error">
      <message-text>
        {{ $t('rp12.reasonMissing.summarySubHeader') }}
      </message-text>
      <message-list v-if="!courseSelectionValid">
        <li>{{ $t('rp03.noMedicinesSelected') }}</li>
      </message-list>
      <message-list v-if="!specialRequestValid">
        <li>{{ $t('rp03.specialRequestRequired') }}</li>
      </message-list>
    </message-dialog>

    <div v-if="showRepeatCourses" :class="$style.info">
      <p>{{ $t('rp03.subHeader') }}</p>
    </div>

    <div v-if="showRepeatCourses">
      <no-js-form :action="confirmCoursesPath" :value="{}" method="post">
        <div :class="$style.panel">
          <div :class="{
            [$style['validation-inline']]: (error && !courseSelectionValid),
            [$style['validation-border-left']]: (error && !courseSelectionValid)}">
            <error-message v-if="error && !courseSelectionValid" id="error-type"
                           :class="$style['validatioin-text']">
              {{ $t('rp03.noMedicinesSelected') }}
            </error-message>
            <fieldset>
              <legend>{{ $t('rp03.subHeader') }}</legend>
              <repeat-prescription
                v-for="repeatPrescription in repeatPrescriptionCourses"
                :key="repeatPrescription.id"
                :selected="repeatPrescription.selected"
                :prescription-details="repeatPrescription" />
            </fieldset>
          </div>
        </div>
        <input :value="specialRequestNecessity" type="hidden" name="specialRequestNecessity">
        <div v-if="specialRequestNecessity !== 'NotAllowed'"
             :class="[$style.form, {
               [$style['validation-inline']]: (error && !specialRequestValid),
               [$style['validation-border-left']]: (error && !specialRequestValid)}]"
             role="form">
          <error-message v-if="error && !specialRequestValid" id="error-type"
                         :class="$style['validatioin-text']">
            {{ $t('rp03.specialRequestRequired') }}
          </error-message>
          <label v-if="specialRequestNecessity === 'Optional'" for="specialRequest">
            {{ $t('rp03.specialRequestsLabelOptional') }}
          </label>
          <label v-if="specialRequestNecessity === 'Mandatory'" for="specialRequest">
            {{ $t('rp03.specialRequestsLabelMandatory') }}
          </label>
          <generic-text-area id="specialRequest"
                             v-model="specialRequest"
                             :required="(specialRequestNecessity === 'Mandatory')"
                             text-area-ref="specialRequest"
                             name="specialRequest"
                             maxlength="1000"/>
          <p id="maxSpecialRequest" class="char">{{ $t('rp03.maxSpecialRequest') }}</p>
          <p id="disclaimer">{{ $t('rp03.disclaimer') }}</p>
        </div>
        <div :class="$style['info']">
          <p>
            {{ $t('rp03.changePharmacyText') }}
          </p>
        </div>
        <generic-button id="btn_order_prescription"
                        :button-classes="['button' , 'green',
                                          !$store.state.device.isNativeApp && 'medium']"
                        @click.prevent="validate">
          {{ $t('rp03.continueButton') }}
        </generic-button>
      </no-js-form>
    </div>

    <div v-if="showNoRepeatCourses" :class="$style.info">
      <h3>{{ $t('rp06.empty.subHeader') }}</h3>
      <p>
        {{ $t('rp06.empty.body') }}
      </p>
    </div>
    <form v-if="$store.state.device.isNativeApp"
          :action="prescriptionsPath" method="get">
      <generic-button id="back-to-prescriptions"
                      :button-classes="['button' , 'grey']"
                      @click.stop.prevent="backToPrescriptionsClicked">
        {{ $t('rp03.backButton') }}
      </generic-button>
    </form>
    <desktopGenericBackLink v-else
                            :path="prescriptionsPath"
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backToPrescriptionsClicked"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RepeatPrescription from '@/components/RepeatPrescription';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import NoJsForm from '@/components/no-js/NoJsForm';
import GlossaryHeader from '@/components/GlossaryHeader';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import { PRESCRIPTIONS, PRESCRIPTION_CONFIRM_COURSES, NOMINATED_PHARMACY_CHECK } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    RepeatPrescription,
    MessageDialog,
    MessageText,
    MessageList,
    NoJsForm,
    ErrorMessage,
    GlossaryHeader,
    GenericTextArea,
    DesktopGenericBackLink,
  },
  data() {
    return {
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest ? this.$store.state.repeatPrescriptionCourses.specialRequest : '',
    };
  },
  computed: {
    error() {
      const { validated } = this.$store.state.repeatPrescriptionCourses;

      const errors = [];

      if (validated && !this.courseSelectionValid) {
        errors.push(this.$t('rp03.noMedicinesSelected'));
        return true;
      }
      if (validated && !this.specialRequestValid) {
        errors.push(this.$t('rp03.noMedicinesSelected'));
        return true;
      }

      if (validated && errors.length > 0) {
        this.$store.app.$analytics.validationError(errors);
      }

      return false;
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
    specialRequestNecessity() {
      return this.$store.state.repeatPrescriptionCourses
        .specialRequestNecessity;
    },
    courseSelectionValid() {
      const selectedCourses = [];
      this.$store.state.repeatPrescriptionCourses.repeatPrescriptionCourses.forEach((course) => {
        if (course.selected) {
          selectedCourses.push(course);
        }
      });

      return selectedCourses.length > 0;
    },
    specialRequestValid() {
      if (this.specialRequestNecessity === 'Mandatory') {
        if (!this.specialRequest || this.specialRequest === '') {
          return false;
        }
      }
      return true;
    },
    prescriptionsPath() {
      return PRESCRIPTIONS.path;
    },
    checkNominatedPharmacyPath() {
      return NOMINATED_PHARMACY_CHECK.path;
    },
    confirmCoursesPath() {
      return PRESCRIPTION_CONFIRM_COURSES.path;
    },
  },
  async asyncData({ store, route }) {
    const hasQueryError = route.query.noneSelected || route.query.missingSpecialRequest;

    if (!store.state.repeatPrescriptionCourses.hasLoaded) {
      await store.dispatch('repeatPrescriptionCourses/load');
    }
    if (store.state.repeatPrescriptionCourses.validated || hasQueryError) {
      await store.dispatch('repeatPrescriptionCourses/validate', {
        submitted: store.state.repeatPrescriptionCourses.validated ? false : hasQueryError,
      });
    }
  },
  methods: {
    validate() {
      if (this.courseSelectionValid && this.specialRequestValid) {
        let specialRequest = null;
        if (this.specialRequest) {
          specialRequest = this.specialRequest.trim();
        }
        const repeatPrescriptionCoursesAdditionalInfo = {
          specialRequest,
        };
        this.$store.dispatch('repeatPrescriptionCourses/updateAdditionalInfo', repeatPrescriptionCoursesAdditionalInfo);
        redirectTo(this, this.confirmCoursesPath, null);
      } else {
        const validationObj = {
          isValid: this.courseSelectionValid && this.specialRequestValid,
          submitted: true,
        };
        this.$store.dispatch('repeatPrescriptionCourses/validate', validationObj);
        window.scrollTo(0, 0);
      }
    },
    backToPrescriptionsClicked() {
      redirectTo(this, this.checkNominatedPharmacyPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/forms";
  @import "../../style/panels";
  @import "../../style/info";
  @import "../../style/errorvalidation";
  .pull-content {
    fieldset {
      border: none;
      legend {
        display: none;
      }
    }
    .validatioin-text {
      font-weight: normal !important;
      color: $error !important;
    }
    &.desktopWeb {
      font-family: $default-web;
      &>* {
        max-width: 540px;
      }
    }
    .form {
      label {
        margin-top: 1em;
      }
    }
  }
</style>
