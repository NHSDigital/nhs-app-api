<template>
  <div v-if="showTemplate && !gpSessionApiError">
    <div class="nhsuk-grid-row nhsuk-u-padding-bottom-6">
      <div class="nhsuk-grid-column-full">
        <div role="alert" aria-atomic="true">
          <message-dialog v-if="error"
                          message-type="error"
                          :focusable="true">
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
        </div>

        <div v-if="showRepeatCourses" class="break">
          <h2 ref="repeatPrescriptions" class="nhsuk-u-padding-bottom-3">
            {{ $t('prescriptions.repeatCourses.currentlyAvailableToOrder') }}</h2>
          <div :class="selectMedicationErrorStyle">
            <error-message v-if="error && !courseSelectionValid" id="error-type">
              {{ $t('prescriptions.repeatCourses.errors.selectAtLeastOne') }}
            </error-message>
            <repeat-prescription v-model="selected"/>
          </div>
          <div v-if="specialRequestNecessity !== 'NotAllowed'"
               role="form"
               :class="mandatoryReasonErrorStyle">
            <div v-if="specialRequestNecessity === 'Optional'">
              <label class="nhsuk-body-m" for="specialRequest">
                <h2>
                  <strong>
                    {{ $t('prescriptions.repeatCourses.specialRequestsOptionalLabel') }}
                  </strong>
                </h2>
              </label>
              <p id="disclaimer"
                 class="nhsuk-body-m" >
                {{ $t('prescriptions.repeatCourses.specialRequestsOptionalMessage') }}</p>
            </div>
            <div v-else-if="specialRequestNecessity === 'Mandatory'">
              <label class="nhsuk-body-m" for="specialRequest">
                <h2>
                  <strong>
                    {{ $t('prescriptions.repeatCourses.specialRequestsMandatoryLabel') }}
                  </strong>
                </h2>
              </label>
              <p id="disclaimer"
                 class="nhsuk-body-m" >
                {{ $t('prescriptions.repeatCourses.specialRequestsMandatoryMessage') }}</p>
            </div>
            <error-message v-if="showMandatoryReasonError" id="error-type">
              {{ $t('prescriptions.repeatCourses.errors.enterSpecialRequests') }}
            </error-message>
            <generic-text-area id="specialRequest"
                               ref="specialRequest"
                               v-model="specialRequest"
                               :required="(specialRequestNecessity === 'Mandatory')"
                               :error.sync="showMandatoryReasonError"
                               :text-area-classes="['nhsuk-u-margin-bottom-0']"
                               text-area-ref="specialRequest"
                               :data-maxlength="`${specialRequestCharacterLimit}`"
                               aria-describedby="specialRequestCharactersRemaining"
                               @focus.once="onFocusSpecialRequest"/>
            <p id="specialRequestCharactersRemaining"
               class="nhsuk-u-padding-bottom-4"
               :aria-live="specialRequestAriaLive">
              {{ remainingCharacters }}</p>
            <div v-if="stopExcessEntry"/>
          </div>
          <generic-button id="btn_order_prescription"
                          :button-classes="['nhsuk-button']"
                          @click.prevent="validate">
            {{ $t('prescriptions.repeatCourses.continue') }}
          </generic-button>
          <collapsible-details id="medicalAbbreviationCollapsible">
            <template slot="header">
              {{ $t('prescriptions.repeatCourses.helpWithMedicalAbbreviation.label') }}
            </template>
            <p>{{ $t('prescriptions.repeatCourses.helpWithMedicalAbbreviation.message') }}</p>
            <span>{{ $t('prescriptions.repeatCourses.helpWithMedicalAbbreviation.guidance') }}
              <a id="medicalAbbreviationHelpLink"
                 style="display: inline; vertical-align: baseline"
                 :href="medicalAbbreviationsPath"
                 target="_blank" rel="noopener noreferrer">
                {{ $t('prescriptions.repeatCourses.helpWithMedicalAbbreviation.linkText') }}</a>
            </span>
          </collapsible-details>
        </div>

        <div v-if="showNoRepeatCourses">
          <h3>{{ $t('prescriptions.repeatCourses.youDoNotHaveAny') }}</h3>
          <p>{{ $t('prescriptions.repeatCourses.ifYouHaveThatAreNotShown') }}</p>
        </div>
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="getBackPath"
                                :button-text="'generic.back'"
                                @clickAndPrevent="backButtonClicked"/>
      </div>
    </div>
  </div>
  <div v-else-if="gpSessionApiError">
    <prescription-errors :error="gpSessionApiError"
                         :reference-code="gpSessionApiError.serviceDeskReference"
                         :try-again-route="tryAgainPath"/>
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
import PrescriptionErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import CollapsibleDetails from '@/components/widgets/collapsible/CollapsibleDetails';

import {
  NOMINATED_PHARMACY_CHECK_PATH,
  PRESCRIPTIONS_PATH,
  PRESCRIPTION_CONFIRM_COURSES_PATH,
  PRESCRIPTION_REPEAT_COURSES_PATH,
} from '@/router/paths';
import { CLINICAL_ABBREVIATIONS_URL } from '@/router/externalLinks';
import { EventBus, UPDATE_HEADER, UPDATE_TITLE, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import { redirectTo, gpSessionErrorHasRetried, GP_SESSION_ERROR_STATUS } from '@/lib/utils';
import showShutterPage from '@/lib/proxy/shutter';
import vueScrollTo from 'vue-scrollto';

const loadData = async (store) => {
  if (!store.state.repeatPrescriptionCourses.hasLoaded) {
    await store.dispatch('repeatPrescriptionCourses/load');
  }

  const { error } = store.state.repeatPrescriptionCourses;

  if (error && error.status === GP_SESSION_ERROR_STATUS && gpSessionErrorHasRetried(store)) {
    EventBus.$emit(UPDATE_HEADER, 'gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions');
    EventBus.$emit(UPDATE_TITLE, 'gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions');
  }
};

export default {
  name: 'RepeatCoursesPage',
  components: {
    RepeatPrescription,
    MessageDialog,
    MessageText,
    MessageList,
    ErrorMessage,
    GenericButton,
    GenericTextArea,
    DesktopGenericBackLink,
    PrescriptionErrors,
    CollapsibleDetails,
  },
  data() {
    return {
      specialRequestAriaLive: '',
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest || '',
      selected: this.$store.getters['repeatPrescriptionCourses/selectedIds'],
      tryAgainPath: PRESCRIPTION_REPEAT_COURSES_PATH,
      medicalAbbreviationsPath: CLINICAL_ABBREVIATIONS_URL,
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
    specialRequestCharacterLimit() {
      return this.$store.state.repeatPrescriptionCourses.specialRequestCharacterLimit;
    },
    remainingCharacters() {
      const requestLength = this.specialRequest.length;
      const remaining = this.specialRequestCharacterLimit - requestLength;

      return this.$tc(
        'prescriptions.repeatCourses.specialRequestCharacterLimit',
        remaining,
        { n: remaining },
      );
    },
    stopExcessEntry() {
      return this.specialRequest;
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
    stopExcessEntry() {
      if (this.specialRequest.length > this.specialRequestCharacterLimit) {
        this.specialRequest = this.specialRequest.slice(0, this.specialRequestCharacterLimit);
      }
    },
  },
  async created() {
    if (this.$route.query.hr) {
      this.$store.dispatch('session/setRetry', true);
    }
    await loadData(this.$store, this.$t);
  },
  mounted() {
    if (this.$route.hash) {
      const ref = this.$refs[this.$route.hash.substring(1)];
      if (ref) {
        const element = ref.$el || ref;
        vueScrollTo.scrollTo(element, 250, { easing: vueScrollTo['ease-in'] });
      }
    }
  },
  methods: {
    validate() {
      this.$store.dispatch('repeatPrescriptionCourses/initValidate');

      if (this.courseSelectionValid && this.specialRequestValid) {
        let specialRequest = null;
        if (this.specialRequest) {
          specialRequest = this.specialRequest.trim();
        }
        this.$store.dispatch('repeatPrescriptionCourses/updateAdditionalInfo', { specialRequest });
        redirectTo(this, this.confirmCoursesPath);
      } else {
        this.$nextTick(() => {
          const validationObj = {
            isValid: this.courseSelectionValid && this.specialRequestValid,
            submitted: true,
          };
          this.$store.dispatch('repeatPrescriptionCourses/validate', validationObj);
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
        });
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
    onFocusSpecialRequest() {
      this.specialRequestAriaLive = 'polite';
    },
  },
};
</script>

<style module scoped lang="scss">
  @import "@/style/custom/break";
</style>
