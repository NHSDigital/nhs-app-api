<template>
  <Card class="nhsuk-u-margin-bottom-5">

    <div class="nhs-app-card__title nhsuk-u-padding-top-2 nhsuk-u-margin-bottom-3">
      <div class="nhs-app-card__title-text">
        <h3 class="nhsuk-u-margin-bottom-0 nhsuk-u-padding-bottom-0 nhsuk-u-padding-top-0"
            :class="[headerStyle]">
          {{ $t('wayfinder.referrals.readyToBook.title') }}
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
      {{ $t('wayfinder.referrals.readyToBook.youNeedToRebookYour', null, {specialty}) }}
    </p>
    <p v-else
       data-purpose="no-specialty-info"
       class="nhsuk-body-s nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.readyToBook.youNeedToRebookYourNoSpecialty') }}
    </p>

    <primary-button data-purpose="book-or-manage-button"
                    @click="goToUrlViaRedirector(deepLinkUrl)">
      {{ $t('wayfinder.referrals.readyToBook.bookOrManageThisReferral') }}
    </primary-button>
  </Card>
</template>

<script>
import Card from '@/components/widgets/card/Card';
import PrimaryButton from '@/components/PrimaryButton';
import RedirectorMixin from '@/components/wayfinder/RedirectorMixin';
import { isBlankString } from '@/lib/utils';

export default {
  name: 'ReferralReadyToRebookCard',
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
      return this.$t('wayfinder.referrals.readyToBook.tagText');
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
