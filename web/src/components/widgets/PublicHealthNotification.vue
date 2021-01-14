<!-- Disabling vue/no-v-html as html content is returned from SJR and is trusted -->
<!-- eslint-disable vue/no-v-html -->
<template>
  <div v-if="validConfiguration" class="public-health-notification">
    <warning-callout
      :title="title">
      <div data-purpose="warning-callout-body" v-html="body"/>
    </warning-callout>
  </div>
</template>

<script>
import WarningCallout from '@/components/widgets/WarningCallout';

/*
 When adding new types/urgencies here for alternative configurations,
 some unit tests will need added for the validConfigurationsProp as
 due to the property validators, it's not possible to get an invalid
 combination at this time
*/
const types = ['callout'];
const urgencies = ['warning'];

const typeUrgencies = {
  callout: ['warning'],
};

export default {
  name: 'PublicHealthNotification',
  components: {
    WarningCallout,
  },
  props: {
    type: {
      type: String,
      required: true,
      validator: value => types.includes(value),
    },
    urgency: {
      type: String,
      required: true,
      validator: value => urgencies.includes(value),
    },
    title: {
      type: String,
      required: true,
    },
    body: {
      type: String,
      required: true,
    },
  },
  computed: {
    validConfiguration() {
      return (typeUrgencies[this.type] || []).includes(this.urgency);
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/public-health-notification";
</style>
