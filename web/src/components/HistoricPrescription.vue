<template>
  <div>
    <component :is="dateHeader"
               v-if="prescriptionCourse.orderDate != null"
               :class="$style['date-header']">
      <div>
        <div :class="$style.dateHeaderTitle"
             data-label="dateHeader">{{ $t('rp02.orderDate') }}
        </div>
        <div :class="$style.date" data-label="order-date">
          {{ prescriptionCourse.orderDate | longDate }}
        </div>
      </div>

      <div v-if="prescriptionCourse.status != null">
        <div :class="getStatusStyle()">
          <b>
            <component :is="getIcon()"
                       data-label="status-icon"
                       :data-status="getStatusText()"
                       focusable="false"/>
            <span data-label="status-description">
              {{ getStatusDescription() }}
            </span>
          </b>
        </div>
      </div>
    </component>

    <hr v-if="prescriptionCourse.orderDate != null" aria-hidden="true">
    <b data-label="course-name">{{ prescriptionCourse.name }}</b>
    <p data-label="detail">{{ prescriptionCourse.details }}</p>
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
      default: 'h3',
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
@import "../style/colours";

.dateHeaderTitle {
 font-weight: bold;
 font-size: 18px;
}

.date {
 font-weight: normal;
 font-size: 25px;
 line-height: 1.5em;
 margin-bottom: 1em
}

 hr {
  margin-bottom: 0.5em
 }

.requested {
 b {
  color: $awaiting !important;
 }
}

.issued {
 b {
  color: $approved !important;
 }
}

.rejected {
 b {
  color: $error !important;
 }
}
</style>
