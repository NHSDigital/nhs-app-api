<template>
  <div class="nhsuk-u-padding-top-1"
       :class="[$style['no-chevron'], $style['list-menu-container_prescriptions']]">
    <div v-if="prescriptionCourse.orderDate != null"
         class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-2">
      <h3 class="nhs-app-card__title-text nhsuk-heading-xs nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0"
          data-label="order-date">
        {{ prescriptionCourse.orderDate | fullDate }}
      </h3>
      <span data-label="status-text"
            class="nhsuk-tag nhsuk-tag--no-border nhs-app-card__title-tag"
            :class="status.style"
            :aria-label="statusAriaLabel">
        {{ $t(status.text) }}
      </span>
    </div>

    <div v-if="prescriptionCourse.orderDate == null && prescriptionCourse.status == null">
      <h3 data-label="course-name" class="nhsuk-heading-xs nhsuk-u-margin-bottom-0">
        {{ prescriptionCourse.name }}
      </h3>
      <p data-label="detail" class="nhsuk-u-margin-bottom-0 nhsuk-body-s"
         :aria-label="prescriptionCourse.details">
        {{ prescriptionCourse.details }}
      </p>
    </div>

    <div v-else>
      <p class="nhsuk-u-margin-bottom-0 nhsuk-body-s"
         :class="[$style['nhs-app-prescription__medicine'],
                  $style['nhs-app-prescription__instructions']]">
        <span data-label="course-name" :aria-label="prescriptionCourse.name">
          {{ prescriptionCourse.name }}
        </span>
      </p>
      <p data-label="detail"
         class="nhsuk-body-s"
         :class="[$style['nhs-app-prescription__dosage'],
                  $style['nhs-app-prescription__instructions']]"
         :aria-label="prescriptionCourse.details">
        {{ prescriptionCourse.details }}
      </p>
    </div>

    <div v-if="prescriptionCourse.status != null && prescriptionCourse.orderDate != null"
         class="nhsuk-u-margin-top-1 nhsuk-u-margin-bottom-0">
      <p v-if="prescriptionCourse.orderedBy != null && prescriptionCourse.orderedBy != ''"
         data-purpose="ordered-by"
         class="nhsuk-u-margin-bottom-1 nhsuk-body-s"
         :class="$style['nhs-app-prescription__instructions']">
        {{ $t('prescriptions.historic.orderedBy') + prescriptionCourse.orderedBy }}
      </p>
      <p data-label="status-description"
         :class="$style['nhs-app-prescription__instructions']"
         class="nhsuk-u-margin-bottom-0 nhsuk-body-s">
        {{ $t(status.description) }}
      </p>
    </div>
  </div>
</template>

<script>
import MedicationCourseStatus from '@/lib/medication-course-status';

export default {
  name: 'HistoricPrescription',
  props: {
    prescriptionCourse: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      statuses: {
        [MedicationCourseStatus.Requested]: {
          style: 'nhsuk-tag--orange',
          text: 'prescriptions.historic.status.requested.header',
          description: 'prescriptions.historic.status.requested.description',
        },
        [MedicationCourseStatus.Approved]: {
          style: 'nhsuk-tag--green',
          text: 'prescriptions.historic.status.approved.header',
          description: 'prescriptions.historic.status.approved.description',
        },
        [MedicationCourseStatus.Rejected]: {
          style: 'nhsuk-tag--red',
          text: 'prescriptions.historic.status.rejected.header',
          description: 'prescriptions.historic.status.rejected.description',
        },
      },
    };
  },
  computed: {
    status() {
      return this.statuses[this.prescriptionCourse.status];
    },
    statusAriaLabel() {
      return `${this.$t('prescriptions.historic.theStatusIs')}${this.$t(this.status.text)}`;
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "@/style/custom/historic-prescription";
</style>
