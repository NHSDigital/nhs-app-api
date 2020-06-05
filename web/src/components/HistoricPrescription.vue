<template>
  <div class="nhsuk-u-padding-top-1" :class="[$style['no-chevron'],
                                              $style['list-menu-container_prescriptions']]">
    <div v-if="prescriptionCourse.orderDate != null">
      <h3 class="nhsuk-heading-xs nhsuk-u-margin-bottom-2">
        <span data-label="order-date">{{ prescriptionCourse.orderDate | fullDate }}</span>
        <span class="nhsuk-u-visually-hidden"> {{ $t('rp02.statusHelpText') }}</span>
        <span id="status-text" data-label="status-text"
              :class="[$style['nhsuk-tag'], getStatusStyle(),
                       $style['nhsuk-tag--no-border'],
                       $style['nhs-app-message__date']]">
          {{ getStatusText() }}</span>
      </h3>

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
    <div v-else >
      <p class="nhsuk-u-margin-bottom-0 nhsuk-body-s"
         :class="[$style['nhs-app-prescription__medicine'],
                  $style['nhs-app-prescription__instructions']]">
        <span data-label="course-name" :aria-label="prescriptionCourse.name">
          {{ prescriptionCourse.name }}</span>
      </p>
      <p data-label="detail" class="nhsuk-body-s"
         :class="[$style['nhs-app-prescription__dosage'],
                  $style['nhs-app-prescription__instructions']]"
         :aria-label="prescriptionCourse.details">
        {{ prescriptionCourse.details }}
      </p>
    </div>

    <div v-if="prescriptionCourse.status != null &&
           prescriptionCourse.orderDate != null"
         class="nhsuk-u-margin-top-1 nhsuk-u-margin-bottom-0">
      <p v-if="prescriptionCourse.orderedBy != null && prescriptionCourse.orderedBy != ''"
         id="orderedByValue"
         class="nhsuk-u-margin-bottom-1 nhsuk-body-s"
         :class="$style['nhs-app-prescription__instructions']"
         :aria-label="ariaLabelCaption(
           'rp02.proxyOrderedByDisplayPrefix',
           prescriptionCourse.orderedBy)">
        {{ $t('rp02.proxyOrderedByDisplayPrefix') + prescriptionCourse.orderedBy }}
      </p>
      <p data-label="status-description" :class="$style['nhs-app-prescription__instructions']"
         class="nhsuk-u-margin-bottom-0 nhsuk-body-s" :aria-label="getStatusDescription">
        {{ getStatusDescription() }}
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
  created() {
    this.statusStyling = {
      [MedicationCourseStatus.Rejected]: {
        style: 'nhsuk-tag--red',
        text: this.$t('rp02.statusRejected.subHeader'),
        description: this.$t('rp02.statusRejected.description'),
      },
      [MedicationCourseStatus.Requested]: {
        style: 'nhsuk-tag--orange',
        text: this.$t('rp02.statusRequested.subHeader'),
        description: this.$t('rp02.statusRequested.description'),
      },
      [MedicationCourseStatus.Approved]: {
        style: 'nhsuk-tag--green',
        text: this.$t('rp02.statusApproved.subHeader'),
        description: this.$t('rp02.statusApproved.description'),
      },
    };
  },
  methods: {
    getStatusStyle() {
      return `${this.$style[this.statusStyling[this.prescriptionCourse.status].style]}`;
    },
    getStatusText() {
      return this.statusStyling[this.prescriptionCourse.status].text;
    },
    getStatusDescription() {
      return this.statusStyling[this.prescriptionCourse.status].description;
    },
    ariaLabelCaption(header, body) {
      return `${this.$t(header)}. ${(body)}`;
    },
  },
};

</script>

<style module lang="scss" scoped>
@import "../style/colours";
@import "../style/tags";
@import '~nhsuk-frontend/packages/core/settings/typography';
@import '~nhsuk-frontend/packages/core/tools/typography';
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';

.list-menu-container_prescriptions {
  padding: 0.8em 2em 0.8em 1em;
}

.no-chevron {
  background: $color_nhsuk-white;
  border-bottom: none !important;
  border-top: none !important;
}

.nhs-app-prescription__medicine {
  padding-top: 2px;
  padding-bottom: 0;
  font-weight: 600;
}

.nhs-app-prescription__dosage {
  margin-bottom: 12px;
}

@media (min-width: 40.0625em) {
  .nhs-app-prescription__instructions {
    font-size: 1rem;
    line-height: 1.5;
    margin-bottom: 24px;
  }
}

.nhs-app-prescription__instructions{
  line-height: 1.71429;
  display: block;
  margin-top: 0;
  margin-bottom: 12px;
  color: #425563;
}

.nhs-app-message__date {
  flex-shrink: 0;
  margin-left: nhsuk-spacing(2);
  font-size: 1em;
  float: right;
  @include nhsuk-typography-responsive(16);
  font-weight: 600;
}
</style>
