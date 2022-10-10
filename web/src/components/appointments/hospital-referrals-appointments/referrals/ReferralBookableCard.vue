<template>
  <Card>
    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <div class="nhs-app-card__title-text">
        <h3 class="nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0"
            :class="[headerStyle]">
          {{ $t('wayfinder.referrals.bookable.title') }}
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
    <primary-button data-purpose="book-or-manage-referral-button"
                    @click="goToUrlViaRedirector(deepLinkUrl)">
      {{ $t('wayfinder.referrals.bookable.bookOrManageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/appointments/hospital-referrals-appointments/RedirectorMixin';
import { isBlankString } from '@/lib/utils';

export default {
  name: 'ReferralBookableCard',
  components: {
    Card,
    PrimaryButton,
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
      specialty: this.item.serviceSpecialty,
      deepLinkUrl: this.item.deepLinkUrl,
    };
  },
  computed: {
    hasSpecialty() {
      return !isBlankString(this.specialty);
    },
    tagText() {
      return this.$t('wayfinder.referrals.bookable.tagText');
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
