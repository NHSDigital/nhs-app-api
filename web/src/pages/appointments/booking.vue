<template>
  <div v-if="showTemplate">
    <div v-if="error">
      <error-container v-if="error.status===403"
                       :id="generateErrorId()"
                       override-style="plain"
                       aria-live="polite">
        <error-title title="appointments.error.title.unavailable"/>
        <error-paragraph from="appointments.error.403.line1"/>
        <error-paragraph from="appointments.error.403.line2"/>
      </error-container>
      <error-container v-else-if="error.status===502 || error.status===500"
                       :id="generateErrorId()">
        <error-title title="appointments.error.title.problemLoading"/>
        <error-paragraph from="appointments.error.message.goBackAndTryContact"
                         :variable="error.serviceDeskReference"/>
        <error-paragraph from="appointments.error.message.ifItContinues"/>
        <error-link from="generic.contactUsButton.text"
                    :action="contactUsUrl"
                    target="_blank"/>
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true"/>
      </error-container>
      <error-container v-else-if="error.status===504" :id="generateErrorId()">
        <error-title title="appointments.error.title.problemLoading"/>
        <error-paragraph from="appointments.error.message.tryAgainNow"
                         :variable="error.serviceDeskReference"/>
        <error-paragraph from="appointments.error.message.ifItContinues"/>
        <error-button from="generic.tryAgainButton.text" @click="reload" />
        <error-link from="generic.contactUsButton.text"
                    :action="contactUsUrl"
                    target="_blank" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
    </div>
    <div v-else>
      <form :action="bookingPath">
        <input :name="noJsInputName" :value="JSON.stringify(noJsData)" type="hidden">
        <ul :class="$style['sr-only']" role="list"
            aria-live="polite" aria-relevant="additions" aria-atomic="false">
          <li v-for="(text, index) in availableAppointmentsScreenReaderMessage" :key="index">
            <span :aria-hidden="availableAppointmentsScreenReaderMessage.length !== (index + 1)">
              {{ text }}
            </span>
          </li>
        </ul>

        <div v-if="noAvailableAppointments" class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <message-dialog message-type="warning">
              <message-text :is-header="true">
                {{ $t('appointments.booking.noAppointmentsAvailable.title') }}
              </message-text>
              <message-text>
                {{ $t('appointments.booking.noAppointmentsAvailable.line1') }}
              </message-text>
              <message-text>
                {{ $t('appointments.booking.noAppointmentsAvailable.line2') }}
              </message-text>
            </message-dialog>
          </div>
        </div>

        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <filters
              v-if="availableAppointments"
              v-model="selectedOptions"
              :options="filtersOptions"
              :guidance-msg="bookingGuidanceMsg"/>
          </div>
        </div>

        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
            <noscript inline-template>
              <button class="nhsuk-button">
              {{ $t('appointments.booking.nojs.findButton') }}
              </button>
            </noscript>
          </div>
        </div>

        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <slot-list ref="slot_list" :available-slots="availableSlots" />
          </div>
        </div>
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <div ref="noMatching" tabindex="-1">
              <message-dialog v-if="showNoMatchingWarning"
                              :icon-text="$t('appointments.booking.adjustSearch.title')"
                              message-type="warning">
                <message-text>
                  {{ $t('appointments.booking.adjustSearch.line1') }}
                </message-text>
                <message-text>
                  {{ $t('appointments.booking.adjustSearch.line2') }}
                </message-text>
              </message-dialog>
            </div>
          </div>
        </div>
      </form>

      <div v-if="hasLoaded && !$store.state.device.isNativeApp" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <desktop-generic-back-link :path="appointmentsPath" @clickAndPrevent="goBack"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import qs from 'qs';
import VueScrollTo from 'vue-scrollto';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import Filters from '@/components/appointments/booking/Filters';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import SlotList from '@/components/appointments/booking/SlotList';
import { APPOINTMENT_BOOKING, GP_APPOINTMENTS } from '@/lib/routes';
import { get, isEmpty } from 'lodash/fp';
import { noJsParameterName } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';

const FILTER_PARAMETERS = [
  'clinician',
  'location',
  'time-period',
  'type',
];

const containsFilter = parameters => (!parameters
  ? false
  : Object.keys(parameters)
    .some(x => FILTER_PARAMETERS.includes(x)));

const load = async ({ $store }, query) => {
  $store.dispatch('availableAppointments/init');
  await $store.dispatch('availableAppointments/load');

  if (containsFilter(query)) {
    $store.dispatch('availableAppointments/setSelectedFilters', { ...query, date: query['time-period'] });
    $store.dispatch('availableAppointments/filter');
  }
};

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    ErrorContainer,
    ErrorLink,
    ErrorParagraph,
    ErrorTitle,
    Filters,
    MessageDialog,
    MessageText,
    SlotList,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      appointmentsPath: GP_APPOINTMENTS.path,
      availableAppointmentsScreenReaderMessage: [],
      bookingPath: APPOINTMENT_BOOKING.path,
      contactUsUrl: this.$env.CONTACT_US_URL,
      filtered: false,
    };
  },
  computed: {
    availableAppointments() {
      return this.hasLoaded && !isEmpty(this.slots);
    },
    availableSlots() {
      return get('state.availableAppointments.filteredSlots')(this.$store);
    },
    bookingGuidanceMsg() {
      return get('state.availableAppointments.bookingGuidance')(this.$store);
    },
    error() {
      return this.$store.state.availableAppointments.error;
    },
    filtersOptions() {
      return get('state.availableAppointments.filtersOptions')(this.$store);
    },
    hasLoaded() {
      return get('state.availableAppointments.hasLoaded')(this.$store);
    },
    noAvailableAppointments() {
      return this.hasLoaded && isEmpty(this.slots);
    },
    noJsData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    noJsInputName() {
      return noJsParameterName;
    },
    numberOfAvailableAppointments() {
      let count = 0;
      this.availableSlots.forEach((value) => {
        count += value[1].length;
      });
      return count;
    },
    selectedOptions: {
      get() {
        return get('state.availableAppointments.selectedOptions')(this.$store);
      },
      set(val) {
        this.filterSlots(val);
      },
    },
    showNoMatchingWarning() {
      return !!(this.filtered &&
             get('selectedOptions.location')(this) &&
             get('selectedOptions.type')(this) &&
             isEmpty(this.availableSlots));
    },
    slots() {
      return get('state.availableAppointments.slots')(this.$store);
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await load(this, this.$route.query);
      this.filtered = containsFilter(this.$route.query);
    },
  },
  async asyncData({ store, req }) {
    const query = req ? qs.parse(req.url.substr(req.url.indexOf('?') + 1)) : undefined;

    await load({ $store: store }, query);
    return { filtered: containsFilter(query) };
  },
  methods: {
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
    goBack() {
      redirectTo(this, this.appointmentsPath);
    },
    filterSlots(val) {
      this.$store.dispatch('availableAppointments/setSelectedFilters', val);
      this.$store.dispatch('availableAppointments/filter');
      this.filtered = true;

      if (this.showNoMatchingWarning) {
        VueScrollTo.scrollTo(this.$refs.noMatching, 500, { easing: VueScrollTo['ease-in'] });
        const errors = [];
        errors.push(this.$t('appointments.booking.adjustSearch.title'));
        this.$store.app.$analytics.logicError(errors);
      }

      const screenReaderMessage = this.$tc(
        'appointments.booking.availableAppointmentsScreenReaderMessage',
        this.numberOfAvailableAppointments,
        { appointmentsCount: this.numberOfAvailableAppointments },
      );
      this.trackDropDown(this.numberOfAvailableAppointments);
      this.availableAppointmentsScreenReaderMessage.push(screenReaderMessage);
    },
    trackDropDown(numberOfAvailableAppointments) {
      let trackEvent;
      if (numberOfAvailableAppointments === 0) {
        trackEvent = 'appointmentError';
      } else {
        trackEvent = 'appointmentSuccess';
      }

      const objectToTrack = {
        appointmentType: this.selectedOptions.type,
        location: this.selectedOptions.location,
        practiceMember: this.selectedOptions.clinician,
        filterByDate: this.selectedOptions.date,
      };

      this.$store.dispatch('analytics/satelliteTrack', trackEvent, objectToTrack);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/accessibility";

div:focus {
  outline: none !important;
}

</style>
