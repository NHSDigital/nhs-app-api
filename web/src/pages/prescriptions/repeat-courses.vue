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
          <no-js-form :action="confirmCoursesPath" :value="{formData}" method="post">
            <div>
              <div>
                <error-message v-if="error && !courseSelectionValid" id="error-type">
                  {{ $t('rp03.noMedicinesSelected') }}
                </error-message>
                <fieldset class="nhsuk-fieldset">
                  <CardGroup role="list" class="nhsuk-grid-row">
                    <CardGroupItem class="nhsuk-grid-column-full">
                      <Card>
                        <legend class="nhsuk-fieldset__legend"
                                role="heading">{{ $t('rp03.subHeader') }}</legend>
                        <repeat-prescription v-model="selected"/>
                      </Card>
                    </CardGroupItem>
                  </CardGroup>
                </fieldset>
              </div>
            </div>
            <input :value="specialRequestNecessity" type="hidden" name="specialRequestNecessity">
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
                                 name="specialRequest"
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
        <form v-if="$store.state.device.isNativeApp"
              :action="getBackPath" method="get">
          <generic-button id="back-to-prescriptions"
                          :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                          @click.stop.prevent="backButtonClicked">
            {{ $t('rp03.backButton') }}
          </generic-button>
        </form>
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
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RepeatPrescription from '@/components/RepeatPrescription';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import NoJsForm from '@/components/no-js/NoJsForm';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import { PRESCRIPTIONS, PRESCRIPTION_CONFIRM_COURSES, NOMINATED_PHARMACY_CHECK } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
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
      selected: this.$store.state.repeatPrescriptionCourses.selected,
    };
  },
  computed: {
    formData() {
      return {
        repeatCourses: this.$store.state.repeatPrescriptionCourses,
        selectedCourses: this.$store.state.repeatPrescriptionCourses.selected,
      };
    },
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
      return this.$store.state.repeatPrescriptionCourses.repeatPrescriptionCourses
        .some(course => course.selected);
    },
    specialRequestValid() {
      if (this.specialRequestNecessity === 'Mandatory') {
        if (!this.specialRequest || this.specialRequest === '') {
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
    backButtonClicked() {
      redirectTo(this, this.getBackPath, null);
    },
  },
};
</script>
