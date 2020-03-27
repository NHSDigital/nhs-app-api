<template>
  <div class="nhsuk-care-card" :class="urgencyStyle">
    <div class="nhsuk-care-card__heading-container">
      <p class="nhsuk-care-card__heading nhsuk-heading-m">
        <span role="text">
          <span class="nhsuk-u-visually-hidden">{{ headingPrefix }}</span>
          {{ heading }}
        </span>
      </p>
      <span class="nhsuk-care-card__arrow" aria-hidden="true"/>
    </div>
    <div class="nhsuk-care-card__content">
      <slot/>
    </div>
  </div>
</template>

<script>
const urgencyClasses = {
  nonUrgent: 'nhsuk-care-card--non-urgent',
  urgent: 'nhsuk-care-card--urgent',
  immediate: 'nhsuk-care-card--immediate',
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
      return this.$t(`careCard.headingPrefix.${this.urgency}`);
    },
  },
};
</script>
