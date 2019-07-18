<template>

  <div v-if="showTemplate">
    <div class="nhsuk-grid-row nhsuk-u-padding-bottom-6">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="error" message-type="error" role="alert">
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

        <div v-if="showRepeatCourses">
          <no-js-form :action="repeatCoursesPath" :value="{}" method="post">
            <div>
              <div>
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
            <input value="true" type="hidden" name="nojs.repeatPrescriptionCourses.submitted">
            <input :value="specialRequestNecessity"
                   type="hidden"
                   name="nojs.repeatPrescriptionCourses.specialRequestNecessity">
            <div v-if="specialRequestNecessity !== 'NotAllowed'"
                 role="form">
              <error-message v-if="error && !specialRequestValid" id="error-type">
                {{ $t('rp03.specialRequestRequired') }}
              </error-message>
              <label v-if="specialRequestNecessity === 'Optional'" for="specialRequest"
                     class="nhsuk-u-padding-bottom-2">
                {{ $t('rp03.specialRequestsLabelOptional') }}
              </label>
              <label v-if="specialRequestNecessity === 'Mandatory'" for="specialRequest"
                     class="nhsuk-u-padding-bottom-2">
                {{ $t('rp03.specialRequestsLabelMandatory') }} </label>
              <p id="maxSpecialRequest" class="char nhsuk-u-padding-bottom-2">
                {{ $t('rp03.maxSpecialRequest') }}
              </p>
              <p id="disclaimer" class="nhsuk-u-padding-bottom-2">{{ $t('rp03.disclaimer') }}</p>
              <div>
                <p class="nhsuk-body-s nhsuk-u-padding-bottom-3">
                  {{ $t('rp03.changePharmacyText') }}
                </p>
              </div>
              <generic-text-area id="specialRequest"
                                 v-model="specialRequest"
                                 :required="(specialRequestNecessity === 'Mandatory')"
                                 text-area-ref="specialRequest"
                                 name="nojs.repeatPrescriptionCourses.specialRequest"
                                 maxlength="1000"/>
            </div>
            <button id="btn_order_prescription"
                    class="nhsuk-button"
                    @click.prevent="validate">
              {{ $t('rp03.continueButton') }}
            </button>
          </no-js-form>
        </div>

        <div v-if="showNoRepeatCourses" class="nhsuk-u-padding-bottom-6">
          <h3>{{ $t('rp06.empty.subHeader') }}</h3>
          <p>
            {{ $t('rp06.empty.body') }}
          </p>
        </div>
        <desktopGenericBackLink v-else
                                :path="getBackPath"
                                :button-text="'rp03.backButton'"
                                @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RepeatPrescription from '@/components/RepeatPrescription';
import NoJsForm from '@/components/no-js/NoJsForm';
import {
  NOMINATED_PHARMACY_CHECK,
  PRESCRIPTIONS,
  PRESCRIPTION_CONFIRM_COURSES,
  PRESCRIPTION_REPEAT_COURSES,
} from '@/lib/routes';
import { isEmpty } from 'lodash/fp';
import { redirectTo } from '@/lib/utils';
import { ensureNoJsPostedValueIsArray } from '@/lib/noJs';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',
  components: {
    RepeatPrescription,
    MessageDialog,
    MessageText,
    MessageList,
    NoJsForm,
    ErrorMessage,
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
        if (process.client) {
          this.$store.app.$analytics.validationError(errors);
        }
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
        NOMINATED_PHARMACY_CHECK.path : PRESCRIPTIONS.path;
    },
    confirmCoursesPath() {
      return PRESCRIPTION_CONFIRM_COURSES.path;
    },
    repeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES.path;
    },
  },
  async fetch({ store }) {
    const storeData = store;
    if (!store.state.repeatPrescriptionCourses.hasLoaded) {
      await store.dispatch('repeatPrescriptionCourses/load');
    }

    if (store.state.repeatPrescriptionCourses.submitted) {
      if (isEmpty(store.state.repeatPrescriptionCourses.specialRequest)) {
        storeData.state.repeatPrescriptionCourses.specialRequest = null;
      }

      storeData.state.repeatPrescriptionCourses.submitted = false;
      const { selectedCoursesNoJs } = store.state.repeatPrescriptionCourses;

      if (selectedCoursesNoJs) {
        const courseSelection = ensureNoJsPostedValueIsArray(selectedCoursesNoJs).map(String);

        storeData.state.repeatPrescriptionCourses.repeatPrescriptionCourses.forEach((course) => {
          const courseToCheckIsSelected = course;
          courseSelection.forEach((selection) => {
            if (courseToCheckIsSelected.id === selection) {
              courseToCheckIsSelected.selected = true;
            }
          });
        });
      }

      const courseSelectionValid = store.getters['repeatPrescriptionCourses/isValid'];
      const specialRequestValid = store.getters['repeatPrescriptionCourses/specialRequestValid'];

      if (courseSelectionValid && specialRequestValid) {
        store.app.router.push(PRESCRIPTION_CONFIRM_COURSES.path);
      } else {
        const validationObj = {
          isValid: courseSelectionValid && specialRequestValid,
          submitted: true,
        };
        store.dispatch('repeatPrescriptionCourses/validate', validationObj);
      }
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
    backButtonClicked() {
      redirectTo(this, this.getBackPath, null);
    },
  },
};
</script>
