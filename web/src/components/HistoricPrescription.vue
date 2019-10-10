<template>
  <div class="nhsuk-u-padding-top-0">
    <component :is="dateHeader"
               v-if="prescriptionCourse.orderDate != null"
               class="nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-0">
      <div>
        <p data-label="dateHeader" class="nhsuk-u-margin-bottom-0">
          <strong>{{ $t('rp02.orderDate') }}</strong>
        </p>
        <p class="nhsuk-body-l nhsuk-u-margin-bottom-0" data-label="order-date">
          <strong>{{ prescriptionCourse.orderDate | longDate }}</strong>
        </p>
      </div>
    </component>

    <div v-if="prescriptionCourse.status != null &&
      prescriptionCourse.orderDate != null">
      <p :class="getStatusStyle()">
        <strong>
          <component :is="getIcon()"
                     data-label="status-icon"
                     :data-status="getStatusText()"
                     focusable="false"/>
          <span data-label="status-description">
            {{ getStatusDescription() }}
          </span>
        </strong>
      </p>
    </div>

    <hr v-if="prescriptionCourse.orderDate != null"
        class="nhsuk-u-margin-bottom-3 nhsuk-u-margin-top-2"
        aria-hidden="true">
    <p class="nhsuk-u-margin-bottom-0">
      <b data-label="course-name">{{ prescriptionCourse.name }}</b>
    </p>
    <p data-label="detail" class="nhsuk-u-margin-bottom-0">
      {{ prescriptionCourse.details }}
    </p>
  </div>
</template>

<script>
import MedicationCourseStatus from '@/lib/medication-course-status';
import ClockIcon from '@/components/icons/ClockIcon';
import ReadyIcon from '@/components/icons/ReadyIcon';
import InfoIcon from '@/components/icons/InfoIcon';

export default {
  name: 'HistoricPrescription',
  components: {
    ClockIcon,
    ReadyIcon,
    InfoIcon,
  },
  props: {
    prescriptionCourse: {
      type: Object,
      required: true,
    },
    dateHeader: {
      type: String,
      default: 'h2',
      validator: value => ['h2', 'h3'].indexOf(value) !== -1,
    },
  },
  created() {
    this.statusStyling = {
      [MedicationCourseStatus.Rejected]: {
        style: 'rejected',
        text: this.$t('rp02.statusRejected.subHeader'),
        description: this.$t('rp02.statusRejected.description'),
        icon: 'InfoIcon',
      },
      [MedicationCourseStatus.Requested]: {
        style: 'requested',
        text: this.$t('rp02.statusRequested.subHeader'),
        description: this.$t('rp02.statusRequested.description'),
        icon: 'ClockIcon',
      },
      [MedicationCourseStatus.Approved]: {
        style: 'issued',
        text: this.$t('rp02.statusApproved.subHeader'),
        description: this.$t('rp02.statusApproved.description'),
        icon: 'ReadyIcon',
      },
    };
  },
  methods: {
    getStatusStyle() {
      return `${this.$style[this.statusStyling[this.prescriptionCourse.status].style]} nhsuk-u-margin-bottom-3`;
    },
    getStatusText() {
      return this.statusStyling[this.prescriptionCourse.status].text;
    },
    getStatusDescription() {
      return this.statusStyling[this.prescriptionCourse.status].description;
    },
    getIcon() {
      return this.statusStyling[this.prescriptionCourse.status].icon;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import "../style/colours";

.requested {
 strong {
  color: $awaiting !important;
 }
}

.issued {
 strong {
  color: $approved !important;
 }
}

.rejected {
 strong {
  color: $error !important;
 }
}
</style>
