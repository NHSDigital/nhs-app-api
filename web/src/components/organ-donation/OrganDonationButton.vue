<template>
  <div>
    <button :class="[style, $style['decision-button']]" @click.prevent="chooseDecision()">
      <div>
        <component :is="icon" :class="$style['button-content', 'icon']" :title-id="headerId" />
        <h2 :id="headerId" class="nhsuk-heading-l" aria-hidden="true">{{ $t(headerKey) }}</h2>
        <p class="nhsuk-heading-m">{{ $t(subHeaderKey) }}</p>
      </div>
    </button>
  </div>
</template>

<script>
import { ORGAN_DONATION_ADDITIONAL_DETAILS, ORGAN_DONATION_YOUR_CHOICE } from '@/lib/routes';
import NoIcon from '@/components/icons/organ-donation/NoIcon';
import { DECISION_OPT_OUT } from '@/store/modules/organDonation/mutation-types';
import { redirectTo } from '@/lib/utils';
import YesIcon from '@/components/icons/organ-donation/YesIcon';

export default {
  name: 'OrganDonationButton',
  components: {
    NoIcon,
    YesIcon,
  },
  props: {
    decision: {
      type: String,
      required: true,
    },
  },
  data() {
    const isOptOut = this.decision === DECISION_OPT_OUT;

    return {
      nextRoute: isOptOut
        ? ORGAN_DONATION_ADDITIONAL_DETAILS.path
        : ORGAN_DONATION_YOUR_CHOICE.path,
      style: isOptOut ? this.$style['no-button'] : this.$style['yes-button'],
      headerKey: isOptOut ? 'organDonation.register.noButton.header' : 'organDonation.register.yesButton.header',
      subHeaderKey: isOptOut ? 'organDonation.register.noButton.subheader' : 'organDonation.register.yesButton.subheader',
      icon: isOptOut ? NoIcon : YesIcon,
      headerId: `${isOptOut ? 'no' : 'yes'}-header-id`,
    };
  },
  methods: {
    chooseDecision() {
      this.$store.dispatch('organDonation/makeDecision', this.decision);
      redirectTo(this, this.nextRoute);
    },
  },
};
</script>

<style module lang="scss">
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/ifff';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';

  .decision-button {
    @include nhsuk-responsive-padding(2, "top");
    @include nhsuk-responsive-padding(3, "right");
    @include nhsuk-responsive-padding(0, "bottom");
    @include nhsuk-responsive-padding(3, "left");
    background: $color_nhsuk-white;
    border: none;
    cursor: pointer;
    outline: none;
    width: 100%;

    div {
      height: 100%;
      vertical-align: top;
    }

    .icon {
      @include nhsuk-responsive-margin(4);
    }

    h2 {
      @include nhsuk-responsive-padding(1, "bottom");
      @include nhsuk-responsive-padding(0, "top");
    }
  }

  .no-button {
    h2 {
      color: $color_nhsuk-red;
    }
    p {
      color: $color_nhsuk-red;
    }
  }

  .yes-button {
    h2 {
      color: $color_nhsuk-green;
    }
    p {
      color: $color_nhsuk-green;
    }
  }

  .button-content {
    vertical-align: top;
  }
</style>
