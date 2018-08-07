<template>
  <div :class="$style.panel">
    <p v-if="prescriptionCourse.orderDate != null">
      <b>{{ $t('rp02.orderDate') }}:</b>
      <span data-label="order-date">
        {{ prescriptionCourse.orderDate | shortDate }}
      </span>
    </p>
    <hr v-if="prescriptionCourse.orderDate != null">
    <b data-label="course-name">{{ prescriptionCourse.name }}</b>
    <p data-label="detail">{{ prescriptionCourse.details }}</p>
    <div v-if="prescriptionCourse.status != null">
      <hr>
      <div :class="getStatusStyle()">
        <b>
          <component :is="getIcon()" />
          <span data-label="status">
            {{ getStatusText() }}
          </span>
        </b>
        <p data-label="status-description">{{ getStatusDescription() }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import
{ MedicationCourseStatus } from '@/lib/medication-course-status';
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

<style module lang="scss" scoped>
@import "../style/panels";
</style>
