<template>
  <Card class="nhsuk-u-margin-bottom-5">
    <h3 class="nhsuk-u-margin-bottom-1">
      {{ $t('wayfinder.referrals.readyToBook.title') }}
    </h3>

    <template v-if="hasSpecialty">
      <p data-purpose="specialty"
         class="nhsuk-u-margin-bottom-3">
        <strong>{{ specialty }}</strong>
      </p>

      <p data-purpose="specialty-info"
         class="nhsuk-u-margin-bottom-3">
        {{ $t('wayfinder.referrals.readyToBook.youNeedToRebookYour', null, {specialty}) }}
      </p>
    </template>

    <p v-else
       data-purpose="no-specialty-info"
       class="nhsuk-u-margin-bottom-3">
      {{ $t('wayfinder.referrals.readyToBook.youNeedToRebookYourNoSpecialty') }}
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      <strong>
        <span data-purpose="referral-date-header">
          {{ $t('wayfinder.referrals.referredDate') }}
        </span>
      </strong>
      <br>
      <span data-purpose="referral-date">
        {{ referralDate | longDate }}
      </span>
    </p>

    <p class="nhsuk-u-margin-bottom-3">
      <strong>
        <span data-purpose="referrer-header">
          {{ $t('wayfinder.referrals.referredBy') }}
        </span>
      </strong>
      <br>
      <span data-purpose="referrer">
        {{ referrer }}
      </span>
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
      referrer: this.item.referrerOrganisation,
      referralDate: this.item.referredDateTime,
      specialty: this.item.serviceSpecialty,
      deepLinkUrl: this.item.deepLinkUrl,
    };
  },
  computed: {
    hasSpecialty() {
      return !isBlankString(this.specialty);
    },
  },
};
</script>
