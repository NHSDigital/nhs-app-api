<template>
  <div v-if="showTemplate && !hasApiError && !gpSessionApiError">
    <div class="nhsuk-grid-column-full">

      <div v-if="hasLoaded" class="break">
        <nhs-uk-radio-group
          id="radiogroup-repeatPrescriptionType"
          v-model="selectedValue"
          :error="showErrors"
          :error-text="inlineErrorMessage"
          :heading="$t('navigation.pages.headers.prescriptionType')"
          :items="prescriptionTypeChoices"
          :required="true"
          name="prescriptionType"
          @validate="onAnswerValidate"/>
        <generic-button
          id="continue-button"
          :button-classes="['nhsuk-button']"
          @click.prevent="continueClicked"
        >
          {{ $t("prescriptions.prescriptionType.continueButton") }}
        </generic-button>
      </div>
      <desktopGenericBackLink
        v-if="!$store.state.device.isNativeApp"
        id="back-link"
        :button-text="'generic.back'"
        :path="getBackPath"
        @clickAndPrevent="backButtonClicked"
      />
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
import GenericButton from '@/components/widgets/GenericButton';
import { GP_SESSION_ERROR_STATUS, gpSessionErrorHasRetried, redirectTo } from '@/lib/utils';
import { getNavigationPathFromPrescriptionType } from '@/lib/prescriptions/navigation';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import PrescriptionErrors from '@/components/errors/pages/prescriptions/PrescriptionsErrors';
import { PRESCRIPTION_TYPE_PATH, PRESCRIPTIONS_CONTACT_SURGERY_PATH, PRESCRIPTIONS_PATH } from '@/router/paths';
import showShutterPage from '@/lib/proxy/shutter';
import { EventBus, FOCUS_ERROR_ELEMENT, UPDATE_HEADER, UPDATE_TITLE } from '@/services/event-bus';
import vueScrollTo from 'vue-scrollto';
import sjrIf from '@/lib/sjrIf';
import InterruptBackTo from '@/lib/pharmacy-detail/interrupt-back-to';

const PRESCRIPTION_TYPE_REPEAT = 'PRESCRIPTION_TYPE_REPEAT';
const PRESCRIPTION_TYPE_NON_REPEAT = 'PRESCRIPTION_TYPE_NON_REPEAT';

const loadData = async (store) => {
  store.dispatch('repeatPrescriptionCourses/init');
  if (!store.state.repeatPrescriptionCourses.hasLoaded) {
    await store.dispatch('repeatPrescriptionCourses/load');
  }
  if (sjrIf({ $store: store, journey: 'nominatedPharmacy' })) {
    store.dispatch('nominatedPharmacy/clearInterruptBackTo');

    if (store.state.nominatedPharmacy.hasLoaded === false) {
      store.dispatch('nominatedPharmacy/clear');
      await store.dispatch('nominatedPharmacy/load');
    }
  }

  const { error } = store.state.repeatPrescriptionCourses;

  if (error && error.status === GP_SESSION_ERROR_STATUS && gpSessionErrorHasRetried()) {
    EventBus.$emit(UPDATE_HEADER, 'gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions');
    EventBus.$emit(UPDATE_TITLE, 'gpSessionErrors.prescriptions.youCanNotOrderOrViewPrescriptions');
  }
};
export default {
  name: 'PrescriptionTypePage',
  components: {
    NhsUkRadioGroup,
    GenericButton,
    DesktopGenericBackLink,
    PrescriptionErrors,
  },
  data() {
    const nominatedPharmacyEnabled = this.$store.getters['nominatedPharmacy/nominatedPharmacyEnabled'];

    return {
      nominatedPharmacyEnabled,
      hasTriedToContinue: false,
      inlineErrorMessage: this.$t(
        'prescriptions.prescriptionType.errors.chooseTypeOfPrescription',
      ),
      selectedValue: null,
      prescriptionTypeChoices: [
        {
          label: this.$t('prescriptions.prescriptionType.repeatPrescription'),
          value: PRESCRIPTION_TYPE_REPEAT,
          hint: {
            text: this.$t('prescriptions.prescriptionType.repeatPrescriptionHint'),
          },
          id: 'radioButton-repeatPrescription',
        },
        {
          label: this.$t(
            'prescriptions.prescriptionType.nonRepeatPrescription',
          ),
          value: PRESCRIPTION_TYPE_NON_REPEAT,
          hint: {
            text: this.$t('prescriptions.prescriptionType.nonRepeatPrescriptionHint'),
          },
          id: 'radioButton-nonRepeatPrescription',
        },
      ],
      tryAgainPath: PRESCRIPTION_TYPE_PATH,
    };
  },
  computed: {
    gpSessionApiError() {
      return this.$store.state.repeatPrescriptionCourses.error;
    },
    currentChoice() {
      return this.selectedValue;
    },
    showErrors() {
      return this.hasTriedToContinue && !this.selectedValue;
    },
    getBackPath() {
      return PRESCRIPTIONS_PATH;
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    hasRetried() {
      return gpSessionErrorHasRetried();
    },
    hasLoaded() {
      return this.$store.state.repeatPrescriptionCourses.hasLoaded;
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
    await loadData(this.$store, this.$t);
    this.$store.dispatch('flashMessage/show');
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
    onAnswerValidate(validation) {
      this.isValid = validation.isValid;
    },
    continueClicked() {
      this.hasTriedToContinue = false;

      this.$nextTick(() => {
        this.hasTriedToContinue = true;
        if (this.currentChoice !== undefined) {
          if (this.currentChoice === PRESCRIPTION_TYPE_REPEAT) {
            const redirectPath = getNavigationPathFromPrescriptionType(this.$store);
            this.$store.app.$analytics.trackButtonClick(
              redirectPath,
              true,
            );
            this.$store.dispatch('nominatedPharmacy/setInterruptBackTo', InterruptBackTo.NOMINATED_PHARMACY_CHECK);
            redirectTo(this, redirectPath);
          }
          if (this.currentChoice === PRESCRIPTION_TYPE_NON_REPEAT) {
            this.$store.app.$analytics.trackButtonClick(
              PRESCRIPTIONS_CONTACT_SURGERY_PATH,
              true,
            );
            redirectTo(this, PRESCRIPTIONS_CONTACT_SURGERY_PATH);
          }
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
        }
      });
    },
    selected(value) {
      this.selectedValue = value;
    },
    backButtonClicked() {
      redirectTo(this, this.getBackPath);
    },
  },
};
</script>

<style lang="scss" module scoped>
@import "../../style/info";
@import "../../style/buttons";
@import "../../style/spacings";
</style>

