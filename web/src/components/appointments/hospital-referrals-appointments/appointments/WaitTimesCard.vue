<template>
  <Card>
    <!-- additional div needed here !-->
    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <div class="nhs-app-card__title-text">
        <h3 class="nhsuk-body-s nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0">
          {{ $t('wayfinder.waitTimes.card.title') }}
        </h3>
        <h4 data-purpose="specialty"
            class="nhsuk-body-l nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0">
          {{ speciality }}
        </h4>
      </div>
    </div>
    <p class="nhsuk-body-s nhsuk-u-margin-bottom-5" data-purpose="provider-name">
      {{ $t('wayfinder.waitTimes.card.subtitle',null,{providerName}) }}
    </p>
    <ol class="nhsuk-timeline__events nhsuk-u-margin-bottom-3">
      <li class="nhsuk-timeline__events--entry-wrapper">
        <div class="nhsuk-timeline__events--visual-wrapper" aria-hidden="true">
          <div class="nhsuk-timeline__events--visual-terminal">&nbsp;</div>
          <div class="nhsuk-timeline__events--visual-line">&nbsp;</div>
        </div>
        <div class="nhsuk-timeline__events--entry">
          <h2 class="nhsuk-heading-s nhsuk-u-margin-0 nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
            Date referred
          </h2>
          <p class="nhsuk-body-s nhsuk-u-margin-bottom-3" data-purpose="referred-date">
            {{ referredDate | longDate }}
          </p>
        </div>
      </li>
      <li class="nhsuk-timeline__events--entry-wrapper">
        <div class="nhsuk-timeline__events--visual-wrapper" aria-hidden="true">
          <div class="nhsuk-timeline__events--visual-terminal alternate">&nbsp;</div>
          <div class="nhsuk-timeline__events--visual-line alternate">&nbsp;</div>
        </div>
        <div class="nhsuk-timeline__events--entry">
          <h2 class="nhsuk-heading-s nhsuk-u-margin-0 nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
            {{ $t('wayfinder.waitTimes.card.youAreHere') }}
          </h2>
          <p class="nhsuk-body-s nhsuk-u-margin-bottom-3">
            {{ $t('wayfinder.waitTimes.card.waitingForConsultOrFollowUp') }}
          </p>
        </div>
      </li>
      <li class="nhsuk-timeline__events--entry-wrapper">
        <div class="nhsuk-timeline__events--visual-wrapper" aria-hidden="true">
          <div class="nhsuk-timeline__events--visual-terminal final">&nbsp;</div>
        </div>
        <div class="nhsuk-timeline__events--entry">
          <h2 class="nhsuk-heading-s nhsuk-u-margin-0 nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
            {{ $t('wayfinder.waitTimes.card.estimatedFinalTreatment') }}
          </h2>
          <p class="nhsuk-body-s nhsuk-u-margin-bottom-3" data-purpose="estimated-wait-date">
            {{ estimatedWaitToTreatmentDisplayDate }}
          </p>
          <p class="nhsuk-body-s nhsuk-u-margin-bottom-3">
            {{ $t('wayfinder.waitTimes.card.waitingTimesReviewed') }}
          </p>
        </div>
      </li>
    </ol>
    <p class="nhsuk-body-s nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.waitTimes.card.averageWaitingTime',null,{providerName}) }}
    </p>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';

export default {
  name: 'WaitTimesCard',
  components: {
    Card,
  },
  mixins: [RedirectorMixin],
  props: {
    item: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      estimatedWaitToTreatmentDisplayDate: this.item.estimatedWaitToTreatmentDisplayDate,
      plannedWaitTime: this.item.plannedWaitTime,
      providerName: this.item.providerName,
      referredDate: this.item.referredDate,
      speciality: this.item.speciality,
    };
  },
};
</script>
