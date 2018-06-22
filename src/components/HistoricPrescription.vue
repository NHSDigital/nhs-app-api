<template>
  <div :class="$style.container">
    <div>
      <b>
        {{ $t('rp02.orderDate') }}
      </b>
      : <span aria-label="order-date">{{ prescriptionCourse.orderDate | shortDate }}</span>
    </div>
    <hr>
    <b aria-label="course-name">{{ prescriptionCourse.name }}</b>
    <div
      aria-label="dosage">
      {{ prescriptionCourse.details }}
    </div>
    <div :class="getStatusStyle()">
      <hr>
      <component :is="getIcon()"/>
      <b :class="$style.statusText">{{ getStatusText() }}</b>
      <div>
        <p area-label="status">{{ getStatusDescription() }}</p>
      </div>
    </div>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { MedicationCourseStatus } from '@/lib/medication-course-status';
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
  },
  created() {
    this.statusStyling = {
      [MedicationCourseStatus.Rejected]: {
        style: 'medication-status-rejected',
        text: this.$t('prescriptions.prescriptionStatus.rejected.text'),
        description: this.$t('prescriptions.prescriptionStatus.rejected.description'),
        icon: 'InfoIcon',
      },
      [MedicationCourseStatus.Requested]: {
        style: 'medication-status-requested',
        text: this.$t('prescriptions.prescriptionStatus.requested.text'),
        description: this.$t('prescriptions.prescriptionStatus.requested.description'),
        icon: 'ClockIcon',
      },
      [MedicationCourseStatus.Approved]: {
        style: 'medication-status-approved',
        text: this.$t('prescriptions.prescriptionStatus.approved.text'),
        description: this.$t('prescriptions.prescriptionStatus.requested.description'),
        icon: 'ReadyIcon',
      },
    };
  },
  methods: {
    getStatusStyle() {
      return this.$style[this.statusStyling[this.prescriptionCourse.status].style];
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

<style module lang="scss">
  @import "../style/html";
  @import "../style/elements";
  @import "../style/buttons";
  @import "../style/fonts";
  @import "../style/spacings";
  @import "../style/colours";

  .container {
    border: solid 1px $mid_grey;
    border-radius: 5px;
    background: $white;
    @include space(padding, all, $three);
    transition: all ease 0.5s;
    hr {
      height: 1px;
      border: none;
      background-color: $dark_grey;
      opacity: 0.2;
      @include space(margin, top, $two);
      @include space(margin, bottom, $two);
    }
  }

  .medication-status-rejected {
     color: $medication_status_rejected;
  }

  .medication-status-requested {
    color: $medication_status_requested;
  }

  .medication-status-approved {
    color: $medication_status_approved;
  }

  .statusText {
    margin-left: 10px;
  }
</style>
