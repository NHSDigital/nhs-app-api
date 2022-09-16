<template>
  <Card class="nhsuk-u-margin-bottom-5">

    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <div class="nhs-app-card__title-text">
        <h3 class="nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0"
            :class="[headerStyle]">
          {{ $t('wayfinder.referrals.inReview.title') }}
        </h3>
        <h4 v-if="hasSpecialty"
            data-purpose="specialty"
            class="nhsuk-body-l nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0 nhsuk-u-margin-bottom-0">
          <strong>{{ specialty }}</strong>
        </h4>
      </div>
      <span data-label="status-text"
            class="nhsuk-tag nhs-app-card__title-tag"
            :class="[tagStyle]"
            :aria-label="tagAriaLabel">
        {{ tagText }}
      </span>
    </div>

    <p v-if="hasSpecialty"
       data-purpose="specialty-info"
       class="nhsuk-body-s nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.inReview.yourHealthcareProviderHasRequested', null, {specialty}) }}
    </p>
    <p v-else
       data-purpose="no-specialty-info"
       class="nhsuk-body-s nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.inReview.yourHealthcareProviderHasRequestedNoSpecialty') }}
    </p>

    <p v-if="hasReviewDueDate" class="nhsuk-body nhsuk-u-margin-bottom-3">
      <strong>
        <span data-purpose="review-due-date-header">
          {{ $t('wayfinder.referrals.reviewDate') }}
        </span>
      </strong>
      <br>
      <span data-purpose="review-due-date" >
        {{ reviewDueDate | longDate }}
      </span>
    </p>

    <p class="nhsuk-body nhsuk-u-margin-bottom-3">
      <strong>
        <a data-purpose="manage-referral-link"
           href="#"
           @click="goToUrlViaRedirector(deepLinkUrl)">
          {{ $t('wayfinder.referrals.manageThisReferral') }}
        </a>
      </strong>
    </p>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { isBlankString } from '@/lib/utils';

export default {
  name: 'ReferralInReviewCard',
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
      reviewDueDate: this.item.reviewDueDate,
      specialty: this.item.serviceSpecialty,
      deepLinkUrl: this.item.deepLinkUrl,
    };
  },
  computed: {
    hasSpecialty() {
      return !isBlankString(this.specialty);
    },
    hasReviewDueDate() {
      return !isBlankString(this.reviewDueDate);
    },
    tagText() {
      return this.$t('wayfinder.referrals.inReview.tagText');
    },
    tagAriaLabel() {
      return `${this.$t('wayfinder.referrals.tagPromptText')} ${this.tagText}`;
    },
    headerStyle() {
      return this.hasSpecialty ? 'nhsuk-body-s' : '';
    },
    tagStyle() {
      return this.tagText === 'Action' ? 'nhsuk-tag--red' : 'nhsuk-tag--yellow';
    },
  },
};
</script>
