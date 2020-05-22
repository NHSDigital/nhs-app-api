<template>
  <div v-if="error">
    <error-container v-if="error.status===403"
                     :id="generateErrorId()"
                     message-type="error"
                     override-style="plain"
                     aria-live="off">
      <error-title title="appointments.error.title.unavailable" />
      <error-paragraph from="appointments.error.403.line1" />
      <error-paragraph from="appointments.error.403.line2" />
      <error-header from="appointments.error.403.coronaHeader" />
      <error-paragraph from="appointments.error.403.coronaText" />
      <error-link :class="$style['inline-link']"
                  from="appointments.error.403.coronaServiceLink"
                  :action="coronaServiceUrl"
                  data-purpose="corona-service"
                  target="_blank"/>
    </error-container>
    <error-container v-else-if="error.status===400" :id="generateErrorId()">
      <error-title title="appointments.error.title.problem"
                   header="appointments.error.header.problem" />
      <error-paragraph from="appointments.error.400.message" />
      <error-link from="generic.backButton.text"
                  :action="backToHomeUrl"
                  :desktop-only="true"/>
    </error-container>
    <error-container v-else-if="error.status===500 || error.status===502" :id="generateErrorId()">
      <error-title title="appointments.error.title.problemLoading"/>
      <error-paragraph from="appointments.error.message.tryAgainNow"
                       :variable="error.serviceDeskReference"/>
      <error-paragraph from="appointments.error.message.ifItContinues"/>
      <error-button from="generic.tryAgainButton.text" @click="reload" />
      <error-link from="generic.contactUsButton.text"
                  :action="contactUsUrl"
                  target="_blank"/>
      <error-link from="generic.backButton.text"
                  :action="backToHomeUrl"
                  :desktop-only="true"/>
    </error-container>
    <error-container v-else-if="error.status===504" :id="generateErrorId()">
      <error-title title="appointments.error.title.problemLoading"/>
      <error-paragraph from="appointments.error.message.tryAgainNow"
                       :variable="error.serviceDeskReference"/>
      <error-paragraph from="appointments.error.message.ifItContinuesBookOrCancel"/>
      <error-button from="generic.tryAgainButton.text" @click="reload" />
      <error-link from="generic.contactUsButton.text"
                  :action="contactUsUrl"
                  target="_blank"/>
    </error-container>
  </div>
  <div v-else-if="hasLoaded">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <corona-virus-message />
        <no-js-form :action="guidanceUrl" :value="formData">
          <generic-button id="book-appointments-button"
                          :button-classes="['nhsuk-button',
                                            'nhsuk-u-margin-bottom-3',
                                            'nhsuk-u-margin-top-3']"
                          @click.stop.prevent="onBookButtonClicked">
            {{ $t('appointments.guidance.bookButtonText') }}
          </generic-button>
        </no-js-form>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="showNoUpcomingAppointments"
             class="nhsuk-u-margin-bottom-3"
             data-purpose="upcoming-info">
          <h2 class="nhsuk-u-margin-bottom-0">{{ $t('appointments.index.empty.header') }}</h2>
          <p class="nhsuk-u-margin-top-0">{{ $t('appointments.index.empty.text1') }}</p>
        </div>
        <upcoming-appointments v-if="showUpcomingAppointments"
                               :appointments="upcomingAppointments"
                               :cancellation-disabled="cancellationDisabled"/>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div v-if="showNoPastAppointments" data-purpose="past-info">
          <h2 class="nhsuk-u-margin-bottom-0">{{ $t('appointments.index.emptyPast.header') }}</h2>
          <div>
            <p class="nhsuk-u-padding-bottom-3">
              {{ $t('appointments.index.emptyPast.text1') }}
            </p>
          </div>
        </div>
        <past-appointments v-if="showPastAppointments"
                           :appointments="pastAppointments"/>
      </div>
    </div>
  </div>
</template>

<script>
import isEmpty from 'lodash/fp/isEmpty';
import CoronaVirusMessage from '@/components/widgets/CoronaVirusMessage';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorHeader from '@/components/errors/ErrorHeader';
import ErrorTitle from '@/components/errors/ErrorTitle';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import PastAppointments from '@/components/appointments/PastAppointments';
import UpcomingAppointments from '@/components/appointments/UpcomingAppointments';
import showShutterPage from '@/lib/proxy/shutter';
import { APPOINTMENT_BOOKING_GUIDANCE, INDEX, findByName } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

const loadData = async (store) => {
  store.dispatch('myAppointments/clear');
  await store.dispatch('myAppointments/load');
};

export default {
  name: 'GPAppointments',
  layout: 'nhsuk-layout',
  components: {
    CoronaVirusMessage,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorHeader,
    ErrorParagraph,
    ErrorTitle,
    GenericButton,
    NoJsForm,
    PastAppointments,
    UpcomingAppointments,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      backToHomeUrl: INDEX.path,
      contactUsUrl: this.$env.CONTACT_US_URL,
      coronaServiceUrl: this.$env.CORONA_SERVICE_URL,
      guidanceUrl: APPOINTMENT_BOOKING_GUIDANCE.path,
    };
  },
  computed: {
    cancellationDisabled() {
      return this.$store.state.myAppointments.disableCancellation;
    },
    currentRoute() {
      return findByName(this.$route.name);
    },
    error() {
      return this.$store.state.myAppointments.error;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    hasLoaded() {
      return this.$store.state.myAppointments.hasLoaded;
    },
    pastAppointments() {
      return this.$store.state.myAppointments.pastAppointments;
    },
    showNoPastAppointments() {
      return this.hasLoaded &&
          this.$store.state.myAppointments.pastAppointmentsEnabled &&
          isEmpty(this.pastAppointments);
    },
    showNoUpcomingAppointments() {
      return this.hasLoaded && isEmpty(this.upcomingAppointments);
    },
    showPastAppointments() {
      return this.hasLoaded &&
          this.$store.state.myAppointments.pastAppointmentsEnabled &&
          !isEmpty(this.pastAppointments);
    },
    showUpcomingAppointments() {
      return this.hasLoaded && !isEmpty(this.upcomingAppointments);
    },
    upcomingAppointments() {
      return this.$store.state.myAppointments.upcomingAppointments;
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      loadData(this.$store);
    },
    error(value) {
      if (value) {
        showShutterPage(this.currentRoute, this);
      }
    },
    hasLoaded() {
      if (this.hasLoaded) {
        this.$store.dispatch('flashMessage/show');
      }
    },
  },
  async asyncData({ store }) {
    if (process.server) {
      await loadData(store);
    }
  },
  mounted() {
    loadData(this.$store);

    if (this.hasLoaded) {
      this.$store.dispatch('flashMessage/show');
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearAppointments');
  },
  methods: {
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
    onBookButtonClicked() {
      this.$store.app.$analytics.trackButtonClick(this.guidanceUrl, true);
      redirectTo(this, this.guidanceUrl);
    },
  },
};
</script>

<style module lang="scss" scoped>
.inline-link > a {
  display: inline-block;
}
</style>
