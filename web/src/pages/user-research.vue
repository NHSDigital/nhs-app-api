<template>
  <div>
    <p>{{ $t('user_research.contactYou') }}</p>
    <collapsible-details>
      <template slot="header">
        {{ $t('user_research.whatIsInvolved.header') }}
      </template>
      <p>{{ $t('user_research.whatIsInvolved.addYou') }}</p>
      <p>{{ $t('user_research.whatIsInvolved.signUp.label') }}</p>
      <ul>
        <li v-for="(benefit, index) in $t('user_research.whatIsInvolved.signUp.benefits')"
            :key="index">
          {{ benefit }}
        </li>
      </ul>
      <p>{{ $t('user_research.whatIsInvolved.signUp.isOptional') }}</p>
    </collapsible-details>
    <p>
      {{ $t('user_research.whatIsInvolved.restriction.prefix')
      }}<a class="inline"
           :href="privacyPolicyUrl"
           target="_blank"
           rel="noopener noreferrer">{{
             $t('user_research.whatIsInvolved.restriction.linkText') }}</a>{{
        $t('user_research.whatIsInvolved.restriction.suffix') }}
    </p>
    <radio-group v-model="selectedValue"
                 :radios="choices"/>
    <primary-button @click.stop.prevent="onClick">
      {{ $t('user_research.continue') }}
    </primary-button>
  </div>
</template>

<script>
import CollapsibleDetails from '@/components/CollapsibleDetails';
import PrimaryButton from '@/components/PrimaryButton';
import RadioGroup from '@/components/RadioGroup';
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'termsAndConditions',
  components: {
    CollapsibleDetails,
    PrimaryButton,
    RadioGroup,
  },
  data() {
    return {
      choices: [
        { label: this.$t('user_research.question.yes'), value: true },
        { label: this.$t('user_research.question.no'), value: false },
      ],
      privacyPolicyUrl: this.$store.app.$env.PRIVACY_POLICY_URL,
      selectedValue: undefined,
    };
  },
  methods: {
    onClick() {
      redirectTo(this, INDEX.path);
    },
  },
};
</script>
