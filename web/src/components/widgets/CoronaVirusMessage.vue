<template>
  <div>
    <contact-111
      :text="$t('appointments.coronavirus.forUrgentMedicalAdvice.text')"
      :aria-label="$t('appointments.coronavirus.forUrgentMedicalAdvice.label')"/>
    <p>
      <a :href="coronaConditionsUrl"
         data-purpose="coronavirus-link"
         target="_blank"
         rel="noopener noreferrer">
        {{ $t('appointments.coronavirus.findOutWhatToDo') }}
      </a>
    </p>
  </div>
</template>

<script>
import Contact111 from '@/components/widgets/Contact111';

export default {
  name: 'CoronaVirusMessage',
  components: {
    Contact111,
  },
  data() {
    return {
      coronaConditionsUrl: this.$store.$env.CORONA_CONDITIONS_URL,
    };
  },
};
</script>
