<template>
  <div v-if="showTemplate && !gpSessionApiError">
    <div class="nhsuk-grid-row nhsuk-u-padding-bottom-6">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="error" message-type="error" role="alert">
          <message-text>
            {{ $t('prescriptions.repeatCourses.errors.thereIsAProblem') }}
          </message-text>
          <message-list class="nhsuk-u-margin-bottom-3">
            <li v-if="!courseSelectionValid">
              {{ $t('prescriptions.repeatCourses.errors.selectAtLeastOne') }}</li>
            <li v-if="!specialRequestValid">
              {{ $t('prescriptions.repeatCourses.errors.enterSpecialRequests') }}</li>
          </message-list>
        </message-dialog>

        <div v-if="showRepeatCourses">
          <p class="nhsuk-u-padding-bottom-3">
            {{ $t('prescriptions.repeatCourses.currentlyAvailableRepeatPrescriptions') }}</p>
          <no-js-form :action="repeatCoursesPath" :value="{}" method="post">
            <div>
              <div :class="selectMedicationErrorStyle">
                <error-message v-if="error && !courseSelectionValid" id="error-type">
                  {{ $t('prescriptions.repeatCourses.errors.selectAtLeastOne') }}
                </error-message>
                <CardGroup role="list" class="nhsuk-grid-row">
                  <CardGroupItem class="nhsuk-grid-column-full">
                    <Card>
                      <fieldset class="nhsuk-fieldset">
                        <legend class="nhsuk-fieldset__legend"
                                role="heading">
                          {{ $t('prescriptions.repeatCourses.currentlyAvailableToOrder') }}
                        </legend>
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
                  {{ $t('prescriptions.repeatCourses.changePharmacy') }}
                </p>
              </div>
              <label v-if="specialRequestNecessity === 'Optional'" for="specialRequest"
                     class="nhsuk-body-m">
                <strong>{{ $t('prescriptions.repeatCourses.specialRequestsOptional') }}</strong>
              </label>
              <label v-if="specialRequestNecessity === 'Mandatory'" for="specialRequest"
                     class="nhsuk-body-m">
                <strong>{{ $t('prescriptions.repeatCourses.specialRequestsMandatory') }} </strong>
              </label>
              <p id="disclaimer" class="nhsuk-body-m">
                {{ $t('prescriptions.repeatCourses.thisTextMayNotBeSeen') }}</p>
              <error-message v-if="showMandatoryReasonError" id="error-type">
                {{ $t('prescriptions.repeatCourses.errors.enterSpecialRequests') }}
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
                {{ $t('prescriptions.repeatCourses.specialRequestCharacterLimit') }}</p>
            </div>
            <generic-button id="btn_order_prescription"
                            :button-classes="['nhsuk-button']"
                            @click.prevent="validate">
              {{ $t('prescriptions.repeatCourses.continue') }}
            </generic-button>
          </no-js-form>
        </div>

        <div v-if="showNoRepeatCourses">
          <h3>{{ $t('prescriptions.repeatCourses.youDoNotHaveAny') }}</h3>
          <p>
            {{ $t('prescriptions.repeatCourses.ifYouHaveThatAreNotShown') }}
          </p>
        </div>
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="getBackPath"
                                :button-text="'generic.backButton.text'"
                                @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
  <div v-else-if="gpSessionApiError">
    <prescriptions-gp-session-error v-if="hasRetried"
                                    id="presciptionsGpSessionError"
                                    :code="gpSessionApiError.serviceDeskReference"/>
    <error-container v-else id="error-dialog-599">
      <error-title title="gpSessionErrors.prescriptions.tryAgainHeader"/>
      <error-paragraph from="gpSessionErrors.prescriptions.youAreNotCurrentlyAble"/>
      <error-paragraph from="gpSessionErrors.temporaryProblem"/>
      <error-button from="generic.tryAgainButton.text" @click="tryAgain" />
      <error-link from="generic.backButton.text"
                  :action="getBackPath"
                  :desktop-only="true"/>
    </error-container>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import RepeatPrescription from '@/components/RepeatPrescription';
import NoJsForm from '@/components/no-js/NoJsForm';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import PrescriptionsGpSessionError from '@/components/errors/gp-session-errors/PrescriptionsGpSessionError';

import {
  NOMINATED_PHARMACY_CHECK_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import { redirectTo, gpSessionErrorHasRetried, GP_SESSION_ERROR_STATUS } from '@/lib/utils';
import showShutterPage from '@/lib/proxy/shutter';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

const loadData = async (store) => {
  if (!store.state.repeatPrescriptionCourses.hasLoaded) {
    await store.dispatch('repeatPrescriptionCourses/load');
  }

  const { error } = store.state.repeatPrescriptionCourses;

  if (error && error.status === GP_SESSION_ERROR_STATUS && gpSessionErrorHasRetried(store)) {
    EventBus.$emit(UPDATE_HEADER, 'gpSessionErrors.prescriptions.header');
    EventBus.$emit(UPDATE_TITLE, 'gpSessionErrors.prescriptions.header');
  }
};

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
    PrescriptionsGpSessionError,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorParagraph,
    ErrorTitle,
  },
  data() {
    return {
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest || '',
      prescriptionChoices: this.$store.state.repeatPrescriptionCourses,
      selected: this.$store.getters['repeatPrescriptionCourses/selectedIds'],
    };
  },
  computed: {
    gpSessionApiError() {
      return this.$store.state.repeatPrescriptionCourses.error;
    },
    hasRetried() {
      return gpSessionErrorHasRetried(this.$store);
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    error() {
      const { validated } = this.$store.state.repeatPrescriptionCourses;

      const errors = [];

      if (validated && !this.courseSelectionValid) {
        errors.push(this.$t('prescriptions.repeatCourses.errors.selectAtLeastOne'));
      }
      if (validated && !this.specialRequestValid) {
        errors.push(this.$t('prescriptions.repeatCourses.errors.enterSpecialRequests'));
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
  watch: {
    '$route.query.ts': function watchTimestamp() {
      loadData(this.$store, this.$t);
    },
    hasApiError(value) {
      if (value) {
        showShutterPage(this.$router.currentRoute, this);
      }
    },
  },
  async created() {
    if (this.$route.query.hr) {
      this.$store.dispatch('session/setRetry', true);
    }

    await loadData(this.$store, this.$t);
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
    tryAgain() {
      if (this.$store.state.device.isNativeApp) {
        sessionStorage.setItem('hasRetried', true);
      }
      this.$store.dispatch('session/setRetry', true);
      redirectTo(this, PRESCRIPTION_REPEAT_COURSES_PATH, { hr: true }, true);
    },
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>
