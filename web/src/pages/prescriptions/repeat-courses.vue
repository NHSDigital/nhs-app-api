<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row nhsuk-u-padding-bottom-6">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="error" message-type="error" role="alert">
          <message-text>
            {{ $t('rp12.reasonMissing.summarySubHeader') }}
          </message-text>
          <message-list class="nhsuk-u-margin-bottom-3">
            <li v-if="!courseSelectionValid">{{ $t('rp03.noMedicinesSelected') }}</li>
            <li v-if="!specialRequestValid">{{ $t('rp03.specialRequestRequired') }}</li>
          </message-list>
        </message-dialog>

        <div v-if="showRepeatCourses">
          <p class="nhsuk-u-padding-bottom-3">{{ $t('rp03.medicationCourse.line1') }}</p>
          <no-js-form :action="repeatCoursesPath" :value="{}" method="post">
            <div>
              <div :class="selectMedicationErrorStyle">
                <error-message v-if="error && !courseSelectionValid" id="error-type">
                  {{ $t('rp03.noMedicinesSelected') }}
                </error-message>
                <CardGroup role="list" class="nhsuk-grid-row">
                  <CardGroupItem class="nhsuk-grid-column-full">
                    <Card>
                      <fieldset class="nhsuk-fieldset">
                        <legend class="nhsuk-fieldset__legend"
                                role="heading">{{ $t('rp03.subHeader') }}</legend>
                        <repeat-prescription v-model="selected"/>
                      </fieldset>
                    </Card>
                  </CardGroupItem>
                </CardGroup>
              </div>
            </div>
            <div v-if="specialRequestNecessity !== 'NotAllowed'"
                 role="form"
                 :class="mandatoryReasonErrorStyle">
              <div>
                <p>
                  {{ $t('rp03.changePharmacyText') }}
                </p>
              </div>
              <label v-if="specialRequestNecessity === 'Optional'" for="specialRequest"
                     class="nhsuk-body-m">
                <strong>{{ $t('rp03.specialRequestsLabelOptional') }}</strong>
              </label>
              <label v-if="specialRequestNecessity === 'Mandatory'" for="specialRequest"
                     class="nhsuk-body-m">
                <strong>{{ $t('rp03.specialRequestsLabelMandatory') }} </strong>
              </label>
              <p id="disclaimer" class="nhsuk-body-m">{{ $t('rp03.disclaimer') }}</p>
              <error-message v-if="showMandatoryReasonError" id="error-type">
                {{ $t('rp03.specialRequestRequired') }}
              </error-message>
              <generic-text-area id="specialRequest"
                                 v-model="specialRequest"
                                 :required="(specialRequestNecessity === 'Mandatory')"
                                 :error.sync="showMandatoryReasonError"
                                 :text-area-classes="['nhsuk-u-margin-bottom-0']"
                                 text-area-ref="specialRequest"
                                 name="nojs.repeatPrescriptionCourses.specialRequest"
                                 maxlength="1000"/>
              <p id="maxSpecialRequest" class="nhsuk-u-padding-bottom-4">
                {{ $t('rp03.maxSpecialRequest') }}</p>
            </div>
            <generic-button id="btn_order_prescription"
                            :button-classes="['nhsuk-button']"
                            @click.prevent="validate">
              {{ $t('rp03.continueButton') }}
            </generic-button>
          </no-js-form>
        </div>

        <div v-if="showNoRepeatCourses">
          <h3>{{ $t('rp06.empty.subHeader') }}</h3>
          <p>
            {{ $t('rp06.empty.body') }}
          </p>
        </div>
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="getBackPath"
                                :button-text="'rp03.backButton'"
                                @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RepeatPrescription from '@/components/RepeatPrescription';
import NoJsForm from '@/components/no-js/NoJsForm';
import {
  NOMINATED_PHARMACY_CHECK_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  name: 'RepeatCoursesPage',
  components: {
    RepeatPrescription,
    MessageDialog,
    MessageText,
    MessageList,
    NoJsForm,
    ErrorMessage,
    GenericButton,
    GenericTextArea,
    DesktopGenericBackLink,
    Card,
    CardGroupItem,
    CardGroup,
  },
  data() {
    return {
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest || '',
      prescriptionChoices: this.$store.state.repeatPrescriptionCourses,
      selected: this.$store.getters['repeatPrescriptionCourses/selectedIds'],
    };
  },
  computed: {
    error() {
      const { validated } = this.$store.state.repeatPrescriptionCourses;

      const errors = [];

      if (validated && !this.courseSelectionValid) {
        errors.push(this.$t('rp03.noMedicinesSelected'));
      }
      if (validated && !this.specialRequestValid) {
        errors.push(this.$t('rp03.noMedicinesSelected'));
      }

      if (validated && errors.length > 0) {
        this.$store.app.$analytics.validationError(errors);
        return true;
      }

      return false;
    },
    repeatPrescriptionCourses() {
      const { repeatPrescriptionCourses } = this.$store.state.repeatPrescriptionCourses;
      return (repeatPrescriptionCourses || []).length ? repeatPrescriptionCourses : null;
    },
    showNoRepeatCourses() {
      const { repeatPrescriptionCourses, hasLoaded } = this.$store.state.repeatPrescriptionCourses;
      return hasLoaded && repeatPrescriptionCourses.length === 0;
    },
    showRepeatCourses() {
      const { repeatPrescriptionCourses, hasLoaded } = this.$store.state.repeatPrescriptionCourses;
      return hasLoaded && repeatPrescriptionCourses.length > 0;
    },
    showMandatoryReasonError() {
      return this.error && !this.specialRequestValid;
    },
    hasLoaded() {
      return this.$store.state.repeatPrescriptionCourses.hasLoaded;
    },
    specialRequestNecessity() {
      return this.$store.state.repeatPrescriptionCourses
        .specialRequestNecessity;
    },
    courseSelectionValid() {
      return this.$store.getters['repeatPrescriptionCourses/isValid'];
    },
    specialRequestValid() {
      if (this.specialRequestNecessity === 'Mandatory') {
        if (!this.specialRequest || this.specialRequest.trim() === '') {
          return false;
        }
      }
      return true;
    },
    getBackPath() {
      return this.$store.state.nominatedPharmacy.nominatedPharmacyEnabled ?
        NOMINATED_PHARMACY_CHECK_PATH : PRESCRIPTIONS_PATH;
    },
    confirmCoursesPath() {
      return PRESCRIPTION_CONFIRM_COURSES_PATH;
    },
    repeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES_PATH;
    },
    mandatoryReasonErrorStyle() {
      if (this.specialRequestNecessity === 'Mandatory' &&
        this.error && !this.specialRequestValid) {
        return 'nhsuk-form-group--error';
      }
      return '';
    },
    selectMedicationErrorStyle() {
      return (this.error && !this.courseSelectionValid) ? 'nhsuk-form-group--error' : '';
    },
  },
  async mounted() {
    if (!this.$store.state.repeatPrescriptionCourses.hasLoaded) {
      await this.$store.dispatch('repeatPrescriptionCourses/load');
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
        redirectTo(this, this.confirmCoursesPath);
      } else {
        const validationObj = {
          isValid: this.courseSelectionValid && this.specialRequestValid,
          submitted: true,
        };
        this.$store.dispatch('repeatPrescriptionCourses/validate', validationObj);
        window.scrollTo(0, 0);
      }
    },
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
