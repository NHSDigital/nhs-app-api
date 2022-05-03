<template>
  <div :class="urgencyStyle">
    <div class="nhsuk-card--care__heading-container">
      <p class="nhsuk-card--care__heading">
        <span>
          <span class="nhsuk-u-visually-hidden">{{ headingPrefix }}</span>
          {{ heading }}
        </span>
      </p>
      <span class="nhsuk-card--care__arrow" aria-hidden="true"/>
    </div>
    <div class="nhsuk-card__content">
      <slot/>
    </div>
  </div>
</template>

<script>
const urgencyClasses = {
  nonUrgent: 'nhsuk-card nhsuk-card--care nhsuk-card--care--non-urgent',
  urgent: 'nhsuk-card nhsuk-card--care nhsuk-card--care--urgent',
  immediate: 'nhsuk-card nhsuk-card--care nhsuk-card--care--emergency',
};

export default {
  name: 'CareCard',
  props: {
    urgency: {
      type: String,
      default: 'nonUrgent',
      validator: value => [
        'nonUrgent',
        'urgent',
        'immediate',
      ].includes(value),
    },
    heading: {
      type: String,
      required: true,
    },
  },
  computed: {
    urgencyStyle() {
      return urgencyClasses[this.urgency];
    },
    headingPrefix() {
      switch (this.urgency) {
        case 'nonUrgent':
          return this.$t('generic.nonUrgentAdvice');
        case 'urgent':
          return this.$t('generic.urgentAdvice');
        case 'immediate':
          return this.$t('generic.immediateAdvice');
        default:
          return this.urgency;
      }
    },
  },
};
</script>
